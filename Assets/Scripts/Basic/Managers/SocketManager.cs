
using Basic.Net.Protocol;
using Basic.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Basic.Managers
{
    public class SocketManager
    {
        private ByteSocket _socket;	//底层socket	
        private string _ip;		//ip地址
        private int _port;		//端口号		

        private static SocketManager _instance = new SocketManager();
        public static SocketManager instance
        {
            get { return _instance; }
        }

        public ByteSocket socket
        {
            get { return _socket; }
        }

        public SocketManager()
        {
            if (_instance != null)
            {
                throw new UnityEngine.UnityException("Error: Please use instance to get SocketManager.");
            }
        }

        public void Init()
        {
            NetManager.sendFunction = SocketManager.instance.SendPackage;

            _socket = new ByteSocket(false);
            EventManager.instance.AddEventListener(_socket, SocketEvent.SOCKET_CONNECT, _SocketConnected);
            EventManager.instance.AddEventListener(_socket, SocketEvent.SOCKET_CLOSE, _SocketClose);
            EventManager.instance.AddEventListener(_socket, SocketEvent.SOCKET_DATA, _SocketData);
            EventManager.instance.AddEventListener(_socket, SocketEvent.SOCKET_ERROR, _SocketError);
        }

        public void SendMessage(int type, object data = null)
        {
            ProtocolManager.instance.SendMessage(type, data);
        }

        public void Connect(string ip, int port)
        {
            _socket.Connect(ip, port);
            _ip = ip;
            _port = port;
        }

        public void Close()
        {
            _socket.Close();
        }

        public void Close(bool isEvent)
        {
            _socket.Close(isEvent);
        }

        public void Reconnect()
        {
            _socket.ResetKey();
            _socket.Connect(_ip, _port);
        }

        private void _SocketClose(params object[] args)
        {
            EventManager.instance.DispatchEvent(SocketManager.instance, SocketEvent.SOCKET_CLOSE);
        }

        private void _SocketConnected(params object[] args)
        {
            EventManager.instance.DispatchEvent(SocketManager.instance, SocketEvent.SOCKET_CONNECT);
        }

        private void _SocketError(params object[] args)
        {
            EventManager.instance.DispatchEvent(SocketManager.instance, SocketEvent.SOCKET_ERROR);
        }

        private void _SocketData(params object[] args)
        {
            PackageIn pkg = args[0] as PackageIn;
            ProtocolManager.instance.ReadMessage(pkg.code, pkg);
        }

        public void SendPackage(params object[] args)
        {
            PackageOut pkg = args[0] as PackageOut;
            _socket.Send(pkg);

            Debug.Log("Socket SendPackage: [" + pkg.code.ToString() + "," + BitConverter.ToString(pkg.GetByteArray(0, pkg.length)) + "]");
        }

        public int port
        {
            get { return _port; }
            set { _port = value; }
        }

        public string ip
        {
            get { return _ip; }
            set { _ip = value; }
        }
    }
}