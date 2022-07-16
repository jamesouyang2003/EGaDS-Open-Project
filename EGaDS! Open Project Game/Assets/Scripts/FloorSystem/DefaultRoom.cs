using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The default room to fall back to if a room type doesn't exist
public class DefaultRoom : Room
{
    new public RoomExitSide ExitSides 
    {
        get => ExitSides;
        set => ExitSides = value;
    }

    new public RoomTypes RoomType 
    {
        get => RoomType;
        set => RoomType = value;
    }
}
