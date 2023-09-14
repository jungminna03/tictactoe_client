using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define : MonoBehaviour
{
    public enum ClientMsg
    {
        Login = 0,
        Register = 1,
        Logout = 2,
        RoomCreate = 3,
        RoomStart = 4,
        RoomExit = 5,
        RoomJoin = 6,
        RoomLoad = 7,
        PlayerLoad = 8,
        PlayerTurn = 9,
        RankLoad = 10,
    };
}