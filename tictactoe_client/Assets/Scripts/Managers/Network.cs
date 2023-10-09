using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

/// <summary>
/// ������ ����, ���� �� Send, Recv�� ����
/// </summary>
public class Network : MonoBehaviour
{
    [SerializeField] GameObject _disconnetedUI;

    #region �̱���
    static Network _instance = null;

    public static Network GetInst()
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
                go.AddComponent<Network>();
            }

            _instance = go.GetComponent<Network>();
            _instance.Connect();
        }
    }

    public static void Clear()
    {
        _instance.Disconnect();
    }
    #endregion

    Socket _socket;

    SocketAsyncEventArgs _sendArgs;
    SocketAsyncEventArgs _recvArgs;


    private void OnApplicationQuit()
    {
        Clear();
    }

    /// <summary>
    /// <para>
    /// �������� ���� ��û�� ����
    /// </para>
    /// <para>
    /// Recv�� Connect���� �ڵ����� ó����
    /// </para>
    /// </summary>
    public void Connect()
    {
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9988);
        _socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            _socket.Connect(endPoint);
            Debug.Log($"[Client]Connected To {_socket.RemoteEndPoint.ToString()}");
        }
        catch (Exception e)
        {
            // TODO : �˸�â ���� ����
            Debug.LogException(e);
        }
        
        _recvArgs = new SocketAsyncEventArgs();
        _recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
        _recvArgs.SetBuffer(new byte[1024], 0, 1014);


        _sendArgs = new SocketAsyncEventArgs();
        _sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);


        RegisterRecv();
    }

    /// <summary>
    /// �������� ������ ������
    /// </summary>
    public void Disconnect()
    {
        _socket.Shutdown(SocketShutdown.Both);
        _socket.Close();
    }

    /// <summary>
    /// �������� �񵿱� �������� send�� ��
    /// </summary>
    /// <param name="sendBuff">���� ����</param>
    public void Send(byte[] sendBuff)
    {
        if (_socket.Connected == false)
        {
            _disconnetedUI.SetActive(true);
        }

        _sendArgs.SetBuffer(sendBuff, 0, sendBuff.Length);


        bool pending = _socket.SendAsync(_sendArgs);
        if (pending == false)
        {
            OnSendCompleted(null, _sendArgs);
        }
    }

    /// <summary>
    /// <para>
    /// �ܺο� �������� �ʰ� �ڵ����� ó����
    /// </para>
    /// Send �Ϸ� �� ��ó��
    /// </summary>
    /// <param name="sender">�̺�Ʈ�� ���� ��ü</param>
    /// <param name="args">�̺�Ʈ�� ���� �� �ڼ��� ����</param>
    void OnSendCompleted(object sender, SocketAsyncEventArgs args)
    {
        if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
        {

        }
    }

    /// <summary>
    /// <para>
    /// �ܺο� �������� �ʰ� �ڵ����� ó����
    /// </para>
    /// �񵿱�� Recv�� ���
    /// </summary>
    void RegisterRecv()
    {
        bool pending = _socket.ReceiveAsync(_recvArgs);

        if (pending == false)
        {
            OnRecvCompleted(null, _recvArgs);
        }
    }

    /// <summary>
    /// <para>
    /// �ܺο� �������� �ʰ� �ڵ����� ó����
    /// </para>
    /// Recv �Ϸ� �� ��ó��
    /// </summary>
    /// <param name="sender">�̺�Ʈ�� ���� ��ü</param>
    /// <param name="args">�̺�Ʈ�� ���� �� �ڼ��� ����</param>
    void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
    {
        if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
        {
            // TODO : ��������Ʈ�� ����
            byte[] recvBuff = new byte[1024];
            Array.Copy(args.Buffer, recvBuff, args.BytesTransferred);

            Debug.Log(recvBuff);
        }

        RegisterRecv();
    }
}
