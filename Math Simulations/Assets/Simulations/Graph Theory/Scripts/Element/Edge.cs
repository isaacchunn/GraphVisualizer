using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge : MonoBehaviour
{
    public Node rootNode;
    public  List<Node> connectedNodes;

    private List<Vector3> nodePositions;
    private LineRenderer lr;
    private EdgeCollider2D edgeCollider;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        edgeCollider = GetComponent<EdgeCollider2D>();
        nodePositions = new List<Vector3>();
    }

    public void HandleLines()
    {
        List<Vector2> lines = new List<Vector2>();
        nodePositions.Clear();
        for (int i = 0; i < connectedNodes.Count; i++)
        {
            if (connectedNodes[i] != null)
            {
                nodePositions.Add(connectedNodes[i].transform.localPosition);
                lines.Add(new Vector2(connectedNodes[i].transform.localPosition.x,
                                      connectedNodes[i].transform.localPosition.y));
            }

        }
        if (lr)
        {
            lr.positionCount = connectedNodes.Count;
            lr.SetPositions(nodePositions.ToArray());
        }
        //Regenerate colliders
        if (edgeCollider)
            edgeCollider.SetPoints(lines);
    }

    public bool ContainsNode(Node node)
    {
        return connectedNodes.Contains(node);
    }

    public void AddNode(Node node)
    {
        //For now, no curve graphs I guess?
        if (!connectedNodes.Contains(node))
            connectedNodes.Add(node);
    }

    public void RemoveNode(Node node)
    {
        //If this node is root, we should remove this edge too.
        connectedNodes.Remove(node);
        //Check if minimum two
        if (connectedNodes.Count <= 1)
        {
            //Make sure this edge is properly deleted
            DeleteEdge();
        }
        else
        {
            HandleLines();
        }
    }

    public void DeleteEdge()
    {
        for (int i = 0; i < connectedNodes.Count; i++)
        {
            connectedNodes[i].RemoveEdge(this);
        }
        Destroy(this.gameObject);
    }
}