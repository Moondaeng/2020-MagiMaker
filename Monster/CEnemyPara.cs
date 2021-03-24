using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CEnemyPara : CharacterPara
{
    #region 몬스터 피격 처리
    public class MonsterHitEvent : UnityEvent<CEnemyPara> { }
    [System.NonSerialized] public MonsterHitEvent monsterHitEvent = new MonsterHitEvent();

    public override void TakeUseEffect(CUseEffect effect)
    {
        if (effect == null)
        {
            return;
        }

        monsterHitEvent.Invoke(this);
        Debug.Log("hit monster");

        ApplyInstantEffect(effect.instantEffect);
        ApplyConditionalEffect(effect.conditionalEffect);
        ApplyPersistEffect(effect.persistEffect);
    }

    private void OnEnable()
    {
        _spawnID = CMonsterManager.instance.AddMonsterInfo(gameObject);
        Debug.Log($"{_spawnID}");
        monsterHitEvent.AddListener(CMonsterManager.instance.MonsterHit);
        deadEvent.AddListener(SetOffMonster);
    }

    private void OnDisable()
    {
        monsterHitEvent.RemoveAllListeners();
        deadEvent.RemoveAllListeners();
    }
    #endregion

    public string _name;
    string _originTag = "Monster";
    [HideInInspector] public GameObject _myRespawn;
    Vector3 _originPos;

    public override void InitPara()
    {
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



    public void SetOffMonster()
    {
        Invoke("SetActiveFalse", 2f);
    }

    public void SetActiveFalse()
    {
        // 코드 개선 필요 - 콜백을 통해 몬스터 매니저에게 호출하는 구조가 되어야 함
        Debug.Log("Dead");
        CMonsterManager.instance.RemoveMonster(_spawnID);
        CGameEvent.instance.EarnMoneyEvent?.Invoke(_rewardMoney);
        //_myRespawn.GetComponent<CRespawn>().RemoveMonster(_spawnID);
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

    protected virtual void DropItem()
    {
        //CItemDropTable.instance.DropRandomItem();
    }
}