using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

/// <summary>
/// 서버에 연결, 해제 및 Send, Recv를 해줌
/// </summary>
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

    private void OnApplicationQuit()
    {
        Clear();
    }

    /// <summary>
    /// <para>
    /// 서버에게 연결 요청을 보냄
    /// </para>
    /// <para>
    /// Recv는 Connect이후 자동으로 처리함
    /// </para>
    /// </summary>
    public void Connect()
    {
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9988);
        _socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        _socket.Connect(endPoint);
        Debug.Log($"[Client]Connected To {_socket.RemoteEndPoint.ToString()}");


        _recvArgs = new SocketAsyncEventArgs();
        _recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
        _recvArgs.SetBuffer(new byte[1024], 0, 1014);


        _sendArgs = new SocketAsyncEventArgs();
        _sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);


        RegisterRecv();
    }

    /// <summary>
    /// 서버와의 연결을 해제함
    /// </summary>
    public void Disconnect()
    {
        if (Interlocked.Exchange(ref _disconnected, 1) == 1)
            return;

        Debug.Log("Disconnected");

        _socket.Shutdown(SocketShutdown.Both);
        _socket.Close();
    }

    /// <summary>
    /// 서버에게 Send할 버퍼를 받음
    /// </summary>
    /// <param name="sendBuff">보낼 버퍼</param>
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

    /// <summary>
    /// <para>
    /// 외부에 공개되지 않고 자동으로 처리됨
    /// </para>
    /// 비동기식 Send를 등록
    /// </summary>
    void RegisterSend()
    {
        _pending = true;


        byte[] buff = _sendQueue.Dequeue();
        _sendArgs.SetBuffer(buff, 0, buff.Length);


        bool pending = _socket.SendAsync(_sendArgs);
        if (pending == false)
            OnSendCompleted(null, _sendArgs);
    }

    /// <summary>
    /// <para>
    /// 외부에 공개되지 않고 자동으로 처리됨
    /// </para>
    /// Send 완료 후 후처리
    /// </summary>
    /// <param name="sender">이벤트를 보낸 주체</param>
    /// <param name="args">이벤트의 종류 및 자세한 정보</param>
    void OnSendCompleted(object sender, SocketAsyncEventArgs args)
    {
        lock (_lock)
        {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                if (_sendQueue.Count > 0)
                    RegisterSend();

                else
                    _pending = false;
            }
        }
    }

    /// <summary>
    /// <para>
    /// 외부에 공개되지 않고 자동으로 처리됨
    /// </para>
    /// 비동기식 Recv를 등록
    /// </summary>
    void RegisterRecv()
    {
        bool pending = _socket.ReceiveAsync(_recvArgs);

        if (pending == false)
            OnRecvCompleted(null, _recvArgs);
    }

    /// <summary>
    /// <para>
    /// 외부에 공개되지 않고 자동으로 처리됨
    /// </para>
    /// Recv 완료 후 후처리
    /// </summary>
    /// <param name="sender">이벤트를 보낸 주체</param>
    /// <param name="args">이벤트의 종류 및 자세한 정보</param>
    void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
    {
        if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
        {
            // TODO : 델리게이트로 변경
            byte[] recvBuff = new byte[1024];
            Array.Copy(args.Buffer, recvBuff, args.BytesTransferred);

            Debug.Log(recvBuff);
        }

        RegisterRecv();
    }
}
