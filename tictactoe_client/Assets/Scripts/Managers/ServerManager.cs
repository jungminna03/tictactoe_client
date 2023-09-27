using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
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

    public void Disconnect()
    {
        if (Interlocked.Exchange(ref _disconnected, 1) == 1)
            return;

        Debug.Log("Disconnected");

        _socket.Shutdown(SocketShutdown.Both);
        _socket.Close();
    }

    public void Send(byte[] sendBuff)
    {
        lock (_lock)
        {
            if (_socket.Connected == false)
                Connect();

            _sendQueue.Enqueue(sendBuff);

            if (_pending == false)
                RegisterSend();
        }
    }

    public void RegisterSend()
    {
        _pending = true;
        byte[] buff = _sendQueue.Dequeue();
        _sendArgs.SetBuffer(buff, 0, buff.Length);

        bool pending = _socket.SendAsync(_sendArgs);
        if (pending == false)
            OnSendCompleted(null, _sendArgs);
    }

    public void OnSendCompleted(object sender, SocketAsyncEventArgs args)
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
            else
            {
                Disconnect();
            }
        }
    }

    void RegisterRecv()
    {
        bool pending = _socket.ReceiveAsync(_recvArgs);
        if (pending == false)
            OnRecvCompleted(null, _recvArgs);
    }
    void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
    {
        if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
        {
            byte[] recvBuff = new byte[1024];
            Array.Copy(args.Buffer, recvBuff, args.BytesTransferred);

            Debug.Log(recvBuff);
        }
        else
        {
            Disconnect();
        }

        RegisterRecv();
    }
}
