using System;
using System.Text;
using System.IO;
using System.Net;
using System.Threading;
using System.Net.Sockets;

namespace DistanceVector
{
    public class Broadcaster {
        // Sending to 
        private Neighbor going {get;set;}
        // Receiving from
        private Neighbor comming {get;set;}
        // Constructor
        public Broadcaster(Neighbor comming, Neighbor going) {
            this.comming = comming;
            this.going = going;
        }
        // Thread method 
        public void run() {
            String dvs = convert(comming.dv);
            Socket sock = null;
            bool done = false;
            int tries = 0;
            // Try five times to send packet, otherwise there is an error
            while (!done && tries < 5) {
                try {
                    Thread.Sleep(1000);
                    IPAddress hostAddress = going.address;
                    int conPort = going.port;
                    IPEndPoint hostEndPoint = new IPEndPoint(hostAddress, conPort);
                    // Connect to router
                    sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    sock.Connect(hostEndPoint);
                    if (!sock.Connected) {
                        // Connection failed, try next IPaddress.
                        sock = null;
                        break;
                    }
                    // Send table 
                    sock.Send(Encoding.UTF8.GetBytes(dvs));
                    sock.Close();
                    done = true;
                }
                catch (Exception) {
                    tries++;
                    if (tries >= 5)
                        Console.WriteLine("{0}: Cannot connect to :{1}\nat Address:{2} port:{3} ", comming.id, going.id, going.address, going.port);
                }
            }
        }
        // Convert table into string for UTF writing 
        public String convert(int[] dv) {
            // Fix offset for spaces following 
            String result = dv[0] + "";
            for (int i = 1; i < dv.Length; i++) {
                result += " " + dv[i];
            }
            return result;
        }
    }
}