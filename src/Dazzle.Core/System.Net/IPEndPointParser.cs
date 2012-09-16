using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;

namespace System.Net
{
    /// <summary>
    /// Parses IPv4 and IPv6 notation.
    /// </summary>
    public static class IPEndPointParser
    {
        /// <summary>
        /// Creates an IPEndPoint from IPv4 and IPv6 notation.
        /// </summary>
        /// <param name="endPoint">Endpoint address.</param>
        /// <returns>IPEndPoint with the address and port.</returns>
        public static IPEndPoint Parse(string endPoint)
        {
            string[] ep = endPoint.Split(':');
            if (ep.Length < 2) throw new FormatException("Invalid endpoint format");
            IPAddress ip;
            if (ep.Length > 2)
            {
                if (!IPAddress.TryParse(string.Join(":", ep, 0, ep.Length - 1), out ip))
                {
                    throw new FormatException("Invalid ip-adress");
                }
            }
            else
            {
                if (!IPAddress.TryParse(ep[0], out ip))
                {
                    throw new FormatException("Invalid ip-adress");
                }
            }
            int port;
            if (!int.TryParse(ep[ep.Length - 1], NumberStyles.None, NumberFormatInfo.CurrentInfo, out port))
            {
                throw new FormatException("Invalid port");
            }
            return new IPEndPoint(ip, port);
        }
    }
}
