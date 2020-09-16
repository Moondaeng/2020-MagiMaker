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
    CManager _manager;
    bool key1, key2, key3, key4, key5, key6;
    public GameObject[] Prefabs;
    private GameObject currentPrefabObject;
    private CMonsterSkillBase currentPrefabScript;
    int COUNT = 0;

    private void Start()
    {
        _myPara = GetComponent<CPlayerPara>();
        _respawn = GameObject.FindGameObjectWithTag("Respawn");
        _manager = _respawn.GetComponent<CManager>();
        _distance = new List<float>();
    }

    void SearchMonster(float searchLength)
    {
        for (int i = 0; i < _manager._monsters.Count; i++)
        {
            float distance = Vector3.Distance(transform.position, _manager._monsters[i].transform.position);
            _distance.Add(distance);
        }
    }
    

    protected void AttackCheck()
    {
        _manager.Say();
        //BeginEffect();
        SearchMonster(10f);
        for (int i = 0; i < _manager._monsters.Count; i++)
        {
            if (IsTargetInSight(30f, _manager._monsters[i].transform) && _distance[i] < 5f)
            {
                _enemyPara = _manager._monsters[i].transform.gameObject.GetComponent<CEnemyPara>();
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

    private void BeginEffect()
    {
        Vector3 pos;
        float yRot = transform.rotation.eulerAngles.y;
        Vector3 forwardY = Quaternion.Euler(0.0f, yRot, 0.0f) * Vector3.forward;
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        Vector3 up = transform.up;
        Quaternion rotation = Quaternion.identity;
        currentPrefabObject = GameObject.Instantiate(Prefabs[COUNT]);
        currentPrefabScript = currentPrefabObject.GetComponent<CMonsterConstant>();

        if (currentPrefabScript == null)
        {
            // temporary effect, like a fireball
            currentPrefabScript = currentPrefabObject.GetComponent<CMonsterSkillBase>();
            if (currentPrefabScript.IsProjectile)
            {
                // set the start point near the player
                rotation = transform.rotation;
                pos = transform.position + 2 * forward + 2 * up;
                Debug.Log(pos);
            }
            else
            {
                // set the start point in front of the player a ways
                pos = transform.position + (forwardY * 10.0f) + 2 * up;
            }
        }
        else
        {
            // set the start point in front of the player a ways, rotated the same way as the player
            pos = transform.position + (forwardY * 5.0f);
            rotation = transform.rotation;
            pos.y = 0.0f;
        }

        CMonsterSkillProjectile projectileScript = currentPrefabObject.GetComponentInChildren<CMonsterSkillProjectile>();
        if (projectileScript != null)
        {
            // make sure we don't collide with other fire layers
            projectileScript.ProjectileCollisionLayers &= (~UnityEngine.LayerMask.NameToLayer("FireLayer"));
        }

        currentPrefabObject.transform.position = pos;
        currentPrefabObject.transform.rotation = rotation;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Alpha1)) key1 = true;
        else key1 = false;
        if (Input.GetKey(KeyCode.Alpha2)) key2 = true;
        else key2 = false;
        if (Input.GetKey(KeyCode.Alpha3)) key3 = true;
        else key3 = false;
        if (Input.GetKey(KeyCode.Alpha4)) key4 = true;
        else key4 = false;
        if (Input.GetKey(KeyCode.Alpha5)) key5 = true;
        else key5 = false;
        if (Input.GetKey(KeyCode.Alpha6)) key6 = true;
        else key6 = false;

        if (key1)
        {
            COUNT = 0;
        }
        else if (key2)
        {
            COUNT = 1;
        }
        else if (key3)
        { 
            COUNT = 2;
        }
        else if (key4)
        {
            COUNT = 3;
        }
        else if (key5)
        {
            COUNT = 4;
        }
        else if (key6)
        {
            COUNT = 5;
        }
    }
}
