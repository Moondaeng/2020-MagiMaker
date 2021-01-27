using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 아이템 프리팹으로부터 아이템 리스트를 만들고 관리하는 클래스
 */
public class CItemManager : MonoBehaviour
{
    public static CItemManager instance = null;

    private Dictionary<int, GameObject> _itemDict = new Dictionary<int, GameObject>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        DefinedUseEffectSetting();

        // 아이템 프리팹 폴더에서 로드
        GameObject[] items = Resources.LoadAll<GameObject>("Item");
        foreach(var item in items)
        {
            var itemClone = Instantiate(item);
            itemClone.transform.SetParent(gameObject.transform);
            itemClone.SetActive(true);
            // 설정 누락 등 검사
            TestFault(itemClone);
            _itemDict.Add(itemClone.GetComponent<CItemComponent>().Item.ItemCode, itemClone);
            itemClone.SetActive(false);
        }

        // 아이템 드랍 테이블에 아이템 딕셔너리 내용물 넣기
        foreach (var item in _itemDict.Values)
        {
            CItemDropTable.instance.AddItem(item);
        }
    }

    // 정의된 UseEffect 설정
    private void DefinedUseEffectSetting()
    {
        GameObject[] definedEffects = Resources.LoadAll<GameObject>("UseEffect");
        foreach (var effect in definedEffects)
        {
            var effectClone = Instantiate(effect);
            effect.SetActive(true);
            var prop = effectClone.GetComponent<CUseEffect>();
            CUseEffectExplain.DefinedUseEffectNameDict.Add(prop.persistEffect.id, prop.EffectName);
            effect.SetActive(false);
        }
    }

    private void TestFault(GameObject item)
    {
        List<CUseEffectHandle> effects = null;
        if (item.GetComponent<CEquipComponent>() != null)
        {
            effects = (item.GetComponent<CEquipComponent>().Item as Item.CEquip).passiveEffect;
        }
        else if (item.GetComponent<CConsumableComponent>() != null)
        {
            effects = (item.GetComponent<CConsumableComponent>().Item as Item.CConsumable).UseEffectList;
        }
        if (effects != null && CheckUseEffectOmission(effects))
        {
            Debug.LogError($"{item.name}'s UseEffect is omission");
        }
    }

    private bool CheckUseEffectOmission(List<CUseEffectHandle> effects)
    {
        for(int i = 0; i < effects.Count; i++)
        {
            if(effects[i] == null)
            {
                return true;
            }
        }
        return false;
    }
    
    public GameObject GetItemObject(int itemCode)
    {
        GameObject item = null;
        bool isFind = _itemDict.TryGetValue(itemCode, out item);
        if(isFind)
        {
            return item;
        }
        else
        {
            Debug.Log("Fail to get item object");
            return null;
        }
    }

    public Item.CItem GetItemInfo(int itemCode)
    {
        var itemObject = GetItemObject(itemCode);
        CItemComponent itemInfo;
        if (itemObject != null && (itemInfo = itemObject.GetComponent<CItemComponent>()) != null)
        {
            return itemInfo.Item;
        }
        else
        {
            Debug.Log("Fail to get item info");
            return null;
        }
    }
}

//public bool IsEmptyWithoutShopItem() //장착형 아이템들 남아있는 거 있는 지 체크. 배열들이 전부 비었는지 확인해준다. 이 함수에선 상점은 제외되었다.
//{
//    for (int i = 0; i < CConstants.MAX_ITEM_IN_GAME; i++) //안비어있는가 체크 일단 낫띵이 아닌가 체크하고 낫띵이 아니면 used 체크해서 0이면 보내기
//    {
//        //if (_equipCommonItemArray.items[i]._name != "nothing")
//        //    if(_equipCommonItemArray.items[i]._used == 0)
//        //        return false;

//        //if (_equipSpecialItemArray.items[i]._name != "nothing")
//        //    if (_equipSpecialItemArray.items[i]._used == 0)
//        //        return false;

