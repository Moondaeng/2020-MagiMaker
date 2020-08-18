using System.Collections;
using System.Collections.Generic;
using UnityEngine;

<<<<<<< HEAD

public class CPlayerPara : CharacterPara
{
    public string name { get; set; }
    public int curExp { get; set; }
    public int expToNextLevel { get; set; }
    public int money { get; set; }

    public override void InitPara()
    {
        name = "hong";
        maxHp = 1000;
        curHp = maxHp;
        attackMin = 50;
        attackMax = 80;
        defense = 30;
        eLevel = 0;
        eType = EElementType.none;
        money = 0;
        isAnotherAction = false;
        isStunned = false;
        isDead = false;
        CUIManager.instance.UpdatePlayerUI(this);
    }

    protected override void UpdateAfterReceiveAttack()
    {
        base.UpdateAfterReceiveAttack();

        CUIManager.instance.UpdatePlayerUI(this);
    }
    //석래가 추가한 부분 시작
    public List<CItem> _items = new List<CItem>();
    private string _itemTarget;
    public void EquipItem(CItem item)
    {
        Debug.Log("before equip armor : " + defense);
        _items.Add(item);
        defense += item._armor;
        Debug.Log("after equip armor : " + defense);
    }
    //석래가 추가한 부분 끝

    /*void OnTriggerEnter(Collider other)
    {
        EElementType attacker;
        float damage;
        if (other.gameObject.tag == "Punch")
        {
            attacker = other.transform.parent.parent.gameObject.GetComponent<CEnemyPara>().eType;
            damage = GetRandomAttack(eType, attacker);
            curHp -= (int)damage;
            UpdateAfterReceiveAttack();
        }
    }*/
=======
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
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
}