# Distance-Vector-Algorithm
Created a Distance Vector Algorithm for .NET framework. 
The purpose of this program is to implement a Distance Vector Routing Protocol in c# 
This project will eventually become a .NET library for finding distances between routers. 

# Start Program # 
in your terminal launch `mono DistanceVector.exe <router name> `

# Layout # 
*Program.cs* is the main script that creates the router nodes in the network. In program its set up for a network size of 5, you can change it to whatever you want. Takes size n (in this case 5) arguments each representing the routers name 
*Router.cs* Contains the distance vector routing algorithm. The router compares its own distance vector table with other incoming distance vector tables and takes the shortest path.  
*Neighbor.cs* Wraps the information about a router inside a class. It reuses info from the router, so the class can be removed in the future to optimize the program. 
*Broadcaster.cs* This class sends a packet from one router to another.
