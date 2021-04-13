using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class CSearching : MonoBehaviour
{
    private GameObject _respawn;
    private List<float> _distance;
    private List<GameObject> _monster;
    private CPlayerPara _myPara;
    private CCntl _myControl;
    private CEnemyPara _enemyPara;

    private void Start()
    {
        _myPara = GetComponent<CPlayerPara>();
        _myControl = GetComponent<CCntl>();
        _distance = new List<float>();
    }

    private void SearchMonster(float searchLength)
    {
        int count = CMonsterManager.instance.GetMonsterCount();
        if (count == 0) return;
        for (int i = 0; i < count; i++)
        {
            float distance = 
                Vector3.Distance(transform.position, 
                CMonsterManager.instance.GetMonsterInfo(i).transform.position);
            _distance.Add(distance);
        }
        if (_distance == null)
            return;
    }
    
    private void TurnAttackHand()
    {
        //if (_myControl._skillStartPoint)
    }

    //protected void AttackCheck()
    //{
    //    int count = CMonsterManager.instance.GetMonsterCount();
    //    for (int i = 0; i < count; i++)
    //    {
    //        if (IsTargetInSight(30f, CMonsterManager.instance.GetMonsterInfo(i).transform) 
    //            && _distance[i] < 5f)
    //        {
    //            _enemyPara = CManager.instance._monsters[i].GetComponent<CEnemyPara>();
    //            _enemyPara.DamegedRegardDefence(_myPara.GetRandomAttack());
    //        }
    //    }
    //}

    //protected bool IsTargetInSight(float SightAngle, Transform Target)
    //{
    //    //타겟의 방향 
    //    Vector3 targetDir = (Target.position - transform.position).normalized;
    //    float dot = Vector3.Dot(transform.forward, targetDir);

    //    //내적을 이용한 각 계산하기
    //    // thetha = cos^-1( a dot b / |a||b|)
    //    float theta = Mathf.Acos(dot) * Mathf.Rad2Deg;

    //    if (theta <= SightAngle) return true;
    //    else return false;
    //}
}
