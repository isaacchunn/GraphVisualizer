using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class GraphUI
{
    //Default Elements
    public TextMeshProUGUI graphMode;
    public TextMeshProUGUI noVertices;
    public TextMeshProUGUI noEdges;
    public TextMeshProUGUI selectedNode;
    public TextMeshProUGUI adjacencyMatrix;
    public TextMeshProUGUI connectedGraph;

    //Euler Elements
    public TextMeshProUGUI eulerPath;
    public TextMeshProUGUI eulerCircuit;

 
    //Hamiltonian Elements
    public TextMeshProUGUI hamiltonianPath;
    public TextMeshProUGUI hamiltonianCircuit;
    public TextMeshProUGUI hamiltonianCircuitCount;
    public TextMeshProUGUI hamiltonianCurrentPath;
}
