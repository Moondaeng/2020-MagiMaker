using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CManager : MonoBehaviour
{
    public static CManager instance;

<<<<<<< HEAD
    List<GameObject> monsters = new List<GameObject>();
=======
    List<GameObject> _monsters = new List<GameObject>();
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
<<<<<<< HEAD

=======
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
    }

    //외부에서 전달된 몬스터가 기존에 리스트에 보관하고 있는 몬스터와 일치하는지 여부를 체크
    public void AddNewMonsters(GameObject mon)
    {

        //인자로 넘어온 몬스터가 기존의 리스트에 존재하면 sameExist = true 아니면 false
        bool sameExist = false;
<<<<<<< HEAD
        for (int i = 0; i < monsters.Count; i++)
        {
            if (monsters[i] == mon)
=======
        for (int i = 0; i < _monsters.Count; i++)
        {
            if (_monsters[i] == mon)
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
            {
                sameExist = true;

                break;
            }
        }

        if (sameExist == false)
        {
<<<<<<< HEAD
            monsters.Add(mon);
=======
            _monsters.Add(mon);
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
        }

    }

    public void RemoveMonster(GameObject mon)
    {
<<<<<<< HEAD
        foreach (GameObject monster in monsters)
        {
            if (monster == mon)
            {
                monsters.Remove(monster);
=======
        foreach (GameObject monster in _monsters)
        {
            if (monster == mon)
            {
                _monsters.Remove(monster);
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
                break;
            }

        }
    }

<<<<<<< HEAD
    //현재 플레이어가 클릭한 몬스터만 선택마크가 표시
=======
    //현재 플레이어가 클릭한 몬스터만 피통, 선택마크가 표시
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
    public void ChangeCurrentTarget(GameObject mon)
    {
        DeselectAllMonsters();
        mon.GetComponent<CEnemyPara>().ShowSelection();
<<<<<<< HEAD
=======
        mon.GetComponent<CEnemyPara>().ShowHpBar();
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
    }

    public void DeselectAllMonsters()
    {
<<<<<<< HEAD
        for (int i = 0; i < monsters.Count; i++)
        {
            monsters[i].GetComponent<CEnemyPara>().HideSelection();
=======
        for (int i = 0; i < _monsters.Count; i++)
        {
            _monsters[i].GetComponent<CEnemyPara>().HideSelection();
            _monsters[i].GetComponent<CEnemyPara>().HideHpBar();
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}