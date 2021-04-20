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
            if (CGlobal.isEvent == false) // 오브젝트 1개만 있는 이벤트 맵의 경우에 오브젝트 발동 시 몹이 나오는 상황에선 true 
                CGlobal.isClear = true;
        }
    }
}
