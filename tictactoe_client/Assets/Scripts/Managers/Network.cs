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
public class Network : MonoBehaviour
{
    [SerializeField] GameObject _disconnetedUI;

    #region 싱글톤
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

        try
        {
            _socket.Connect(endPoint);
            Debug.Log($"[Client]Connected To {_socket.RemoteEndPoint.ToString()}");
        }
        catch (Exception e)
        {
            // TODO : 알림창 띄우고 종료
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
    /// 서버와의 연결을 해제함
    /// </summary>
    public void Disconnect()
    {
        _socket.Shutdown(SocketShutdown.Both);
        _socket.Close();
    }

    /// <summary>
    /// 서버에게 비동기 형식으로 send를 함
    /// </summary>
    /// <param name="sendBuff">보낼 버퍼</param>
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
    /// 외부에 공개되지 않고 자동으로 처리됨
    /// </para>
    /// Send 완료 후 후처리
    /// </summary>
    /// <param name="sender">이벤트를 보낸 주체</param>
    /// <param name="args">이벤트의 종류 및 자세한 정보</param>
    void OnSendCompleted(object sender, SocketAsyncEventArgs args)
    {
        if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
        {

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
        {
            OnRecvCompleted(null, _recvArgs);
        }
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
