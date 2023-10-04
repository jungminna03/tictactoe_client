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
        // IP 주소 가져오기
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9988);
        // 소켓 생성
        _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        // 소켓 바인드
        _listenSocket.Bind(endPoint);
        _listenSocket.Listen(10);

        // accept용 args 생성
        SocketAsyncEventArgs args = new SocketAsyncEventArgs();
        // accept 완료시 실행할 함수 바인딩
        args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);

        // accept 등록
        RegisterAccept(args);
    }

    // 수락 등록
    void RegisterAccept(SocketAsyncEventArgs args)
    {
        // 비동기 accept 실행
        bool pending = _listenSocket.AcceptAsync(args);
        // 바로 수락이 완료되면 바로 후처리
        if (!pending)
            OnAcceptCompleted(null, args);
    }

    // 수락 완료 후 후처리
    void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
    {
        if (args.SocketError == SocketError.Success)
        {
            // 단순히 받아지면 보낸 정보 받아서 그대로 출력
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
