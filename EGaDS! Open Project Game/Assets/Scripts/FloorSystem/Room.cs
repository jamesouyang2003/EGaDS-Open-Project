using UnityEngine;



public class Room : MonoBehaviour
{
    public enum RoomTypes { Regular, Start, End }

    const int ROOM_SIZE = 20; // width and height of rooms    

    [SerializeField]
    private RoomTypes _roomType;

    // [EnumMask(typeof(ExitLocation))]

    [Header("Exits")]

    [SerializeField, EnumMask(typeof(RoomExitSide))] 
    private RoomExitSide _exitSides;

    [SerializeField, Range(0, ROOM_SIZE), EnumMaskConditional("_exitLocations", (int)RoomExitSide.Left)]
    private float _leftExitPosition = ROOM_SIZE/2;
    [SerializeField, Range(0, ROOM_SIZE), EnumMaskConditional("_exitLocations", (int)RoomExitSide.Right)]    
    private float _rightExitPosition = ROOM_SIZE/2;
    [SerializeField, Range(0, ROOM_SIZE), EnumMaskConditional("_exitLocations", (int)RoomExitSide.Top)]      
    private float _topExitPosition = ROOM_SIZE/2;
    [SerializeField, Range(0, ROOM_SIZE), EnumMaskConditional("_exitLocations", (int)RoomExitSide.Bottom)]   
    private float _bottomExitPosition = ROOM_SIZE/2;

    public RoomTypes RoomType => _roomType;

    public RoomExitSide ExitSides => _exitSides;
    public float LeftExitPosition => _leftExitPosition;
    public float RightExitPosition => _rightExitPosition;
    public float TopExitPosition => _topExitPosition;
    public float BottomExitPosition => _bottomExitPosition;

    private Vector2 GetExitLocation(RoomExitSide side)
    {
        switch (side)
        {
            case RoomExitSide.Left: return new Vector2(-ROOM_SIZE/2, -ROOM_SIZE/2+LeftExitPosition);
            case RoomExitSide.Right: return new Vector2(ROOM_SIZE/2, -ROOM_SIZE/2+RightExitPosition);
            case RoomExitSide.Top: return new Vector2(-ROOM_SIZE/2+TopExitPosition, ROOM_SIZE/2);
            case RoomExitSide.Bottom: return new Vector2(-ROOM_SIZE/2+BottomExitPosition, -ROOM_SIZE/2);
            default: return Vector2.zero;
        }
    }
    
    void OnDrawGizmos()
    {
        // x and y offsets representing each direction (left, right, up, down)
        int[] dx = {-1, 1, 0, 0};
        int[] dy = {0, 0, 1, -1};
        for (int i = 0; i < 4; i++) 
        {
            // draw side
            Gizmos.color = _exitSides.HasFlag((RoomExitSide)(1<<i)) ? Color.green : Color.red;
            Vector2 center = new Vector2(dx[i], dy[i]) * ROOM_SIZE/2;
            Vector2 cornerOffset = new Vector2(dy[i], dx[i]) * ROOM_SIZE/2;
            Gizmos.DrawLine(center+cornerOffset, center-cornerOffset);

            // draw exit location
            if (_exitSides.HasFlag((RoomExitSide)(1<<i))) 
            {
                Vector2 location = GetExitLocation((RoomExitSide)(1<<i));
                Vector2 offset = new Vector2(dx[i], dy[i]) * 0.5f;
                Gizmos.DrawLine(location-offset, location+offset);
            }
        }
    }
}
