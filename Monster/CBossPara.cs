using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CBossPara : CEnemyPara
{
    [Tooltip("방어 확률")] public int _defenceSuccessProbability;
    private bool _useSkillOnce, _useSkillTwoTimes, _useSkillThreeTimes;
    private bool _defendon;
    public UnityEvent SkillUsingHPEvent = new UnityEvent();
    public UnityEvent DefenceUsingEvent = new UnityEvent();

    public override void InitPara()
    {
        base.InitPara();
        _isStunned = false;
        _isDead = false;
        _useSkillOnce = false;
        _useSkillTwoTimes = false;
        _useSkillThreeTimes = false;
        _defendon = false;
        _curHp = _maxHp;
    }
    
    public void DefendUsingPercent()
    {
        Debug.Log("가드 이벤트 보내기 중간");
        int lottoGuard = Random.Range(0, 100);
        if (lottoGuard < _defenceSuccessProbability)
        {
            DefenceUsingEvent.Invoke();
        }
    }

    public void OnDefend()
    {
        _defendon = true;
    }
    public void OffDefend()
    {
        _defendon = false;
    }

    // 보스 전용  가드 확률
    public new void DamegedRegardDefence(int enemyAttack)
    {
        int damage = enemyAttack * 1000 / (950 + 10 * TotalDefenece);
        DamagedDisregardDefence(damage);
    }

    public new void DamagedDisregardDefence(int enemyAttack)
    {
        // 초과딜 방지
        if (enemyAttack >= _curHp)
        {
            enemyAttack = _curHp;
        }
        _curHp -= enemyAttack;
        if (!_defendon && _curHp != 0)
        {
            Debug.Log("히트 이벤트 보내기 전");
            hitGaugeEvent?.Invoke(enemyAttack);
        }
        UpdateAfterReceiveAttack();
    }

    protected override void UpdateAfterReceiveAttack()
    {
        base.UpdateAfterReceiveAttack();
        Debug.Log(_curHp * 100 / _maxHp < 90);
        if (_curHp * 100 / _maxHp <= 50f && !_useSkillOnce && _curHp != 0)
        {
            _useSkillOnce = true;
            SkillUsingHPEvent.Invoke();
        }
    }
}