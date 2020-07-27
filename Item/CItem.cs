using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CItem 
{
    public string _name;        // 아이템 이름
    public int _itemCode;       // 천번대 등급, 백번대 아이템 타입, 00부터 아이템 번호
    /*
     * 등급 기준
     * 흔함 0, 특별 1, 희귀 2, 신비 3, 상점 4,
     * 타입 기준
     * 소비형 0, 장착형 1
     * 아이템 번호 엑셀에 서술할 것.
     */
    public Sprite _sprite;      // 아이템 이미지
    public int _attackSpeed;     // 공격속도
    public int _attackDamage;    // 공격력
    public int _MaxHP;           // 최대 체력
    public int _armor;           // 방어력
    public int _moveSpeed;       // 이동속도
    public int _skillCoolTime;   // 쿨타임
    public int _damageTakenRate; // 받는 피해율
    public int _skillRange;      // 스킬 범위
    public int _HPRegenPerSec;   // 초당 체력 리젠
    public string _specialEffect;// 아이템 특수 효과
    public int _used;            // 중복 아이템 방지용

    public CItem(string name, int itemCode, Sprite sprite, int attackSpeed, int attackDamage, int MaxHP, int armor, int moveSpeed, int skillCoolTime, int damageTakenRate, int skillRange, int HPRegenPerSec, string specialEffect, int used)
    {
        _name = name;
        _sprite = sprite;
        _itemCode = itemCode;
        _attackSpeed = attackSpeed;
        _attackDamage = attackDamage;
        _MaxHP = MaxHP;
        _armor = armor;
        _moveSpeed = moveSpeed;
        _skillCoolTime = skillCoolTime;
        _damageTakenRate = damageTakenRate;
        _skillRange = skillRange;
        _HPRegenPerSec = HPRegenPerSec;
        _specialEffect = specialEffect;
        _used = used;
    }

    public CItem()
    {
        _name = "nothing";
        _sprite = null;
        _itemCode = -1;
        _attackSpeed = 0;
        _attackDamage = 0;
        _MaxHP = 0;
        _armor = 0;
        _moveSpeed = 0;
        _skillCoolTime = 0;
        _damageTakenRate = 0;
        _skillRange = 0;
        _HPRegenPerSec = 0;
        _specialEffect = null;
        _used = 0;
    }

    public void RemoveItem()
    {
        _used++;
    }

    public void Print()
    {
        Debug.Log("name = " + _name);
        Debug.Log("itemCode = " + _itemCode);
    }
}
