using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController2 : MonoBehaviour
{
    public GameObject cubecontroller2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void changeColor(Color a)
    {
        this.transform.GetComponent<MeshRenderer>().material.color = a;
    }
}
