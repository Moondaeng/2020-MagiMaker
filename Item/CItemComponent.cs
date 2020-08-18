using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Item
{

    [System.Serializable]
    public class CItem
    {
        public string ItemName { get; protected set; }
        public Sprite ItemImage { get; protected set; }

        /*
         * 등급 기준
         * 흔함 0, 특별 1, 희귀 2, 신비 3, 상점 4,
         * 타입 기준
         * 소비형 0, 장착형 1
         * 아이템 번호 엑셀에 서술할 것.
         */
        /// <summary>
        /// 천번대 등급, 백번대 아이템 타입, 00부터 아이템 번호
        /// </summary>
        public int ItemCode { get; protected set; }

        public CItem()
        {
            ItemName = "";
            ItemCode = 0;
            ItemImage = null;
        }

        public CItem(string _itemName, int _itemCode, Sprite _itemImage)
        {
            ItemName = _itemName;
            ItemCode = _itemCode;
            ItemImage = _itemImage;
        }

        public void Print()
        {
            Debug.Log("name = " + ItemName);
            Debug.Log("itemCode = " + ItemCode);
        }
    }
}

public class CItemComponent : MonoBehaviour
{
    Item.CItem item;

    public virtual void CallItemUI()
    {

    }
}
