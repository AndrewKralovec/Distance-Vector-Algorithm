# Distance-Vector-Algorithm
Created a Distance Vector Algorithm for .NET framework. 
The purpose of this program is to implement a Distance Vector Routing Protocol in c# 

# Start Program # 
in your terminal launch `mono DistanceVector.exe`

# Layout # 
*Program.cs* is the main script that creates the router nodes in the network. In program its set up for a network size of 5, you can change it to whatever you want.   
*Router.cs* Contains the distance vector routing algorithm. The router compares its own distance vector table with other incoming distance vector tables and takes the shortest path.  
*Neighbor.cs* Wraps the information about a router inside a class. It reuses info from the router, so the class should be removed in the future to optimize the program. 
*NeigborNode.cs* Dictionary class to hold the node and its edge cost. 
*Broadcaster.cs* This class sends a packet from one router to another.
