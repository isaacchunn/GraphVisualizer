using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PathFinder
{
    public enum MODE
    {
        EULERPATH,
        EULERCIRCUIT,
        HAMILTONIANPATH,
        HAMILTONIANCIRCUIT
    }

    public MODE mode;
    public List<GraphPath> hamiltonianCircuits;
    public List<GraphPath> hamiltonianPaths;

    public PathFinder()
    {
        //Initialize hamiltonian path list
        hamiltonianCircuits = new List<GraphPath>();
        hamiltonianPaths = new List<GraphPath>();
        mode = MODE.HAMILTONIANCIRCUIT;
    }
    public GraphPath EulerPath(List<Node> nodeList)
    {
        Node n1 = new Node();
        int count = 0;
        for (int i = 0; i < nodeList.Count; i++)
        {
            //Odd vertices
            if (nodeList[i].Degree % 2 != 0)
            {
                count++;
                if (n1 == null)
                    n1 = nodeList[i];
            }
        }
        //If more than 2 vertices with 
        if (count > 2)
            return null;



        return null;
    }
    public bool ContainsEulerPath(List<Node> nodeList)
    {
        int count = 0;
        for (int i = 0; i < nodeList.Count; i++)
        {
            //Odd vertices
            if (nodeList[i].Degree % 2 != 0)
                count++;
        }
        if (count == 0 || count > 2)
            return false;

        return true;
    }

    public bool ContainsEulerCircuit(List<Node> nodeList)
    {
        for (int i = 0; i < nodeList.Count; i++)
        {
            //Odd vertices
            if (nodeList[i].Degree % 2 != 0)
                return false;
        }
        return true;
    }

    //Main function to call to get the hamiltonian path
    public GraphPath HamiltonianCircuit(List<Node> nodeList, AdjacencyMatrix adj, int startIndex = 1)
    {
        if (nodeList.Count == 0)
        {
            Debug.Log("There are no nodes! No path is found!");
            return null;
        }
        //Debug.Log("Running Hamiltonian Path.");
        //Clear current paths
        hamiltonianCircuits.Clear();
        //Find the number of nodes 
        int numberNodes = nodeList.Count;
        //Then initialize a new graph path.
        GraphPath hPath = new GraphPath(numberNodes);
        //First node
        //Add in first index as we are starting from 1.
        hPath.Path[0] = startIndex;
        //Run the Hamiltonian Algorithm
        Hamiltonian(1, hPath, adj);

        //Fix all nodes
        foreach (GraphPath graph in hamiltonianCircuits)
        {
            graph.FixPaths(nodeList);
        }

        //Debug.Log(hPath.ReturnPath());
        return null;
    }

    //Recursive function test
    public void Hamiltonian(int k, GraphPath hPath, AdjacencyMatrix adj)
    {
        while (true)
        {
            NextVertex(k, hPath, adj);
            if (hPath.Path[k] == 0)
            {
                return;
            }

            if (k == hPath.PathSize - 1) //If k reached the end index then we found our path, e.g. 5 indexes, so 01234, path size is 5-1
            {
                GraphPath currPath = new GraphPath(hPath);
                //Add back start index
                currPath.Path.Add(currPath.Path[0]);
                hamiltonianCircuits.Add(currPath);
                return;
            }
            else
            {
                Hamiltonian(k + 1, hPath, adj);
            }
        }
    }

    public void NextVertex(int k, GraphPath hPath, AdjacencyMatrix adj)
    {
        do
        {
            //Add one to this vertex to try to see if this vertex can be matched up
            hPath.Path[k] = (hPath.Path[k] + 1) % (hPath.PathSize + 1);
            //Ending condition when all paths have been added up and it reaches back to 0
            if (hPath.Path[k] == 0)
            {
                return;
            }
            //Else we compare if theres an edge from previous path to this path
            if (adj.GetOne(hPath.Path[k - 1], hPath.Path[k]))
            {
                int j;
                //If there exists a path , check against all previous edges to see if theres a similar path
                for (j = 0; j < k; j++)
                {
                    //Loop through past indexes 
                    if (hPath.Path[j] == hPath.Path[k])
                    {
                        break;
                    }
                }
                //Else if we reached the end index - 1
                if (j == k)
                {
                    //Then we check if k is less than or equal to path size, and check if theres an edge to the first index.
                    if (k < hPath.PathSize - 1)
                    {
                        return;
                    }
                    //If reached the end and detected an edge then
                    if (k == hPath.PathSize - 1 && adj.GetOne(hPath.Path[0], hPath.Path[hPath.PathSize - 1]))
                    {
                        return;
                    }
                }
            }
        } while (true);
    }

    //Checks if graph are connected. fix later
    public bool ConnectedGraph(List<Edge> edgeList, List<Node> nodeList)
    {
        List<Node> nodes = new List<Node>();
        //Loop through all the edges and get nodes
        for (int i = 0; i < edgeList.Count; i++)
        {
            for (int j = 0; j < edgeList[i].connectedNodes.Count; ++j)
            {
                if (!nodes.Contains(edgeList[i].connectedNodes[j]))
                {
                    nodes.Add(edgeList[i].connectedNodes[j]);
                    if (nodes.Count == nodeList.Count)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
