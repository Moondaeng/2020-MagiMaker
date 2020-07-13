using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerPara : CharacterPara
{
    public string _name;
    public int _curExp { get; set; }
    public int _expToNextLevel { get; set; }
    public int _money { get; set; }

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

    //석래가 추가한 부분 시작
    public List<CItem> _items = new List<CItem>();
    private string _itemTarget;
    public void EquipItem(CItem item)
    {
        Debug.Log("before equip armor : " + _defense);
        _items.Add(item);
        _defense += item._armor;
        Debug.Log("after equip armor : " + _defense);
    }
    //석래가 추가한 부분 끝

    protected override void UpdateAfterReceiveAttack()
    {
        base.UpdateAfterReceiveAttack();
    }
}