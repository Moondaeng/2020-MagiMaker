using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseClearCondition : MonoBehaviour
{
    public Vector3 position;

    private void OnTriggerEnter(Collider coll)
    {
        Debug.Log("Use ClearCondition");

        if (coll.tag == "Player")
        {
            CGlobal.isClear = true;
            CCreateMap.instance.NotifyPortal();
        }
    }
}
