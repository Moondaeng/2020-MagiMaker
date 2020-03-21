using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CProjectileSkill : MonoBehaviour
{
    public GameObject fireballObject;
    public float fireballSpeed = 30;

    public void Fireball(GameObject user, Vector3 targetPos)
    {
        var userStat = user.GetComponent<CharacterPara>();

        var fireball = Instantiate(fireballObject, user.transform.position, user.transform.rotation);
        fireball.tag = user.tag;
        fireball.transform.position =
            Vector3.MoveTowards(fireball.transform.position, targetPos, fireballSpeed * Time.deltaTime);
    }

}
