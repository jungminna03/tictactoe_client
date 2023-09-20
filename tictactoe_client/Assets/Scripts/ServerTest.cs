using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using System;
using MessagePack;

public class ServerTest : MonoBehaviour
{
    Socket _listenSocket;
    
    void Start()
    {
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9988);
        _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        _listenSocket.Bind(endPoint);
        _listenSocket.Listen(10);

        SocketAsyncEventArgs args = new SocketAsyncEventArgs();
        args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);

        RegisterAccept(args);
    }

    void RegisterAccept(SocketAsyncEventArgs args)
    {
        bool pending = _listenSocket.AcceptAsync(args);
        if (pending)
            OnAcceptCompleted(null, args);
    }

    void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
    {
        if (args.SocketError == SocketError.Success)
        {
            Debug.Log($"[Server] Conected");

            byte[] recvBuff = new byte[1024];
            int recvBytes = args.AcceptSocket.Receive(recvBuff);

            byte[] validBuff = new byte[recvBytes];
            Array.Copy(recvBuff, validBuff, recvBytes);
            MsgPack msgPack = Util.ByteToClass<MsgPack>(validBuff);

            string json = MessagePackSerializer.ConvertToJson(msgPack.Data);
            Debug.Log(json);
        }

        RegisterAccept(args);
    }
}
