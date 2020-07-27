using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalRoomClear : MonoBehaviour
{
    public Vector3 position;
    GameObject player;

    private void OnCollisionEnter(Collision coll)
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (coll.collider.tag == "Player")
        {
            CGlobal.isClear = true;
        }
    }
}
