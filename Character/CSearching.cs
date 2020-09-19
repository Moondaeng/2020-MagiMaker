using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class CSearching : MonoBehaviour
{
    GameObject _respawn;
    List<float> _distance;
    List<GameObject> _monster;
    CPlayerPara _myPara;
    CEnemyPara _enemyPara;

    private void Start()
    {
        _myPara = GetComponent<CPlayerPara>();
        _distance = new List<float>();
    }

    private void SearchMonster(float searchLength)
    {
        for (int i = 0; i < CManager.instance._monsters.Count; i++)
        {
            float distance = Vector3.Distance(transform.position, CManager.instance._monsters[i].transform.position);
            _distance.Add(distance);
        }
        if (_distance == null)
            return;
    }
    
    protected void AttackCheck()
    {
        _distance.Clear();
        SearchMonster(5f);
        for (int i = 0; i < CManager.instance._monsters.Count; i++)
        {
            if (IsTargetInSight(30f, CManager.instance._monsters[i].transform) && _distance[i] < 5f)
            {
                _enemyPara = CManager.instance._monsters[i].GetComponent<CEnemyPara>();
                _enemyPara.SetEnemyAttack(_myPara._attackMax);
            }
        }
    }

    protected bool IsTargetInSight(float SightAngle, Transform Target)
    {
        //타겟의 방향 
        Vector3 targetDir = (Target.position - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, targetDir);

        //내적을 이용한 각 계산하기
        // thetha = cos^-1( a dot b / |a||b|)
        float theta = Mathf.Acos(dot) * Mathf.Rad2Deg;

        if (theta <= SightAngle) return true;
        else return false;
    }
}
