using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GraphPath
{
    //Variables
    [SerializeField]
    private List<Node> nodePath;
    [SerializeField]
    private List<Vector3> nodePositions;
    [SerializeField]
    public List<int> path;
    [SerializeField]
    private int pathSize;

    //Properties
    public List<Node> NodePath
    {
        get { return nodePath; }
    }
    public List<Vector3> NodePositions
    {
        get { return nodePositions; }
    }
    public List<int> Path
    {
        get { return path; }
    }
    public int PathSize
    {
        get { return pathSize; }
    }

    //Constructors
    public GraphPath()
    {
        nodePath = new List<Node>();
    }

    public GraphPath(int size)
    {
        pathSize = size;
        nodePath = new List<Node>();
        nodePositions = new List<Vector3>();
        path = new List<int>();
        for (int i = 0; i < size; i++)
        {
            Path.Add(0);
        }
    }

    public GraphPath(GraphPath graphPath)
    {
        this.pathSize = graphPath.pathSize;
        nodePath = new List<Node>();
        path = new List<int>();
        nodePositions = new List<Vector3>();
        for (int i = 0; i < this.pathSize; i++)
        {
            Path.Add(graphPath.Path[i]);
        }
    }

    public string ReturnPath()
    {
        if (nodePath.Count == 0)
            return "Empty path.";

        string text = "";
        for (int i = 0; i < nodePath.Count; i++)
        {
            if (nodePath[i] == null)
                continue;

            //text += nodePath[i].name;
            text += nodePath[i].NodeNumber.ToString();
            if (i < nodePath.Count - 1)
                text += "->";
        }
        return text;
    }

    public void FixPaths(List<Node> nodeList)
    {
        for (int i = 0; i < path.Count; i++)
        {
            nodePath.Add(nodeList[path[i] - 1]);
            nodePositions.Add(nodeList[path[i] - 1].transform.localPosition);
        }
    }

    public void UpdatePositions()
    {
        for (int i = 0; i < nodePath.Count; i++)
        {
            nodePositions[i] = nodePath[i].transform.localPosition;
        }
    }


}
