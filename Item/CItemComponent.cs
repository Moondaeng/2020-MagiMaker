using UnityEngine;

namespace Item
{
    [System.Serializable]
    public class CItem
    {
        public string ItemName
        {
            get { return _itemName; }
        }
        public Sprite ItemImage
        {
            get { return _itemImage; }
        }

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
        public int ItemCode
        {
            get { return _itemCode; }
        }

        [SerializeField]
        protected string _itemName;
        [SerializeField]
        protected Sprite _itemImage;
        [SerializeField]
        protected int _itemCode;

        public CItem()
            : this("", 0, null)
        {
        }

        public CItem(string itemName, int itemCode, Sprite itemImage)
        {
            _itemName = itemName;
            _itemImage = itemImage;
            _itemCode = itemCode;
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
    public Item.CItem Item { get; protected set; }

    protected void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Item");
    }
}
