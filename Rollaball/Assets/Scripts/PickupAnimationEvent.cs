﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupAnimationEvent : MonoBehaviour
{
    public GameObject prefab;

    public bool isReset = false;

    private GameObject instance;
    private bool isSetup = false;

    public void PrintEvent(string s)
    {
        //Debug.Log("PrintEvent: " + s + " called at: " + Time.time);
        isReset = true;
        instance.SetActive(false);
    }

    void Awake()
    {
        if(!isSetup)
        {
            instance = Instantiate(prefab);
            instance.transform.SetParent(transform);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localScale = Vector3.one;
            instance.transform.localEulerAngles = Vector3.zero;
            isSetup = true;
        }
        instance.SetActive(true);
    }

    public void Reset()
    {
        isReset = false;
        instance.SetActive(true);
    }
}
