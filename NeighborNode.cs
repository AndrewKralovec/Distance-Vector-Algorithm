using System;

namespace DistanceVector
{
    public class NeighborNode {
        // This neighbor
        public Neighbor self{get; set;}
        // This cost
        public int cost{get; set;}
        //Constructor
        public NeighborNode(Neighbor self, int cost) {
            this.self = self;
            this.cost = cost;
        }
    }
}