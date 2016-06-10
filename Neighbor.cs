using System;
using System.IO;
using System.Net;
namespace DistanceVector {
    public class Neighbor {
        // Number of neighbors
        public int n { get; set; }
        // This neighbor id
        public int id { get; set; }
        // This neighbor ip
        public IPAddress address { get; set; }
        // This neighbor port
        public int port { get; set; }
        // This neighbor id table 
        public int[] dv { get; set; }
        // This neighbors next neighbor 
        public int[] next { get; set; }
        public const int INFINITY = int.MaxValue;

        // Constructor:
        public Neighbor(int n, int id, IPAddress address, int port, int[] dv) {
            this.n = n;
            this.id = id;
            this.address = address;
            this.port = port;
            this.dv = dv;

            next = new int[n];
            for (int i = 0; i < n; i++) {
                if (dv[i] > 0 && dv[i] < INFINITY)
                    next[i] = i;
                else
                    next[i] = -1;
            }
        }
    }
}