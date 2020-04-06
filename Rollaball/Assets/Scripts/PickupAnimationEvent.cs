using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupAnimationEvent : MonoBehaviour
{
    public GameObject prefab;
    public bool isReset = false;
    private GameObject instance;
    private bool isSetup = false;
    Quaternion rotation;

    public void PrintEvent(string s)
    {
        isReset = true;
        SetActive(false);
    }

    void Awake()
    {
        if(!isSetup)
        {
            instance = Instantiate(prefab);
            rotation = instance.transform.rotation;
            instance.transform.SetParent(transform);
            instance.transform.localPosition = Vector3.zero;
            SetActive(false);
            isSetup = true;
        }
    }

    void LateUpdate()
    {
        instance.transform.rotation = rotation;
    }

    public void Reset()
    {
        isReset = false;
        SetActive(false);
    }

    public void SetActive(bool active)
    {
        //Debug.Log("Pickup SetActive");
        instance.SetActive(active);
    }

}
