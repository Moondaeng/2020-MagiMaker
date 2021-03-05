using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CEventRoom0_5Portal : MonoBehaviour
{
    private GameObject _npc;
    // Start is called before the first frame update
    void Start()
    {
        _npc = GameObject.FindGameObjectWithTag("NPC");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            other.transform.position = _npc.transform.position + new Vector3(1, 0, 0);
        }
    }
}
