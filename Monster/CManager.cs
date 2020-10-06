using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* @help
 * 이 클래스는 CRespawn에서 사용되는 맵 전체의 몬스터 배열을 관리하는 클래스이다.
 * 
 */
public class CManager : MonoBehaviour
{
    public List<GameObject> _monsters = new List<GameObject>();

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