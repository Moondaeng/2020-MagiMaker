﻿using UnityEngine;
using System.Collections;
using System;

public class CPlayerPara : CharacterPara
{
    public string _name;
    public bool _invincibility;
    public bool _invincibilityChecker;
    private Animator _myAnimator;
    BoxCollider _col;
    [SerializeField] public Renderer _obj;
    Color _originColor;

    [SerializeField]
    public CInventory Inventory;

    // ((캐릭터 공격력 * 공격력 증가) + 장비 공격력) * 버프로 올라가는 공격력 %
    public override int TotalAttackMin
    {
        get
        {
            return (int)(((_attackMin * Inventory.AtkIncreaseRate) + Inventory.EquipAtkIncreaseSize)
              * buffParameter.AttackCoef * buffParameter.AttackDebuffCoef);
        }
    }
    public override int TotalAttackMax
    {
        get
        {
            return (int)(((_attackMax * Inventory.AtkIncreaseRate) + Inventory.EquipAtkIncreaseSize)
              * buffParameter.AttackCoef * buffParameter.AttackDebuffCoef);
        }
    }
    public override int TotalDefenece
    {
        get
        {
            return (int)(_defense + Inventory.DefIncreaseSize
              * buffParameter.DefenceCoef * buffParameter.DefenceDebuffCoef);
        }
    }
    public int TotalMaxHp
    {
        get { return (int)(_maxHp + Inventory.MaxHpIncreaseSize); }
    }
    public int TotalHpRegen
    {
        get { return (int)(Inventory.HpRegenIncreaseSize); }
    }

    protected override void Awake()
    {
        base.Awake();
        Inventory = new CInventory();
    }

    public override void InitPara()
    {
        _col = GetComponent<BoxCollider>();
        _maxHp = 1000;
        _curHp = _maxHp;
        _attackMin = 5;
        _attackMax = 8;
        _defense = 3;
        _eLevel = 0;
        _eType = EElementType.none;
        _isAnotherAction = false;
        _isStunned = false;
        _isDead = false;
        _invincibility = false;
        _hitGauge = 0;
        _originColor = _obj.material.color;
        _myAnimator = GetComponent<Animator>();
        _myAnimator.SetInteger("Hp", _curHp);
        //CUIManager.instance.UpdatePlayerUI(this);
    }
    
    protected override void UpdateAfterReceiveAttack()
    {
        if (_invincibility) return;
        _myAnimator.SetInteger("Hp", _curHp);
        print(name + "'s HP: " + _curHp);
        if (_curHp <= 0)
        {
            _curHp = 0;
            _isDead = true;
            deadEvent.Invoke();
        }
    }
    #region 무적판정
    public void OffInvincibility()
    {
        _invincibility = false;
        StopCoroutine(PowerOverwhelming());
        _obj.material.color = _originColor;
        gameObject.layer = LayerMask.NameToLayer("DeadBody");
    }

    public void OnInvincibility()
    {
        if (_invincibilityChecker)
        {
            _invincibilityChecker = false;
            StartCoroutine(PowerOverwhelming());
        }
    }

    IEnumerator PowerOverwhelming()
    {
        while (_invincibility)
        {
            float flicker = Mathf.Abs(Mathf.Sin(Time.time * 10));
            _obj.material.color = _originColor * flicker;
            yield return null;
        }
    }

    #endregion
}