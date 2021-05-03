using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMonsterCheck : MonoBehaviour
{
    public static CMonsterCheck instance = null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        foreach (GameObject monster in monsters)
            monster.SetActive(false);
    }

    public void ForceDeadMonster()
    {        //debug
        Debug.Log("ForceDeadMonster");
        StartCoroutine("ForceDeadMonsterCoru");
    }

    IEnumerator ForceDeadMonsterCoru()
    {
        yield return new WaitForSeconds(2.0f);

        GameObject _monsterGroup = GameObject.Find("MonsterGroup");

        for (int i = 0; i < _monsterGroup.transform.childCount; i++)
            _monsterGroup.transform.GetChild(i).gameObject.GetComponent<CharacterPara>().deadEvent.Invoke();

        CGlobal.isEvent = false;
        CCreateMap.instance.NotifyPortal();
    }

    public void CheckMonsterCountZero()
    {
        Debug.Log("invoke");
        StartCoroutine("CheckMonsterCountCoru");
    }

    IEnumerator CheckMonsterCountCoru()
    {
        yield return new WaitForSeconds(5.0f); //오브젝트 삭제 대기

        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");

        if (monsters.Length == 0) //몬스터 오브젝트 없으면 이벤트 종료
        {
            switch (CCreateMap.instance.UserSelectRoom)
            {
                case CCreateMap.ERoomType._event:
                    CGlobal.isEvent = false;
                    CCreateMap.instance.NotifyPortal();
                    break;

                case CCreateMap.ERoomType._boss:
                    break;
            }        
        }
    }
}
