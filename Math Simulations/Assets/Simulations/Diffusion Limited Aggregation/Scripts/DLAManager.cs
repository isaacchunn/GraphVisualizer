using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DLAManager : MonoBehaviour
{
    //Singleton Instance
    public static DLAManager Instance { get; private set; }

    [Header("Mesh References")]
    [SerializeField]
    private GameObject circlePrefab;
    [SerializeField]
    private GameObject walkerPrefab;
    [SerializeField]
    private GameObject pointParent;


    [Header("DLA")]
    [SerializeField] //visualization of points
    private List<GameObject> tree;

    [Header("Particle Sizes")]
    [SerializeField]
    private float pointSize = 0.1f;
    [SerializeField]
    private float walkerSize = 0.1f;
    [SerializeField]
    public float walkerSpeed = 0.002f;
    [SerializeField]
    public float walkerSpeedScale = 1.0f;
    [SerializeField]
    private float walkerSpawnAreaOffset = 0.5f;

    [Header("Color References")]
    [SerializeField]
    private Color startingPointColor = Color.grey;
    [SerializeField]
    private Color firstColor = Color.blue;
    [SerializeField]
    private Color secondColor = Color.green;
    [SerializeField]
    private Color thirdColor = Color.cyan;
    [SerializeField]
    private Color fourthColor = Color.red;
    [SerializeField]
    private float firstTime = 15f;
    [SerializeField]
    private float secondTime = 30f;
    [SerializeField]
    private float thirdTime = 45f;

    //Variables
    public Camera mainCamera { get; private set; }
    //Has point been spawned
    private bool pointSpawned = false;
    //Elapsed time
    public float elapsedTime;
    public float iterations;

    //Spawning algo
    [Header("Spawning algo")]
    public float xSpawn;
    public float ySpawn;
    public float spawnSpread = 1f;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        //Get the main camera once only at start
        mainCamera = Camera.main;
        //Start the tree creation
        CreateTree();
    }

    // Update is called once per frame
    void Update()
    {
        if (pointSpawned)
        {
            xSpawn += Time.deltaTime * spawnSpread;
            ySpawn += Time.deltaTime * spawnSpread;
            //Update time
            elapsedTime += Time.deltaTime;
            //if (Input.GetMouseButton(0))
            //    SpawnPointEdges();
        }
        //else
        //    CreateTreeAtMouseClick();


    }

    /*
     * Methods
     */

    //Creates the initial tree
    private void CreateTree()
    {
        //Instantiate the circlePrefab
        GameObject startingPoint = Instantiate(circlePrefab);
        Vector2 camPosition = new Vector2(mainCamera.transform.position.x, mainCamera.transform.position.y);
        //Position it in the middle of the camera
        startingPoint.transform.localPosition = camPosition;
        //Set parent
        startingPoint.transform.parent = pointParent.transform;
        //Edit scale
        startingPoint.transform.localScale = new Vector2(pointSize, pointSize);
        //Edit color 
        startingPoint.GetComponent<SpriteRenderer>().color = startingPointColor;
        //Add this obj in tree
        tree.Add(startingPoint);
        //Point spawned = true
        pointSpawned = true;
        //Start spawning points
        StartCoroutine(SpawnPoints());
    }

    private void CreateTreeAtMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Instantiate the circlePrefab
            GameObject startingPoint = Instantiate(circlePrefab);
            Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            //Vector2 camPosition = new Vector2(mainCamera.transform.position.x, mainCamera.transform.position.y);
            //Position it in the middle of the camera
            startingPoint.transform.localPosition = mousePosition;
            //Set parent
            startingPoint.transform.parent = pointParent.transform;
            //Edit scale
            startingPoint.transform.localScale = new Vector2(pointSize, pointSize);
            //Edit color 
            startingPoint.GetComponent<SpriteRenderer>().color = startingPointColor;
            //Add this obj in tree
            tree.Add(startingPoint);
            //Point spawned = true
            pointSpawned = true;
            //Start spawning points;
            //StartCoroutine(SpawnPoints());
        }
    }

    //Spawn a walker to start moving around the game world
    private void SpawnPoint()
    {
        if (!walkerPrefab)
            return;

        //create the gameObject
        GameObject walker = Instantiate(walkerPrefab);
        //Find a random point to generate an object first
        float bounds = mainCamera.orthographicSize - (walker.transform.localScale.x * 0.5f);
        ////Generate the x and y coordinates for spawning of object
        //float x = Random.Range(-bounds, bounds);
        //float y = Random.Range(-bounds, bounds);
        //Set parent
        walker.transform.parent = pointParent.transform;
        //Set the position and size of walker
        walker.transform.localScale = new Vector2(walkerSize, walkerSize);
        // Vector3 offset = (Random.onUnitSphere * walkerSpawnAreaOffset);
        Vector3 offset = RandomAround(mainCamera.transform.localPosition, bounds, mainCamera.orthographicSize);
        offset.z = 0;
        walker.transform.localPosition = offset;
    }

    private Vector3 RandomAround(Vector3 center, float minDist, float maxDist)
    {
        var v3 = Quaternion.AngleAxis(Random.Range(0.0f, 360.0f), Vector3.forward) * Vector3.up;
        v3 = v3 * Random.Range(minDist, maxDist);
        return center + v3;
    }



    //Spawn walker at edges
    private void SpawnPointEdges()
    {
        if (!walkerPrefab)
            return;

        //create the gameObject
        GameObject walker = Instantiate(walkerPrefab);
        //Find a random point to generate an object first
        float bounds = mainCamera.orthographicSize - (walker.transform.localScale.x * 0.5f);
        //Generate the x and y coordinates for spawning of object
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        //float x = Random.Range(-bounds, bounds);
        //float y = Random.Range(-bounds, bounds);
        //Set parent
        walker.transform.parent = pointParent.transform;
        //Set the position and size of walker
        walker.transform.localScale = new Vector2(walkerSize, walkerSize);
        walker.transform.localPosition = mousePosition;
        //.transform.localPosition = new Vector2(x, y);
    }

    //Add a walker to the tree
    public void AddWalkerToTree(Walker walker)
    {
        if (!tree.Contains(walker.gameObject) && walker)
        {
            tree.Add(walker.gameObject);
        }

    }

    //Returns if the tree contains this object
    public bool TreeContains(GameObject go)
    {
        return tree.Contains(go);
    }

    //Returns based on time
    public Color ReturnColor()
    {
        if (elapsedTime < firstTime)
            return firstColor;
        if (elapsedTime > firstTime && elapsedTime < secondTime)
            return secondColor;
        if (elapsedTime > secondTime && elapsedTime < thirdTime)
            return thirdColor;

        return fourthColor;
    }


    IEnumerator SpawnPoints()
    {
        for (int i = 0; i < iterations; i++)
        {
            SpawnPoint();
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {

        float bounds = Camera.main.orthographicSize - (walkerSize * 0.5f);
        Gizmos.color = Color.yellow;
        //Draw bounds
        //Vector3 topLeft = mainCamera.transform.localPosition - new Vector3(-bounds + walkerSpawnAreaOffset, bounds - walkerSpawnAreaOffset,0);
        //Vector3 topRight = mainCamera.transform.localPosition - new Vector3(bounds - walkerSpawnAreaOffset, bounds - walkerSpawnAreaOffset,0);
        //Vector3 bottomLeft = mainCamera.transform.localPosition - new Vector3(-bounds + walkerSpawnAreaOffset, -bounds + walkerSpawnAreaOffset,0);
        //Vector3 bottomRight = mainCamera.transform.localPosition - new Vector3(bounds - walkerSpawnAreaOffset, -bounds + walkerSpawnAreaOffset,0);


        Gizmos.DrawWireSphere(Camera.main.transform.localPosition, walkerSpawnAreaOffset);
    }
}
