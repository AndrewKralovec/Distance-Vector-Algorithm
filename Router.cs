using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Net.Sockets; 

namespace DistanceVector
{
    public class Router {
        private int n;
        private int id;
        private IPAddress address;
        private int port;
        private List <NeighborNode> neighborList;
        private int[] dv;
        private int[] next;
        private Neighbor self;
        private const int INFINITY = 10000000;
        private int[] nextHop;
        private int fromHop = 0;
        private string data = null;
        private byte[] bytes = new byte[1024];

        // Constructor
        public Router(int n, int id, IPAddress address, int port, List <NeighborNode> neighborList) {
            this.n = n;
            this.id = id;
            this.address = address;
            this.port = port;
            this.neighborList = neighborList;
            dv = new int[n];
            next = new int[n];
            nextHop  = new int[n];
        }
        // Thread method 
        public void run() {
            // Init distance vector
            for (int i = 0; i < n; i++) {
                dv[i] = INFINITY;
                next[i] = -1;
                nextHop[i] = i;
            }

            // Set distance vector neighbors 
            foreach (NeighborNode nn in neighborList) {
                dv[nn.self.id] = nn.cost ; 
                next[nn.self.id] = nn.self.id ; 
            }

            // Distance to self is 0 
            dv[id] = 0;
            next[id] = id;
            self = new Neighbor(n, id, address, port, dv);
            distribute();

            // Read from neighbors
            IPEndPoint localEndPoint = new IPEndPoint(address, port);

            // Create a TCP/IP socket.
            Socket listener = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp );
            try {
                listener.Bind(localEndPoint);
                listener.Listen(10);
            } catch (Exception ex) {
                Console.WriteLine(ex); 
            }
            while (true) {
                // Read distance vectors from socket  
                try {
                    Socket handler = listener.Accept();
                    int bytesRec = handler.Receive(bytes);
                    data = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    String[] dvTable = data.Split(' ');
                    int[] calcTable = compareDv(dvTable, dv);
                    int[] newTable = delayDv(calcTable, dv);
                    Console.WriteLine(printDv(newTable)); 
                } catch (Exception ex) {
                    Console.WriteLine(ex);
                    break;
                }
            }
        }
        // Print out distance vectors [debugging] 
        public String printDv(int[] dv) {
            StringBuilder builder = new StringBuilder();
            builder.Append("Router Table for "+this.id+" \n"); 
            int count = 0;
            foreach (int d in dv) {
                builder.Append(count + ":" + d + ":" + nextHop[count] + " \n");
                count++;
            }
            return builder.ToString(); 
        }

        // Compare delay times 
        public int[] compareDv(String[] newDv, int[] oldDv) {
            int[] result = new int[oldDv.Length];
            // Find index at which to calc the distances 
            for (int index = 0; index < oldDv.Length; index++) {
                if (oldDv[index] == 0) {
                    for (int j = 0; j < oldDv.Length; j++) {
                        // Add offset to new table 
                        result[j] = (int.Parse(newDv[j]) + int.Parse(newDv[index]));
                    }
                }
            }
            return result;
        }
        // Return table with the shortest delay 
        public int[] delayDv(int[] newDv, int[] oldDv) {
            int[] result = oldDv;
            for (int i = 0; i < oldDv.Length; i++) {
                if (oldDv[i] > newDv[i]) {
                    result[i] = newDv[i];
                    nextHop[i] = fromHop;
                }
            }
            return result;
        }
        // Send distance vector to all neighbors 
        public void distribute() {
            foreach(NeighborNode nn in neighborList) {
                Broadcaster br = new Broadcaster(self, nn.self);
                Thread brThread = new Thread(new ThreadStart(br.run));
                brThread.Start();
            }
        }
    }
}
