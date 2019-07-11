using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ProcessConnectionsLib.Win32
{
    public class UdpTable
    {
        //sort values by declartion order in memory
        [StructLayout(LayoutKind.Sequential)]
        public struct MIB_UDPROW_OWNER_PID
        {
            public uint LocalAddr;
            public byte localPort1;
            public byte localPort2;
            public byte localPort3;
            public byte localPort4;
            public int owningPid;
        }


        //sort values by declartion order in memory
        [StructLayout(LayoutKind.Sequential)]
        public struct MIB_UDPTABLE_OWNER_PID
        {
            public uint dwNumEntries;
            MIB_UDPROW_OWNER_PID table;
        }


        enum UDP_TABLE_CLASS
        {
            UDP_TABLE_BASIC,
            UDP_TABLE_OWNER_PID,
            UDP_TABLE_OWNER_MODULE
        }

        public enum UDP_ROW_STATE
        {
            ESTABLISHED
        }

        //expose signature, importing ip helper api
        [DllImport("iphlpapi.dll", SetLastError = true)]
        static extern uint GetExtendedUdpTable(IntPtr pUddpTable, ref int dwOutBufLen, bool sort, int ipVersion, UDP_TABLE_CLASS tblClass, int reserved);
        //return the MIB_UDPROW_OWNER_PID array


        public MIB_UDPROW_OWNER_PID[] GetAllUdpConnections()
        {


            MIB_UDPROW_OWNER_PID[] tTable;


            int AF_INET = 2;    // IP_v4
            int buffSize = 0;


            // what the size of the memory we need to allocate for the table?
            uint ret = GetExtendedUdpTable(IntPtr.Zero, ref buffSize, true, AF_INET, UDP_TABLE_CLASS.UDP_TABLE_OWNER_PID, 0);
            //set pointer to buffer
            IntPtr buffTable = Marshal.AllocHGlobal(buffSize);


            try
            {
                //getting the buffer
                ret = GetExtendedUdpTable(buffTable, ref buffSize, true, AF_INET, UDP_TABLE_CLASS.UDP_TABLE_OWNER_PID, 0);
                if (ret != 0)
                {
                    return null;
                }


                //convert pointer to MIB_UDPTABLE_OWNER_PID pointer
                MIB_UDPTABLE_OWNER_PID tab = (MIB_UDPTABLE_OWNER_PID)Marshal.PtrToStructure(buffTable, typeof(MIB_UDPTABLE_OWNER_PID));


                IntPtr rowPtr = (IntPtr)((long)buffTable + Marshal.SizeOf(tab.dwNumEntries));


                tTable = new MIB_UDPROW_OWNER_PID[tab.dwNumEntries];


                //reading the row from buffer using next position pointer
                //size of MIB_UDPROW_OWNER_PID     
                for (int i = 0; i < tab.dwNumEntries; i++)
                {
                    //convert pointer to MIB_UDPROW_OWNER_PID pointer
                    MIB_UDPROW_OWNER_PID udpRow = (MIB_UDPROW_OWNER_PID)Marshal.PtrToStructure(rowPtr, typeof(MIB_UDPROW_OWNER_PID));
                    //save row in table
                    tTable[i] = udpRow;
                    //go to the next entry.
                    rowPtr = (IntPtr)((long)rowPtr + Marshal.SizeOf(udpRow));
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
