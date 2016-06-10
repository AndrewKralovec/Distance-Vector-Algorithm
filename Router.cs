using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Net.Sockets;

namespace DistanceVector {
    public class Router {
        private static int fromHop = 0;
        private int n;
        private int id;
        private IPAddress address;
        private int port;
        public Dictionary<Neighbor, int> neighborList = new Dictionary<Neighbor, int>();
        private string[] nodes; 
        private int[] dv;
        private int[] next;
        private Neighbor self;
        private const int INFINITY = int.MaxValue;
        private int[] nextHop;
        private string data = null;
        private byte[] bytes = new byte[1024];
        
        // Constructor
        public Router(int n, int id, IPAddress address, int port,
        Dictionary<Neighbor, int> neighborList, string[] nodes) {
            this.n = n;
            this.id = id;
            this.address = address;
            this.port = port;
            this.neighborList = neighborList;
            this.nodes = nodes; 
            dv = new int[n];
            next = new int[n];
            nextHop = new int[n];
        }
        // Thread method 
        public void run() {
            // Init distance vector
            for (int i = 0; i < n; i++) {
                dv[i] = INFINITY;
                next[i] = -1;// next router is empty  
                nextHop[i] = i;
            }

            // Set distance vector neighbors 
            foreach (var nn in neighborList) {
                dv[nn.Key.id] = nn.Value;
                next[nn.Key.id] = nn.Value;
            }

            // Distance to self is 0 
            dv[id] = 0;
            next[id] = id;
            self = new Neighbor(n, id, address, port, dv);
            send();

            // Read from neighbors
            IPEndPoint localEndPoint = new IPEndPoint(address, port);

            // Create a TCP/IP socket.
            Socket listener = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);
            try {
                listener.Bind(localEndPoint);
                listener.Listen(10);
            } catch (Exception ex) {
                Console.WriteLine(ex);
            }
            int finished = 0 ; 
            int neighborCount = neighborList.Count ; 
            while (true && finished < neighborCount) {
                // Read distance vectors from socket  
                try {
                    Socket handler = listener.Accept();
                    int bytesRec = handler.Receive(bytes);
                    data = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    string[] dvTable = data.Split(' ');
                    delayDv(compareDv(dvTable, dv));
                    finished++; 
                } catch (Exception ex) {
                    Console.WriteLine(ex);
                    break;
                }
            }
            // Print dv table when done 
            Console.WriteLine(printDv(dv));
        }
        // Print out distance vectors
        public string printDv(int[] dv) {
            StringBuilder builder = new StringBuilder();
            builder.Append("Router Table for " + nodes[id] + " \n");
            int count = 0;
            foreach (int d in dv) {
                string value = d.ToString(); 
                if (d == INFINITY)
                    value = "INFINITY"; 
                builder.Append(nodes[count] + ":" + value + ":" + nodes[nextHop[count]] + " \n");
                count++;
            }
            return builder.ToString();
        }

        // Compare delay times 
        public int[] compareDv(string[] newDv, int[] oldDv) {
            fromHop = int.Parse(newDv[(newDv.Length-1)]); 
            int[] result = new int[oldDv.Length];
            // Find index at which to calc the distances 
            for (int index = 0; index < oldDv.Length; index++) {
                if (oldDv[index] == 0) {
                    for (int j = 0; j < oldDv.Length; j++) {
                        // Add offset to new table 
                        int temp = int.Parse(newDv[j]);
                        // Check if we cant reach neighbor 
                        if (temp == INFINITY)
                            result[j] = temp; 
                        else 
                            result[j] = (int.Parse(newDv[j]) + int.Parse(newDv[index]));
                    }
                }
            }
            return result;
        }
        // Set table with the shortest delay 
        public void delayDv(int[] newDv) {
            for (int i = 0; i < dv.Length; i++) {
                if (dv[i] > newDv[i]) {
                    dv[i] = newDv[i];
                    nextHop[i] = fromHop;
                }
            }
        }
        // Send distance vector to all neighbors 
        public void send() {
            foreach (var nn in neighborList) {
                Broadcaster br = new Broadcaster(self, nn.Key);
                Thread brThread = new Thread(new ThreadStart(br.run));
                brThread.Start();
            }
        }
    }
}