using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor.PackageManager.Requests;
using UnityEngine;


public class Util
{
    const int ProtocolLength = 4;
    const int LengthLength = 4;

    public static byte[] ClassToByte<T>(T data, Define.ClientMsg clientMsg) where T : class
    {
        // Jsonȭ
        string strData = JsonUtility.ToJson(data);

        // ����Ʈȭ
        byte[] byteProtocol = BitConverter.GetBytes((int)clientMsg);
        byte[] byteData = Encoding.UTF8.GetBytes(strData);
        byte[] byteLength = BitConverter.GetBytes(byteData.Length);

        // ��� ����Ʈ �迭 �ʱ�ȭ
        int totalLength = ProtocolLength + LengthLength + byteData.Length;
        byte[] buffer = new byte[totalLength];

        int currentIndex = 0;

        // Protocol ����
        Buffer.BlockCopy(byteProtocol, 0, buffer, currentIndex, ProtocolLength);
        currentIndex += ProtocolLength;

        // Length ����
        Buffer.BlockCopy(byteLength, 0, buffer, currentIndex, LengthLength);
        currentIndex += LengthLength;

        // Data ����
        Buffer.BlockCopy(byteData, 0, buffer, currentIndex, byteData.Length);

        return buffer;
    }

    public static T ByteToClass<T>(ArraySegment<byte> data) where T : class
    {
        return null;
    }
}
