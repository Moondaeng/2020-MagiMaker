﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class CItemDropTable : MonoBehaviour
{
    [System.Serializable]
    public class DropItemGradeChanceList
    {
        [Range(0,1)] public float Normal;
        [Range(0,1)] public float Special;
        [Range(0,1)] public float Rare;
        [Range(0,1)] public float Unique;
    }

    public enum ItemGrade
    {
        Normal, Special, Rare, Unique, Shop, Event
    }

    public static CItemDropTable instance = null;

    [SerializeField]
    private DropItemGradeChanceList[] DropItemChanceInStages = new DropItemGradeChanceList[6];

    private List<GameObject>[] _equipObjectLists = new List<GameObject>[Enum.GetValues(typeof(ItemGrade)).Length];
    private List<GameObject> _consumableObjectList = new List<GameObject>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        for(int i = 0; i < _equipObjectLists.Length; i++)
        {
            _equipObjectLists[i] = new List<GameObject>();
        }
    }

    public void ClearDropList()
    {
        foreach(var list in _equipObjectLists)
        {
            list.Clear();
        }
    }

    public GameObject DropRandomItem(int stage)
    {
        Debug.Log("Drop Random Item");
        float chanceSum = 0f;
        float[] chanceList = new float[4];
        chanceSum += DropItemChanceInStages[stage].Normal;
        chanceList[0] = chanceSum;
        chanceSum += DropItemChanceInStages[stage].Special;
        chanceList[1] = chanceSum;
        chanceSum += DropItemChanceInStages[stage].Rare;
        chanceList[2] = chanceSum;
        chanceSum += DropItemChanceInStages[stage].Unique;
        chanceList[3] = chanceSum;

        // 랜덤한 리스트 선택
        ItemGrade itemGrade = ItemGrade.Normal;
        float randomChance = UnityEngine.Random.Range(0f, 1f);
        if(randomChance < chanceList[0])
        {
            itemGrade = ItemGrade.Normal;
        }
        else if (randomChance < chanceList[1])
        {
            itemGrade = ItemGrade.Special;
        }
        else if (randomChance < chanceList[2])
        {
            itemGrade = ItemGrade.Rare;
        }
        else if (randomChance < chanceList[3])
        {
            itemGrade = ItemGrade.Unique;
        }

        // 확률 합이 1이 안 넘을 경우, 아이템을 드랍하지 않음
        // 드랍 테이블이 빈 경우 처리 방안 필요
        return PopRandomItemByGrade(itemGrade);
    }

    public GameObject DropConsumable()
    {
        List<GameObject> itemList = _consumableObjectList;

        if (itemList.Count == 0)
        {
            Debug.Log("Consumable Item List is empty");
            return null;
        }

        return FindRandomItemInList(itemList);
    }

    /// <summary>
    /// ItemCompoenent를 가지고 있는 item 추가
    /// </summary>
    /// <param name="item"></param>
    public void AddItem(GameObject item)
    {
        if(item.GetComponent<CItemComponent>() == null)
        {
            Debug.Log("Error - item hasn't CItemComponent");
            return;
        }

        int itemCode = item.GetComponent<CItemComponent>().Item.ItemCode / 100;
        int itemType = itemCode % 10;
        int itemGrade = itemCode / 10;

        // 소비 아이템
        if (itemType == 0)
        {
            _consumableObjectList.Add(item);
        }
        // 장비 아이템
        else if (itemType == 1)
        {
            _equipObjectLists[itemGrade].Add(item);
        }
        // 기타 케이스는 오류 간주
        else
        {
            Debug.Log($"Can't Add Item By Item Code : {itemCode}");
        }
    }

    public GameObject PopRandomItemByGrade(ItemGrade itemGrade)
    {
        List<GameObject> itemList = _equipObjectLists[(int)itemGrade];

        if(itemList.Count == 0)
        {
            Debug.Log($"{itemGrade} Item List is empty");
            return null;
        }

        return PopRandomItemInList(itemList);
    }

    public GameObject PopItemByItemCode(int itemCode)
    {
        itemCode /= 100;
        int itemType = itemCode % 10;
        int itemGrade = itemCode / 10;
        GameObject item = null;

        item = CItemManager.instance.GetItemObject(itemCode);
        // 소비 아이템
        if (itemType == 0)
        {
            _consumableObjectList.Remove(item);
        }
        // 장비 아이템
        else if(itemType == 1)
        {
            _equipObjectLists[itemGrade].Remove(item);
        }
        // 기타 케이스는 오류 간주
        {
            Debug.Log("Can't Pop Item By Item Code");
        }

        return item;
    }

    private GameObject FindRandomItemInList(List<GameObject> list)
    {
        int randInt = UnityEngine.Random.Range(1, list.Count) - 1;
        return list[randInt];
    }

    private GameObject PopRandomItemInList(List<GameObject> list)
    {
        int randInt = UnityEngine.Random.Range(1, list.Count) - 1;
        var item = list[randInt];
        list.RemoveAt(randInt);
        return item;
    }
}
