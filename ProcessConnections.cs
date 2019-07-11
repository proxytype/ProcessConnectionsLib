using ProcessConnectionsLib.Utilities;
using ProcessConnectionsLib.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ProcessConnectionsLib
{
    public class ProcessConnections
    {

        private Dictionary<int, List<ConnectionPayload>> tcpPayloads = null;
        private Dictionary<int, List<ConnectionPayload>> udpPayloads = null;


        public ProcessConnections(bool initialize)
        {
            if (initialize) {
                initializeTables();
            }
        }

        public void initializeTables()
        {

            try
            {
                if (tcpPayloads == null)
                {
                    tcpPayloads = new Dictionary<int, List<ConnectionPayload>>();
                }
                else
                {
                    tcpPayloads.Clear();
                }

                if (udpPayloads == null)
                {
                    udpPayloads = new Dictionary<int, List<ConnectionPayload>>();
                }
                else
                {
                    udpPayloads.Clear();
                }

                TcpTable win32TcpTable = new TcpTable();
                UdpTable win32UdpTable = new UdpTable();

                TcpTable.MIB_TCPROW_OWNER_PID[] win32TcpRows = win32TcpTable.GetAllTcpConnections();
                UdpTable.MIB_UDPROW_OWNER_PID[] win32UdpRows = win32UdpTable.GetAllUdpConnections();

                if (win32TcpRows.Length != 0)
                {

                    for (int i = 0; i < win32TcpRows.Length; i++)
                    {
                        ConnectionPayload connection = new ConnectionPayload();
                        connection.protocol = ConnectionPayload.NETWORK_PROTOCOL.TCP;
                        connection.state = ((TcpTable.TCP_ROW_STATE)Enum.Parse(typeof(TcpTable.TCP_ROW_STATE), win32TcpRows[i].state.ToString())).ToString();
                        connection.address = NetworkTranslator.convertUintToIP(win32TcpRows[i].remoteAddr);
                        connection.sourcePort = NetworkTranslator.convertBytesToPort(win32TcpRows[i].localPort1, win32TcpRows[i].localPort2, win32TcpRows[i].localPort3, win32TcpRows[i].localPort4);
                        connection.destinationPort = NetworkTranslator.convertBytesToPort(win32TcpRows[i].remotePort1, win32TcpRows[i].remotePort1, win32TcpRows[i].remotePort1, win32TcpRows[i].remotePort1);
                        connection.PID = win32TcpRows[i].owningPid;

                        if (!tcpPayloads.ContainsKey(connection.PID))
                        {
                            tcpPayloads.Add(connection.PID, new List<ConnectionPayload>());
                        }

                        tcpPayloads[connection.PID].Add(connection);
                    }

                }

                if (win32UdpRows.Length != 0)
                {

                    for (int i = 0; i < win32UdpRows.Length; i++)
                    {
                        ConnectionPayload connection = new ConnectionPayload();
                        connection.protocol = ConnectionPayload.NETWORK_PROTOCOL.UDP;
                        connection.state = UdpTable.UDP_ROW_STATE.ESTABLISHED.ToString();
                        connection.address = NetworkTranslator.convertUintToIP(win32UdpRows[i].LocalAddr);
                        connection.sourcePort = NetworkTranslator.convertBytesToPort(win32UdpRows[i].localPort1, win32UdpRows[i].localPort2, win32UdpRows[i].localPort3, win32UdpRows[i].localPort4);
                        connection.PID = win32UdpRows[i].owningPid;

                        if (!udpPayloads.ContainsKey(connection.PID))
                        {
                            udpPayloads.Add(connection.PID, new List<ConnectionPayload>());
                        }

                        udpPayloads[connection.PID].Add(connection);
                    }

                }

            }
            catch (Exception ex)
            {

            }

        }

        public ProcessConnectionsPayload[] getAllProcessesConnections()
        {

            Process[] processes = Process.GetProcesses();
            return GetProcessesConnections(processes);
            
        }

      
        public ProcessConnectionsPayload[] getProcessConnectionsByName(string name)
        {
            Process[] processes = Process.GetProcessesByName(name);
            return GetProcessesConnections(processes);
        }

        public ProcessConnectionsPayload getProcessConnectionsByID(int id)
        {
            Process process = Process.GetProcessById(id);

            if (process == null) {
                return null;
            }

            return getProcessConnections(process);
        }


        private ProcessConnectionsPayload[] GetProcessesConnections(Process[] processes)
        {

            if (processes.Length == 0)
            {
                return null;
            }

            ProcessConnectionsPayload[] payloads = new ProcessConnectionsPayload[processes.Length];

            for (int i = 0; i < processes.Length; i++)
            {
                payloads[i] = getProcessConnections(processes[i]);
            }

            return payloads;

        }

        private ProcessConnectionsPayload getProcessConnections(Process process)
        {
            ProcessConnectionsPayload payload = new ProcessConnectionsPayload();

            payload.name = process.ProcessName;
            payload.PID = process.Id;

            if (tcpPayloads.ContainsKey(process.Id))
            {
                payload.tcpConnections = tcpPayloads[process.Id];
            }

            if (udpPayloads.ContainsKey(process.Id))
            {
                payload.udpConnections = udpPayloads[process.Id];
            }

            return payload;

        }

    }
}
