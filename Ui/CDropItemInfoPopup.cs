using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class CDropItemInfoPopup : MonoBehaviour
{
    public static CDropItemInfoPopup instance;

    private static string[] _equipAbilityExplainArr 
        = new string[] {
            "공격력",
            "방어력",
            "공격속도",
            "이동속도",
            "체력",
            "체젠"
        };

    private int _currentViewingItemID;
    private Image _itemImage;
    private Text _itemName;
    private Text _itemGrade;
    private Text _itemExplain;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        _currentViewingItemID = -1;
    }

    private void Start()
    {
        _itemImage = transform.GetChild(0).GetComponent<Image>();
        _itemName = transform.GetChild(1).GetComponent<Text>();
        _itemGrade = transform.GetChild(2).GetComponent<Text>();
        _itemExplain = transform.GetChild(3).GetComponent<Text>();
    }

    public void DrawEquipExplain(Item.CEquip equip)
    {
        _itemExplain.text += Item.CEquipExplainText.CreateAbilityText(equip.equipAbilities);
        _itemExplain.text += Item.CEquipExplainText.CreatePassiveText(
            equip.PassiveCondition, equip.PassiveUseCount, equip.PassiveConditionOption, equip.EquipEffectList);
        _itemExplain.text += Item.CEquipExplainText.CreateUpgradeText(equip.UpgradeCondition, equip.UpgradeCount, equip.upgradeAbilities);
    }

    public void DrawConsumableExplain(Item.CConsumable consumable)
    {
    }

    public void DrawItemInfo(Item.CItem item)
    {
        if (_currentViewingItemID == item.ItemCode)
        {
            return;
        }

        _itemImage.sprite = item.ItemImage;
        _itemName.text = item.ItemName;
        _itemExplain.text = "";

        if(item is Item.CEquip)
        {
            DrawEquipExplain(item as Item.CEquip);
        }
        else if(item is Item.CConsumable)
        {
            DrawConsumableExplain(item as Item.CConsumable);
        }
    }

    private void DrawUseEffectText()
    {

    }
}
