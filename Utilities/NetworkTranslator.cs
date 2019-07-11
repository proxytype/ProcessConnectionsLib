using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ProcessConnectionsLib.Utilities
{
    public class NetworkTranslator
    {
        public static string convertUintToIP(uint ipnum)
        {
            string ipAddress = new IPAddress(BitConverter.GetBytes(ipnum)).ToString();
            return ipAddress;
        }


        public static ushort convertBytesToPort(byte p0, byte p1, byte p2, byte p3)
        {

            ushort port;
            byte[] arr = new byte[4];

            //check if the system is litte endian, or big endian and sort the array
            //reading the first 2 bytes
            if (BitConverter.IsLittleEndian)
            {
                arr[0] = p3;
                arr[1] = p2;
                arr[2] = p1;
                arr[3] = p0;
                port = BitConverter.ToUInt16(arr, 2);
            }
            else
            {
                arr[0] = p0;
                arr[1] = p1;
                arr[2] = p2;
                arr[3] = p3;
                port = BitConverter.ToUInt16(arr, 0);
            }

            return port;
        }
    }
}