//        //if (_equipUniqueItemArray.items[i]._name != "nothing")
//        //    if (_equipUniqueItemArray.items[i]._used == 0)
//        //        return false;

//        //if (_equipMysteryItemArray.items[i]._name != "nothing")
//        //    if (_equipMysteryItemArray.items[i]._used == 0)
//        //        return false;
//    }
//    return true;
//}

//public Item.CItem FindItem(ref CItemArray itemArray, int itemType)
//{
//    Item.CItem temp = new Item.CItem();
//    System.Random r = new System.Random();

//    if (itemType == 1) //장착형
//        for (int i = 0; i < CConstants.MAX_ITEM_IN_GAME; i++)
//        {
//            if (itemArray.items[i].ItemName != "nothing")
//            {
//                temp = itemArray.items[i];
//                //itemArray.items[i].RemoveItem();
//                return temp;
//            }
//        }
//    else //소비형
//    {
//        while (true)
//        {
//            int i = r.Next() % CConstants.MAX_ITEM_IN_GAME;
//            if (itemArray.items[i].ItemName != "nothing")
//                return itemArray.items[i];
//        }
//    }

//    return temp;
//}

//public Item.CItem GetItemInfo(int itemCode)
//{
//    Item.CItem temp = new Item.CItem();
//    if (itemCode == -1)
//        return temp;

//    switch (itemCode % 1000 / 100)
//    {
//        case 0: //소비형템은 아예 랜덤으로 줘야함
//            switch (itemCode / 1000)
//            {
//                case 0:
//                    return FindItem(ref _consumeCommonItemArray, 0);
//                case 1:
//                    return FindItem(ref _consumeSpecialItemArray, 0);
//                case 2:
//                    return FindItem(ref _consumeUniqueItemArray, 0);
//                case 3:
//                    return FindItem(ref _consumeMysteryItemArray, 0);
//                case 4:
//                    return FindItem(ref _consumeShopItemArray, 0);
//            }
//            break;
//        case 1: //장착형
//            switch (itemCode / 1000)
//            {
//                case 0:
//                    return FindItem(ref _equipCommonItemArray, 1);
//                case 1:
//                    return FindItem(ref _equipSpecialItemArray, 1);
//                case 2:
//                    return FindItem(ref _equipUniqueItemArray, 1);
//                case 3:
//                    return FindItem(ref _equipMysteryItemArray, 1);
//                case 4:
//                    return FindItem(ref _equipShopItemArray, 1);
//            }
//            break;
//    }
//    return temp;
//}

//void SetItemRandom()
//{
//    int CCCnt = 0; //ConsumeCommon
//    int CSpecialCnt = 0, CUCnt = 0, CMCnt = 0, CShopCnt = 0, ECCnt = 0, ESpecialCnt = 0, EUCnt = 0, EMCnt = 0, EShopCnt = 0;

//    SortItem(ref CCCnt, ref CSpecialCnt, ref CUCnt, ref CMCnt, ref CShopCnt, ref ECCnt, ref ESpecialCnt, ref EUCnt, ref EMCnt, ref EShopCnt);

//    //소비 흔함 특별 희귀 신비 상점
//    ShuffleItemArray(CCCnt, ref _consumeCommonItemArray);
//    ShuffleItemArray(CSpecialCnt, ref _consumeSpecialItemArray);
//    ShuffleItemArray(CUCnt, ref _consumeUniqueItemArray);
//    ShuffleItemArray(CMCnt, ref _consumeMysteryItemArray);
//    ShuffleItemArray(CShopCnt, ref _consumeShopItemArray);

//    //장착 흔함 특별 희귀 신비 상점
//    ShuffleItemArray(ECCnt, ref _equipCommonItemArray);
//    ShuffleItemArray(ESpecialCnt, ref _equipSpecialItemArray);
//    ShuffleItemArray(EUCnt, ref _equipUniqueItemArray);
//    ShuffleItemArray(EMCnt, ref _equipMysteryItemArray);
//    ShuffleItemArray(EShopCnt, ref _equipShopItemArray);
//}

