using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    // width and height determine where exit triggers are (room is centered at 0,0)
    [SerializeField] private float _roomWidth;
    [SerializeField] private float _roomHeight;

    [SerializeField] private GameObject _fadeGameObject;

    /// <summary>
    /// The current floor number. The first floor is 1, second floor is 2, etc.
    /// </summary>
    public int CurrentFloor { get; private set; }

    /// <summary>
    /// The rooms in the current floor
    /// </summary>
    public List<List<GameObject>> Rooms { get; private set; }

    /// <summary>
    /// The row of the room the player is currently in
    /// </summary>
    private int CurrentRoomRow = 0;

    /// <summary>
    /// The column of the room the player is currently in
    /// </summary>
    private int CurrentRoomCol = 0;

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
