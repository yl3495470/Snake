using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Basic.Interfaces
{
    public interface IProtocolSender
    {
        void Send(int code, object data = null);
        void ReSend();
        bool existUnsendPkg { get; }
    }
}
