using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* @help
 * 이 클래스는 CRespawn에서 사용되는 맵 전체의 몬스터 배열을 관리하는 클래스이다.
 * 
 */
public class CManager : MonoBehaviour
{
    public static CManager instance;

    public List<GameObject> _monsters = new List<GameObject>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // 몬스터 일치 체크
    public void AddNewMonsters(GameObject mon)
    {
        bool sameExist = false;
        for (int i = 0; i < _monsters.Count; i++)
        {
            if (_monsters[i] == mon)
            {
                sameExist = true;
                break;
            }
        }
        if (sameExist == false) _monsters.Add(mon);
    }

    public void Say()
    {
        for (int i = 0; i < _monsters.Count; i++)
        {
            Debug.Log(_monsters[i].name);
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