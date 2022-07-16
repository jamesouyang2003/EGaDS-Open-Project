using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class FloorGenerator
{ 
    // row col offsets for left, right, up, and down
    private static readonly int[] dr = {0, 0, -1, 1};
    private static readonly int[] dc = {-1, 1, 0, 0};

    private struct RoomStruct
    {
        public readonly int R, C;
        public RoomStruct(int r, int c)
            => (R, C) = (r, c);
        public override string ToString() => $"{R}, {C}";
        public RoomStruct GetNeighbor(int dr, int dc) => new RoomStruct(R+dr, C+dc);
    }
    private struct WallStruct
    {
        public readonly RoomStruct Room1, Room2;
        public WallStruct(RoomStruct room1, RoomStruct room2)
        {   // sort so that the same row with swapped rooms are made into the same
            if (room1.R < room2.R || room1.C < room2.C) (Room1, Room2) = (room1, room2);
            else (Room1, Room2) = (room2, room1);
        }
        public override string ToString() => $"[[{Room1}, {Room2}]]";
    }

    private readonly int _roomCount;
    private readonly int _floorSize;
    private readonly float _proportionWallsRemoved;

    private readonly List<Room>[] _roomPrefabs;
    private readonly List<Room>[] _startRoomPrefabs;
    private readonly List<Room>[] _finalRoomPrefabs;

    public FloorGenerator(int floorNumber, int roomCount, int floorSize, float proportionWallsRemoved)
    {
        if (roomCount <= 0 || floorSize <= 0)
            throw new ArgumentOutOfRangeException("Room count and floor size must be greater than 0");
        if (roomCount > floorSize*floorSize) 
            throw new ArgumentException("Room count cannot be greater than floor size squared");
        if (proportionWallsRemoved < 0 || proportionWallsRemoved > 1)
            throw new ArgumentOutOfRangeException("Proportion walls removed must be between 0 and 1");

        this._roomCount = roomCount;
        this._floorSize = floorSize;
        this._proportionWallsRemoved = proportionWallsRemoved;

        _roomPrefabs = LoadRooms($"Rooms/Floor{floorNumber}");
        _startRoomPrefabs = LoadRooms($"Rooms/Floor{floorNumber}/StartRoom");
        _finalRoomPrefabs = LoadRooms($"Rooms/Floor{floorNumber}/FinalRoom");
    }

    private static T RandomElement<T>(ICollection<T> collection)
    {
        if (!collection.Any()) throw new InvalidOperationException("Collection is empty");
        return collection.ElementAt(UnityEngine.Random.Range(0, collection.Count));
    }

    private bool IsValid(RoomStruct room) 
        => room.R >= 0 && room.R < _floorSize && room.C >= 0 && room.C < _floorSize;

    private List<RoomStruct> GetValidNeighbors(RoomStruct room)
    {
        var neighbors = new List<RoomStruct>();
        for (int i = 0; i < 4; i++)
        {
            var neighbor = room.GetNeighbor(dr[i], dc[i]);
            if (IsValid(neighbor)) neighbors.Add(neighbor);
        }
        return neighbors;
    }
    private RoomStruct? GetRandomNeighbor(RoomStruct room)
    {
        var neighbors = GetValidNeighbors(room);
        if (!neighbors.Any()) return null;
        return RandomElement(neighbors);
    }


    /// <summary>
    /// Loads the room prefabs in a path in the Resources folder into
    /// an array indexed by RoomExitSide
    /// </summary>
    private static List<Room>[] LoadRooms(string path)
    {
        var prefabs = Resources.LoadAll<GameObject>(path);
        var rooms = new List<Room>[1 << 4];
        for (int i = 0; i < rooms.Length; i++)
            rooms[i] = new List<Room>();
        
        // add the prefabs to the list
        foreach (var prefab in prefabs)
        {
            var room = prefab.GetComponent<Room>();
            if (room is not null) 
                rooms[(int)room.ExitSides].Add(room);
        }

        // if any room type is empty, then use a default room
        for (int i = 0; i < rooms.Length; i++)
            if (!rooms[i].Any())
            {
                var prefab = new GameObject();
                prefab.hideFlags = HideFlags.HideAndDontSave;
                var room = prefab.AddComponent<Room>();
                room.ExitSides = (RoomExitSide)i;
                rooms[i].Add(room);
            }
        
        return rooms;
    }


    public List<List<Room>> GenerateFloor()
    {
        var rooms = new HashSet<RoomStruct>();
        var walls = new HashSet<WallStruct>();

        // add starting room
        var startRoom = new RoomStruct(_floorSize/2, _floorSize/2);
        rooms.Add(startRoom);

        // create spanning tree of rooms
        while (rooms.Count < _roomCount)
        {
            // choose a random room to branch off of
            var room = RandomElement(rooms);

            // choose random of 4 sides for next room
            var maybeNewRoom = GetRandomNeighbor(room);
            if (maybeNewRoom is null) continue;
            var newRoom = (RoomStruct)maybeNewRoom;

            // add the new room if it's valid and not already existing
            if (!rooms.Contains(newRoom))
            {
                rooms.Add(newRoom);
                
                // Add walls between new room and existing neighboring rooms
                foreach (var neighbor in GetValidNeighbors(newRoom))
                    if (rooms.Contains(neighbor))
                        walls.Add(new WallStruct(newRoom, neighbor));
                // remove wall between room and new room 
                walls.Remove(new WallStruct(room, newRoom));
            }
        }
        
        // remove walls to add loops
        int removeCount = (int)(walls.Count * _proportionWallsRemoved);
        for (int i = 0; i < removeCount; i++)
            walls.Remove(RandomElement(walls));

        // BFS to find farthest room from start room, and set it to final room
        var queue = new Queue<RoomStruct>();
        var dist = new Dictionary<RoomStruct, int>();
        queue.Enqueue(startRoom);
        dist[startRoom] = 0;
        while (queue.Any())
        {
            var room = queue.Dequeue();
            foreach (var neighbor in GetValidNeighbors(room))
            {
                if (rooms.Contains(neighbor) && !walls.Contains(new WallStruct(room, neighbor))
                 && !dist.ContainsKey(neighbor))
                {
                    dist[neighbor] = dist[room] + 1;
                    queue.Enqueue(neighbor);
                }
            }
        }
        RoomStruct finalRoom = startRoom;
        foreach (var (room, distance) in dist)
            if (distance > dist[finalRoom])
                finalRoom = room;

        // generate floor
        var floor = new List<List<Room>>(_floorSize);
        for (int i = 0; i < _floorSize; i++)
            floor.Add(new List<Room>(new Room[_floorSize]));

        RoomExitSide GetRoomExitSide(RoomStruct room)
        {
            RoomExitSide exitSides = RoomExitSide.None;
            for (int i = 0; i < 4; i++)
            {
                var neighbor = room.GetNeighbor(dr[i], dc[i]);
                if (!walls.Contains(new WallStruct(room, neighbor)) && rooms.Contains(neighbor))
                    exitSides = exitSides | (RoomExitSide)(1<<i);
            }
            return exitSides;
        }

        foreach (var room in rooms)
            floor[room.R][room.C] = RandomElement(_roomPrefabs[(int)GetRoomExitSide(room)]);

        // set start room
        var start = RandomElement(_startRoomPrefabs[(int)GetRoomExitSide(startRoom)]);
        start.RoomType = Room.RoomTypes.Start;
        floor[startRoom.R][startRoom.C] = start;

        // set final room
        var final = RandomElement(_finalRoomPrefabs[(int)GetRoomExitSide(finalRoom)]);
        final.RoomType = Room.RoomTypes.Final;
        floor[finalRoom.R][finalRoom.C] = final;

        return floor;
    }
}
