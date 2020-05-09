using Basic.Net.Protocol;
using System;
using System.Collections.Generic;

namespace Basic.Net.Sockets
{
    public class PackageIn : ByteBuffer
    {
        public const int HEADER_SIZE = 16;

        /// <summary>
        /// 包大小
        /// </summary>
        private int _size;
        public int size { get { return _size; } }
        /// <summary>
        /// 入包大小
        /// </summary>
        private int _inSize;
        public int inSize { get { return _inSize; } }
        /// <summary>
        /// 协议号
        /// </summary>
        private int _code;
        /// <summary>
        /// 错误码
        /// </summary>
        private int _errCode;
        public int pkgIndex { get { return _pkgIndex; } }
        private int _pkgIndex;
        private bool _isHeaderRead = false;
        private List<int> _indexs = new List<int>();
        private object _result = null;
        private bool _isComplete = false;
        public bool isComplete { get { return _isComplete; } }

        public PackageIn(int code = 0)
        {
            _code = code;
        }

        public void Load(byte[] src, int index, int len)
        {
            if (_size > 0)
            {
                _inSize += len;
                lock (_tempBytes)
                {
                    _PushByteArray(src, index, len);
                }
            }
            else
            {
                _inSize = len;
                _PushByteArray(src, index, len);
                this.position = 0;
                _ReadHeader();
            }
        }

        private void _ReadHeader()
        {
            _size = PopInt();
            _code = PopInt();
            _errCode = PopInt();
            _pkgIndex = PopInt();
        }

        public int code
        {
            get { return _code; }
        }

        public int errCode
        {
            get { return _errCode; }
        }

        public bool HasBody()
        {
            return _size > HEADER_SIZE;
        }

        public object ConvertObject(Type type)
        {
            _isComplete = PackageConverter.StreamToObject(this, type, _indexs, _result, out _result);
			if (_isComplete || _inSize >= _size) return _result;
            _Trim();
            return null;
        }

        private void _Trim()
        {
            int move = _length - _position;
            lock (_tempBytes)
            {
                _BlockCopy(_tempBytes, _position, _tempBytes, 0, move);
            }
            _length = move;
            _position = 0;
        }

        public bool CanPopByte()
        {
            return _length > _position;
        }

        public bool CanPopShort()
        {
            return _length > _position + 1;
        }

        public bool CanPopInt()
        {
            return _length > _position + 3;
        }

        public bool CanPopLong()
        {
            return _length > _position + 7;
        }

        public bool CanPopUTF()
        {
            if (CanPopShort())
            {
                short len = PopShort();
                _position -= 2;
                return CanPopByteArray(len + 2);
            }
            return false;
        }

        public bool CanPopByteArray(int length)
        {
            return _length >= _position + length;
        }
    }
}
