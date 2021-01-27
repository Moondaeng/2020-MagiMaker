using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFlamethrowerFireController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            CController.instance.player.GetComponent<CPlayerPara>().DamagedDisregardDefence(1);
            Debug.Log("aaa");
        }
    }
}
