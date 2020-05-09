using Basic.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Basic.Net.Sockets;

namespace Basic.Net.Protocol
{
    public delegate void PtAction(int errCode);
    public delegate void PtAction<T>(int errCode, T down = default(T));

    public delegate void CmdHandler(int code);
    public delegate void CmdIndexHandler(int code, int pkgIndex);

    public class CmdAttribute : Attribute
    {
        private int _code;
        public int code { get { return _code; } }
        private string _desc;
        public string desc { get { return _desc; } }
        private Type _type;
        public Type type { get { return _type; } }
        private bool _needShowErrorCode;
        public bool needShowErrorCode { get { return _needShowErrorCode; } }

        //public CmdAttribute(int code, Type type, string desc = "")
        //{
        //    _code = code;
        //    _type = type;
        //    _desc = desc;
        //    _needShowErrorCode = true;
        //}

        public CmdAttribute(int code, Type type, string desc = "",bool needShow = true)
        {
            _code = code;
            _type = type;
            _desc = desc;
            _needShowErrorCode = needShow;
        }
         
    }
    
    public interface ICommand
    { 
        /**
		 * 
		 * @return 获得协议代码
		 * 
		 */
		int code{get;}
		
		/**
		 * 
		 * @return 获得协议描述
		 * 
		 */
        string desc{get;}

        bool needShowErrorCode { get; }
		
		/**
		 * 
		 * @return 获得该协议是否需要服务端反馈 
		 * 
		 */
        bool needWaitResponse{get;}
		
		/**
		 * 读取包，主要对二进制流到model的转换
		 * @param pkg 读包的结构
		 * 
		 */
        void ReadPackage(PackageIn pkg);
		
		/**
		 * 写包，主要进行model向二进制流的转换
		 * @param params  写包的参数
		 * 
		 */
        void WritePackage(object data);
		
		/**
		 * 向服务端发送包
		 * @param pkg 需要发送的包
		 * 
		 */
        void SendPackage(PackageOut pkg);
		
		/**
		 * 添加对于本协议的回调函数
		 * @param callback 协议处理的回调函数
		 * 
		 */
        void AddProtocolEventListent(Delegate callback);
		
		/**
		 * 移除对于本协议的处理的回调函数
		 * @param callback 协议处理的回调函数
		 * 
		 */
        void RemoveProtocolEventListent(Delegate callback);
		
		/**
		 * 通知所有对于本协议的回调函数 
		 * @param parametes  参数
		 * 
		 */
		void Notify(params object[] parameters);
    }

    public class NetManager
    {
        public static Basic.Managers.EventListener sendFunction;
    }
    
    public class Command:ICommand
    {
		protected int _code;
		protected string _desc;

        protected Type _resultType;
		private bool _needWaitResponse = true;
        private List<Delegate> _handles = new List<Delegate>();

        private bool _needShowErrorCode = true;
        public bool needShowErrorCode { get { return _needShowErrorCode; } }


		public Command()
		{
            Type type = this.GetType();
            CmdAttribute attribute = Attribute.GetCustomAttribute(type, typeof(CmdAttribute)) as CmdAttribute;
            if (attribute != null)
            {
                _code = attribute.code;
                _resultType = attribute.type;
                _desc = attribute.desc;
                _needShowErrorCode = attribute.needShowErrorCode;
            }
		}
		
		public int code
		{
			get{return _code;}
		}
		
		public string desc
        {
			get{return _desc;}
		}
		
		public bool needWaitResponse
		{
			get{return _needWaitResponse;}
            set{_needWaitResponse = value;}
		}
		
		public virtual void ReadPackage(PackageIn pkg)
		{
            if (pkg == null)
            {
                throw new Exception("Error: Package is null when ReadPackage.");
            }
            else
            {
                if (_resultType != null)
                {
                    if (pkg.errCode == 0 && pkg.HasBody())
                    {
                        //object result = PackageConverter.PackageToObject(pkg, _resultType);
                        if (!pkg.isComplete)
                        {
                            object result = pkg.ConvertObject(_resultType);
                            if (result != null) 
                                ProtocolManager.instance.SendNotifyListener(code, pkg.pkgIndex, pkg.errCode, result);
                        }
                    }
                    else
                    {
                        ProtocolManager.instance.SendNotifyListener(code, pkg.pkgIndex, pkg.errCode, null);
                    }
                }
                else
                {
                    ProtocolManager.instance.SendNotifyListener(code, pkg.pkgIndex, pkg.errCode);
                }
            }
		}
		
		public virtual void WritePackage(object data)
		{
            PackageOut pkg = _GetPackageOut();
            if (data != null) {
                PackageConverter.ObjectToPackage(data, pkg);
            }
            SendPackage(pkg);
		}
		
		public void SendPackage(PackageOut pkg)
		{
			NetManager.sendFunction(pkg);
		}

        public void AddProtocolEventListent(Delegate callback)
		{
			if (!_handles.Exists(cb=>cb==callback))
			{
				_handles.Add(callback);
			}
		}

        public void RemoveProtocolEventListent(Delegate callback)
		{
            if(_handles.Exists(cb=>cb==callback))
            {
                _handles.Remove(callback);
            }
		}
		
		public void Notify(params object[] parameters)
		{
            //发的什么协议，可以查看该协议的监听是否有或是函数参数错误
            _handles.ForEach(cb => {
                try
                {
                    if (cb is EventListener)
                    {
                        (cb as EventListener)(parameters);
                    }
                    else
                    {
                        (cb as Delegate).DynamicInvoke(parameters);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Notify" + e.ToString());
                }
            });
		}
		
		protected PackageOut _GetPackageOut()
		{
			return new PackageOut(code);
		}
    }
}
