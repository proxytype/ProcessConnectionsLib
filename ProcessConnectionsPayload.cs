using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessConnectionsLib
{
    public class ProcessConnectionsPayload
    {
        public int PID { get; set; }
        public string name { get; set; }

        public List<ConnectionPayload> tcpConnections { get; set; }
        public List<ConnectionPayload> udpConnections { get; set; }


        public ProcessConnectionsPayload()
        {
            tcpConnections = new List<ConnectionPayload>();
            udpConnections = new List<ConnectionPayload>();
        }
    }
}
