using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlantFSM : CEnemyFSM
{
    #region 식물만 가지는 Properties
    public GameObject Prefabs;
    private GameObject currentPrefabObject;
    private CMonsterSkillBase currentPrefabScript;
    #endregion

    protected override void InitStat()
    {
        _moveSpeed = 0f;
        _attackDistance = 5f;
        _attackRadius = 10f;
        _anim = GetComponent<Animator>();
        _myPara = GetComponent<CEnemyPara>();
        _myPara.deadEvent.AddListener(CallDeadEvent);

        _spawnID = _myPara.GetComponent<CEnemyPara>()._spawnID;
        _myRespawn = _myPara.GetComponent<CEnemyPara>()._myRespawn;
        
        // 몬스터 마다 다른 행동양식들
        _idleState = Animator.StringToHash("Base Layer.Idle");
        _standState = Animator.StringToHash("Base Layer.Stand");
        _attackState1 = Animator.StringToHash("Base Layer.Attack1");
        _waitState = Animator.StringToHash("Base Layer.Wait");
        _skillState1 = Animator.StringToHash("Base Layer.Skill1");
        _skillState2 = Animator.StringToHash("Base Layer.Skill2");
        _deadState1 = Animator.StringToHash("Base Layer.Death");

        _cooltime = 1f;
        
        _skillCooltime1 = 3f;
        _skillCooltime2 = 5f;
        _originCooltime = _cooltime;
        _originSkillCooltime1 = _skillCooltime1;
        _originSkillCooltime2 = _skillCooltime2;
    }

    #region 통상적인 State 관련 함수들
    protected override void UpdateState()
    {
        if (_actionStart)
        {
            _skillCooltime1 -= Time.deltaTime;
            _skillCooltime2 -= Time.deltaTime;
            _skillCoolDown1 = true;
            _skillCoolDown2 = true;
        }

        if (_currentBaseState.nameHash == _standState)          StartState();
        else if (_currentBaseState.nameHash == _attackState1)   AttackState();
        else if (_currentBaseState.nameHash == _waitState)      AttackWaitState();
        else if (_currentBaseState.nameHash == _skillState1)    SkillState1();
        else if (_currentBaseState.nameHash == _skillState2)    SkillState2();

        if (_skillCooltime1 < 0f)
        {
            _anim.SetTrigger("Skill1");
            _skillCoolDown1 = false;
        }
        if (_skillCooltime2 < 0f)
        {
            _anim.SetTrigger("Skill2");
            _skillCoolDown2 = false;
        }
    }

    private void StartState()
    {
        _actionStart = true;
    }
    
    private void AttackState()
    {
        if (!_lookAtPlayer)
        {
            _lookAtPlayer = false;
        }
        _coolDown = true;
    }
    
    private void AttackWaitState()
    {
        _cooltime -= Time.deltaTime; 
        _lookAtPlayer = true;
        if (_cooltime < 0)
        {
            _coolDown = false;
            _cooltime = _originCooltime;
        }
        else if (GetDistanceFromPlayer(_distances) > _attackDistance)
        {
            _coolDown = false;
            _cooltime = _originCooltime;
        }
    }

    private void SkillState1()
    {
        _skillCooltime1 = _originSkillCooltime1;
        _skillCoolDown1 = true;
        _lookAtPlayer = true;
    }

    private void RangeSkillShot()
    {
        Vector3 pos;
        float yRot = transform.rotation.eulerAngles.y;
        Vector3 forwardY = Quaternion.Euler(0.0f, yRot, 0.0f) * Vector3.forward;
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        Vector3 up = transform.up;
        Quaternion rotation = Quaternion.identity;
        currentPrefabObject = GameObject.Instantiate(Prefabs);
        
        // temporary effect, like a fireball
        currentPrefabScript = currentPrefabObject.GetComponent<CMonsterSkillBase>();
        // set the start point near the player
        rotation = transform.rotation;
        pos = transform.position + 2 * forward + 2 * up;

        CMonsterSkillProjectile projectileScript = currentPrefabObject.GetComponentInChildren<CMonsterSkillProjectile>();
        if (projectileScript != null)
        {
            // make sure we don't collide with other fire layers
            projectileScript.ProjectileCollisionLayers &= (~UnityEngine.LayerMask.NameToLayer("MonsterSkillLayer"));
        }

        currentPrefabObject.transform.position = pos;
        currentPrefabObject.transform.rotation = rotation;
    }

    private void SkillState2()
    {
        _skillCooltime2 = _originSkillCooltime2;
        _skillCoolDown2 = true;
    }
    #endregion

    protected override void Update()
    {
        _anim.SetBool("CoolDown", _coolDown);
        _anim.SetBool("CoolDown1", _skillCoolDown1);
        _anim.SetBool("CoolDown2", _skillCoolDown2);
        _anim.SetBool("AnotherAction", _anotherAction);
        base.Update();
    }
}