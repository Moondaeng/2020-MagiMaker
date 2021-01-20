using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class CDropItemInfoPopup : MonoBehaviour
{
    public static CDropItemInfoPopup instance;

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
        _itemExplain.text += Item.CEquipExplainText.CreateExplainText(equip);
    }

    public void DrawConsumableExplain(Item.CConsumable consumable)
    {
        _itemExplain.text += "사용 시 " + CUseEffectExplain.CreateUseEffectText(consumable.UseEffectList);
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
