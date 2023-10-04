using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MessagePack;

// ·Î±×ÀÎ
public class LoginReqest
{
    public string email;
    public string password;
}

[MessagePackObject]
public record MsgPack
{
    [Key(0)]
    public byte[] Protocol;
    [Key(1)]
    public byte[] Length;
    [Key(2)]
    public byte[] Data;
}