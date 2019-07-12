# ProcessConnectionsLib
Simple Library to expose network connections (TCP/UDP) per process, can be searchable by process id or process name.
you can initialize the connections tables when you create instance of ProcessConnection class or overwrite the tables using **initializeTables()** function.

![alt text](https://raw.githubusercontent.com/proxytype/ProcessConnectionsLib/master/connectionLib.gif)

Usage:

```csharp
using ProcessConnectionsLib;
using System;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            int pid = -1;

            ProcessConnections connections = new ProcessConnections(true);

            if (args.Length != 0)
            {
                if (int.TryParse(args[0], out pid))
                {
                    //by processID
                    printConnections(new ProcessConnectionsPayload[]
                    { connections.getProcessConnectionsByID(pid) }
                    );
                }
                else
                {
                    //by name
                    printConnections(connections.getProcessConnectionsByName(args[0]));
                }
            }
            else
            {
                //all processes
                printConnections(connections.getAllProcessesConnections());
            }
        }

        static void printConnections(ProcessConnectionsPayload[] payloads)
        {

            if (payloads != null)
            {
                for (int i = 0; i < payloads.Length; i++)
                {
                    if (payloads[i] == null) {
                        continue;
                    }

                    Console.WriteLine("#" + payloads[i].PID + ": " + payloads[i].name);
                    Console.WriteLine("#TCP:" + payloads[i].tcpConnections.Count);

                    for (int z = 0; z < payloads[i].tcpConnections.Count; z++)
                    {
                        Console.WriteLine("#" + payloads[i].tcpConnections[z].address
                            + " Source:" + payloads[i].tcpConnections[z].sourcePort
                            + " Destination:" + payloads[i].tcpConnections[z].destinationPort
                            + " State:" + payloads[i].tcpConnections[z].state);
                    }
                    Console.WriteLine("UDP:" + payloads[i].udpConnections.Count);
                    for (int z = 0; z < payloads[i].udpConnections.Count; z++)
                    {
                        Console.WriteLine("#" + payloads[i].udpConnections[z].address
                            + " Source:" + payloads[i].udpConnections[z].sourcePort
                            + " Destination:" + payloads[i].udpConnections[z].destinationPort
                            + " State:" + payloads[i].udpConnections[z].state);
                    }
                }
            }
        }
    }
}
