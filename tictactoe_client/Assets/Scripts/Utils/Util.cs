using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using MessagePack;
using MessagePack.Resolvers;
using Unity.VisualScripting;

public class Util
{
    const int ProtocolLength = 4;
    const int LengthLength = 4;

    public static byte[] ClassToByte<T>(T data, Define.ClientMsg clientMsg) where T : class
    {
        // Jsonȭ
        string strData = JsonUtility.ToJson(data);

        // ����Ʈȭ
        byte[] byteData = MessagePackSerializer.ConvertFromJson(strData);

        // �޼����� ����
        MsgPack msgPack = new MsgPack();
        msgPack.Protocol = BitConverter.GetBytes((int)clientMsg);
        msgPack.Length = BitConverter.GetBytes(byteData.Length);
        msgPack.Data = byteData;

        byte[] msgPackData = MessagePackSerializer.Serialize(msgPack);

        MsgPack temp = MessagePackSerializer.Deserialize<MsgPack>(msgPackData);

        return msgPackData;
    }

    public static T ByteToClass<T>(byte[] data) where T : class
    {
        T msgPack = MessagePackSerializer.Deserialize<T>(data);

        return msgPack;
    }
}