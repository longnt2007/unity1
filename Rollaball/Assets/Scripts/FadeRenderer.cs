using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeRenderer : MonoBehaviour
{
    public bool isSpawn = false;
    private float time;
    private float spawnTime = 2.0f;
    // Start is called before the first frame update
    void Start()
    {
        //foreach (Transform child in transform)
        //{
            //Debug.Log("Child: " + child.name);
            //Renderer _rend = child.GetComponent<Renderer>();
            //_rend.material.color = new Vector4(_rend.material.color.r, _rend.material.color.g, _rend.material.color.b, 0.1f);
        //}
    }

    // Update is called once per frame
    void Update()
    {
        if(isSpawn)
        {
            foreach (Transform child in transform)
            {
                Renderer _rend = child.GetComponent<Renderer>();
                _rend.material.color = new Vector4(_rend.material.color.r, _rend.material.color.g, _rend.material.color.b, Mathf.Lerp(0, 1.0f, (Time.time - time) / spawnTime));
            }

            if(Time.time > (time + spawnTime))
            {
                isSpawn = false;
            }
        }
    }

    public void Spawn()
    {
        isSpawn = true;
        time = Time.time;
    }
}
