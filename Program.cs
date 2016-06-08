using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading;
using System.Net.Sockets; 

namespace DistanceVector
{
  public class Program {
		public static int n = 5; // size of network
		public const int INFINITY = 10000000;
		public static string[] dns = new string[] {"127.0.0.1", "127.0.0.1", "127.0.0.1", "127.0.0.1", "127.0.0.1" }; 
		public static int[] port = new int[] {4444,3000,3080,3030,3333,3344};
		public static IPAddress[] address = new IPAddress[n];
		public static int[][] network = new int[][] {
			new int[] {0,5,8,INFINITY,INFINITY},
			new int[] {5,0,4,3,INFINITY},
			new int[] {8,4,0,2,11},
			new int[] {INFINITY,3,2,0,6},
			new int[] {INFINITY,INFINITY,11,6,0}
		};
		public static Neighbor[] node;
		
		// Main Method
		public static void Main(string[] args) {
			if (args.Length == 0) {
				Console.Write("\n Usage: <router id> ");
				Console.Write(" where id (between 0 and n-1)\n");
				Console.ReadLine(); 
				Environment.Exit(1);
			}
			for (int j = 0; j < args.Length ; j++) {
				int id = int.Parse(args[j]);
					for (int i = 0; i < n; i++){
						address[i] = IPAddress.Parse(dns[i]);
					}
					node = new Neighbor[n];
					for (int i = 0; i < n; i++) {
						node[i] = new Neighbor(n,i, address[i],port[i],network[i]);
					}
					List<NeighborNode> neighborList = new List<NeighborNode>();
					for (int i = 0; i < n; i++) {
						int cost = network[id][i]; // get cost from id to node i
						if (cost > 0 && cost < INFINITY) {
							neighborList.Add(new NeighborNode(node[i],cost));
						}
					}
					Router router = new Router(n,id, address[id],port[id], neighborList);
					Thread rThread = new Thread(new ThreadStart(router.run));
					rThread.Start();			
			}
		}
	}
}
