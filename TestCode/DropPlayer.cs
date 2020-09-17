using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropPlayer : MonoBehaviour
{
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.name == "Restart")
        {
            transform.position = new Vector3(0f, 0f, 0f);
        }
    }
}