/// <summary>
/// Represents the sides of a room.
/// Is a flag enum, so multiple values can be set by setting each of the 4 bits.
/// The bits are Left, Right, Top, Bottom, from bit 0 to 3
/// </summary>
public enum RoomExitSide 
{
    None = 0,
    Left = 1 << 0, 
    Right = 1 << 1, 
    Top = 1 << 2, 
    Bottom = 1 << 3,
    All = ~0
}

public static class RoomExitSideMethods
{
    /// <summary>
    /// returns a RoomExitSide where the sides are mirrored across
    /// basically swaps the left/right bits and top/bottom bits
    /// </summary>
    public static RoomExitSide GetOpposite(this RoomExitSide side)
    {
        bool left = side.HasFlag(RoomExitSide.Left);
        bool right = side.HasFlag(RoomExitSide.Right);
        bool x = left ^ right;
        side = side ^ (x ? (RoomExitSide.Left | RoomExitSide.Right) : RoomExitSide.None);

        bool top = side.HasFlag(RoomExitSide.Top);
        bool bottom = side.HasFlag(RoomExitSide.Bottom);
        x = top ^ bottom;
        side = side ^ (x ? (RoomExitSide.Top | RoomExitSide.Bottom) : RoomExitSide.None);
        
        return side;
    }
}