using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GraphVisualisation
{
    [Header("Node Visuals")]
    public Material selectedNodeMaterial;
    public Material nodeMaterial;
    public Material edgeMaterial;

    [Header("Node Properties")]
    public float nodeSpawnSize = 1f;
    public float selectedNodeSizeFactor = 1.2f;

    [Header("Universal Line Renderer")]
    [SerializeField]
    private bool showLineRenderer;
    [SerializeField]
    public LineRenderer hamiltonianRenderer;
    [SerializeField]
    public LineRenderer eulerRenderer;
}
