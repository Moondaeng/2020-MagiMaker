using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFountainItemTrigger : MonoBehaviour
{
    protected CPlayerPara _playerPara;
    public virtual void Start()
    {
        _playerPara = CController.instance.player.GetComponent<CPlayerPara>();
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            GetReward();
        else if (other.tag == "WALL" || other.tag == "TILE")
        {
            Destroy(gameObject);
            Debug.Log("Wall or Tile");
        }
    }

    public virtual void GetReward() //코인과 원소, 아이템 각각 다른 함수 구성위함.
    {

    }
}
