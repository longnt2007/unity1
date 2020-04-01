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

    Animator m_Animator;
    private int colorState;

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

        m_Animator = gameObject.GetComponent<Animator>();
        colorState = 0;
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
                moveToPostion = new Vector3(hit.point.x, 0.5f, hit.point.z);
                bool isSpawned = SpawnPool(moveToPostion);
                if(isSpawned)
                {
                    spawnPosArray[currentSpawn++] = moveToPostion;
                }
            }

            /*
            if (m_Animator.GetBool("isIdle"))
            {
                Debug.Log("isIdle = True");
            }
            else
            {
                Debug.Log("isIdle = False");
            }
            */
        }

        if(currentSpawn > currentMove)
        {
            //this.transform.position = Vector3.MoveTowards(this.transform.position, spawnPosArray[currentMove], moveSpeed * Time.deltaTime);
            GameObject parent = GameObject.Find("PlayerPosition");
            parent.transform.position = Vector3.MoveTowards(parent.transform.position, spawnPosArray[currentMove], moveSpeed * Time.deltaTime);

            if (m_Animator.GetBool("isIdle"))
            {
                m_Animator.Rebind();
                m_Animator.SetBool("isIdle", false);
                m_Animator.Play("RunPlayer");
            }
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
        //Debug.Log("Got " + other.gameObject.tag);
        if(other.gameObject.CompareTag("Pick Up"))
        {
            //other.gameObject.SetActive(false);
            other.gameObject.Kill();
            count++;
            currentMove++;
            SetCountText();

            if(currentMove == currentSpawn)
            {
                m_Animator.Rebind();
                m_Animator.SetBool("isIdle", true);
                m_Animator.Play("IdlePlayer");
            }
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

    bool SpawnPool(Vector3 pos)
    {
        //GameObject pickup = ObjectPooler.SharedInstance.GetPooledObject(); 
        GameObject pickup = myPrefab.Spawn();
        if (pickup != null) {
            pickup.transform.position = moveToPostion;
            pickup.GetComponent<Animator>().SetInteger("ColorState", (colorState++)%3);
            Debug.Log("Spawn color: " + pickup.GetComponent<Animator>().GetInteger("ColorState"));
            pickup.SetActive(true);
            return true;
        }                
        return false;    
    }
}
