using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    //Node's degree
    private float degree = 0;
    //List of edges and nodes connected to this node
    private List<Edge> edges = new List<Edge>();
    private List<Node> nodes = new List<Node>();
    //Node number for reference
    private int nodeNumber;

    //Properties
    public float Degree
    {
        get { return degree; }
        set { degree = value; }
    }

    public List<Node> Nodes
    {
        get { return nodes; }
        set { nodes = value; }
    }
    public List<Edge> Edges
    {
        get { return edges; }
        set { edges = value; }
    } 
    public int NodeNumber
    {
        get { return nodeNumber; }
        set { nodeNumber = value; }
    }


    public bool LinkedToNode(Node node)
    {
        //Check through all edges list to see if this is linked
        for (int i = 0; i < edges.Count; i++)
        {
            if (edges[i].ContainsNode(node))
                return true;
        }
        return false;
    }

    public void DeleteNode()
    {
        for (int i = 0; i < edges.Count; i++)
        {
            //Remove the edges node connections
            edges[i].RemoveNode(this);
        }
        //Then destroy this object
        Destroy(this.gameObject);
        degree = edges.Count;
    }
    /// <summary>
    /// Attempts to add an edge.
    /// </summary>
    /// <param name="edge">edge to add</param>
    public void AddEdge(Edge edge)
    {
        if (edges == null)
        {
            Debug.Log("Edge was null.");
            return;
        }
        edges.Add(edge);
        //Set degree to edge count.
        degree = edges.Count;

        //using this edge, we know what it's connected to, so it's easier to calculate hamilton paths
        for (int i = 0; i < edge.connectedNodes.Count; i++)
        {
            //If current node list doesnt contain
            if (!nodes.Contains(edge.connectedNodes[i]) && edge.connectedNodes[i] != this)
            {
                nodes.Add(edge.connectedNodes[i]);
            }
        }
    }

    /// <summary>
    /// Removes an edge from this node
    /// </summary>
    /// <param name="edge"></param>
    public void RemoveEdge(Edge edge)
    {
        if (edges == null)
        {
            Debug.Log("Edge was null.");
            return;
        }
        edges.Remove(edge);
        //Set degree to edge count.
        degree = edges.Count;

        //using this edge, we know what it's connected to
        for (int i = 0; i < edge.connectedNodes.Count; i++)
        {
            //If current node list doesnt contain
            if (nodes.Contains(edge.connectedNodes[i]))
            {
                nodes.Remove(edge.connectedNodes[i]);
            }
        }
    }

    public bool ContainsEdge(Edge edge)
    {
        return edges.Contains(edge);
    }
    public void UpdateEdges()
    {
        for (int i = 0; i < edges.Count; i++)
        {
            edges[i].HandleLines();
        }
    }
}
