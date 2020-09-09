using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CManager : MonoBehaviour
{
    List<GameObject> _monsters = new List<GameObject>();

    public static CManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    //외부에서 전달된 몬스터가 기존에 리스트에 보관하고 있는 몬스터와 일치하는지 여부를 체크
    public void AddNewMonsters(GameObject mon)
    {
        //인자로 넘어온 몬스터가 기존의 리스트에 존재하면 sameExist = true 아니면 false
        bool sameExist = false;
        for (int i = 0; i < _monsters.Count; i++)
        {
            if (_monsters[i] == mon)
            {
                sameExist = true;

                break;
            }
        }

        if (sameExist == false)
        {
            _monsters.Add(mon);
        }

    }

    //현재 플레이어가 클릭한 몬스터만 피통, 선택마크가 표시
    public void ChangeCurrentTarget(GameObject mon)
    {
        DeselectAllMonsters();
        mon.GetComponent<CEnemyPara>().ShowSelection();
        mon.GetComponent<CEnemyPara>().ShowHpBar();
    }

    public void DeselectAllMonsters()
    {
        for (int i = 0; i < _monsters.Count; i++)
        {
            _monsters[i].GetComponent<CEnemyPara>().HideSelection();
            _monsters[i].GetComponent<CEnemyPara>().HideHpBar();
        }
    }

    public void RemoveMonster(GameObject mon)
    {
        foreach (GameObject monster in _monsters)
        {
            if (monster == mon)
            {
                _monsters.Remove(monster);
                break;
            }

        }
    }

    public void DestroyAllMonsters()
    {
        for (int i = 0; i < _monsters.Count; i++)
        {
            Destroy(_monsters[i]);
        }
    }
}