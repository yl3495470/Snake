using Basic.Managers;
using System;

namespace Basic.Net.Sockets
{
    public class SocketEvent
    {
        public const string SOCKET_DATA = "SOCKET_DATA";
        public const string SOCKET_CONNECT = "SOCKET_CONNECT";
        public const string SOCKET_CLOSE = "SOCKET_CLOSE";
        public const string SOCKET_ERROR = "SOCKET_ERROR";
        public const string SOCKET_STATE = "SOCKET_STATE";
    }
    
    class ByteUtil
    {
        /// <summary>
        /// Converts a byte array into a hex dump
        /// </summary>
        /// <param name="description">Dump description</param>
        /// <param name="dump">byte array</param>
        /// <param name="start">dump start offset</param>
        /// <param name="count">dump bytes count</param>
        /// <returns>the converted hex dump</returns>
        public static string ToHexDump(string description, byte[] dump, int start, int count)
        {
            string hexDump = "";
            if (description != null)
            {
                hexDump += description;
                hexDump += "\n";
            }
            int end = start + count;
            for (int i = start; i < end; i += 16)
            {
                string text = "";
                string hex = "";

                for (int j = 0; j < 16; j++)
                {
                    if (j + i < end)
                    {
                        long val = dump[j + i];
                        if (val < 16)
                        {
                            hex += "0" + System.Convert.ToString(val, 16) + " ";
                        }
                        else
                        {
                            hex += System.Convert.ToString(val, 16) + " ";
                        }

                        if (val >= 32 && val <= 127)
                        {
                            text += (char)val;
                        }
                        else
                        {
                            text += ".";
                        }
                    }
                    else
                    {
                        hex += "   ";
                        text += " ";
                    }
                }
                hex += "  ";
                hex += text;
                hex += '\n';
                hexDump += hex;
            }
            return hexDump;
        }
    }

    public class ByteSocket
    {
        //public const int MAX_LENGTH = 40960;
        private NetClient _socket;
        private String _ip;
        private int _port;

        private bool _encrypted = false;  	//是否加密

        //默认key
        private static byte[] KEY = { 0xae, 0xbf, 0x56, 0x78, 0xab, 0xcd, 0xef, 0xf1 };

        public static byte[] RECEIVE_KEY;	//接收key
        public static byte[] SEND_KEY;		//发送key

        private byte[] _readBuffer;   		//读缓冲
        private int _readOffset;		 	//读索引
        private int _writeOffset;        	//写索引
        private byte[] _pkgLength;   		//协议头
        private PackageIn _pkgIn;           //收到的包

        //private FSM _send_fsm;
        //private FSM _receive_fsm;

        public ByteSocket(bool encrypted = false)
        {
            //_send_fsm = new FSM(0x7abcdef7,1501);
            //_receive_fsm = new FSM(0x7abcdef7,1501);

            _readBuffer = new byte[NetBase.MAX_LENGTH_EACH_RECEIVE];
            _pkgLength = new byte[4];
            _encrypted = encrypted;
            SetKey(KEY);
        }

        public void SetKey(byte[] key)
        {
            RECEIVE_KEY = new byte[8];
            SEND_KEY = new byte[8];
            for (int i = 0; i < 8; i++)
            {
                RECEIVE_KEY[i] = key[i];
                SEND_KEY[i] = key[i];
            }
        }

        public void ResetKey()
        {
            SetKey(KEY);
        }
        
        public bool Connected
        {
            get { return _socket != null && _socket.State == SocketState.Connected; }
        }

        public bool IsSame(String ip, int port)
        {
            return _ip == ip && _port == port;
        }

        public void Send(PackageOut pkg)
        {
            if (Connected)
            {
                pkg.Pack();
                if (_encrypted)
                {
                    for (int i = 0; i < pkg.length; i++)
                    {
                        if (i > 0)
                        {
                            SEND_KEY[i % 8] = (byte)((SEND_KEY[i % 8] + pkg.Get(i - 1)) ^ i);
                            pkg.Set(i, (byte)((pkg.Get(i) ^ SEND_KEY[i % 8]) + pkg.Get(i - 1)));
                        }
                        else
                        {
                            pkg.Set(0, (byte)(pkg.Get(0) ^ SEND_KEY[0]));
                        }
                    }
                }
                _socket.Send(pkg.ToByteArray());
            }
        }

        public void SendString(string data)
        {
            if (Connected)
            {
                _socket.Send(System.Text.Encoding.Default.GetBytes(data));
            }
        }

        public void Connect(string ip, int port)
        {
            try
            {
                if (_socket != null)
                {
                    Close(false);
                }
                _socket = new NetClient();
                AddEvent();
                _ip = ip;
                _port = port;
                _readOffset = 0;
                _writeOffset = 0;

                _socket.Connect(new System.Net.IPEndPoint(System.Net.IPAddress.Parse(ip), port));
            }
            catch (Exception error)
            {
                EventManager.instance.DispatchEvent(this, SocketEvent.SOCKET_ERROR, error.Message);
            }
        }

