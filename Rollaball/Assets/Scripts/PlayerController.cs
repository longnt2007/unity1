using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public Vector3 jump;
    public float jumpForce = 2.0f;
    public bool isGround;
    private Rigidbody rb;
    public Text countText;
    public Text winText;
    private int count;

    public float raylength;
    public LayerMask layerMask;
    public Vector3 moveToPostion;
    public float moveSpeed;

    public GameObject myPrefab;
    private int maxSpawn;
    private int currentMove;
    private int currentSpawn;
    private Vector3[] spawnPosArray;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        jump = new Vector3(0.0f, 2.0f, 0.0f);
        SetCountText();
        winText.text = "";

        moveToPostion = this.transform.position;
        maxSpawn = 9999;
        currentMove = 0;
        currentSpawn = 0;
        spawnPosArray = new Vector3[maxSpawn];
    }

    void OnCollisionStay()
    {
        isGround = true;
    }

    void OnJump()
    {
        rb.AddForce(jump * jumpForce, ForceMode.Impulse);
        isGround = false;
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            //Debug.Log("Mouse click");
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray,out hit, raylength, layerMask))
            {
                //Debug.Log("Spawn:" + hit.point.x.ToString("F3") + "/" + hit.point.y.ToString("F3"));
                
                //if(moveToPostion == this.transform.position)
                {
                    moveToPostion = new Vector3(hit.point.x, 0.5f, hit.point.z);
                    spawnPosArray[currentSpawn++] = moveToPostion;
                    GameObject spawnPickup = Instantiate(myPrefab, new Vector3(hit.point.x, 0.5f, hit.point.z), Quaternion.identity);
                    //spawnPickup.gameObject.tag = "Pick Up";
                    //spawnPickup.GetComponent<Collider>().enabled = false;
                    //spawnPickup.GetComponent<Collider>().enabled = true;
                }
            }
        }

        //if(this.transform.position != moveToPostion)
        //{
            //this.transform.position = Vector3.MoveTowards(this.transform.position, moveToPostion, moveSpeed * Time.deltaTime );
        //}

        if(currentSpawn > currentMove)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, spawnPosArray[currentMove], moveSpeed * Time.deltaTime );
        }
    }

    void FixedUpdate()
    {
        float moveH = Input.GetAxis("Horizontal");
        float moveV = Input.GetAxis("Vertical");

        Vector3 mov = new Vector3 (moveH, 0.0f, moveV);

        rb.AddForce(mov * speed);

        if((Input.GetKeyDown(KeyCode.Space) || count >= 12) && isGround)
        {
            //OnJump();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Got " + other.gameObject.tag);
        if(other.gameObject.CompareTag("Pick Up"))
        {
            other.gameObject.SetActive(false);
            count++;
            currentMove++;
            SetCountText();
        }
    }

    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();
        if(count >= 12)
        {
            winText.text = "You Win !";
        }
    }
}
