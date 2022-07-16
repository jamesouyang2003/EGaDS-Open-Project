using UnityEngine;
using UnityEditor;


public class Room : MonoBehaviour
{
    public enum RoomTypes { Regular, Start, End }

    public const int ROOM_SIZE = 20; // width and height of rooms    

    [SerializeField]
    private RoomTypes _roomType;

    // [EnumMask(typeof(ExitLocation))]

    [Header("Exits")]

    [SerializeField, EnumMask(typeof(RoomExitSide))] 
    protected RoomExitSide _exitSides;

    [SerializeField, EnumMaskConditional("_exitSides", (int)RoomExitSide.Left), Range(0, ROOM_SIZE)]
    private float _leftExitPosition = ROOM_SIZE/2;
    [SerializeField, EnumMaskConditional("_exitSides", (int)RoomExitSide.Right), Range(0, ROOM_SIZE)]    
    private float _rightExitPosition = ROOM_SIZE/2;
    [SerializeField, EnumMaskConditional("_exitSides", (int)RoomExitSide.Top), Range(0, ROOM_SIZE)]      
    private float _topExitPosition = ROOM_SIZE/2;
    [SerializeField, EnumMaskConditional("_exitSides", (int)RoomExitSide.Bottom), Range(0, ROOM_SIZE)]   
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
            bool isExit = _exitSides.HasFlag((RoomExitSide)(1<<i));
            // Gizmos.color = _exitSides.HasFlag((RoomExitSide)(1<<i)) ? Color.green : Color.red;
            Handles.color = isExit ? Color.green : Color.red;
            Vector2 center = transform.position + new Vector3(dx[i], dy[i]) * ROOM_SIZE/2;
            Vector2 cornerOffset = new Vector2(dy[i], dx[i]) * ROOM_SIZE/2;
            // Gizmos.DrawLine(center+cornerOffset, center-cornerOffset);
            if (isExit) Handles.DrawDottedLine(center+cornerOffset, center-cornerOffset, Handles.lineThickness*2);
            else Handles.DrawLine(center+cornerOffset, center-cornerOffset, Handles.lineThickness);

            // draw exit location
            if (_exitSides.HasFlag((RoomExitSide)(1<<i))) 
            {
                Vector2 location = transform.position + (Vector3)GetExitLocation((RoomExitSide)(1<<i));
                Vector2 offset = new Vector2(dx[i], dy[i]) * 0.5f;
                // Gizmos.DrawLine(location-offset, location+offset);
                if (isExit) Handles.DrawDottedLine(location-offset, location+offset, Handles.lineThickness*2);
                else Handles.DrawLine(location-offset, location+offset, Handles.lineThickness);
            }
        }
    }
}
