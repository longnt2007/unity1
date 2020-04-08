using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    private Vector3 startPosition;
    private bool isDie = false;
    private IEnumerator coroutine;
    private GameObject malcom;
    public GameObject spawnEffect;
    private GameObject spawnInstance;
    Quaternion spawnrotation;

    public Button playButton;
    public Button optionButton;
    public Button exitButton;
    public Button backButton;
    private int gameState;
    private GameObject optionMenu;
    private GameObject mainMenu;
    private GameObject cutScene;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        jump = new Vector3(0.0f, 2.0f, 0.0f);
        SetCountText();
        countText.gameObject.SetActive(false);
        winText.text = "";

        startPosition = moveToPostion = this.transform.position;
        maxSpawn = 9999;
        currentMove = 0;
        currentSpawn = 0;
        spawnPosArray = new Vector3[maxSpawn];

        m_Animator = gameObject.GetComponent<Animator>();
        colorState = 0;

        m_malcomAnimator = GameObject.Find("malcolm").GetComponent<Animator>();
        malcom = GameObject.Find("malcolm");
        malcomOrginalRotation = malcom.transform.rotation;

        spawnInstance = Instantiate(spawnEffect);
        spawnrotation = spawnInstance.transform.rotation;
        spawnInstance.transform.SetParent(transform);
        spawnInstance.transform.localPosition = Vector3.zero;
        spawnInstance.SetActive(false);

		Button btnPlay = playButton.GetComponent<Button>();
		btnPlay.onClick.AddListener(OnClickPlayButton);
		Button btnOption = optionButton.GetComponent<Button>();
		optionButton.onClick.AddListener(OnClickOptionButton);
		Button btnExit = exitButton.GetComponent<Button>();
		exitButton.onClick.AddListener(OnClickExitButton);
		Button btnBack = backButton.GetComponent<Button>();
		backButton.onClick.AddListener(OnClickBackButton);

        gameState = 0;
        mainMenu = GameObject.Find("MainMenu");
        mainMenu.SetActive(true);
        optionMenu = GameObject.Find("OptionMenu");
        optionMenu.SetActive(false);
        cutScene = GameObject.Find("Timeline");
        cutScene.SetActive(false);
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
        if(gameState != 1)
            return;

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

        if(currentSpawn > currentMove)
        {
            //this.transform.position = Vector3.MoveTowards(this.transform.position, spawnPosArray[currentMove], moveSpeed * Time.deltaTime);
            GameObject parent = GameObject.Find("PlayerPosition");
            parent.transform.position = Vector3.MoveTowards(parent.transform.position, spawnPosArray[currentMove], moveSpeed * Time.deltaTime);
            winText.text = "";
            isDie = false;

            if (m_malcomAnimator.GetBool("isIdle"))
            {
                m_malcomAnimator.Rebind();
                m_malcomAnimator.SetBool("isIdle", false);
                m_malcomAnimator.Play("Walking");

                Vector3 relativePos = spawnPosArray[currentMove] - this.transform.position;
                Quaternion rotation = Quaternion.LookRotation(relativePos);
                malcom.transform.rotation = rotation;
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
            int color = other.gameObject.GetComponent<Animator>().GetInteger("ColorState");
            //Debug.Log("OnTriggerEnter " + color);
            if(color == 1)
            {
                other.gameObject.GetComponent<PickupAnimationEvent>().SetActive(true);
                isDie = true;
                ResetPlayer();
            }

            other.gameObject.GetComponent<BoxCollider>().isTrigger = false;
            other.gameObject.GetComponent<Animator>().Rebind();
            other.gameObject.GetComponent<Animator>().SetBool("isActive", false);
            other.gameObject.GetComponent<Animator>().Play("PickupFadeout");
            other.gameObject.tag = "PickupExit";
            count++;
            if(isDie)
                currentMove = currentSpawn;
            else
                currentMove++;
            SetCountText();

            if(currentMove == currentSpawn)
            {
                m_malcomAnimator.Rebind();
                m_malcomAnimator.SetBool("isIdle", true);
                m_malcomAnimator.Play("Idle");
                
                //Transform malcom = GameObject.Find("malcolm").transform;
                float currentRotY = malcom.transform.rotation.eulerAngles.y;
                malcom.transform.localRotation = Quaternion.Euler(0, currentRotY, 0);
            }
            else
            {
                //Transform malcom = GameObject.Find("malcolm").transform;
                Vector3 relativePos = spawnPosArray[currentMove] - this.transform.position;
                Quaternion rotation = Quaternion.LookRotation(relativePos);
                malcom.transform.rotation = rotation;
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
        if(count >= 12 && !isDie)
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
            int color = Random.Range(0,100)%3;
            pickup.GetComponent<Animator>().SetInteger("ColorState", color);
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

    private void ResetPlayer()
    {
        print("ResetPlayer: " + Time.time + " seconds");
        GameObject parent = GameObject.Find("PlayerPosition");
        parent.transform.position = startPosition;
        currentMove = currentSpawn;
        winText.text = "You Die !";
        count = 0;
        //malcom.transform.localPosition = new Vector3(999,999,999);
        malcom.GetComponent<FadeRenderer>().Spawn();
        //spawnInstance.SetActive(true); //disable smoke and replace with fadein texture
        coroutine = WaitAndRespawn(2.0f);
        StartCoroutine(coroutine);        
    }

    private IEnumerator WaitAndRespawn(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        spawnInstance.SetActive(false);
        //malcom.SetActive(true);
        m_malcomAnimator.Rebind();
        m_malcomAnimator.SetBool("isIdle", true);
        m_malcomAnimator.Play("Idle");
        GameObject parent = GameObject.Find("PlayerPosition");
        malcom.transform.localPosition = new Vector3(0,-0.5f,0);
        print("Respwan: " + Time.time + " seconds");
    }    

    void OnClickPlayButton()
    {
        Debug.Log("OnClickPlayButton");
        mainMenu.SetActive(false);
        gameState = 1;
        cutScene.SetActive(true);
    }
    void OnClickOptionButton()
    {
        Debug.Log("OnClickOptionButton");
        optionMenu.SetActive(true);
        mainMenu.SetActive(false);
        gameState = 2;
    }
    void OnClickExitButton()
    {
        Debug.Log("OnClickExitButton");
        Application.Quit();
    }
    void OnClickBackButton()
    {
        Debug.Log("OnClickBackButton");
        mainMenu.SetActive(true);
        optionMenu.SetActive(false);
        gameState = 0;
    }
}
