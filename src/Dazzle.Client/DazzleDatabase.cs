using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Dazzle;

namespace Dazzle.Client
{
    public class DazzleDatabase : IDazzleDatabase, IDisposable
    {
        private bool disposed;
        private TcpClient client;

        ~DazzleDatabase()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called. 
            if (!this.disposed)
            {
                this.Close();
                disposed = true;
            }
        }

        public void Open(string endPointAddress)
        {
            var endPoint = IPEndPointParser.Parse(endPointAddress);
            this.client = new TcpClient();
            this.client.Client.Connect(endPoint);
        }

        public void Close()
        {
            this.client.Close();
        }

        public string Get(string key)
        {
            this.client.Client.Send(Encoding.UTF8.GetBytes(string.Format("get:{0}", key)));

            byte[] data = new byte[2048];
            this.client.Client.Receive(data);

            string response = Encoding.UTF8.GetString(data);
            string[] args = response.Split(':');

            if (args.Length > 1)
                return args[1];
            else
                return null;
        }

        public void Set(string key, string value)
        {
            this.client.Client.Send(Encoding.UTF8.GetBytes(string.Format("put:{0}:{1}", key, value)));

            byte[] data = new byte[1024];
            this.client.Client.Receive(data);

            string response = Encoding.UTF8.GetString(data);
        }

        public void Delete(string key)
        {
            // TODO: Implement.
            throw new NotImplementedException();
        }
    }
}
