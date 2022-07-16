using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public static class FloorGenerator
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

        public bool IsValid(int floorSize) => R >= 0 && R < floorSize && C >= 0 && C < floorSize;

        public List<RoomStruct> GetValidNeighbors(int floorSize)
        {
            var neighbors = new List<RoomStruct>();
            for (int i = 0; i < 4; i++)
            {
                var neighbor = GetNeighbor(dr[i], dc[i]);
                if (neighbor.IsValid(floorSize)) neighbors.Add(neighbor);
            }
            return neighbors;
        }
        public RoomStruct? RandomNeighbor(int floorSize)
        {
            var neighbors = GetValidNeighbors(floorSize);
            if (!neighbors.Any()) return null;
            return neighbors.RandomElement();
        }
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
                // var prefab = Resources.Load<GameObject>(DEFAULT_PREFAB); 
                var prefab = new GameObject();
                prefab.hideFlags = HideFlags.HideAndDontSave;
                var room = prefab.AddComponent<Room>();
                // var room = prefab.GetComponent<DefaultRoom>();
                room.ExitSides = (RoomExitSide)i;
                rooms[i].Add(room);
            }
        
        return rooms;
    }

    private static T RandomElement<T>(this ICollection<T> collection)
    {
        if (!collection.Any()) throw new InvalidOperationException("Collection is empty");
        return collection.ElementAt(UnityEngine.Random.Range(0, collection.Count));
    }

    public static List<List<GameObject>> GenerateFloor
    (int floorNumber, int roomCount, int floorSize, float proportionWallsRemoved)
    {
        if (roomCount <= 0 || floorSize <= 0)
            throw new ArgumentOutOfRangeException("Room count and floor size must be greater than 0");

        if (roomCount > floorSize*floorSize) 
            throw new ArgumentException("Room count cannot be greater than floor size squared");
        
        if (proportionWallsRemoved < 0 || proportionWallsRemoved > 1)
            throw new ArgumentOutOfRangeException("Proportion walls removed must be between 0 and 1");

        var rooms = new HashSet<RoomStruct>();
        var walls = new HashSet<WallStruct>();

        // add starting room
        var startRoom = new RoomStruct(floorSize/2, floorSize/2);
        rooms.Add(startRoom);

        // create spanning tree of rooms
        while (rooms.Count < roomCount)
        {
            // choose a random room to branch off of
            var room = rooms.RandomElement();

            // choose random of 4 sides for next room
            var maybeNewRoom = room.RandomNeighbor(floorSize);
            if (maybeNewRoom is null) continue;
            var newRoom = (RoomStruct)maybeNewRoom;

            // add the new room if it's valid and not already existing
            if (!rooms.Contains(newRoom))
            {
                rooms.Add(newRoom);
                
                // Add walls between new room and existing neighboring rooms
                foreach (var neighbor in newRoom.GetValidNeighbors(floorSize))
                    if (rooms.Contains(neighbor))
                        walls.Add(new WallStruct(newRoom, neighbor));
                // remove wall between room and new room 
                walls.Remove(new WallStruct(room, newRoom));
            }
        }
        
        // remove walls to add loops
        int removeCount = (int)(walls.Count * proportionWallsRemoved);
        for (int i = 0; i < removeCount; i++)
            walls.Remove(walls.RandomElement());

        // BFS to find farthest room from start room, and set it to final room
        var queue = new Queue<RoomStruct>();
        var dist = new Dictionary<RoomStruct, int>();
        queue.Enqueue(startRoom);
        dist[startRoom] = 0;
        while (queue.Any())
        {
            var room = queue.Dequeue();
            foreach (var neighbor in room.GetValidNeighbors(floorSize))
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
        
        // load all prefabs for the floor
        var possibleRooms = LoadRooms($"Rooms/Floor{floorNumber}");
        var possibleStartRooms = LoadRooms($"Rooms/Floor{floorNumber}/StartRoom");
        var possibleFinalRooms = LoadRooms($"Rooms/Floor{floorNumber}/FinalRoom");

        // generate floor
        var floor = new List<List<GameObject>>(floorSize);
        for (int i = 0; i < floorSize; i++)
            floor.Add(new List<GameObject>(new GameObject[floorSize]));

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
            floor[room.R][room.C] = possibleRooms[(int)GetRoomExitSide(room)].RandomElement().gameObject;

        // set start room
        var start = possibleStartRooms[(int)GetRoomExitSide(startRoom)].RandomElement();
        start.RoomType = Room.RoomTypes.Start;
        floor[startRoom.R][startRoom.C] = start.gameObject;
        // rooms.Remove(START_ROOM);

        // set final room
        var final = possibleFinalRooms[(int)GetRoomExitSide(finalRoom)].RandomElement();
        final.RoomType = Room.RoomTypes.Final;
        floor[finalRoom.R][finalRoom.C] = final.gameObject;
        // rooms.Remove(finalRoom);

        return floor;
    }
}
