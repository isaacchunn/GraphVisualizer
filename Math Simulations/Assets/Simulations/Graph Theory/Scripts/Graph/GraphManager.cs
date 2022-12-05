using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GraphManager : MonoBehaviour
{
    //Graph mover
    public enum MOVEMENT
    {
        ADDDELETE,
        MOVE,
        TOTAL
    };

    public enum MODE
    {
        HAMILTONIAN,
        EULER,
        DEFAULT
    };

    [Header("Prefab Assignments")]
    [SerializeField]
    private GameObject background;
    [SerializeField]
    private GameObject nodeParent;
    [SerializeField]
    private GameObject edgeParent;
    [SerializeField]
    private GameObject nodePrefab;
    [SerializeField]
    private GameObject edgePrefab;
    [SerializeField]
    private GameObject arrowPrefab;
    [SerializeField]
    private GameObject arrowParent;
    [SerializeField]
    private Box box;

    [Header("Edge Spawn Properties")]
    private bool spawnConsecutive = true;
    public Node selectedNode;
    private float initialNodeSize;

    [Header("Move Mode")]
    private MOVEMENT graphMovement;
    private MODE graphMode;

    [Header("Graph Visualisations")]
    [SerializeField]
    private GraphVisualisation graphVisualisation;
    [Header("Graph UI")]
    [SerializeField]
    private GraphUI graphUI;
    [SerializeField]
    private GameObject defaultObject;
    [SerializeField]
    private GameObject eulerObject;
    [SerializeField]
    private GameObject hamiltonianObject;

    [Header("Graph Finder")]
    [SerializeField]
    private PathFinder pathFinder;

    //Graphs
    //List
    [SerializeField]
    private List<Node> nodeList;
    [SerializeField]
    private List<Edge> edgeList;
    [SerializeField]
    private AdjacencyMatrix adjacencyMatrix;

    //Private objects
    private Camera mainCamera;
    private List<GameObject> arrowList;

    //Hamiltonian
    private int currentHamiltonianIndex = 0;
    private bool startOnSelNode = true;

    // Start is called before the first frame update
    void Start()
    {
        //Get camera refernce
        mainCamera = Camera.main;
        float cameraSize = mainCamera.orthographicSize;
        Vector3 camSize = Vector3.one * cameraSize * mainCamera.aspect * 2f;
        //Update spriteSize;
        background.transform.localScale = camSize;
        //Initialize adhjacency matrix
        adjacencyMatrix = new AdjacencyMatrix();
        //Initialize pathfinder
        pathFinder = new PathFinder();
        //Set default mode
        graphMode = MODE.DEFAULT;
        //Update arrow List
        arrowList = new List<GameObject>();

        switch (graphMode)
        {
            case MODE.DEFAULT:
                EnableDefaultUI();
                break;
            case MODE.HAMILTONIAN:
                EnableHamiltonianUI();
                break;
            case MODE.EULER:
                EnableEulerUI();
                break;
        }
    }


    // Update is called once per frame
    void Update()
    {
        //if (graphMode == MODE.DEFAULT)
        {
            //Handles our input
            HandleInput();
        }
        //Handle UI
        HandleUI();
    }

    //what is this rubbish lazy
    void HandleInput()
    {
        switch (graphMovement)
        {
            case MOVEMENT.ADDDELETE:
                {
                    //If left click detected
                    if (Input.GetMouseButtonDown(0))
                        AddElements();
                    else if (Input.GetMouseButtonDown(1))
                        DeleteElements();
                    else if (Input.GetMouseButtonDown(2)) //Middle mouse click
                        ResetSelectedNode();
                    break;
                }

            case MOVEMENT.MOVE:
                {
                    if (Input.GetMouseButton(0))
                    {
                        MoveNode();
                    }
                    else
                    {
                        //Else reset node
                        ResetSelectedNode();
                    }
                    break;
                }
            default:
                break;
        }

    }

    /// <summary>
    /// Resets selected node to the original node size and resets the reference.
    /// </summary>
    void ResetSelectedNode()
    {
        if (selectedNode != null)
        {
            //Reset selectedNode to it's original properties
            selectedNode.transform.localScale = Vector3.one * initialNodeSize;
            //Reset material
            selectedNode.transform.GetComponent<SpriteRenderer>().material = graphVisualisation.nodeMaterial;
            selectedNode = null;
        }
    }

    /// <summary>
    /// Generates a node at mouse position
    /// </summary>
    void GenerateNodeAtMousePosition()
    {
        if (!box.InArea)
            return;

        //Else spawn normally and prevent another from spawning
        Vector3 pos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        pos.z = -1f;

        Node obj = Instantiate(nodePrefab).GetComponent<Node>();
        obj.transform.localPosition = pos;
        obj.transform.parent = nodeParent.transform;
        obj.transform.localScale = Vector3.one * graphVisualisation.nodeSpawnSize;
        obj.transform.name = "Node " + (nodeList.Count + 1).ToString();
        obj.NodeNumber = (nodeList.Count + 1);
        obj.GetComponentInChildren<TextMeshPro>().text = (nodeList.Count + 1).ToString();

        //Add to nodeList
        nodeList.Add(obj.GetComponent<Node>());

        //Remove hamiltonian visualization as new node added
        graphVisualisation.hamiltonianRenderer.positionCount = 0;
        //Remove the arrows
        foreach (GameObject arrow in arrowList)
        {
            arrow.SetActive(false);
        }
    }

    /// <summary>
    /// Attempts to add an element or add edges
    /// </summary>
    void AddElements()
    {
        if (!box.InArea)
            return;

        //Get a ray from the screen point to the world for rayhit.
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        //Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
        RaycastHit2D rayHit = Physics2D.Raycast(ray.origin, ray.direction * 15f);
        //Raycast onto the world
        if (rayHit.collider != null)
        {
            //If the selectedNode is null then, allow clicking into new selected nodes for hamiltonian
            if (selectedNode == null || graphMode != MODE.DEFAULT)
            {
                ResetSelectedNode();
                //Set selectedNode, start coroutine that makes it increase in size and change color?
                selectedNode = rayHit.transform.gameObject.GetComponent<Node>();
                if (selectedNode != null)
                {
                    initialNodeSize = selectedNode.transform.localScale.x;
                    //Increase it's size by a factor
                    selectedNode.transform.localScale = Vector3.one * initialNodeSize * graphVisualisation.selectedNodeSizeFactor;
                    rayHit.transform.GetComponent<SpriteRenderer>().material = graphVisualisation.selectedNodeMaterial;
                }

            }
            else
            {
                //Only allow adding if it's in default.
                if (graphMode == MODE.DEFAULT)
                {
                    //Else selectedNode is not null, we want to link to other nodes manually.
                    if (rayHit.transform.gameObject == selectedNode)
                    {
                        Debug.Log("Can't spawn an edge on itself! Choose another node.");
                    }
                    else
                    {
                        //Implement checks whether edges between these 2 nodes are already existent.
                        Node selNode = selectedNode.GetComponent<Node>();
                        Node targetNode = rayHit.transform.GetComponent<Node>();

                        //If its linked to node
                        if (selNode && selNode.LinkedToNode(targetNode))
                        {
                            Debug.Log("This node is already linked to the same node!");
                        }
                        else
                        {
                            //It's a valid node.
                            //Instantiate the line renderer
                            GameObject obj = Instantiate(edgePrefab);
                            //Set parents
                            obj.transform.parent = edgeParent.transform;
                            obj.transform.name = "Edge " + (edgeList.Count + 1).ToString();
                            //Handle edge additions
                            Edge edge = obj.GetComponent<Edge>();
                            //Set root node first 
                            edge.rootNode = selectedNode.GetComponent<Node>();
                            //Then we set the connected nodes
                            edge.AddNode(selectedNode.GetComponent<Node>());
                            edge.AddNode(rayHit.transform.GetComponent<Node>());
                            //And then we generate line renderers to attach these nodes.
                            edge.HandleLines();

                            //Then we add this edge into the nodes
                            selNode.AddEdge(edge);
                            targetNode.AddEdge(edge);

                            //Add for tracking in this edge
                            edgeList.Add(edge);

                            //Reset Selected Node
                            ResetSelectedNode();
                            //Handle behaviour
                            //Allow easier connecting of vertices.
                            if (spawnConsecutive)
                            {
                                //Set selectedNode, start coroutine that makes it increase in size and change color?
                                selectedNode = rayHit.transform.gameObject.GetComponent<Node>();
                                if (selectedNode != null)
                                {
                                    initialNodeSize = selectedNode.transform.localScale.x;
                                    //Increase it's size by a factor
                                    selectedNode.transform.localScale = Vector3.one * initialNodeSize * graphVisualisation.selectedNodeSizeFactor;
                                    rayHit.transform.GetComponent<SpriteRenderer>().material = graphVisualisation.selectedNodeMaterial;
                                }
                            }
                        }
                    }
                }
            }
        }
        else
        {
            //Else we have selected another place
            //Reset the size of selectedNode
            ResetSelectedNode();

            //Then generate a node at the mouse position only if mode is default
            if (graphMode == MODE.DEFAULT)
                GenerateNodeAtMousePosition();
        }
        //Update adjacency matrix.
        adjacencyMatrix.Generate(nodeList);
    }

    /// <summary>
    /// Attempts to delete enemies by using the right click
    /// </summary>
    void DeleteElements()
    {
        if (!box.InArea)
            return;

        //Get a ray from the screen point to the world for rayhit.
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        //Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
        RaycastHit2D rayHit = Physics2D.Raycast(ray.origin, ray.direction * 15f);
        //Raycast onto the world
        if (rayHit.collider != null)
        {
            //Debug.Log(rayHit.transform.gameObject.name);
            //Do some error checking
            Node n = rayHit.transform.GetComponent<Node>();
            if (n != null)
            {
                //Delete the node..., so we destroy the object and remove it's edge and nodes
                //It is sufficient to just remove the edges and we just destroy this. Reassign root node?
                nodeList.Remove(n);
                n.DeleteNode();
                for (int i = 0; i < n.Edges.Count; i++)
                {
                    edgeList.Remove(n.Edges[i]);
                }
                //Update the adjacency matrix
                adjacencyMatrix.Generate(nodeList);

                //Reorder node numbers as we have deleted a node
                ReorderNodes();

                //Return so we dont run the edge part
                return;
            }

            Edge edge = rayHit.transform.GetComponent<Edge>();
            if (edge != null)
            {
                //Handle the removing of edges
                edgeList.Remove(edge);
                edge.DeleteEdge();

                //Update the adjacency matrix
                adjacencyMatrix.Generate(nodeList);
                return;
            }
        }
    }

    /// <summary>
    /// Allows nodes to be moved around the scene for re ordering
    /// </summary>
    void MoveNode()
    {
        if (!box.InArea)
            return;

        if (selectedNode == null)
        {
            //Get a ray from the screen point to the world for rayhit.
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            //Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
            RaycastHit2D rayHit = Physics2D.Raycast(ray.origin, ray.direction * 15f);
            //Raycast onto the world
            if (rayHit.collider != null)
            {
                selectedNode = rayHit.transform.gameObject.GetComponent<Node>();
                if (selectedNode != null)
                {
                    initialNodeSize = selectedNode.transform.localScale.x;
                    //Increase it's size by a factor
                    selectedNode.transform.localScale = Vector3.one * initialNodeSize * graphVisualisation.selectedNodeSizeFactor;
                    rayHit.transform.GetComponent<SpriteRenderer>().material = graphVisualisation.selectedNodeMaterial;
                }
            }
        }
        else
        {
            if (graphMode == MODE.DEFAULT)
            {
                //Else not null, we can move it around
                Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = -1f;
                selectedNode.transform.localPosition = mousePos;

                //Then we must update the edge connections
                selectedNode.UpdateEdges();
            }

            //If theres a hamiltonian already being visualized.
            if (pathFinder.hamiltonianCircuits.Count > 0)
            {
                //Update the positions of the path
                pathFinder.hamiltonianCircuits[currentHamiltonianIndex].UpdatePositions();
                graphVisualisation.hamiltonianRenderer.SetPositions(pathFinder.hamiltonianCircuits[currentHamiltonianIndex].NodePositions.ToArray());

                //Update arrow 
                //Update hamiltonian renderer
                UpdateHamiltonianRenderer();
            }
        }
    }

    /// <summary>
    /// Reorders the node number on the nodes.
    /// </summary>
    void ReorderNodes()
    {
        for (int i = 0; i < nodeList.Count; i++)
        {
            nodeList[i].name = "Node " + (i + 1).ToString();
            nodeList[i].GetComponentInChildren<TextMeshPro>().text = (i + 1).ToString();
        }
    }

    #region UI
    //UI stuff

    /// <summary>
    /// Updates the UI accordingly.
    /// </summary>
    void HandleUI()
    {
        graphUI.graphMode.text = graphMovement == MOVEMENT.ADDDELETE ? "Graph Mode: Add/Delete" : "Graph Mode: Move";
        graphUI.noVertices.text = "Vertices: " + nodeList.Count.ToString();
        graphUI.noEdges.text = "Edges: " + edgeList.Count.ToString();

        if (selectedNode != null)
            graphUI.selectedNode.text = "Selected Node: " + selectedNode.name + "\nDegree: " + selectedNode.Degree.ToString();
        else
            graphUI.selectedNode.text = "Selected Node: NA";

        graphUI.eulerPath.text = pathFinder.ContainsEulerPath(nodeList) ? "Euler Path: Yes" : "Euler Path: NA";
        graphUI.eulerCircuit.text = pathFinder.ContainsEulerCircuit(nodeList) ? "Euler Circuit: Yes" : "Euler Circuit: No";
        graphUI.connectedGraph.text = pathFinder.ConnectedGraph(edgeList, nodeList) ? "Connected Graph? : Yes" : "Connected Graph? : No";
        graphUI.adjacencyMatrix.text = adjacencyMatrix.Print();
    }

    /// <summary>
    /// Wrapper Functions
    /// </summary>
    public void ChangeAddMode()
    {
        graphMovement = MOVEMENT.ADDDELETE;
    }
    public void ChangeMoveMode()
    {
        graphMovement = MOVEMENT.MOVE;
    }
    public void ToggleSpawnConsecutive()
    {
        spawnConsecutive = !spawnConsecutive;
    }
    public void ToggleStartOnSelNode()
    {
        startOnSelNode = !startOnSelNode;
    }
    public void EnableDefaultUI()
    {
        graphMode = MODE.DEFAULT;
        eulerObject.SetActive(false);
        hamiltonianObject.SetActive(false);
        defaultObject.SetActive(true);
    }
    public void EnableEulerUI()
    {
        //Change graph mode
        graphMode = MODE.EULER;

        eulerObject.SetActive(true);
        hamiltonianObject.SetActive(false);
        defaultObject.SetActive(false);
    }
    public void EnableHamiltonianUI()
    {
        graphMode = MODE.HAMILTONIAN;
        eulerObject.SetActive(false);
        hamiltonianObject.SetActive(true);
        defaultObject.SetActive(false);
    }
    #endregion

    //Path Functions
    /// <summary>
    /// Generates a hamiltonian path and updates the line renderer
    /// </summary>
    public void GenerateHamiltonianPath()
    {
        int startIndex = 1;
        if (selectedNode != null && startOnSelNode)
            startIndex = selectedNode.NodeNumber;

        //Reset line renderer
        graphVisualisation.hamiltonianRenderer.positionCount = 0;
        //Generates the path.
        pathFinder.HamiltonianCircuit(nodeList, adjacencyMatrix, startIndex);

        //Reset the text
        graphUI.hamiltonianCircuit.text = pathFinder.hamiltonianCircuits.Count == 0 ? "Hamiltonian Circuits:0" : "Hamiltonian Circuits:" + pathFinder.hamiltonianCircuits.Count.ToString();
        graphUI.hamiltonianCircuitCount.text = "Current Path: ";
        graphUI.hamiltonianCurrentPath.text = "Path: ";

        //Set the line renderer on the first position
        if (pathFinder.hamiltonianCircuits.Count > 0)
        {
            currentHamiltonianIndex = 0;
            //Update positions count
            graphVisualisation.hamiltonianRenderer.positionCount = pathFinder.hamiltonianCircuits[currentHamiltonianIndex].Path.Count;
            graphVisualisation.hamiltonianRenderer.SetPositions(pathFinder.hamiltonianCircuits[currentHamiltonianIndex].NodePositions.ToArray());

            //Update hamiltonian renderer
            UpdateHamiltonianRenderer();

            //Update text
            graphUI.hamiltonianCircuitCount.text = "Current Path: 1/" + pathFinder.hamiltonianCircuits.Count.ToString();
            graphUI.hamiltonianCurrentPath.text = "Path: " + pathFinder.hamiltonianCircuits[currentHamiltonianIndex].ReturnPath();
        }
    }

    public void DisplayNextHamiltonianPath()
    {
        //If there are paths
        if (pathFinder.hamiltonianCircuits.Count > 0)
        {
            currentHamiltonianIndex += 1;
            if (currentHamiltonianIndex >= pathFinder.hamiltonianCircuits.Count)
                currentHamiltonianIndex = 0;

            //Update positions count
            graphVisualisation.hamiltonianRenderer.positionCount = pathFinder.hamiltonianCircuits[currentHamiltonianIndex].Path.Count;
            graphVisualisation.hamiltonianRenderer.SetPositions(pathFinder.hamiltonianCircuits[currentHamiltonianIndex].NodePositions.ToArray());

            //Update hamiltonian renderer
            UpdateHamiltonianRenderer();

            //Update text
            graphUI.hamiltonianCircuitCount.text = "Current Path: " + (currentHamiltonianIndex + 1).ToString() + "/" + pathFinder.hamiltonianCircuits.Count.ToString();
            graphUI.hamiltonianCurrentPath.text = "Path: " + pathFinder.hamiltonianCircuits[currentHamiltonianIndex].ReturnPath();
        }
    }

    public void DisplayPreviousHamiltonianPath()
    {
        //If there are paths
        if (pathFinder.hamiltonianCircuits.Count > 0)
        {
            currentHamiltonianIndex -= 1;
            if (currentHamiltonianIndex < 0)
                currentHamiltonianIndex = pathFinder.hamiltonianCircuits.Count - 1;

            //Update positions count
            graphVisualisation.hamiltonianRenderer.positionCount = pathFinder.hamiltonianCircuits[currentHamiltonianIndex].Path.Count;
            graphVisualisation.hamiltonianRenderer.SetPositions(pathFinder.hamiltonianCircuits[currentHamiltonianIndex].NodePositions.ToArray());

            //Update hamiltonian renderer
            UpdateHamiltonianRenderer();

            //Update text
            graphUI.hamiltonianCircuitCount.text = "Current Path: " + (currentHamiltonianIndex + 1).ToString() + "/" + pathFinder.hamiltonianCircuits.Count.ToString();
            graphUI.hamiltonianCurrentPath.text = "Path: " + pathFinder.hamiltonianCircuits[currentHamiltonianIndex].ReturnPath();
        }
    }

    public void UpdateHamiltonianRenderer()
    {
        //Can be changed later 
        GraphPath currPath = pathFinder.hamiltonianCircuits[currentHamiltonianIndex];
        int currentArrowCount = arrowList.Count;
        //Try to instantiate more if there isnt enough.
        for (int i = currentArrowCount; i < currPath.Path.Count; i++)
        {
            GameObject arrow = Instantiate(arrowPrefab);
            arrow.transform.parent = arrowParent.transform;
            arrow.SetActive(false);
            arrowList.Add(arrow);
        }

        //Disable all the arrows at the start.
        foreach (GameObject arrow in arrowList)
        {
            arrow.SetActive(false);
        }

        //Instantiate arrows
        for (int i = 1; i < currPath.Path.Count; i++)
        {
            //Updates position and orientation
            Vector3 curr = currPath.NodePositions[i];
            Vector3 prev = currPath.NodePositions[i - 1];
            Vector3 midPoint = Vector3.Lerp(curr, prev, 0.5f);
            Vector3 direction = (curr - prev).normalized;
            arrowList[i - 1].transform.localPosition = midPoint;
            arrowList[i - 1].transform.right = -direction;
            //Enable this arrow.
            arrowList[i - 1].SetActive(true);
        }
    }
}