using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 아이템 없는 버튼 인터렉티브 false 주기 ex) slot.GetComponent<UnityEngine.UI.Button>().interactable = false;

[System.Serializable]
public class CItemArray
{
<<<<<<< HEAD
    public CItem[] items = new CItem[CConstants.MAX_ITEM_IN_GAME];
=======
    public Item.CItem[] items = new Item.CItem[CConstants.MAX_ITEM_IN_GAME];
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3

    public CItemArray()
    {

        for (int i = 0; i < CConstants.MAX_ITEM_IN_GAME; i++)
<<<<<<< HEAD
            items[i] = new CItem();
=======
            items[i] = new Item.CItem();
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
    }

    public void SetItemArray(ref CItemArray itemArray)
    {
        CItemJsonConvert.instance.loadToItem("Items", ref itemArray);
    }
}
