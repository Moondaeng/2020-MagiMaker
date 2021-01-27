using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CEventRoomMonsterCheck : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (CGlobal.CheckMonsterCount)
            StartCoroutine("CheckMonsterCount");
    }

    private void Start()
    {
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        foreach (GameObject monster in monsters)
            monster.SetActive(false);
    }

    IEnumerator CheckMonsterCount()
    {
        yield return new WaitForSeconds(1.0f); //오브젝트 삭제 대기

        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");

        if (monsters.Length == 0) //몬스터 오브젝트 없으면 이벤트 종료
        {
            CGlobal.isEvent = false;
            CCreateMap.instance.NotifyPortal();
        }

        CGlobal.CheckMonsterCount = false;
    }
}
