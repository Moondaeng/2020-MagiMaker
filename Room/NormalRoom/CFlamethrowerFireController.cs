using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFlamethrowerFireController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            CController.instance.player.GetComponent<CPlayerPara>().DamagedDisregardDefence(100);
            Debug.Log("aaa");
        }
    }
}