//void ShuffleItemArray(int arrayCount, ref CItemArray itemArray)
//{
//    Item.CItem temp;
//    int random;
//    System.Random r = new System.Random();

//    for (int i = 0; i < arrayCount; i++)
//    {
//        random = r.Next() % arrayCount;
//        temp = itemArray.items[i];
//        itemArray.items[i] = itemArray.items[random];
//        itemArray.items[random] = temp;
//    }
//}

//void SortItem(ref int CCCnt, ref int CSpecialCnt, ref int CUCnt, ref int CMCnt, ref int CShopCnt, ref int ECCnt, ref int ESpecialCnt, ref int EUCnt, ref int EMCnt, ref int EShopCnt)
//{

//    for (int i = 0; i < CConstants.MAX_ITEM_IN_GAME; i++)
//    {
//        int itemCode = _itemArray.items[i].ItemCode;

//        if (itemCode == -1) //아무것도 없을때 예외처리
//            continue;

//        switch (itemCode % 1000 / 100)
//        {
//            case 0: //소비형
//                switch (itemCode / 1000)
//                {
//                    case 0:
//                        _consumeCommonItemArray.items[CCCnt] = _itemArray.items[i];
//                        CCCnt++;
//                        break;
//                    case 1:
//                        _consumeSpecialItemArray.items[CSpecialCnt] = _itemArray.items[i];
//                        CSpecialCnt++;
//                        break;
//                    case 2:
//                        _consumeUniqueItemArray.items[CUCnt] = _itemArray.items[i];
//                        CUCnt++;
//                        break;
//                    case 3:
//                        _consumeMysteryItemArray.items[CMCnt] = _itemArray.items[i];
//                        CMCnt++;
//                        break;
//                    case 4:
//                        _consumeShopItemArray.items[CShopCnt] = _itemArray.items[i];
//                        CShopCnt++;
//                        break;
//                }
//                break;
//            case 1: //장착형
//                switch (itemCode / 1000)
//                {
//                    case 0:
//                        _equipCommonItemArray.items[ECCnt] = _itemArray.items[i];
//                        ECCnt++;
//                        break;
//                    case 1:
//                        _equipSpecialItemArray.items[ESpecialCnt] = _itemArray.items[i];
//                        ESpecialCnt++;
//                        break;
//                    case 2:
//                        _equipUniqueItemArray.items[EUCnt] = _itemArray.items[i];
//                        EUCnt++;
//                        break;
//                    case 3:
//                        _equipMysteryItemArray.items[EMCnt] = _itemArray.items[i];
//                        EMCnt++;
//                        break;
//                    case 4:
//                        _equipShopItemArray.items[EShopCnt] = _itemArray.items[i];
//                        EShopCnt++;
//                        break;
//                }
//                break;
//        }
//    }
//}

//    void SearchItem(string name)
//    {
//        if (_itemDict.ContainsKey(name))
//        {
//            CItem item = _itemDict[name];
//            item.Print();
//        }
//    }

//    void RemoveItems(string name)
//    {
//        bool result = _itemDict.Remove(name);
//        if (result)
//            Debug.Log(name + " Remove Complete");
//    }

//    void RemoveAll_Items()
//    {
//        _itemDict.Clear();
//    }

//    void PrintAll_Items()
//    {
//        var enumerator = _itemDict.GetEnumerator();

//        while(enumerator.MoveNext())
//        {
//            var pair = enumerator.Current;
//            CItem item = pair.Value;
//            item.Print();
//        }
//    }
//    void SetItemToDict()
//    {
//        string name;

//        for (int i = 0; i < _itemArray.items.Length; i++)
//        {
//            name = _itemArray.items[i]._name;

//            if(name != "nothing" && name != null)
//                _itemDict.Add(name, _itemArray.items[i]);
//        }
//    }
//}
