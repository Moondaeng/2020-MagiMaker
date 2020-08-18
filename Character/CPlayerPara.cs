using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerPara : CharacterPara
{
    public string _name;
    public int _curExp { get; set; }
    public int _expToNextLevel { get; set; }
    public int _money { get; set; }
    private string _itemTarget;

    [SerializeField]
    public CInventory _inventory;

    public override int TotalAttackMin
    {
        get { return (int)(((_attackMin * _inventory.AtkIncreaseRate) + _inventory.EquipAtkIncreaseSize)
                * buffParameter.AttackCoef * buffParameter.AttackDebuffCoef); }
    }
    public override int TotalAttackMax
    {
        get { return (int)(((_attackMax * _inventory.AtkIncreaseRate) + _inventory.EquipAtkIncreaseSize)
                * buffParameter.AttackCoef * buffParameter.AttackDebuffCoef); }
    }
    public override int TotalDefenece
    {
        get { return (int)(_defense + _inventory.DefIncreaseSize 
                * buffParameter.DefenceCoef * buffParameter.DefenceDebuffCoef); }
    }
    public int TotalMaxHp
    {
        get { return (int)(_maxHp + _inventory.MaxHpIncreaseSize); }
    }
    public int TotalHpRegen
    {
        get { return (int)(_inventory.HpRegenIncreaseSize); }
    }

    public override void InitPara()
    {
        _maxHp = 1000;
        _curHp = _maxHp;
        _attackMin = 50;
        _attackMax = 80;
        _defense = 30;
        _eLevel = 0;
        _eType = EElementType.none;
        _money = 0;
        _isAnotherAction = false;
        _isStunned = false;
        _isDead = false;
    }

    protected override void UpdateAfterReceiveAttack()
    {
        base.UpdateAfterReceiveAttack();
    }
}