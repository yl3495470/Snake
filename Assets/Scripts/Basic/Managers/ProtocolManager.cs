using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Basic.Net.Sockets;
using Basic.Net.Protocol;

using Basic.Interfaces;

namespace Basic.Managers
{
    public class ProtocolManager
    {
        private List<CmdHandler> _sendHandler = new List<CmdHandler>();
        private List<CmdIndexHandler> _receiveHandler = new List<CmdIndexHandler>();
        private CmdHandler _errorCodeHandler;

        private IDictionary<int, ICommand> _commands = new Dictionary<int, ICommand>();
        private List<int> _lockList = new List<int>();
        public IProtocolSender offlineSender = null;

        private static ProtocolManager _instance = new ProtocolManager();
        public static ProtocolManager instance
        {
            get
            {
                return _instance;
            }
        }

        public ProtocolManager()
        {
            if (_instance != null)
            {
                throw new UnityException("Error: Please use instance to get ProtocalManager.");
            }
        }

        public void RegistSendHandler(CmdHandler sendHandler)
        {
            _sendHandler.Add(sendHandler);
        }

        public void RegistReceiveHandler(CmdIndexHandler receiveHandler)
        {
            _receiveHandler.Add(receiveHandler);
        }

        public void RegistErrorHandler(CmdHandler errorHandler)
        {
            _errorCodeHandler = errorHandler;
        }

        public void Init(System.Reflection.Assembly assembly)
        {
            _Inject(assembly);
        }

        private void _Inject(System.Reflection.Assembly assembly)
        {
            //Type[] types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
            Type[] types = assembly.GetTypes();
            foreach (Type t in types)
            {
                CmdAttribute attribute = Attribute.GetCustomAttribute(t, typeof(CmdAttribute)) as CmdAttribute;
                if (attribute != null)
                {
                    ICommand command = t.Assembly.CreateInstance(t.FullName) as ICommand;
                    if (command != null)
                    {
                        AddCommand(command);
                    }
                }
            }
        }

        public void AddCommand(ICommand command)
        {
            if (_commands.ContainsKey(command.code))
            {
                Debug.Log("协议号:[" + command.code.ToString() + ", " + command.desc + "] 已存在 ");
                return;
            }
            _commands[command.code] = command;
        }

        public static void AddListener(int code, EventListener handler)
        {
            _instance.AddProtocolListent<EventListener>(code, handler);
        }

        public static void AddListener<T>(int code, T handler)
        {
            _instance.AddProtocolListent<T>(code, handler);
        }

        public void AddProtocolListent<T>(int code, T handler)
        {
            ICommand command = _GetCommand(code);

            if (command != null)
            {
                command.AddProtocolEventListent(handler as Delegate);
            }
        }

        public void AddProtocolListent(int code, EventListener handler)
        {
            AddProtocolListent<EventListener>(code, handler);
        }

        public static void RemoveListener(int code, EventListener handler)
        {
            _instance.RemoveProtocolListent<EventListener>(code, handler);
        }

        public static void RemoveListener<T>(int code, T handler)
        {
            _instance.RemoveProtocolListent<T>(code, handler);
        }

        public void RemoveProtocolListent<T>(int code, T handler)
        {
            ICommand command = _GetCommand(code);

            if (command != null)
            {
                command.RemoveProtocolEventListent(handler as Delegate);
            }
        }

        public void RemoveProtocolListent(int code, EventListener handler)
        {
            RemoveProtocolListent<EventListener>(code, handler);
        }

        public static void Send(int code, object data = null)
        {
            _instance.SendMessage(code, data);
        }

        /// <summary>
        /// 出包;
        /// </summary>
        public void SendMessage(int code, object data = null)
        {
            if (offlineSender != null)
            {
                offlineSender.Send(code, data);
                return;
            }

            //如果没有自定义发送器，采用默认方式发送;
            ICommand command = _GetCommand(code);
            if (command != null && command.needWaitResponse)
            {
                if (_lockList.Exists(c => c == code))
                {
                    Debug.Log("协议[" + code.ToString() + "," + command.desc + "]暂时锁定");
                    return;
                }
            }

            if (command != null)
            {
                Debug.Log("Socket send: [" + code.ToString() + "," + command.desc + "]");
                command.WritePackage(data);
                for (int i = 0; i < _sendHandler.Count; i++)
                {
                    _sendHandler[i](code);
                }
            }
        }

        /// <summary>
        /// 收包
        /// </summary>
        public void SendNotifyListener(int code, int pkgIndex, params object[] parameters)
        {
            ICommand command = _GetCommand(code);

            if (command != null)
            {
                command.Notify(parameters);

                for (int i = 0; i < _receiveHandler.Count; i++)
                {
                    _receiveHandler[i](code, pkgIndex);
                }
                if (command.needShowErrorCode && (int)parameters[0] != 0 && _errorCodeHandler != null)
                {
                    _errorCodeHandler((int)parameters[0]);
                }
            }
        }

        public void ClearLock()
        {
            _lockList.Clear();
        }

        public bool ReadMessage(int code, PackageIn pkg)
        {

            ICommand command = _GetCommand(code);
            ClearLock();
            if (command != null)
            {
                Debug.Log("Socket Receive: [0x" + code.ToString() + "," + command.desc + "," + BitConverter.ToString(pkg.GetByteArray(0, pkg.length)) + "]");
                command.ReadPackage(pkg);
                return true;
            }
            return false;
        }

        private ICommand _GetCommand(int code)
        {
            if (_commands.ContainsKey(code))
            {
                return _commands[code];
            }
            return null;
        }
    }
}