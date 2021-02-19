using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CEventBulletTrigger : CEventBulletController
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "WALL" || other.gameObject.tag == "TILE")
        {
            hideObject();
        }

        if (other.tag == "Player")
        {
            CController.instance.player.GetComponent<CPlayerPara>().DamagedDisregardDefence(bulletDamage);
            Debug.Log("Bullet");
            hideObject();
        }
    }

    void hideObject()
    {
        transform.parent.gameObject.SetActive(false);
    }
}
