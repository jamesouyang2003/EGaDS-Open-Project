using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    [SerializeField] private GameObject _fadeGameObject;

    [SerializeField, Range(1, 100)] private int _roomCount = 5;
    [SerializeField, Range(1, 20)] private int _floorSize = 7;
    [SerializeField, Range(0, 1)] private float _proportionWallsRemoved = 0.5f;

    public int RoomCount => _roomCount;
    public int FloorSize => _floorSize;
    public float ProportionWallsRemoved => _proportionWallsRemoved;

    /// <summary>
    /// The current floor number. The first floor is 1, second floor is 2, etc.
    /// </summary>
    public int CurrentFloor { get; private set; } = 1;

    /// <summary>
    /// The rooms in the current floor
    /// </summary>
    public List<List<GameObject>> Rooms { get; private set; }

    /// <summary>
    /// The row of the room the player is currently in
    /// </summary>
    public int CurrentRoomRow { get; private set; } = 0;

    /// <summary>
    /// The column of the room the player is currently in
    /// </summary>
    public int CurrentRoomCol { get; private set; } = 0;

    /// <summary>
    /// The room the player is currently in
    /// </summary>
    public GameObject CurrentRoom => Rooms[CurrentRoomCol][CurrentRoomRow];

    // Start is called before the first frame update
    void Start()
    {
        // call FloorGenerator to generate floor 1
    }

    // Update is called once per frame
    void Update()
    {
        // detect if player went beyond room bounds
        // or use colliders?
    }

    void OnDrawGizmos() {
        // TODO: Draw room exit triggers for current room
    }
}
