using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 아이템 없는 버튼 인터렉티브 false 주기 ex) slot.GetComponent<UnityEngine.UI.Button>().interactable = false;

[System.Serializable]
public class CItemArray
{
    public CItem[] items = new CItem[CConstants.MAX_ITEM_IN_GAME];

    public CItemArray()
    {

        for (int i = 0; i < CConstants.MAX_ITEM_IN_GAME; i++)
            items[i] = new CItem();
    }

    public void SetItemArray(ref CItemArray itemArray)
    {
        CItemJsonConvert.instance.loadToItem("Items", ref itemArray);
    }
}
