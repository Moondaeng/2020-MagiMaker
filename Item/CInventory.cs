using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChangeConsumableEvent : UnityEvent<Sprite, string, int> { }

[System.Serializable]
public class CInventory
{
    [System.Serializable]
    public class ConsumableWithStack
    {
        public Item.CConsumable consumable;
        public int stack;

        public ConsumableWithStack(Item.CConsumable _consumable, int _stack)
        {
            consumable = _consumable;
            stack = _stack;
        }
    }

    private List<Item.CEquip> _equipItems;

    private List<ConsumableWithStack> _consumableItems;
    private int _selectedConsumableNumber;

    private GameObject _inventoryUser;

    private int _gold;

    public int GetGold()
    {
        return _gold;
    }

    public void SetGold(int gold)
    {
        _gold += gold;
    }

    public int EquipAtkIncreaseSize
    {
        get
        {
            return _equipAbilityIncreaseSizeArr[(int)Item.EEquipAbility.Attack];
        }
    }
    public int DefIncreaseSize
    {
        get
        {
            return _equipAbilityIncreaseSizeArr[(int)Item.EEquipAbility.Defence];
        }
    }
    public float ASIncreaseSize
    {
        get
        {
            return _equipAbilityIncreaseSizeArr[(int)Item.EEquipAbility.AttackSpeed];
        }
    }
    public float SpdIncreaseSize
    {
        get
        {
            return _equipAbilityIncreaseSizeArr[(int)Item.EEquipAbility.Speed];
        }
    }
    //public float AvdIncreaseSize
    //{
    //    get
    //    {
    //        return _equipAbilityIncreaseSizeArr[(int)Item.EEquipAbility.];
    //    }
    //}
    public float MaxHpIncreaseSize
    {
        get
        {
            return _equipAbilityIncreaseSizeArr[(int)Item.EEquipAbility.MaxHp];
        }
    }
    public float HpRegenIncreaseSize
    {
        get
        {
            return _equipAbilityIncreaseSizeArr[(int)Item.EEquipAbility.HpRegen];
        }
    }

    private int[] _equipAbilityIncreaseSizeArr = new int[Enum.GetValues(typeof(Item.EEquipAbility)).Length];

    public ChangeConsumableEvent changeConsumableEvent = new ChangeConsumableEvent();

    public CInventory(GameObject userObject, int equipCapacity = 10, int consumableCapacity = 3, int gold = 0)
    {
        _inventoryUser = userObject;
        _equipItems = new List<Item.CEquip>(equipCapacity);
        _consumableItems = new List<ConsumableWithStack>(consumableCapacity);
        _selectedConsumableNumber = 0;

        _gold = gold;

        RegisterEquipEvent();
    }

    private void RegisterEquipEvent()
    {
        var inventoryUserPara = _inventoryUser.GetComponent<CPlayerPara>();
        var inventoryUserSkill = _inventoryUser.GetComponent<CPlayerSkill>();
        //inventoryUserPara.healEvent.AddListener((string tag, int amount) => CallItemEvent(Item.EEquipEvent.Always, amount));
        inventoryUserSkill.skillUseEvent.AddListener((int skillNum, Vector3 pos) => CallItemEvent(Item.EEquipEvent.UseSkill, 1));
    }

    /// <summary>
    /// 가방에 장비 아이템 넣기
    /// </summary>
    /// <returns>아이템 넣기 성공했는지</returns>
    public bool AddEquip(Item.CEquip newEquip)
    {
        if(_equipItems.Count >= _equipItems.Capacity)
        {
            return false;
        }

        if(HasOverlapEquip(newEquip))
        {
            Debug.Log("overlap equip");
            return false;
        }

        _equipItems.Add(newEquip);
        AddEquipAbility(newEquip);

        return true;
    }

    /// <summary>
    /// 가방에서 아이템 빼기
    /// </summary>
    /// <returns></returns>
    public bool DeleteEquipItem(int itemIndex)
    {
        if (itemIndex < 0 || _equipItems.Count <= itemIndex)
        {
            return false;
        }

        // 귀속 아이템 여부 판단

        DeleteEquipAbility(_equipItems[itemIndex]);
        _equipItems.RemoveAt(itemIndex);

        return true;
    }

