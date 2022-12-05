using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walker : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField]
    private float movementSpeed; //Speed in which this object moves
    [SerializeField] // Is this particle already stuck to another particle
    private bool isStuck;

    //rb movement
    private Rigidbody2D rb;

    //Get the orthographic and bounds
    private float orthographicSize;
    private float bounds;
    public int collisions;

    //Linked reference to another's that is stuck
    public GameObject stuckObj;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        movementSpeed = DLAManager.Instance.walkerSpeed;
        //Set active false, only add when it stucks with another
        //GetComponent<SpriteRenderer>().enabled = false;
        //Get the size and set the bounds
        orthographicSize = DLAManager.Instance.mainCamera.orthographicSize;
        bounds = orthographicSize - (transform.localScale.x * 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isStuck)
            HandleMovement();

    }

    private void HandleMovement()
    {
        Vector3 currentLocation = transform.localPosition;
        currentLocation.x += Random.Range(-movementSpeed, movementSpeed);
        currentLocation.y += Random.Range(-movementSpeed, movementSpeed);

        //Clamp
        currentLocation.x = Mathf.Clamp(currentLocation.x, -bounds, bounds);
        currentLocation.y = Mathf.Clamp(currentLocation.y, -bounds, bounds);

        //Calculate the vector position towwards new desired vector
        Vector2 directionVector = (currentLocation - transform.localPosition).normalized;
        rb.AddForce(directionVector * DLAManager.Instance.walkerSpeedScale);
    }

    ////If collided with something with a collider, do the necessary checks to update
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (stuckObj)
    //        return;

    //    if (collision.gameObject.tag == "Walker")
    //    {
    //        //Return if tree does not have this
    //        if (!DLAManager.Instance.TreeContains(collision.gameObject))
    //            return;

    //        //Get walker
    //        Walker otherWalker = collision.gameObject.GetComponent<Walker>();
    //        if (otherWalker)
    //        {
    //            //Set each other as references
    //            stuckObj = otherWalker.gameObject;
    //            if (!otherWalker.GetComponent<Walker>().stuckObj)
    //                otherWalker.GetComponent<Walker>().stuckObj = this.gameObject;
    //        }
    //        //GetComponent<SpriteRenderer>().enabled = true;
    //        GetComponent<SpriteRenderer>().color = DLAManager.Instance.ReturnColor();
    //        isStuck = true;
    //        DLAManager.Instance.AddWalkerToTree(this);
    //        //collisions += 1;
    //    }
    //    else if (collision.gameObject.tag == "TreePoint")
    //    {
    //        if (!stuckObj)
    //        {
    //            stuckObj = collision.gameObject;
    //            //GetComponent<SpriteRenderer>().enabled = true;
    //            //GetComponent<SpriteRenderer>().color = ColorColor.yellow;
    //            GetComponent<SpriteRenderer>().color = DLAManager.Instance.ReturnColor();
    //            isStuck = true;
    //            DLAManager.Instance.AddWalkerToTree(this);
    //            //collisions += 1;
    //        }

    //    }

    //    ////sAVE PERFORMANCE?
    //    //if (collisions > DLAManager.Instance.maxCollisions)
    //    //{
    //    //    Destroy(GetComponent<CircleCollider2D>());
    //    //    Destroy(GetComponent<Rigidbody2D>());
    //    //}

    //}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (stuckObj)
        {
            return;
        }

        if (collision.gameObject.tag == "Walker")
        {
            //Return if tree does not have this
            if (!DLAManager.Instance.TreeContains(collision.gameObject))
                return;
            //Get walker
            Walker otherWalker = collision.gameObject.GetComponent<Walker>();
            if (otherWalker)
            {
                //Set each other as references
                stuckObj = otherWalker.gameObject;
                if (!otherWalker.GetComponent<Walker>().stuckObj)
                    otherWalker.GetComponent<Walker>().stuckObj = this.gameObject;
            }
            GetComponent<SpriteRenderer>().enabled = true;
            GetComponent<SpriteRenderer>().color = DLAManager.Instance.ReturnColor();
            isStuck = true;
            DLAManager.Instance.AddWalkerToTree(this);

        }
        else if (collision.gameObject.tag == "TreePoint")
        {
            if (!stuckObj)
            {
                stuckObj = collision.gameObject;
                GetComponent<SpriteRenderer>().enabled = true;
                GetComponent<SpriteRenderer>().color = DLAManager.Instance.ReturnColor();
                isStuck = true;
                DLAManager.Instance.AddWalkerToTree(this);

            }

        }
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
    }

}
