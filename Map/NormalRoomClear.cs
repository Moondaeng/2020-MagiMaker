using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalRoomClear : MonoBehaviour
{
    public Vector3 position;
    GameObject player;

    public void OnTriggerEnter(Collider coll)
    {
        player = GameObject.FindGameObjectWithTag("Player");

        Debug.Log("NormalRoomClear");

        if (coll.tag == "Player")
        {
            CGlobal.isClear = true;
        }
    }
}
