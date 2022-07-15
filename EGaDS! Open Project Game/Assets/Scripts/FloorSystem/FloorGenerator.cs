using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class FloorGenerator
{
    public const int ROOM_COUNT = 5;
    public const int FLOOR_SIZE = 7;
    public const float PROPORTION_WALLS_REMOVED = 0.5f;
    
    // row col offsets for left, right, up, and down
    private static readonly int[] dr = {0, 0, -1, 1};
    private static readonly int[] dc = {-1, 1, 0, 0};
    
    private static readonly RoomStruct START_ROOM = new RoomStruct(FLOOR_SIZE/2, FLOOR_SIZE/2);

    private struct RoomStruct
    {
        public readonly int R, C;
        public RoomStruct(int r, int c)
            => (R, C) = (r, c);

        public override string ToString() => $"{R}, {C}";

        public bool IsValid => R >= 0 && R < FLOOR_SIZE && C >= 0 && C < FLOOR_SIZE;

        public List<RoomStruct> GetValidNeighbors()
        {
            var neighbors = new List<RoomStruct>();
            for (int i = 0; i < 4; i++)
            {
                var neighbor = GetNeighbor(dr[i], dc[i]);
                if (neighbor.IsValid) neighbors.Add(neighbor);
            }
            return neighbors;
        }
        public RoomStruct? RandomNeighbor()
        {
            var neighbors = GetValidNeighbors();
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
        foreach (var prefab in prefabs)
        {
            var room = prefab.GetComponent<Room>();
            if (room is not null) 
                rooms[(int)room.ExitSides].Add(room);
        }
        return rooms;
    }

    private static RoomExitSide GetRoomExitSide(ICollection<WallStruct> walls, RoomStruct room)
    {
        RoomExitSide exitSides = RoomExitSide.None;
        for (int i = 0; i < 4; i++)
        {
            if (walls.Contains(new WallStruct(room, room.GetNeighbor(dr[i], dc[i]))))
                exitSides = exitSides | (RoomExitSide)(1<<i);
        }
        return exitSides;
    }

    private static T RandomElement<T>(this ICollection<T> collection) 
        => collection.ElementAt(Random.Range(0, collection.Count));

    public static List<List<GameObject>> GenerateFloor(int floorNumber)
    {
        var rooms = new HashSet<RoomStruct>();
        var walls = new HashSet<WallStruct>();

        // add starting room
        rooms.Add(START_ROOM);

        // create spanning tree of rooms
        while (rooms.Count < ROOM_COUNT)
        {
            // choose a random room to branch off of
            var room = rooms.RandomElement();

            // choose random of 4 sides for next room
            var maybeNewRoom = room.RandomNeighbor();
            if (maybeNewRoom is null) continue;
            var newRoom = (RoomStruct)maybeNewRoom;

            // add the new room if it's valid and not already existing
            if (!rooms.Contains(newRoom))
            {
                rooms.Add(newRoom);
                
                // Add walls between new room and existing neighboring rooms
                foreach (var neighbor in newRoom.GetValidNeighbors())
                    if (rooms.Contains(neighbor))
                        walls.Add(new WallStruct(newRoom, neighbor));
                // remove wall between room and new room 
                walls.Remove(new WallStruct(room, newRoom));
            }
        }
        
        // remove walls to add loops
        int removeCount = (int)(walls.Count * PROPORTION_WALLS_REMOVED);
        for (int i = 0; i < removeCount; i++)
            walls.Remove(walls.RandomElement());

        // BFS to find farthest room from start room, and set it to final room
        var queue = new Queue<RoomStruct>();
        var dist = new Dictionary<RoomStruct, int>();
        queue.Append(START_ROOM);
        dist[START_ROOM] = 0;
        while (queue.Any())
        {
            var room = queue.Dequeue();
            foreach (var neighbor in room.GetValidNeighbors())
                if (rooms.Contains(neighbor) && !walls.Contains(new WallStruct(room, neighbor))
                 && dist.ContainsKey(neighbor))
                {
                    dist[neighbor] = dist[room] + 1;
                    queue.Append(neighbor);
                }
        }
        RoomStruct finalRoom = START_ROOM;
        foreach (var (room, distance) in dist)
            if (distance > dist[finalRoom])
                finalRoom = room;
        
        // load all prefabs for the floor
        var possibleRooms = LoadRooms($"Rooms/Floor{floorNumber}");
        var possibleStartRooms = LoadRooms($"Rooms/Floor{floorNumber}/StartRoom");
        var possibleFinalRooms = LoadRooms($"Rooms/Floor{floorNumber}/StartRoom");

        // generate floor
        var floor = new List<List<GameObject>>(FLOOR_SIZE);
        for (int i = 0; i < FLOOR_SIZE; i++) floor[i] = new List<GameObject>(FLOOR_SIZE);

        // set start room
        var exitSides = GetRoomExitSide(walls, START_ROOM);
        floor[START_ROOM.R][START_ROOM.C] = possibleStartRooms[(int)exitSides].RandomElement().gameObject;
        rooms.Remove(START_ROOM);

        exitSides = GetRoomExitSide(walls, finalRoom);
        floor[finalRoom.R][finalRoom.C] = possibleStartRooms[(int)exitSides].RandomElement().gameObject;
        rooms.Remove(finalRoom);

        foreach (var room in rooms)
        {
            exitSides = GetRoomExitSide(walls, room);
            floor[room.R][room.C] = possibleStartRooms[(int)exitSides].RandomElement().gameObject;
        }

        return floor;
    }
}
