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

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        jump = new Vector3(0.0f, 2.0f, 0.0f);
        SetCountText();
        winText.text = "";

        moveToPostion = this.transform.position;
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
                Debug.Log(hit.point.x.ToString("F3") + "/" + hit.point.y.ToString("F3"));
                
                //if(moveToPostion == this.transform.position)
                {
                    moveToPostion = hit.point;
                }
            }
        }

        if(this.transform.position != moveToPostion)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, moveToPostion, moveSpeed * Time.deltaTime );
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
            OnJump();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Pick Up"))
        {
            other.gameObject.SetActive(false);
            count++;
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
