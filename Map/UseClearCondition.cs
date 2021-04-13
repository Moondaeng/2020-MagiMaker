using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseClearCondition : MonoBehaviour
{
    public Vector3 position;
    GameObject player;

    private void OnTriggerEnter(Collider coll)
    {
        player = GameObject.FindGameObjectWithTag("Player");

        Debug.Log("Use ClearCondition");

        if (coll.tag == "Player")
        {
            CGlobal.isClear = true;
        }
    }
}
