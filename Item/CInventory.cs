using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    public int EquipAtkIncreaseSize { get; private set; }
    public float AtkIncreaseRate { get; private set; }
    public int DefIncreaseSize { get; private set; }
    public float ASIncreaseSize { get; private set; }
    public float SpdIncreaseSize { get; private set; }
    public float AvdIncreaseSize { get; private set; }
    public float MaxHpIncreaseSize { get; private set; }
    public float HpRegenIncreaseSize { get; private set; }

    public ChangeConsumableEvent changeConsumableEvent = new ChangeConsumableEvent();

    public CInventory(int equipCapacity = 10, int consumableCapacity = 3)
    {
        _equipItems = new List<Item.CEquip>(equipCapacity);
        _consumableItems = new List<ConsumableWithStack>(consumableCapacity);
        _selectedConsumableNumber = 0;
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

        RewindEquipUI();

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

        RewindEquipUI();

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
            switch(ability.equipEffect)
            {
                case Item.CEquip.EquipAbility.EAbility.Attack:
                    EquipAtkIncreaseSize += ability.value;
                    break;
                case Item.CEquip.EquipAbility.EAbility.Defence:
                    DefIncreaseSize += ability.value;
                    break;
                case Item.CEquip.EquipAbility.EAbility.AttackSpeed:
                    ASIncreaseSize += ability.value;
                    break;
                case Item.CEquip.EquipAbility.EAbility.Speed:
                    SpdIncreaseSize += ability.value;
                    break;
                case Item.CEquip.EquipAbility.EAbility.MaxHp:
                    MaxHpIncreaseSize += ability.value;
                    break;
                case Item.CEquip.EquipAbility.EAbility.HpRegen:
                    HpRegenIncreaseSize += ability.value;
                    break;
                default:
                    break;
            }
        }

        // 패시브 효과 추가
}

    private void DeleteEquipAbility(Item.CEquip equip)
    {
        foreach (var ability in equip.equipAbilities)
        {
            switch (ability.equipEffect)
            {
                case Item.CEquip.EquipAbility.EAbility.Attack:
                    EquipAtkIncreaseSize -= ability.value;
                    break;
                case Item.CEquip.EquipAbility.EAbility.Defence:
                    DefIncreaseSize -= ability.value;
                    break;
                case Item.CEquip.EquipAbility.EAbility.AttackSpeed:
                    ASIncreaseSize -= ability.value;
                    break;
                case Item.CEquip.EquipAbility.EAbility.Speed:
                    SpdIncreaseSize -= ability.value;
                    break;
                case Item.CEquip.EquipAbility.EAbility.MaxHp:
                    MaxHpIncreaseSize -= ability.value;
                    break;
                case Item.CEquip.EquipAbility.EAbility.HpRegen:
                    HpRegenIncreaseSize -= ability.value;
                    break;
                default:
                    break;
            }
        }

        // 패시브 효과 제거
    }

    /// <summary>
    /// 가방 내 장비 아이템의 이미지 갱신
    /// </summary>
    private void RewindEquipUI()
    {

    }

    private void RewindConsumableUI()
    {

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
        UseConsumableSelectedEffect(consumable, useEffectIndex);
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

    private void UseConsumableSelectedEffect(Item.CConsumable consumable, int useEffectIndex)
    {
        if(useEffectIndex == -1)
        {
            Debug.Log("index error");
            return;
        }

        switch (consumable.UseEffectList[useEffectIndex].useEffect.EffectType)
        {
            case Item.CConsumable.UseEffect.EType.Heal:
                Debug.Log("Heal");
                break;
            default:
                Debug.Log("not implement Effect");
                break;
        }
    }
}
