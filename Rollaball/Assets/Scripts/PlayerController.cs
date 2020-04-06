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

    //Transform malcom;
    Animator m_malcomAnimator;
    Quaternion malcomOrginalRotation;

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

        m_malcomAnimator = GameObject.Find("malcolm").GetComponent<Animator>();
        Transform malcom = GameObject.Find("malcolm").transform;
        malcomOrginalRotation = malcom.rotation;
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
        }

        //Debug.Log(currentSpawn + "/" + currentMove);
        if(currentSpawn > currentMove)
        {
            //this.transform.position = Vector3.MoveTowards(this.transform.position, spawnPosArray[currentMove], moveSpeed * Time.deltaTime);
            GameObject parent = GameObject.Find("PlayerPosition");
            parent.transform.position = Vector3.MoveTowards(parent.transform.position, spawnPosArray[currentMove], moveSpeed * Time.deltaTime);

            /*
            if (m_Animator.GetBool("isIdle"))
            {
                m_Animator.Rebind();
                m_Animator.SetBool("isIdle", false);
                m_Animator.Play("RunPlayer");
            }
            */

            if (m_malcomAnimator.GetBool("isIdle"))
            {
                m_malcomAnimator.Rebind();
                m_malcomAnimator.SetBool("isIdle", false);
                m_malcomAnimator.Play("Walking");

                Transform malcom = GameObject.Find("malcolm").transform;
                Vector3 relativePos = spawnPosArray[currentMove] - this.transform.position;
                Quaternion rotation = Quaternion.LookRotation(relativePos);
                malcom.rotation = rotation;
            }
        }

        GameObject obj = GameObject.FindGameObjectWithTag("PickupExit");
        if(obj != null)
        {
            ResetPickup(obj);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Got " + other.gameObject.tag);
        if(other.gameObject.CompareTag("Pick Up"))
        {
            //other.gameObject.SetActive(false);
            //other.gameObject.Kill();
            other.gameObject.GetComponent<BoxCollider>().isTrigger = false;
            other.gameObject.GetComponent<Animator>().Rebind();
            other.gameObject.GetComponent<Animator>().SetBool("isActive", false);
            other.gameObject.GetComponent<Animator>().Play("PickupFadeout");
            other.gameObject.tag = "PickupExit";
            count++;
            currentMove++;
            SetCountText();

            if(currentMove == currentSpawn)
            {
                //m_Animator.Rebind();
                //m_Animator.SetBool("isIdle", true);
                //m_Animator.Play("IdlePlayer");

                m_malcomAnimator.Rebind();
                m_malcomAnimator.SetBool("isIdle", true);
                m_malcomAnimator.Play("Idle");
                
                Transform malcom = GameObject.Find("malcolm").transform;
                float currentRotY = malcom.rotation.eulerAngles.y;
                malcom.localRotation = Quaternion.Euler(0, currentRotY, 0);
            }
            else
            {
                Transform malcom = GameObject.Find("malcolm").transform;
                Vector3 relativePos = spawnPosArray[currentMove] - this.transform.position;
                Quaternion rotation = Quaternion.LookRotation(relativePos);
                malcom.rotation = rotation;
            }
        }
    }

    void LateUpdate()
    {
        this.transform.localRotation = Quaternion.Euler(new Vector3(0,0,0));
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
            //pickup.GetComponent<Animator>().SetInteger("ColorState", (colorState++)%3);
            pickup.GetComponent<Animator>().SetInteger("ColorState", (Random.Range(0,100)%3));
            pickup.GetComponent<Animator>().SetBool("isActive", true);
            //Debug.Log("Spawn color: " + pickup.GetComponent<Animator>().GetInteger("ColorState"));
            pickup.SetActive(true);
            return true;
        }                
        return false;    
    }

    private void ResetPickup(GameObject obj)
    {
        if(obj.gameObject.GetComponent<PickupAnimationEvent>().isReset)
        {
            obj.gameObject.GetComponent<PickupAnimationEvent>().Reset();
            obj.gameObject.GetComponent<BoxCollider>().isTrigger = true;
            obj.gameObject.gameObject.tag = "Pick Up";
            obj.gameObject.SetActive(false);
            obj.gameObject.Kill();
            Debug.Log("Reset Pickups");
        }
    }
}
