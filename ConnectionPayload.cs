using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessConnectionsLib
{
    public class ConnectionPayload
    {
        public enum NETWORK_PROTOCOL
        {
            TCP,
            UDP,
            ICMP
        }

        public string address { get; set; }
        public int sourcePort { get; set; }
        public int destinationPort { get; set; }
        public NETWORK_PROTOCOL protocol;
        public string state { get; set; }
        public int PID { get; set; }
    }
}
