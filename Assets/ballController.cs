using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class ballController : MonoBehaviour
{
    private int aux = 0;
    private GameObject sphereTransform;
    // Start is called before the first frame update
    void Start()
    {
        EventsManager.instance.MoveHandTrigger += changeSize;
    }

    void Update()
    {
        aux += 1;
        if(aux == 250)
        {
            EventsManager.instance.OnUploadImagesTrigger(gameObject.GetInstanceID());
        }
    }

    private void changeSize(int id, Boolean isOpenHand)
    {
        if (isOpenHand)
        {
            transform.localScale = new Vector3(10f, 10f, 10f);
        } else
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        
    }
}
