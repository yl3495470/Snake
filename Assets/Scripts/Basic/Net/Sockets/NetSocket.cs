using System;
using System.Collections.Generic;
using System.Text;

using System.Net;
using System.Net.Sockets;

using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Basic.Net.Sockets
{
    #region Enums
    public enum SocketState
    {
        Closed,
        Closing,
        Connected,
        Connecting,
        Listening,
    }
    #endregion

    public class EdianConvert
    {
        public static byte[] GetConvertEdian(byte[] data)
        {
            int len = data.Length;
            byte[] result = new byte[len];
            for (int i = 0; i < len; i++)
            {
                result[i] = data[len - i - 1];
            }
            return result;
        }
    }

    #region Event Args
    public class NetSocketConnectedEventArgs
    {
        public IPAddress SourceIP;
        public NetSocketConnectedEventArgs(IPAddress ip)
        {
            this.SourceIP = ip;
        }
    }

    public class NetSocketDisconnectedEventArgs
    {
        public string Reason;
        public NetSocketDisconnectedEventArgs(string reason)
        {
            this.Reason = reason;
        }
    }

    public class NetSockStateChangedEventArgs
    {
        public SocketState NewState;
        public SocketState PrevState;
        public NetSockStateChangedEventArgs(SocketState newState, SocketState prevState)
        {
            this.NewState = newState;
            this.PrevState = prevState;
        }
    }

    public class NetSockDataArrivalEventArgs
    {
        public byte[] Data;
        public int size;
        public NetSockDataArrivalEventArgs(byte[] data, int size)
        {
            this.Data = data;
            this.size = size;
        }
    }

    public class NetSockErrorReceivedEventArgs
    {
        public string Function;
        public Exception Exception;
        public NetSockErrorReceivedEventArgs(string function, Exception ex)
        {
            this.Function = function;
            this.Exception = ex;
        }
    }

    public class NetSockConnectionRequestEventArgs
    {
        public Socket Client;
        public NetSockConnectionRequestEventArgs(Socket client)
        {
            this.Client = client;
        }
    }
    #endregion

    #region Socket Classes
    public abstract class NetBase
    {
        public const int MAX_LENGTH_EACH_RECEIVE = 409600;
        public const int WAIT_EACH_RECEIVE = 5;

        #region Fields
        /// <summary>Current socket state</summary>
        protected SocketState state = SocketState.Closed;
        /// <summary>The socket object, obviously</summary>
        protected Socket socket;

        /// <summary>Keep track of when data is being sent</summary>
        protected bool isSending = false;
		/// <summary>Is Robort is Ready</summary>
		protected bool isReady = false;
		/// <summary>Is Getting Position</summary>
		protected bool isRequestPos = false;
		/// <summary>Now joint Pos </summary>
		protected float[] jointPos = new float[3];
        /// <summary>Queue of objects to be sent out</summary>
        protected Queue<byte[]> sendBuffer = new Queue<byte[]>();

        /// <summary>Store incoming bytes to be processed</summary>
        protected byte[] byteBuffer = new byte[MAX_LENGTH_EACH_RECEIVE];

        /// <summary>TCP inactivity before sending keep-alive packet (ms)</summary>
        protected uint KeepAliveInactivity = 500;
        /// <summary>Interval to send keep-alive packet if acknowledgement was not received (ms)</summary>
        protected uint KeepAliveInterval = 100;

        /// <summary>Threaded timer checks if socket is busted</summary>
        protected Timer connectionTimer;
        /// <summary>Interval for socket checks (ms)</summary>
        protected int ConnectionCheckInterval = 1000;
        #endregion

        #region Public Properties
        /// <summary>Current state of the socket</summary>
        public SocketState State { get { return this.state; } }

        /// <summary>Port the socket control is listening on.</summary>
        public int LocalPort
        {
            get
            {
                try
                {
                    return ((IPEndPoint)this.socket.LocalEndPoint).Port;
                }
                catch
                {
                    return -1;
                }
            }
        }

        /// <summary>IP address enumeration for local computer</summary>
        public static string[] LocalIP
        {
            get
            {
                IPHostEntry h = Dns.GetHostEntry(Dns.GetHostName());
                List<string> s = new List<string>(h.AddressList.Length);
                foreach (IPAddress i in h.AddressList)
                    s.Add(i.ToString());
                return s.ToArray();
            }
        }
        #endregion

        #region Events
        /// <summary>Socket is connected</summary>
        public Action<NetSocketConnectedEventArgs> Connected;
        /// <summary>Socket connection closed</summary>
        public Action<NetSocketDisconnectedEventArgs> Disconnected;
        /// <summary>Socket state has changed</summary>
        /// <remarks>This has the ability to fire very rapidly during connection / disconnection.</remarks>
        public Action<NetSockStateChangedEventArgs> StateChanged;
        /// <summary>Recived a new object</summary>
        public Action<NetSockDataArrivalEventArgs> DataArrived;
        /// <summary>An error has occurred</summary>
        public Action<NetSockErrorReceivedEventArgs> ErrorReceived;
		/// <summary>Recived a message</summary>
		public Action<bool> GetOnceMessage;

        #endregion

        #region Constructor
        /// <summary>Base constructor sets up buffer and timer</summary>
        public NetBase()
        {
            this.connectionTimer = new Timer(
                new TimerCallback(this.connectedTimerCallback),
                null, Timeout.Infinite, Timeout.Infinite);
        }
        #endregion

        #region Send
        /// <summary>Send data</summary>
        /// <param name="bytes">Bytes to send</param>
		public virtual void Send(byte[] data)
        {
            try
            {
                if (data == null)
                    throw new NullReferenceException("data cannot be null");
                else if (data.Length == 0)
                    throw new NullReferenceException("data cannot be empty");
                else
                {
                    lock (this.sendBuffer)
                    {
                        this.sendBuffer.Enqueue(data);
                    }

                    if (!this.isSending)
                    {
                        this.isSending = true;
                        this.SendNextQueued();
                    }
                }
            }
            catch (Exception ex)
            {
				
                this.OnErrorReceived("Send", ex);
            }
        }

        /// <summary>Send data for real</summary>
        private void SendNextQueued()
        {
            try
            {
                lock (this.sendBuffer)
                {
                    if (this.sendBuffer.Count == 0)
                    {
                        this.isSending = false;
                        return; // nothing more to send
                    }

                    byte[] data = this.sendBuffer.Dequeue();
                    this.socket.Send(data, SocketFlags.None);
                    this.SendNextQueued();
                    //this.socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(this.SendCallback), this.socket);
                }
            }
            catch (Exception ex)
            {
				UnityEngine.Debug.Log (ex.ToString ());
                this.OnErrorReceived("Sending", ex);
            }
        }

        /// <summary>Callback for BeginSend</summary>
        /// <param name="ar"></param>
        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket sock = (Socket)ar.AsyncState;
                sock.EndSend(ar);
                //int didSend = sock.EndSend(ar);

                if (this.socket != sock)
                {
                    this.Close("Async Connect Socket mismatched");
                    return;
                }

                this.SendNextQueued();
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (SocketException ex)
            {
				UnityEngine.Debug.Log (ex.ToString ());
                if (ex.SocketErrorCode == SocketError.ConnectionReset)
                    this.Close("Remote Socket Closed");
                else
                    throw;
            }
            catch (Exception ex)
            {
				UnityEngine.Debug.Log (ex.ToString ());
                this.Close("Socket Send Exception");
                this.OnErrorReceived("Socket Send", ex);
            }
        }
        #endregion

        #region Close
        /// <summary>Disconnect the socket</summary>
        /// <param name="reason"></param>
        public void Close(string reason)
        {
            try
            {
                if (this.state == SocketState.Closing || this.state == SocketState.Closed)
                    return; // already closing/closed

                this.OnChangeState(SocketState.Closing);

                if (this.socket != null)
                {
                    this.socket.Close();
                    this.socket = null;
                }
            }
            catch (Exception ex)
            {
                this.OnErrorReceived("Close", ex);
            }

            try
            {
                lock (this.sendBuffer)
                {
                    this.sendBuffer.Clear();
                    this.isSending = false;
                }
                this.OnChangeState(SocketState.Closed);
                if (this.Disconnected != null)
                    this.Disconnected(new NetSocketDisconnectedEventArgs(reason));
            }
            catch (Exception ex)
            {
                this.OnErrorReceived("Close Cleanup", ex);
            }
        }
        #endregion

        #region Receive
        /// <summary>Receive data asynchronously</summary>
        protected void Receive()
        {
            try
            {
                Thread thread = new Thread(new ThreadStart(_OnReceive));
                thread.IsBackground = true;
                thread.Start();
                //this.socket.BeginReceive(this.byteBuffer, 0, this.byteBuffer.Length, SocketFlags.None, new AsyncCallback(this.ReceiveCallback), this.socket);
            }
            catch (Exception ex)
            {
				UnityEngine.Debug.Log (ex.ToString ());
                this.OnErrorReceived("Receive", ex);
            }
        }

        void _OnReceive()
        {
            try
            {
                while (this.state == SocketState.Connected)
                {
                    if (this.socket.Available > 0)
                    {
                        int size = this.socket.Receive(byteBuffer);
                        if (this.DataArrived != null && size > 0)
                            this.DataArrived(new NetSockDataArrivalEventArgs(byteBuffer, size));
                    }
                    Thread.Sleep(WAIT_EACH_RECEIVE);
                }
            }
            catch (ObjectDisposedException)
            {
                return; // socket disposed, let it die quietly
            }
            catch (SocketException ex)
            {
				UnityEngine.Debug.Log (ex.ToString ());
                if (ex.SocketErrorCode == SocketError.ConnectionReset)
                    this.Close("Remote Socket Closed");
                else
                    throw;
            }
            catch (Exception ex)
            {
                this.Close("Socket Receive Exception");
                this.OnErrorReceived("Socket Receive", ex);
            }
        }
        /*
        /// <summary>Callback for BeginReceive</summary>
        /// <param name="ar"></param>
        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                Socket sock = (Socket)ar.AsyncState;
                int size = sock.EndReceive(ar);

                if (this.socket != sock)
                {
                    this.Close("Async Receive Socket mismatched");
                    return;
                }

                if (size < 1)
                {
                    this.Close("No Bytes Received");
                    return;
                }
                if (this.DataArrived != null)
                    this.DataArrived(new NetSockDataArrivalEventArgs(byteBuffer, size));
                this.socket.BeginReceive(this.byteBuffer, 0, this.byteBuffer.Length, SocketFlags.None, new AsyncCallback(this.ReceiveCallback), this.socket);
            }
            catch (ObjectDisposedException)
            {
                return; // socket disposed, let it die quietly
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.ConnectionReset)
                    this.Close("Remote Socket Closed");
                else
                    throw;
            }
            catch (Exception ex)
            {
                this.Close("Socket Receive Exception");
                this.OnErrorReceived("Socket Receive", ex);
            }
        }
        */
        #endregion

        #region OnEvents
        protected void OnErrorReceived(string function, Exception ex)
        {
            if (this.ErrorReceived != null)
                this.ErrorReceived(new NetSockErrorReceivedEventArgs(function, ex));
        }

        protected void OnConnected(Socket sock)
        {
            if (this.Connected != null)
                this.Connected(new NetSocketConnectedEventArgs(((IPEndPoint)sock.RemoteEndPoint).Address));
        }

        protected void OnChangeState(SocketState newState)
        {
            SocketState prev = this.state;
            this.state = newState;
            if (this.StateChanged != null)
                this.StateChanged(new NetSockStateChangedEventArgs(this.state, prev));

            if (this.state == SocketState.Connected)
                this.connectionTimer.Change(0, this.ConnectionCheckInterval);
            else if (this.state == SocketState.Closed)
                this.connectionTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }
        #endregion

        #region Keep-alives
        /*
		 * Note about usage of keep-alives
		 * The TCP protocol does not successfully detect "abnormal" socket disconnects at both
		 * the client and server end. These are disconnects due to a computer crash, cable 
		 * disconnect, or other failure. The keep-alive mechanism built into the TCP socket can
		 * detect these disconnects by essentially sending null data packets (header only) and
		 * waiting for acks.
		 */

        /// <summary>Structure for settings keep-alive bytes</summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct tcp_keepalive
        {
            /// <summary>1 = on, 0 = off</summary>
            public uint onoff;
            /// <summary>TCP inactivity before sending keep-alive packet (ms)</summary>
            public uint keepalivetime;
            /// <summary>Interval to send keep-alive packet if acknowledgement was not received (ms)</summary>
            public uint keepaliveinterval;
        }

        /// <summary>Set up the socket to use TCP keep alive messages</summary>
        /*
        protected void SetKeepAlive()
        {
            try
            {
                tcp_keepalive sioKeepAliveVals = new tcp_keepalive();
                sioKeepAliveVals.onoff = (uint)1; // 1 to enable 0 to disable
                sioKeepAliveVals.keepalivetime = this.KeepAliveInactivity;
                sioKeepAliveVals.keepaliveinterval = this.KeepAliveInterval;

                IntPtr p = Marshal.AllocHGlobal(Marshal.SizeOf(sioKeepAliveVals));
                Marshal.StructureToPtr(sioKeepAliveVals, p, true);
                byte[] inBytes = new byte[Marshal.SizeOf(sioKeepAliveVals)];
                Marshal.Copy(p, inBytes, 0, inBytes.Length);
                Marshal.FreeHGlobal(p);

                byte[] outBytes = BitConverter.GetBytes(0);
                this.socket.IOControl(IOControlCode.KeepAliveValues, inBytes, outBytes);
                //this.socket.IOControl(unchecked((int)IOControlCode.KeepAliveValues), inBytes, outBytes);
            }
            catch (Exception ex)
            {
                this.OnErrorReceived("Keep Alive", ex);
            }
        }
        */
        protected void SetKeepAlive()
        {
            ulong keepalive_time = KeepAliveInactivity;
            ulong keepalive_interval = KeepAliveInterval;
            int bytes_per_long = 32 / 8;
            byte[] keep_alive = new byte[3 * bytes_per_long];
            ulong[] input_params = new ulong[3];
            int i1;
            int bits_per_byte = 8;

            if (keepalive_time == 0 || keepalive_interval == 0)
                input_params[0] = 0;
            else
                input_params[0] = 1;
            input_params[1] = keepalive_time;
            input_params[2] = keepalive_interval;
            for (i1 = 0; i1 < input_params.Length; i1++)
            {
                keep_alive[i1 * bytes_per_long + 3] = (byte)(input_params[i1] >> ((bytes_per_long - 1) * bits_per_byte) & 0xff);
                keep_alive[i1 * bytes_per_long + 2] = (byte)(input_params[i1] >> ((bytes_per_long - 2) * bits_per_byte) & 0xff);
                keep_alive[i1 * bytes_per_long + 1] = (byte)(input_params[i1] >> ((bytes_per_long - 3) * bits_per_byte) & 0xff);
                keep_alive[i1 * bytes_per_long + 0] = (byte)(input_params[i1] >> ((bytes_per_long - 4) * bits_per_byte) & 0xff);
            }
            this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, keep_alive);
        }
        #endregion

        #region Connection Sanity Check
        private void connectedTimerCallback(object sender)
        {
            try
            {
                if (this.state == SocketState.Connected &&
                    (this.socket == null || !this.socket.Connected))
                    this.Close("Connect Timer");
            }
            catch (Exception ex)
            {
                this.OnErrorReceived("ConnectTimer", ex);
                this.Close("Connect Timer Exception");
            }
        }
        #endregion
    }

    public class NetServer : NetBase
    {
        #region Events
        /// <summary>A socket has requested a connection</summary>
        public Action<NetSockConnectionRequestEventArgs> ConnectionRequested;

		private Socket _connectedTarget = null;
		private bool _isFirstConnected = false;

		private Thread _mThread;
		private bool _isThreadRunning = true;

        #endregion

		#region Release

		public void Release()
		{
			if (this.socket != null)
				this.socket.Close ();
			if (_mThread != null)
				_mThread.Abort ();
			if (_connectedTarget != null)
				_connectedTarget.Close ();

			_isThreadRunning = false;
		}

		#endregion

        #region Listen
        /// <summary>Listen for incoming connections</summary>
        /// <param name="port">Port to listen on</param>
        public void Listen(int port)
        {
            try
            {
                if (this.socket != null)
                {
                    try
                    {
                        this.socket.Close();
                    }
                    catch { }; // ignore problems with old socket
                }
                this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

				IPEndPoint ipLocal = new IPEndPoint(IPAddress.Parse("192.168.0.30"), port);
                this.socket.Bind(ipLocal);	

				this.socket.Listen(10); 

                this.socket.BeginAccept(new AsyncCallback(this.AcceptCallback), this.socket);
                this.OnChangeState(SocketState.Listening);
            }
			catch (SocketException ex)
			{
				UnityEngine.Debug.Log(ex.ToString());
			}
            catch (Exception ex)
            {
                this.OnErrorReceived("Listen", ex);
            }
        }
			
		public EndPoint GetRobotIPE()
		{
			return this.socket.RemoteEndPoint;
		}

		/// <summary>Callback for BeginAccept</summary>
        /// <param name="ar"></param>
        private void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                Socket listener = (Socket)ar.AsyncState;
                Socket sock = listener.EndAccept(ar);
				_connectedTarget = sock;

				if(!_isFirstConnected)
				{
					_isThreadRunning = true;
					if(_mThread != null)
						_mThread.Abort();
					_mThread = new Thread(ServiceReciveMsg);//开启接收消息线程
					_mThread.IsBackground = true;
					_mThread.Start();
					_isFirstConnected = true;
				}

				if(GetOnceMessage != null)
				{
					GetOnceMessage(true);
				}

                if (this.state == SocketState.Listening)
                {
                    if (this.socket != listener)
                    {
                        this.Close("Async Listen Socket mismatched");
                        return;
                    }

                    if (this.ConnectionRequested != null)
                        this.ConnectionRequested(new NetSockConnectionRequestEventArgs(sock));
                }

                if (this.state == SocketState.Listening)
                    this.socket.BeginAccept(new AsyncCallback(this.AcceptCallback), listener);
                else
                {
                    try
                    {
                        listener.Close();
                    }
                    catch (Exception ex)
                    {
                        this.OnErrorReceived("Close Listen Socket", ex);
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (SocketException ex)
            {
				UnityEngine.Debug.Log(ex.ToString());
                this.Close("Listen Socket Exception");
                this.OnErrorReceived("Listen Socket", ex);
            }
            catch (Exception ex)
            {
				UnityEngine.Debug.Log(ex.ToString());
                this.OnErrorReceived("Listen Socket", ex);
            }
        }
        #endregion

		#region Receive

		public void RequestPos()
		{
			isRequestPos = true;
		}

		public float GetJointPos(int targetJoint)
		{
			if (targetJoint >= jointPos.Length || targetJoint < 0)
				return 0;
			return jointPos [targetJoint];
		}

		public void ServiceReciveMsg()
		{
			while (_isThreadRunning)
			{
				//获取发送方的ip
				EndPoint point = new IPEndPoint(IPAddress.Any, 0);//用来保存发送方的ip和端口号
				//aa = "南音";
				//接收数据
				byte[] buffer2 = new byte[1024];
				try
				{
					int length2 = _connectedTarget.ReceiveFrom(buffer2, ref point);
					string message = System.Text.Encoding.Default.GetString(buffer2);
					UnityEngine.Debug.Log(message);
					if(message.Contains("$ready#"))
					{
						isReady = true;
					}	
					//获取位置,暂时还不知道他会发些什么反馈
					if(isRequestPos)
					{
						if(buffer2[0] == 13)
						{
							jointPos[0] = GetByteValue(buffer2[1]) * 10 + GetByteValue(buffer2[2]) / 10; //BitConverter.ToInt16(buffer2,1);
							jointPos[1] = GetByteValue(buffer2[3]) * 10 + GetByteValue(buffer2[4]) / 10; //BitConverter.ToInt16(buffer2,3);
							jointPos[2] = GetByteValue(buffer2[5]) * 10 + GetByteValue(buffer2[6]) / 10; //BitConverter.ToInt16(buffer2,5);
							isRequestPos = false;
						}
					}
				}
				catch (SocketException e)
				{
					UnityEngine.Debug.Log ("e.Message:" + e.Message);
					continue;
				}
			}
			if(this.socket.Connected)
				this.socket.Disconnect(true);
		}

		public float GetByteValue(byte pValue)
		{
			if (pValue > 127)
				return -(255.0f - pValue + 1);
			return (float)pValue;
		}

		#endregion

		#region Send

		public bool isReadyState()
		{
			return isReady;
		}

		/// <summary>Send data</summary>
		/// <param name="bytes">Bytes to send</param>
		public override void Send(byte[] data)
		{
			try
			{
				if (data == null)
					throw new NullReferenceException("data cannot be null");
				else if (data.Length == 0)
					throw new NullReferenceException("data cannot be empty");
				else
				{
					lock (this.sendBuffer)
					{
						this.sendBuffer.Enqueue(data);
					}

					if (!this.isSending)
					{
						this.isSending = true;
						this.SendNextQueued();
					}
				}
			}
			catch (Exception ex)
			{

				this.OnErrorReceived("Send", ex);
			}
		}

		/// <summary>Send data for real</summary>
		private void SendNextQueued()
		{
			try
			{
				lock (this.sendBuffer)
				{
					if (this.sendBuffer.Count == 0)
					{
						this.isSending = false;
						return; // nothing more to send
					}

					byte[] data = this.sendBuffer.Dequeue();
					_connectedTarget.Send(data);
					isReady = false;
					this.SendNextQueued();
//					this.socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(this.SendCallback), this.socket);
				}
			}
			catch (SocketException ex)
			{
				UnityEngine.Debug.Log (ex.ToString ());
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log (ex.ToString ());
				this.OnErrorReceived("Sending", ex);
			}
		}

		/// <summary>Callback for BeginSend</summary>
		/// <param name="ar"></param>
		private void SendCallback(IAsyncResult ar)
		{
			try
			{
				Socket sock = (Socket)ar.AsyncState;
				sock.EndSend(ar);
				//int didSend = sock.EndSend(ar);

				if (this.socket != sock)
				{
					this.Close("Async Connect Socket mismatched");
					return;
				}

				this.SendNextQueued();
			}
			catch (ObjectDisposedException)
			{
				return;
			}
			catch (SocketException ex)
			{
				UnityEngine.Debug.Log (ex.ToString ());
				if (ex.SocketErrorCode == SocketError.ConnectionReset)
					this.Close("Remote Socket Closed");
				else
					throw;
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log (ex.ToString ());
				this.Close("Socket Send Exception");
				this.OnErrorReceived("Socket Send", ex);
			}
		}
		#endregion

        #region Accept
        /// <summary>Accept the connection request</summary>
        /// <param name="client">Client socket to accept</param>
        public void Accept(Socket client)
        {
            try
            {
                if (this.state != SocketState.Listening)
                    throw new Exception("Cannot accept socket is " + this.state.ToString());

                if (this.socket != null)
                {
                    try
                    {
                        this.socket.Close(); // close listening socket
                    }
                    catch { } // don't care if this fails
                }

                this.socket = client;

                this.socket.ReceiveBufferSize = this.byteBuffer.Length;
                this.socket.SendBufferSize = this.byteBuffer.Length;

                this.SetKeepAlive();

                this.OnChangeState(SocketState.Connected);
                this.OnConnected(this.socket);

                this.Receive();
            }
            catch (Exception ex)
            {
                this.OnErrorReceived("Accept", ex);
            }
        }
        #endregion
    }

    public class NetClient : NetBase
    {
        #region Constructor
        public NetClient() : base() { }
        #endregion

        #region Connect
        /// <summary>Connect to the computer specified by Host and Port</summary>
        public void Connect(IPEndPoint endPoint)
        {
			UnityEngine.Debug.Log ("Pass");
            if (this.state == SocketState.Connected)
                return; // already connecting to something

            try
            {
                if (this.state != SocketState.Closed)
                    throw new Exception("Cannot connect socket is " + this.state.ToString());

                this.OnChangeState(SocketState.Connecting);

                if (this.socket == null)
                    this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                this.socket.BeginConnect(endPoint, new AsyncCallback(this.ConnectCallback), this.socket);
            }
            catch (Exception ex)
            {
				UnityEngine.Debug.Log(ex.ToString());
                this.OnErrorReceived("Connect", ex);
                this.Close("Connect Exception");
            }
        }

        /// <summary>Callback for BeginConnect</summary>
        /// <param name="ar"></param>
        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket sock = (Socket)ar.AsyncState;
                sock.EndConnect(ar);

                if (this.socket != sock)
                {
                    this.Close("Async Connect Socket mismatched");
                    return;
                }

                if (this.state != SocketState.Connecting)
                    throw new Exception("Cannot connect socket is " + this.state.ToString());

                this.socket.ReceiveBufferSize = this.byteBuffer.Length;
                this.socket.SendBufferSize = this.byteBuffer.Length;

                //this.SetKeepAlive();

                this.OnChangeState(SocketState.Connected);
                this.OnConnected(this.socket);

                this.Receive();
            }
            catch (Exception ex)
            {
				UnityEngine.Debug.Log(ex.ToString());
                this.Close("Socket Connect Exception");
                this.OnErrorReceived("Socket Connect", ex);
            }
        }
        #endregion
    }
    #endregion
}
