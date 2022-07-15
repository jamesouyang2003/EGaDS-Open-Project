using UnityEngine;

public class Room : MonoBehaviour
{
    public enum RoomTypes { Regular, Start, End }

    public enum ExitSide {
        None = 0,
        Left = 1 << 0, 
        Right = 1 << 1, 
        Top = 1 << 2, 
        Bottom = 1 << 3,
        All = ~0,
    }

    const int ROOM_SIZE = 20; // width and height of rooms    

    [SerializeField]
    private RoomTypes _roomType;

    // [EnumMask(typeof(ExitLocation))]

    [Header("Exits")]

    [SerializeField, EnumMask(typeof(ExitSide))] 
    private ExitSide _exitLocations;

    [SerializeField, Range(0, ROOM_SIZE), EnumMaskConditional("_exitLocations", (int)ExitSide.Left)]
    private float _leftExitPosition = ROOM_SIZE/2;
    [SerializeField, Range(0, ROOM_SIZE), EnumMaskConditional("_exitLocations", (int)ExitSide.Right)]    
    private float _rightExitPosition = ROOM_SIZE/2;
    [SerializeField, Range(0, ROOM_SIZE), EnumMaskConditional("_exitLocations", (int)ExitSide.Top)]      
    private float _topExitPosition = ROOM_SIZE/2;
    [SerializeField, Range(0, ROOM_SIZE), EnumMaskConditional("_exitLocations", (int)ExitSide.Bottom)]   
    private float _bottomExitPosition = ROOM_SIZE/2;

    public RoomTypes RoomType => _roomType;

    public float LeftExitPosition => _leftExitPosition;
    public float RightExitPosition => _rightExitPosition;
    public float TopExitPosition => _topExitPosition;
    public float BottomExitPosition => _bottomExitPosition;

    private Vector2 GetExitLocation(ExitSide side)
    {
        switch (side)
        {
            case ExitSide.Left: return new Vector2(-ROOM_SIZE/2, -ROOM_SIZE/2+LeftExitPosition);
            case ExitSide.Right: return new Vector2(ROOM_SIZE/2, -ROOM_SIZE/2+RightExitPosition);
            case ExitSide.Top: return new Vector2(-ROOM_SIZE/2+TopExitPosition, ROOM_SIZE/2);
            case ExitSide.Bottom: return new Vector2(-ROOM_SIZE/2+BottomExitPosition, -ROOM_SIZE/2);
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
            Gizmos.color = _exitLocations.HasFlag((ExitSide)(1<<i)) ? Color.green : Color.red;
            Vector2 center = new Vector2(dx[i], dy[i]) * ROOM_SIZE/2;
            Vector2 cornerOffset = new Vector2(dy[i], dx[i]) * ROOM_SIZE/2;
            Gizmos.DrawLine(center+cornerOffset, center-cornerOffset);

            // draw exit location
            if (_exitLocations.HasFlag((ExitSide)(1<<i))) 
            {
                Vector2 location = GetExitLocation((ExitSide)(1<<i));
                Vector2 offset = new Vector2(dx[i], dy[i]) * 0.5f;
                Gizmos.DrawLine(location-offset, location+offset);
            }
        }
    }
}
