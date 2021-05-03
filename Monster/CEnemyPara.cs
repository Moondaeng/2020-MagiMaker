using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CEnemyPara : CharacterPara
{
    [System.Serializable]
    public class SkillType
    {
        [Tooltip("스킬 공격력 : 기본 공격력에서의 배수로 해둠")]
        public int SkillPower;
        [Tooltip("CC 종류 기본 값 None")]
        public CrowdControl CCType;
        public CrowdControlLevel CCLevel;
    }

    [HideInInspector] public Stack<int> _attacker = new Stack<int>();
    [Header ("Monster가 가지고 있는 스킬 속성 정의")]
    public List<SkillType> _skillType = new List<SkillType>();
    string _originTag = "Monster";
    [HideInInspector] public GameObject _myRespawn;
    Vector3 _originPos;
    public string _name;

    #region 몬스터 피격 처리
    public class MonsterHitEvent : UnityEvent<CEnemyPara> { }
    [System.NonSerialized] public MonsterHitEvent monsterHitEvent = new MonsterHitEvent();

    public override void TakeUseEffect(CUseEffect effect)
    {
        monsterHitEvent.Invoke(this);
        base.TakeUseEffect(effect);
    }

    public override void DamegedRegardDefence(int enemyAttack)
    {
        monsterHitEvent.Invoke(this);
        base.DamegedRegardDefence(enemyAttack);
    }

    private void OnEnable()
    {
        _spawnID = CMonsterManager.instance.AddMonsterInfo(gameObject);
        deadEvent.AddListener(SetOffMonster);
    }
    #endregion

    public override void InitPara()
    {
        deadEvent.AddListener(SetOffMonster);
        base.InitPara();
        _isStunned = false;
        _isDead = false;
        _curHp = _maxHp;
    }
    
    public void SetRespawn(GameObject respawn, int spawnID, Vector3 originPos)
    {
        _myRespawn = respawn;
        _spawnID = spawnID;
        _originPos = originPos;
        //Debug.Log("My Respawn is : " + _myRespawn + "  My SpawnID is : " + _spawnID
        //    + "  My originPos is : " + _originPos);
    }

    // 방어력 계산식: 1000 / (950 + 10*방어력)
    public void DamegedRegardDefence(int enemyAttack, int attackEnemy)
    {
        int damage = enemyAttack * 1000 / (950 + 10 * TotalDefenece);
        _attacker.Push(attackEnemy);
        DamagedDisregardDefence(damage);
    }

    public void SetOffMonster()
    {
        Invoke("SetActiveFalse", 2f);
    }

    public void SetActiveFalse()
    {
        CMonsterManager.instance.RemoveMonster(_spawnID);
    }

    public void respawnAgain()
    {
        //  리스폰 오브젝트에서 처음 생성될때의 위치와 같게 함
        transform.position = _originPos;
        tag = _originTag;
        gameObject.layer = LayerMask.NameToLayer("Monster");
        InitPara();
    }

    protected override void UpdateAfterReceiveAttack()
    {
        base.UpdateAfterReceiveAttack();
    }
}