using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//Very bad 30 min visualizations
public class SierpinskiManager : MonoBehaviour
{
    [Header("Mesh References")]
    [SerializeField]
    private GameObject circleSprite;
    [SerializeField]
    private GameObject middleSprite;
    [SerializeField]
    private GameObject pointsParent;
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private GameObject backPlane;

    [Header("Triangle Visualizations")]
    [SerializeField] //For visualizaition
    private List<GameObject> trianglePoints;
    [SerializeField]
    private bool generateTriangleSprite = false;
    [SerializeField]
    private List<Vector2> trianglePositions;
    [SerializeField]
    private float offsetFromMiddle = 1.0f;
    [SerializeField]
    private float startSphereSize = 0.5f;

    [Header("Visualizations")]
    [SerializeField]
    private float numberOfIterations;
    [SerializeField]
    private float waitTime = 0.2f;
    [SerializeField]
    private Color startingPointColor = Color.red;
    [SerializeField]
    private float middlePointSize = 0.1f;

    [Header("Misc Visualizations")]
    [SerializeField]
    private bool updateLivePlaneSize;

    //Variables
    private float currentSize;

    // Start is called before the first frame update
    void Start()
    {
        //Resize the backplane
        ResizeBackPlane();
        //Generate the points;
        GenerateTrianglePoints();
        //Start generating points on the map
        StartCoroutine(GeneratePointsInBetween());
        //GeneratePointsInBetweenInstant();
    }

    // Update is called once per frame
    void Update()
    {
        if (updateLivePlaneSize)
            ResizeBackPlane();
    }

    //Generates the starting triangle points
    void GenerateTrianglePoints()
    {
        //Clamp the values based on camera size
        offsetFromMiddle = Mathf.Min(offsetFromMiddle, mainCamera.orthographicSize - (circleSprite.transform.localScale.x * 0.5f));

        //top point;
        Vector2 point1 = new Vector2(0, offsetFromMiddle);
        //left point;
        Vector2 point2 = new Vector2(-offsetFromMiddle, -offsetFromMiddle);
        //right point
        Vector2 point3 = new Vector2(offsetFromMiddle, -offsetFromMiddle);

        //Add into triangle positions
        trianglePositions.Add(point1);
        trianglePositions.Add(point2);
        trianglePositions.Add(point3);

        //Generate the Sprites
        if (generateTriangleSprite)
        {
            for (int i = 0; i < 3; ++i)
            {
                GameObject obj = Instantiate(circleSprite);
                obj.transform.localPosition = trianglePositions[i];
                obj.transform.localScale = new Vector2(startSphereSize, startSphereSize);
                obj.transform.parent = this.gameObject.transform;

                //Set the point number
                TextMeshPro text = obj.GetComponentInChildren<TextMeshPro>();
                text.text = "Point " + (i + 1).ToString();
            }
        }

    }


    //Handles the simulation frame by frame
    IEnumerator GeneratePointsInBetween()
    {
        //Choose a random point first
        float x = Random.Range(-offsetFromMiddle + 0.01f, offsetFromMiddle - 0.01f);
        float y = Random.Range(-offsetFromMiddle + 0.01f, offsetFromMiddle - 0.01f);

        //Generate a startingPoint;
        GameObject startingPoint = Instantiate<GameObject>(middleSprite);
        startingPoint.transform.parent = pointsParent.transform;
        startingPoint.transform.localScale = new Vector2(middlePointSize, middlePointSize);
        startingPoint.transform.localPosition = new Vector2(x, y);
        //Set a different color;
        startingPoint.GetComponent<SpriteRenderer>().color = startingPointColor;
        //Set currentPosition first;
        Vector2 currentPosition = new Vector2(x, y);

        for (int i = 0; i < numberOfIterations; ++i)
        {
            //Choose a random point, and plot in between currentpoint and the next point;
            int point = Random.Range(0, 3);
            Vector2 newMidPoint = Vector2.Lerp(currentPosition, trianglePositions[point], 0.5f);
            //Update currentPosition;
            currentPosition = newMidPoint;

            //Generate midPoint
            GameObject midPoint = Instantiate<GameObject>(middleSprite);
            midPoint.transform.parent = pointsParent.transform;
            midPoint.transform.localScale = new Vector2(middlePointSize, middlePointSize);
            midPoint.transform.localPosition = currentPosition;

            yield return new WaitForSeconds(waitTime);
            //yield return null;
        }
    }

    //Handles the simulation frame by frame
    void GeneratePointsInBetweenInstant()
    {
        //Choose a random point first
        float x = Random.Range(-offsetFromMiddle + 0.01f, offsetFromMiddle - 0.01f);
        float y = Random.Range(-offsetFromMiddle + 0.01f, offsetFromMiddle - 0.01f);

        //Generate a startingPoint;
        GameObject startingPoint = Instantiate<GameObject>(middleSprite);
        startingPoint.transform.parent = pointsParent.transform;
        startingPoint.transform.localScale = new Vector2(middlePointSize, middlePointSize);
        startingPoint.transform.localPosition = new Vector2(x, y);

        //Set a different color;
        startingPoint.GetComponent<SpriteRenderer>().color = startingPointColor;
        //Set currentPosition first;
        Vector2 currentPosition = startingPoint.transform.localPosition;

        for (int i = 0; i < numberOfIterations; ++i)
        {
            //Choose a random point, and plot in between currentpoint and the next point;
            int point = Random.Range(0, 3);
            Vector2 newMidPoint = Vector2.Lerp(currentPosition, trianglePositions[point], 0.5f);
            //Update currentPosition;
            currentPosition = newMidPoint;

            //Generate midPoint
            GameObject midPoint = Instantiate<GameObject>(middleSprite);
            midPoint.transform.parent = pointsParent.transform;
            midPoint.transform.localScale = new Vector2(middlePointSize, middlePointSize);
            midPoint.transform.localPosition = currentPosition;
        }
    }

    //Resizes the initial back plane to camera size
    void ResizeBackPlane()
    {
        if (backPlane && mainCamera)
        {
            float orthographicSize = mainCamera.orthographicSize * 2;
            if (currentSize != orthographicSize)
            {
                backPlane.transform.localScale = new Vector2(orthographicSize, orthographicSize);
                currentSize = orthographicSize;
            }

        }
    }
}
