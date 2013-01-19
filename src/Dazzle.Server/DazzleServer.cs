using System;
using System.Collections.Generic;
//using System.Linq;
using System.Net;
using System.Text;
using Dazzle.Storage;

namespace Dazzle.Server
{
    public class DazzleServer : AsyncTcpServer, IDisposable
    {
        private bool disposed;
        private DazzleDatabase db;

        public DazzleServer()
            : this(new LevelDBStorage("c:\\tmp"))
        {            
        }

        public DazzleServer(IStorage storage)
            : base(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5223))
        {
            db = new DazzleDatabase(storage);
        }

        ~DazzleServer()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called. 
            if (!this.disposed)
            {
                this.disposed = true;
                if (this.db != null)
                {
                    this.db.Dispose();
                    this.db = null;
                }
            }
        }

        protected override void HandleData(byte[] data, System.Net.Sockets.TcpClient client)
        {
            //client.Client.BeginSend(new byte[1], 0, 1, System.Net.Sockets.SocketFlags.None, null, null);
            //this.Write(client, new byte[1]);
            //return;

            var messageType = (MessageType)data[0];
            ushort size = BitConverter.ToUInt16(data, 1);
            if (messageType == MessageType.Query)
            {
                string query = UTF8Encoding.UTF8.GetString(data, 3, size);
                var result = db.ExecuteQuery(query);
                //client.Client.Send(new byte[1]);
                this.Write(client, new byte[1]);
            }
        }
    }
}
