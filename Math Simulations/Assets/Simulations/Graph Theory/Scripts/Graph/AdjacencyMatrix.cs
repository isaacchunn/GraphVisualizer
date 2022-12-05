using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AdjacencyMatrix
{

    //Variables
    //Adjacency Size (n*n)
    [SerializeField]
    private int adjacencySize;
    //1D List of matrices.
    [SerializeField]
    private List<bool> adjacencyMatrix = new List<bool>();

    /// <summary>
    /// Properties
    /// </summary>
    public int AdjacencySize
    {
        get { return adjacencySize; }
        set { adjacencySize = value; }
    }

    //Constructor
    public AdjacencyMatrix()
    {
        adjacencySize = 0;
        //Set all to false
        adjacencyMatrix = new List<bool>();
    }

    //Generate adjacency matrix based off a nodes list.
    public void Generate(List<Node> nodeList)
    {
        //Clear the matrix
        adjacencyMatrix.Clear();
        //Update the adjacency size
        adjacencySize = nodeList.Count;
        int count = adjacencySize * adjacencySize;
        for (int i = 0; i < count; i++)
        {
            adjacencyMatrix.Add(false);
        }

        //Set all false
        //Double for to handle nodes
        for (int rows = 0; rows < adjacencySize; rows++)
        {
            for (int cols = 0; cols < adjacencySize; cols++)
            {
                if (nodeList[rows].Nodes.Contains(nodeList[cols]))
                    adjacencyMatrix[(rows * nodeList.Count) + cols] = true;
            }
        }
    }

    public string Print()
    {
        string text = "Adjacency Matrix:\n   ";
        //Print the top first
        if (adjacencySize > 9)
        {
            text += "  ";
        }

        for (int i = 0; i < adjacencySize; ++i)
        {
            text += (i + 1).ToString() + " ";
        }
        //Make spacing
        text += "\n";

        for (int rows = 0; rows < adjacencySize; rows++)
        {
            //Print rows       
            text += (rows + 1).ToString() + " ";
            if (adjacencySize > 9 && rows < 9)
            {
                text += "  ";
            }
            for (int cols = 0; cols < adjacencySize; cols++)
            {
                text += adjacencyMatrix[(rows * adjacencySize) + cols] ? "1 " : "0 ";
                if (cols == 8)
                {
                    text += " ";
                }
                else if (cols > 8)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        text += " ";
                    }
                }
            }
            text += "\n";
        }
        return text;
    }

    //Gets  if theres an edge connected in the graph
    public bool GetZero(int rows, int cols)
    {
        return adjacencyMatrix[(rows * adjacencySize) + cols];
    }
    public bool GetOne(int rows, int cols)
    {
        return adjacencyMatrix[((rows - 1) * adjacencySize) + (cols - 1)];
    }
}
