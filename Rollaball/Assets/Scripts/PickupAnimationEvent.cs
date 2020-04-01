using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupAnimationEvent : MonoBehaviour
{
    public bool isReset = false;
    public void PrintEvent(string s)
    {
        //Debug.Log("PrintEvent: " + s + " called at: " + Time.time);
        isReset = true;
    }
}
