using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dazzle
{
    public struct TcpFrame
    {
        public MessageType MessageType;
        public ushort Size;
        public byte[] Data;
    }
}
