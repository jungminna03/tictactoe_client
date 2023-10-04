using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class ServerManager : MonoBehaviour
{
    #region 싱글톤
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
            _instance.Connect();
        }
    }

    public static void Clear()
    {
        _instance.Disconnect();
    }
    #endregion

    Socket _socket;
    int _disconnected = 0;

    Queue<byte[]> _sendQueue = new Queue<byte[]>();
    SocketAsyncEventArgs _sendArgs;
    bool _pending = false;
    object _lock = new object();
    
    SocketAsyncEventArgs _recvArgs;

    // 앱 종료시 접속 종료
    private void OnApplicationQuit()
    {
        Clear();
    }

    public void Connect()
    {
        // IP 주소 가져오기
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9988);
        // 소켓 생성
        _socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        // 소켓을 IP 주소에 연결
        _socket.Connect(endPoint);
        Debug.Log($"[Client]Connected To {_socket.RemoteEndPoint.ToString()}");


        // 비동기 recv용 args 생성
        _recvArgs = new SocketAsyncEventArgs();
        // recv 완료시 실행할 함수 바인딩
        _recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
        // recv 버퍼 생성
        _recvArgs.SetBuffer(new byte[1024], 0, 1014);


        // 비동기 send용 args 생성
        _sendArgs = new SocketAsyncEventArgs();
        // send 완료시 실행할 함수 바인딩
        _sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);


        // recv 등록
        RegisterRecv();
    }

    public void Disconnect()
    {
        // 2중 연결 해제 방지
        if (Interlocked.Exchange(ref _disconnected, 1) == 1)
            return;

        Debug.Log("Disconnected");

        // 연결 해제
        _socket.Shutdown(SocketShutdown.Both);
        _socket.Close();
    }

    // Send할 일거리 투척
    public void Send(byte[] sendBuff)
    {
        lock (_lock)
        {
            // 연결이 안 되어있으면 연결
            if (_socket.Connected == false)
                Connect();

            // 일단 sendQueue에 보낼 버퍼 저장
            _sendQueue.Enqueue(sendBuff);

            // send가 등록되지 않았다면(보내는 중이 아니라면) send 등록
            if (_pending == false)
                RegisterSend();
        }
    }

    // Send 등록(바로 보내짐)
    public void RegisterSend()
    {
        // 보내는 중일 때 _pending 활성화
        _pending = true;


        // 보낼 버퍼 sendQueue에서 추출
        byte[] buff = _sendQueue.Dequeue();
        // 보낼 버퍼 셋팅
        _sendArgs.SetBuffer(buff, 0, buff.Length);


        // 비동기 send 실행
        bool pending = _socket.SendAsync(_sendArgs);
        // 보내자마자 보내지면 바로 후처리
        if (pending == false)
            OnSendCompleted(null, _sendArgs);
    }

    // Send를 한 이후 후처리
    public void OnSendCompleted(object sender, SocketAsyncEventArgs args)
    {
        lock (_lock)
        {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                // 보낼게 또 남아있다면 바로 보냄
                if (_sendQueue.Count > 0)
                    RegisterSend();

                // 보낼게 없다면 보내는 중 상태 해제
                else
                    _pending = false;
            }
        }
    }

    // Recv 등록
    void RegisterRecv()
    {
        // 받기 등록
        bool pending = _socket.ReceiveAsync(_recvArgs);

        // 받기 등록하자마자 받으면 바로 후처리
        if (pending == false)
            OnRecvCompleted(null, _recvArgs);
    }

    // Recv를 한 이후 후처리
    void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
    {
        if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
        {
            // TODO : 델리게이트로 변경
            byte[] recvBuff = new byte[1024];
            Array.Copy(args.Buffer, recvBuff, args.BytesTransferred);

            Debug.Log(recvBuff);
        }

        // 후처리하고 바로 다음 recv등록
        RegisterRecv();
    }
}
