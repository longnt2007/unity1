using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    //public bool enablewhenstart;
    public Vector3 move;
    public float rotationspeed = 0.0f;
    // Start is called before the first frame update
    public GameObject cubecontroller2;
    private Quaternion childFixedRotation;
    private Vector3 childFixedPostion;

    void Start()
    {
        //this.transform.position = new Vector3(1.0f, 1.0f, 1.0f);
        //this.GetComponent<MeshRenderer>().enabled = enablewhenstart;
        childFixedRotation = cubecontroller2.transform.rotation;
        childFixedPostion = cubecontroller2.transform.position;
        rotationspeed = 20.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Color childColor = new Color(Random.value, Random.value, Random.value, 1.0f);;
            //GameObject transcube2 = GameObject.Find("Cube2");
            //transcube2.GetComponent<MeshRenderer>().material.color = new Color(Random.value, Random.value, Random.value, 1.0f);
            //transcube2.GetComponent<CubeController2>().changeColor(childColor);
            //this.GetComponent<MeshRenderer>().material.color = new Color(Random.value, Random.value, Random.value, 1.0f);
            cubecontroller2.GetComponent<CubeController2>().changeColor(childColor);
        }

        this.transform.Rotate(rotationspeed * Time.deltaTime, 0, 0, Space.Self);
        cubecontroller2.transform.rotation = childFixedRotation;
        cubecontroller2.transform.position = childFixedPostion;
    }
}
