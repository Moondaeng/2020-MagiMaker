using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CEventManager : MonoBehaviour
{
    public UnityEvent _eventEvent;

    public static CEventManager _instance;
    // Start is called before the first frame update
    void Start()
    {
        if (_instance == null)
            _instance = this;
    }

    public void InvokeEvent()
    {
        _eventEvent.Invoke();
    }
}
