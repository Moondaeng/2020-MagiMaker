using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFountainItemTrigger : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("FountainItemTrigger");
        if (other.tag == "Player")
            GetReward();
        else if (other.tag == "WALL" || other.tag == "TILE")
            Destroy(gameObject);
    }

    public virtual void GetReward() //코인과 원소, 아이템 각각 다른 함수 구성위함.
    {

    }
}