    /// <summary>
    /// 가방에 소비 아이템 넣기
    /// </summary>
    /// <returns>아이템 넣기 성공했는지</returns>
    public bool AddConsumableItem(Item.CConsumable newItem)
    {
        if (_consumableItems.Count >= _consumableItems.Capacity)
        {
            return false;
        }

        int overlapIndex;
        if ((overlapIndex = HasOverlapConsumable(newItem)) != -1)
        {
            _consumableItems[overlapIndex].stack++;
        }
        else
        {
            _consumableItems.Add(new ConsumableWithStack(newItem, 1));
        }

        changeConsumableEvent.Invoke(
            _consumableItems[_selectedConsumableNumber].consumable.ItemImage,
            _consumableItems[_selectedConsumableNumber].consumable.ItemName,
            _consumableItems[_selectedConsumableNumber].stack);

        return true;
    }

    public void GetNextConsumable()
    {
        if(_consumableItems.Count == 0)
        {
            Debug.Log("Consumable inventory is empty");
            return;
        }

        if(++_selectedConsumableNumber == _consumableItems.Count)
        {
            _selectedConsumableNumber = 0;
        }
        Debug.Log($"Current Consumable Number : {_selectedConsumableNumber}");

        changeConsumableEvent.Invoke(
            _consumableItems[_selectedConsumableNumber].consumable.ItemImage,
            _consumableItems[_selectedConsumableNumber].consumable.ItemName,
            _consumableItems[_selectedConsumableNumber].stack);
    }

    public void UseSelectedConsumable()
    {
        if(_consumableItems.Count == 0)
        {
            Debug.Log("consumable empty");
            return;
        }

        Debug.Log($"Selected Consumable Number : {_selectedConsumableNumber}");
        UseConsumable(_consumableItems[_selectedConsumableNumber].consumable);
        if(--_consumableItems[_selectedConsumableNumber].stack <= 0)
        {
            _consumableItems.RemoveAt(_selectedConsumableNumber);
            _selectedConsumableNumber = 0;
        }

        if(_consumableItems.Count == 0)
        {
            changeConsumableEvent.Invoke(
                null,
                "",
                0);
        }
        else
        {
            changeConsumableEvent.Invoke(
                _consumableItems[_selectedConsumableNumber].consumable.ItemImage,
                _consumableItems[_selectedConsumableNumber].consumable.ItemName,
                _consumableItems[_selectedConsumableNumber].stack);
        }
    }

    /// <summary>
    /// 해당 이벤트가 일어나면 장비 효과 발동(패시브, 성장)
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="count"></param>
    private void CallItemEvent(Item.EEquipEvent condition, int count)
    {
        foreach (var equip in _equipItems)
        {
            if(equip.PassiveCondition == condition)
            {
                ExecuteEquipPassive(equip, count);
            }

            if(equip.UpgradeCondition == condition)
            {
                ExecuteEquipUpgrade(equip);
            }
        }
    }

    /// <summary>
    /// 장비 패시브를 추가 조건에 따라 실행
    /// ex) n회에 한 번 실행 / n 이상일 때 실행 / n 이하일 때 실행
    /// </summary>
    /// <param name="equip">적용 장비</param>
    /// <param name="count"></param>
    private void ExecuteEquipPassive(Item.CEquip equip, int count)
    {
        switch (equip.PassiveConditionOption)
        {
            case Item.EEquipEventCountOption.Accumulate:
                equip.passiveCurrentCount += count;
                if(equip.passiveCurrentCount >= equip.PassiveUseCount)
                {
                    Debug.Log("Use Passive");
                    equip.passiveCurrentCount = 0;
                }
                break;
            case Item.EEquipEventCountOption.Each_Below:
                if (equip.PassiveUseCount >= count)
                {
                    Debug.Log("Use Passive");
                }
                break;
            case Item.EEquipEventCountOption.Each_Over:
                if (equip.PassiveUseCount <= count)
                {
                    Debug.Log("Use Passive");
                }
                break;
            default:
                Debug.Log("Error case");
                break;
        }
    }

