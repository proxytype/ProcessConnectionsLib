# ProcessConnectionsLib
Simple Library for expose process network connections

Usage:

```csharp
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
