using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class ServerManager : MonoBehaviour
{
    #region ΩÃ±€≈Ê
    static ServerManager _instance = null;

    public static ServerManager GetInst()
    {
        Init();
        return _instance;
    }

    public static void Init()
    {
        if (_instance == null)
        {
            GameObject go = GameObject.Find("@ServerManger");
            if (go == null)
            {
                go = new GameObject() { name = "@ServerManger" };
                go.AddComponent<ServerManager>();
            }

            _instance = go.GetComponent<ServerManager>();
        }
    }
    #endregion

    Socket _socket;
    
    public static void Clear()
    {

    }

    public void Connect()
    {
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9988);
        _socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
       _socket.Connect(endPoint);
       Debug.Log($"Connected To {_socket.RemoteEndPoint.ToString()}");
    }

    public void Send(byte[] sendBuff)
    {
        int sendBytes = _socket.Send(sendBuff);
        Debug.Log($"{sendBytes} Bytes send");
    }

    public byte[] Receive()
    {
        byte[] recvBuff = new byte[1024];
        int recvBytes = _socket.Receive(recvBuff);

        Debug.Log($"{recvBytes} Bytes receive");

        return recvBuff;
    }

    public void Close()
    {
        _socket.Shutdown(SocketShutdown.Both);
        _socket.Close();
    }
}
