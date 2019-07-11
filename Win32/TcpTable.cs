using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ProcessConnectionsLib.Win32
{
    public class TcpTable
    {
        //sort values by declartion order in memory
        [StructLayout(LayoutKind.Sequential)]
        public struct MIB_TCPROW_OWNER_PID
        {
            public uint state;
            public uint localAddr;
            public byte localPort1;
            public byte localPort2;
            public byte localPort3;
            public byte localPort4;
            public uint remoteAddr;
            public byte remotePort1;
            public byte remotePort2;
            public byte remotePort3;
            public byte remotePort4;
            public int owningPid;
        }


        //sort values by declartion order in memory
        [StructLayout(LayoutKind.Sequential)]
        public struct MIB_TCPROW
        {
            public uint dwState;
            public uint dwLocalAddr;
            public uint dwLocalPort;
            public uint dwRemoteAddr;
            public uint dwRemotePort;

        }


        //sort values by declartion order in memory
        [StructLayout(LayoutKind.Sequential)]
        public struct MIB_TCPTABLE_OWNER_PID
        {
            public uint dwNumEntries;
            MIB_TCPROW_OWNER_PID table;
        }


        //sort values by declartion order in memory
        //enum of type for tcptable
        public enum TCP_TABLE_CLASS
        {
            TCP_TABLE_BASIC_LISTENER,
            TCP_TABLE_BASIC_CONNECTIONS,
            TCP_TABLE_BASIC_ALL,
            TCP_TABLE_OWNER_PID_LISTENER,
            TCP_TABLE_OWNER_PID_CONNECTIONS,
            TCP_TABLE_OWNER_PID_ALL,
            TCP_TABLE_OWNER_MODULE_LISTENER,
            TCP_TABLE_OWNER_MODULE_CONNECTIONS,
            TCP_TABLE_OWNER_MODULE_ALL
        }


        //enum of the state of single row in the table
        public enum TCP_ROW_STATE
        {
            CLOSED,
            LISTEN,
            SENT,
            RCVD,
            ESTAB,
            FIN_WAIT1,
            FIN_WAIT2,
            CLOSE_WAIT,
            CLOSING,
            LAST_ACK,
            TIME_WAIT,
            DELETE_TCB
        }


        //expose signature, importing ip helper api
        [DllImport("iphlpapi.dll", SetLastError = true)]
        static extern uint GetExtendedTcpTable(IntPtr pTcpTable, ref int dwOutBufLen, bool sort, int ipVersion, TCP_TABLE_CLASS tblClass, int reserved);

        //return the MIB_TCPROW_OWNER_PID array as connection 
        public MIB_TCPROW_OWNER_PID[] GetAllTcpConnections()
        {


            MIB_TCPROW_OWNER_PID[] tTable;
            int AF_INET = 2;    // IP_v4
            int buffSize = 0;


            // what the size of the memory we need to allocate for the table?
            uint ret = GetExtendedTcpTable(IntPtr.Zero, ref buffSize, true, AF_INET, TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL, 0);
            //set pointer to buffer
            IntPtr buffTable = Marshal.AllocHGlobal(buffSize);


            try
            {
                //getting the buffer
                ret = GetExtendedTcpTable(buffTable, ref buffSize, true, AF_INET, TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL, 0);
                if (ret != 0)
                {
                    return null;
                }


                //convert pointer to MIB_TCPTABLE_OWNER_PID pointer
                MIB_TCPTABLE_OWNER_PID tab = (MIB_TCPTABLE_OWNER_PID)Marshal.PtrToStructure(buffTable, typeof(MIB_TCPTABLE_OWNER_PID));
                IntPtr rowPtr = (IntPtr)((long)buffTable + Marshal.SizeOf(tab.dwNumEntries));


                tTable = new MIB_TCPROW_OWNER_PID[tab.dwNumEntries];


                //reading the row from buffer using next position pointer
                //size of MIB_TCPROW_OWNER_PID
                for (int i = 0; i < tab.dwNumEntries; i++)
                {
                    //convert pointer to MIB_TCPROW_OWNER_PID pointer
                    MIB_TCPROW_OWNER_PID tcpRow = (MIB_TCPROW_OWNER_PID)Marshal.PtrToStructure(rowPtr, typeof(MIB_TCPROW_OWNER_PID));
                    //save row in table
                    tTable[i] = tcpRow;
                    //go to the next entry.
                    rowPtr = (IntPtr)((long)rowPtr + Marshal.SizeOf(tcpRow));
                }


            }
            finally
            {
                // clear buffer
                Marshal.FreeHGlobal(buffTable);
            }


            return tTable;
        }
    }
}

