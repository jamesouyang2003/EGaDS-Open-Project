using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public enum RoomTypes { Regular, Start, End }

    [SerializeField]
    private RoomTypes _roomType;

    [Header("Exit Locations")]
    [SerializeField] private bool _leftExit;
    [SerializeField] private bool _rightExit;
    [SerializeField] private bool _topExit;
    [SerializeField] private bool _bottomExit;

    public RoomTypes RoomType => _roomType;

    public bool LeftExit => _leftExit;
    public bool RightExit => _rightExit;
    public bool TopExit => _topExit;
    public bool BottomExit => _bottomExit;
}
