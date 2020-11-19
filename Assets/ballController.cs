using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class ballController : MonoBehaviour
{
    private GameObject sphereTransform;
    // Start is called before the first frame update
    void Start()
    {
        EventsManager.instance.OpenHandTrigger += increaseSize;
        EventsManager.instance.CloseHandTrigger += decreaseSize;
    }

    private void increaseSize(int id, Boolean isOpenHand)
    {
        if (isOpenHand)
        {
            transform.localScale = new Vector3(10f, 10f, 10f);
        } else
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        
    }

    private void decreaseSize(int id)
    {
        transform.localScale = new Vector3(1f, 1f, 1f);
    }
}
