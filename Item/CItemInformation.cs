using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//랜덤 돌려서 아이템 등급 정해서 받기
//아이템 받을때 nothing 이면 다른거 받아와야함.
public class CItemInformation : MonoBehaviour
{
<<<<<<< HEAD
    public CItem _item;
    public GameObject _goitem;
    void Start()
    {
        _item = new CItem();
=======
    public Item.CItem _item;
    public GameObject _goitem;
    void Start()
    {
        _item = new Item.CItem();
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
        GetItemInfoFromItemManager();
        _item.Print();
        _goitem.GetComponent<Rigidbody>().AddForce(new Vector3(0, 30, 0) * 10f); //물체 한번 띄우기용
    }

    void GetItemInfoFromItemManager()
    {

        int itemCode;
        int random;
        int itemType = 1;
        System.Random r = new System.Random();

        if (false) //소비템의 경우 조건문으로 나오게하기
            itemType = 0;


        switch (itemType)
        {
            case 0: //소비형
                random = r.Next() % 100;
                //일단 확률 흔함40, 특별30, 희귀20, 신비10 으로 설정
                if (random >= 0 && random < CConstants.COMMON)
                {
                    itemCode = 0000;
                    _item = CItemManager.instance.GetItemInfo(itemCode);
                }
                else if (random >= CConstants.COMMON && random < CConstants.SPECIAL)
                {
                    itemCode = 1000;
                    _item = CItemManager.instance.GetItemInfo(itemCode);
                }
                else if (random >= CConstants.SPECIAL && random < CConstants.UNIQUE)
                {
                    itemCode = 2000;
                    _item = CItemManager.instance.GetItemInfo(itemCode);
                }
                else //신비등급 필요하면 elseif로 변경
                {
                    itemCode = 3000;
                    _item = CItemManager.instance.GetItemInfo(itemCode);
                }
                break;
            case 1: //장착형
                while (true)
                {
                    random = r.Next() % 100;
                    //일단 확률 흔함40, 특별30, 희귀20, 신비10 으로 설정
                    if (random >= 0 && random < CConstants.COMMON)
                    {
                        itemCode = 0100;
                        _item = CItemManager.instance.GetItemInfo(itemCode);
                    }
                    else if (random >= CConstants.COMMON && random < CConstants.SPECIAL)
                    {
                        itemCode = 1100;
                        _item = CItemManager.instance.GetItemInfo(itemCode);
                    }
                    else if (random >= CConstants.SPECIAL && random < CConstants.UNIQUE)
                    {
                        itemCode = 2100;
                        _item = CItemManager.instance.GetItemInfo(itemCode);
                    }
                    else //신비등급 필요하면 elseif로 변경
                    {
                        itemCode = 3100;
                        _item = CItemManager.instance.GetItemInfo(itemCode);
                    }
                    //나올템이 더 없는 지 체크 있으면 반복 없으면 종료, 장착형인 경우에만 확인
<<<<<<< HEAD
                    if (_item._name == "nothing")
=======
                    if (_item.ItemName == "nothing")
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
                    {
                        if (CItemManager.instance.IsEmptyWithoutShopItem())
                            break;
                    }
                    else
                        break;
                }
                break;
        }
    }
}


