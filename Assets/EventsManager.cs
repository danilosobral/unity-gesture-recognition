using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventsManager : MonoBehaviour
{
    public static EventsManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(this);
        }
    }

    public event Action<int, Boolean> OpenHandTrigger;
    public event Action<int> CloseHandTrigger;

    public void OnOpenHandTrigger(int instanceId, Boolean isOpenHand)
    {
        OpenHandTrigger?.Invoke(instanceId, isOpenHand);
    }

    public void OnCloseHandTrigger(int instanceId)
    {
        CloseHandTrigger?.Invoke(instanceId);
    }
}
