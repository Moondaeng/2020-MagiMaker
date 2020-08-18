using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CInventory
{
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

        if(!HasOverlapEquip(newEquip))
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
        if (_consumableItems.Capacity >= _consumableItems.Count)
        {
            return false;
        }

        int overlapIndex;
        // 중복 체크
        if ((overlapIndex = HasOverlapConsumable(newItem)) != -1)
        {
            _consumableItems[overlapIndex].stack++;
        }
        else
        {
            _consumableItems.Add(new ConsumableWithStack(newItem, 1));
        }

        RewindConsumableUI();

        return true;
    }

    public void GetNextConsumable()
    {
        if(_consumableItems.Count == 0)
        {
            return;
        }

        if(++_selectedConsumableNumber == _consumableItems.Count)
        {
            _selectedConsumableNumber = 0;
        }
    }

    public void UseSelectedConsumable()
    {
        if(_consumableItems.Count == 0)
        {
            Debug.Log("consumable empty");
            return;
        }

        UseConsumable(_consumableItems[_selectedConsumableNumber].consumable);
        if(--_consumableItems[_selectedConsumableNumber].stack <= 0)
        {
            _consumableItems.RemoveAt(_selectedConsumableNumber);
            _selectedConsumableNumber = 0;
        }

        RewindConsumableUI();
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
        EquipAtkIncreaseSize += equip.Atk;
        DefIncreaseSize += equip.Def;
        ASIncreaseSize += equip.As;
        SpdIncreaseSize += equip.Spd;
        //AvdIncreaseSize += equip.avd;
        MaxHpIncreaseSize += equip.MaxHp;
        HpRegenIncreaseSize += equip.HpRegen;

        // 패시브 효과 추가
}

    private void DeleteEquipAbility(Item.CEquip equip)
    {
        EquipAtkIncreaseSize -= equip.Atk;
        DefIncreaseSize -= equip.Def;
        ASIncreaseSize -= equip.As;
        SpdIncreaseSize -= equip.Spd;
        //AvdIncreaseSize -= equip.avd;
        MaxHpIncreaseSize -= equip.MaxHp;
        HpRegenIncreaseSize -= equip.HpRegen;

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
        
    }
}