    /// <summary>
    /// 장비 성장
    /// </summary>
    /// <param name="equip"></param>
    private void ExecuteEquipUpgrade(Item.CEquip equip)
    {
        if(equip.UpgradeCurrentCount >= equip.UpgradeCount)
        {
            return;
        }

        equip.UpgradeCurrentCount++;
        if (equip.UpgradeCurrentCount == equip.UpgradeCount)
        {
            Debug.Log("Equip Upgrade");
        }
    }

    private void SwapEquip(ref Item.CEquip lhs, ref Item.CEquip rhs)
    {
        Item.CEquip temp;
        temp = lhs;
        lhs = rhs;
        rhs = temp;
    }

    /// <summary>
    /// 장비 아이템의 합산 능력치 및 효과 갱신
    /// </summary>
    private void AddEquipAbility(Item.CEquip equip)
    {
        foreach(var ability in equip.equipAbilities)
        {
            _equipAbilityIncreaseSizeArr[(int)ability.equipEffect] += ability.value;
        }
    }

    private void DeleteEquipAbility(Item.CEquip equip)
    {
        foreach (var ability in equip.equipAbilities)
        {
            _equipAbilityIncreaseSizeArr[(int)ability.equipEffect] -= ability.value;
        }
    }

    ///<summary>
    ///장비 상위 / 하위 아이템 중복인지 확인
    ///</summary>
    ///<returns></returns>
    private bool HasOverlapEquip(Item.CEquip newEquip)
    {
        return false;
    }

    /// <summary>
    /// 소비 아이템 중복인지 확인 및 중복인 소비 아이템 인덱스 리턴
    /// </summary>
    /// <param name="newConsumable"></param>
    /// <returns></returns>
    private int HasOverlapConsumable(Item.CConsumable newConsumable)
    {
        for(int index = 0; index < _consumableItems.Count; index++)
        {
            if(_consumableItems[index].consumable.ItemCode == newConsumable.ItemCode)
            {
                return index;
            }
        }
        return -1;
    }

    private void UseConsumable(Item.CConsumable consumable)
    {
        int useEffectIndex = SelectRandomEffect(consumable);
        Debug.Log($"useEffectIndex : {useEffectIndex}");
        UseConsumableSelectedEffect(consumable.UseEffectList[useEffectIndex].useEffect);
    }

    private int SelectRandomEffect(Item.CConsumable consumable)
    {
        if(consumable.UseEffectList.Count == 0)
        {
            Debug.Log("No Use Effect");
            return -1;
        }
        else if(consumable.UseEffectList.Count == 1)
        {
            return 0;
        }

        Debug.Log("Get Random Effect");
        float chanceSum = 0f;
        List<float> chanceSumList = new List<float>(consumable.UseEffectList.Count);
        foreach (var elem in consumable.UseEffectList)
        {
            chanceSum += elem.Chance;
            chanceSumList.Add(chanceSum);
        }
        Debug.Log("set chance");

        // 임의 효과 선택
        float randomChance = UnityEngine.Random.Range(0f, 1f);
        int idx = 0;
        while (idx < chanceSumList.Count - 1 && randomChance >= chanceSumList[idx])
        {
            idx++;
        }

        return idx;
    }

    private void UseConsumableSelectedEffect(CUseEffect useEffect)
    {
        foreach(var damage in useEffect.DamageEffectList)
        {
            switch(damage.type)
            {
                case CUseEffect.DamageType.damage:
                    _inventoryUser.GetComponent<CPlayerPara>().DamagedDisregardDefence(damage.startDamage);
                    break;
                case CUseEffect.DamageType.heal:
                    _inventoryUser.GetComponent<CPlayerPara>().DamagedDisregardDefence(-damage.startDamage);
                    break;
                default:
                    break;
            }
        }
        foreach(var cc in useEffect.CCEffectList)
        {
            switch(cc.type)
            {
                case CUseEffect.CCType.stun:
                    break;
                case CUseEffect.CCType.slow:
                    break;
                default:
                    break;
            }
        }
        foreach (var buff in useEffect.BuffEffectList)
        {
            switch(buff.type)
            {
                case CUseEffect.BuffType.attackBuff:
                    break;
                case CUseEffect.BuffType.defenceBuff:
                    break;
                default:
                    break;
            }
        }
    }
}