        public void Close(bool isEvent = true)
        {
            if (_socket != null)
            {
                RemoveEvent();
                if (_socket.State == Basic.Net.Sockets.SocketState.Connected)
                {
                    try
                    {
                        _socket.Close("Just Close");
                        _socket = null;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                if (isEvent)
                {
                    EventManager.instance.DispatchEvent(this, SocketEvent.SOCKET_CLOSE);
                }
            }
        }

        private void AddEvent()
        {
            _socket.Connected += _ConnectHandler;
            _socket.DataArrived += _DataHandler;
            _socket.Disconnected += _CloseHandler;
            _socket.ErrorReceived += _ErrorHandler;
            _socket.StateChanged += _StateChangedHandler;
        }

        private void RemoveEvent()
        {
            _socket.Connected -= _ConnectHandler;
            _socket.DataArrived -= _DataHandler;
            _socket.Disconnected -= _CloseHandler;
            _socket.ErrorReceived -= _ErrorHandler;
            _socket.StateChanged -= _StateChangedHandler;
        }

        private void _ConnectHandler(NetSocketConnectedEventArgs e)
        {
            try
            {
                //_send_fsm.Reset();
                //_receive_fsm.Reset();
                //_send_fsm.Setup(0x7abcdef7,1501);
                //_receive_fsm.Setup(0x7abcdef7,1501);
                EventManager.instance.DispatchEvent(this, SocketEvent.SOCKET_CONNECT);
            }
            catch (Exception error)
            {
                Console.WriteLine(error.StackTrace);
            }
        }

        private void _CloseHandler(NetSocketDisconnectedEventArgs e)
        {
            try
            {
                RemoveEvent();
                EventManager.instance.DispatchEvent(this, SocketEvent.SOCKET_CLOSE);
            }
            catch (Exception error)
            {
                Console.WriteLine(error.StackTrace);
            }
        }

        private void _DataHandler(NetSockDataArrivalEventArgs e)
        {
            if (e.size > 0)
            {
                int len = e.size;
                lock (_readBuffer)
                {
                    _BlockCopy(e.Data, 0, _readBuffer, _writeOffset, len);
                }
                _writeOffset += len;
                if (_writeOffset > _readOffset)
                {
                    ReadPackage();
                }
            }
        }

        private void _ErrorHandler(NetSockErrorReceivedEventArgs e)
        {
            try
            {
                EventManager.instance.DispatchEvent(this, SocketEvent.SOCKET_ERROR, e.Function + ":" + e.Exception.Message);
            }
            catch (Exception error)
            {
                Console.WriteLine(error.StackTrace);
            }
        }

        private void _StateChangedHandler(NetSockStateChangedEventArgs e)
        {
            try
            {
                EventManager.instance.DispatchEvent(this, SocketEvent.SOCKET_STATE, e.NewState);
            }
            catch (Exception error)
            {
                Console.WriteLine(error.StackTrace);
            }
        }

        private void ReadPackage()
        {
            int dataAvailable = _writeOffset - _readOffset;
            if (_pkgIn != null)
            {
                if (_pkgIn.size <= _pkgIn.inSize + dataAvailable)
                {
                    int lack = _pkgIn.size - _pkgIn.inSize;
                    _pkgIn.Load(_readBuffer, _readOffset, lack);
                    _readOffset += lack;
                    dataAvailable = _writeOffset - _readOffset;
                    HandlePackage(_pkgIn);
                    _pkgIn = null;
                }
                else
                {
                    _pkgIn.Load(_readBuffer, _readOffset, dataAvailable);
                    _readOffset += dataAvailable;
                    dataAvailable = 0;
                    HandlePackage(_pkgIn);
                }
            }
            while (dataAvailable >= PackageIn.HEADER_SIZE)
            {
                int len = 0;
                for (int i = 0; i < 4; i++) _pkgLength[i] = _readBuffer[_readOffset + i];
                if (BitConverter.IsLittleEndian)
                {
                    len = BitConverter.ToInt32(EdianConvert.GetConvertEdian(_pkgLength), 0);
                }
                else
                {
                    len = BitConverter.ToInt32(_pkgLength, 0);
                }
                /*
                dataAvailable = _writeOffset - _readOffset;
                if (dataAvailable < len || len == 0)
                {
                    break;
                }
                */
                _pkgIn = new PackageIn();
                if (len > dataAvailable)
                {
                    _pkgIn.Load(_readBuffer, _readOffset, dataAvailable);
                    _readOffset += dataAvailable;
                }
                else
                {
                    _pkgIn.Load(_readBuffer, _readOffset, len);
                    _readOffset += len;
                }
                dataAvailable = _writeOffset - _readOffset;
                HandlePackage(_pkgIn);
                if (_pkgIn.size <= _pkgIn.inSize) _pkgIn = null;
            }

            if (dataAvailable > 0)
            {
                lock (_readBuffer)
                {
                    _BlockCopy(_readBuffer, _readOffset, _readBuffer, 0, dataAvailable);
                }
            }
            _readOffset = 0;
            _writeOffset = dataAvailable;
        }

        private void _BlockCopy(byte[] src, int srcOffset, byte[] dst, int dstOffset, int len)
        {
            if (len > 8)
            {
                Buffer.BlockCopy(src, srcOffset, dst, dstOffset, len);
            }
            else
            {
                for (int i = 0; i < len; i++)
                {
                    dst[dstOffset + i] = src[srcOffset + i];
                }
            }
        }

        private void HandlePackage(PackageIn pkg)
        {
            try
            {
                EventManager.Dispatch(this, SocketEvent.SOCKET_DATA, pkg);
            }
            catch (Exception error)
            {
                Console.WriteLine("handlePackage: " + error.StackTrace);
            }
        }

        private void TracePkg(byte[] src, string des, int len = -1)
        {
            string str = des;
            int l = len < 0 ? src.Length : len;
            for (int i = 0; i < l; i++)
            {
                str += src[i].ToString() + ", ";
            }
            Console.WriteLine(str);
        }

        private void TraceBytes(byte[] bytes)
        {
            string str = "[";
            for (int i = 0; i < bytes.Length; i++)
            {
                str += (bytes[i].ToString() + " ");
            }
            str += "]";
            Console.WriteLine(str);
        }

        public void Dispose()
        {
            if (_socket.State == SocketState.Connected)
                _socket.Close("Dispose");
            _socket = null;
        }
    }
}
