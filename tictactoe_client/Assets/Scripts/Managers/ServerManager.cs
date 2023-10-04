using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class ServerManager : MonoBehaviour
{
    #region �̱���
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

    // �� ����� ���� ����
    private void OnApplicationQuit()
    {
        Clear();
    }

    public void Connect()
    {
        // IP �ּ� ��������
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9988);
        // ���� ����
        _socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        // ������ IP �ּҿ� ����
        _socket.Connect(endPoint);
        Debug.Log($"[Client]Connected To {_socket.RemoteEndPoint.ToString()}");


        // �񵿱� recv�� args ����
        _recvArgs = new SocketAsyncEventArgs();
        // recv �Ϸ�� ������ �Լ� ���ε�
        _recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
        // recv ���� ����
        _recvArgs.SetBuffer(new byte[1024], 0, 1014);


        // �񵿱� send�� args ����
        _sendArgs = new SocketAsyncEventArgs();
        // send �Ϸ�� ������ �Լ� ���ε�
        _sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);


        // recv ���
        RegisterRecv();
    }

    public void Disconnect()
    {
        // 2�� ���� ���� ����
        if (Interlocked.Exchange(ref _disconnected, 1) == 1)
            return;

        Debug.Log("Disconnected");

        // ���� ����
        _socket.Shutdown(SocketShutdown.Both);
        _socket.Close();
    }

    // Send�� �ϰŸ� ��ô
    public void Send(byte[] sendBuff)
    {
        lock (_lock)
        {
            // ������ �� �Ǿ������� ����
            if (_socket.Connected == false)
                Connect();

            // �ϴ� sendQueue�� ���� ���� ����
            _sendQueue.Enqueue(sendBuff);

            // send�� ��ϵ��� �ʾҴٸ�(������ ���� �ƴ϶��) send ���
            if (_pending == false)
                RegisterSend();
        }
    }

    // Send ���(�ٷ� ������)
    public void RegisterSend()
    {
        // ������ ���� �� _pending Ȱ��ȭ
        _pending = true;


        // ���� ���� sendQueue���� ����
        byte[] buff = _sendQueue.Dequeue();
        // ���� ���� ����
        _sendArgs.SetBuffer(buff, 0, buff.Length);


        // �񵿱� send ����
        bool pending = _socket.SendAsync(_sendArgs);
        // �����ڸ��� �������� �ٷ� ��ó��
        if (pending == false)
            OnSendCompleted(null, _sendArgs);
    }

    // Send�� �� ���� ��ó��
    public void OnSendCompleted(object sender, SocketAsyncEventArgs args)
    {
        lock (_lock)
        {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                // ������ �� �����ִٸ� �ٷ� ����
                if (_sendQueue.Count > 0)
                    RegisterSend();

                // ������ ���ٸ� ������ �� ���� ����
                else
                    _pending = false;
            }
        }
    }

    // Recv ���
    void RegisterRecv()
    {
        // �ޱ� ���
        bool pending = _socket.ReceiveAsync(_recvArgs);

        // �ޱ� ������ڸ��� ������ �ٷ� ��ó��
        if (pending == false)
            OnRecvCompleted(null, _recvArgs);
    }

    // Recv�� �� ���� ��ó��
    void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
    {
        if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
        {
            // TODO : ��������Ʈ�� ����
            byte[] recvBuff = new byte[1024];
            Array.Copy(args.Buffer, recvBuff, args.BytesTransferred);

            Debug.Log(recvBuff);
        }

        // ��ó���ϰ� �ٷ� ���� recv���
        RegisterRecv();
    }
}
