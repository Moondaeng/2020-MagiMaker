using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CShopCntl : MonoBehaviour
{
    public Transform _itemRoot;     //아이템 오브젝트들을 모아둔 루트 오브젝트
    public List<Item.CItem> _items;      //리스트로 관리
    public System.Random r;         //아이템 배치 랜덤용
    public int _commmonItemCount;
    public int _specialItemCount;
    public int _rareMysteryItemCount;
    public int _ShopItemCount;
    public int _ConsumableItemCount;
    void Start()
    {
        FirstShopOff(); //첫 생성 시 차일드 객체 오프
        MakeItemList(); //루트 오브젝트를 조회해서 자식으로 설정되있는 아이템들 전부 리스트에 등록
        SetShop();      //상점에 개수에 맞게 확률 따라서 랜덤으로 아이템 배치, 배치하면서 그 아이템들은 제거(포션류 제외)
    }

    void FirstShopOff()
    {
        for(int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(false);
    }

    void SetShop()
    {
        SetCommonItem();
        SetSpecialItem();
        SetRareMysteryItem();
        SetShopItem();
        SetConsumableItem();
    }

    void SetCommonItem()
    {
        for(int i = 0; i < CConstants.MAX_COMMON_EQUIPMENT_SHOP; i++)
        {
            r = new System.Random();
            r.Next();
        }
    }

    void SetSpecialItem()
    {

    }

    void SetRareMysteryItem()
    {

    }

    void SetShopItem()
    {

    }

    void SetConsumableItem()
    {

    }
    void SetItem(Item.CItem item)
    {

    }

    void MakeItemList()
    {
        _items = new List<Item.CItem>();

        int itemCount = _itemRoot.childCount;

        for (int i = 0; i < itemCount; i++)
        {
            var item = _itemRoot.GetChild(i).GetComponent<Item.CItem>();
            _items.Add(item);
        }
    }
}
