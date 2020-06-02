using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CItemManager : MonoBehaviour
{
    public static CItemManager instance = null;
    CItemArray _itemArray;
    CItemArray _equipCommonItemArray;
    CItemArray _equipSpecialItemArray;
    CItemArray _equipUniqueItemArray;
    CItemArray _equipMysteryItemArray;
    CItemArray _equipShopItemArray;
    CItemArray _consumeCommonItemArray;
    CItemArray _consumeSpecialItemArray;
    CItemArray _consumeUniqueItemArray;
    CItemArray _consumeMysteryItemArray;
    CItemArray _consumeShopItemArray;

    //Dictionary<string, CItem> _itemDict;
    void Start()
    {
        if (instance == null)
            instance = this;

        _itemArray = new CItemArray();
        _itemArray.SetItemArray(ref _itemArray);
        _equipCommonItemArray = new CItemArray();
        _equipSpecialItemArray = new CItemArray();
        _equipUniqueItemArray = new CItemArray();
        _equipMysteryItemArray = new CItemArray();
        _equipShopItemArray = new CItemArray();
        _consumeCommonItemArray = new CItemArray();
        _consumeSpecialItemArray = new CItemArray();
        _consumeUniqueItemArray = new CItemArray();
        _consumeMysteryItemArray = new CItemArray();
        _consumeShopItemArray = new CItemArray();

        //for (int i = 0; i < _itemArray.items.Length; i++)
        //    _itemArray.items[i].Print();

        //_itemDict = new Dictionary<string, CItem>();
        //SetItemToDict();

        //PrintAll_Items();

        SetItemRandom();
    }

    public bool IsEmptyWithoutShopItem() //장착형 아이템들 남아있는 거 있는 지 체크. 배열들이 전부 비었는지 확인해준다. 이 함수에선 상점은 제외되었다.
    {
        for (int i = 0; i < CConstants.MAX_ITEM_IN_GAME; i++) //안비어있는가 체크 일단 낫띵이 아닌가 체크하고 낫띵이 아니면 used 체크해서 0이면 보내기
        {
            if (_equipCommonItemArray.items[i]._name != "nothing")
                if(_equipCommonItemArray.items[i]._used == 0)
                    return false;

            if (_equipSpecialItemArray.items[i]._name != "nothing")
                if (_equipSpecialItemArray.items[i]._used == 0)
                    return false;

            if (_equipUniqueItemArray.items[i]._name != "nothing")
                if (_equipUniqueItemArray.items[i]._used == 0)
                    return false;

            if (_equipMysteryItemArray.items[i]._name != "nothing")
                if (_equipMysteryItemArray.items[i]._used == 0)
                    return false;
        }
        return true;
    }

    public CItem FindItem(ref CItemArray itemArray, int itemType)
    {
        CItem temp = new CItem();
        System.Random r = new System.Random();

        if (itemType == 1) //장착형
            for (int i = 0; i < CConstants.MAX_ITEM_IN_GAME; i++)
            {
                if (itemArray.items[i]._name != "nothing")
                {
                    temp = itemArray.items[i];
                    itemArray.items[i].RemoveItem();
                    return temp;
                }
            }
        else //소비형
        {
            while(true)
            {
                int i = r.Next() % CConstants.MAX_ITEM_IN_GAME;
                if (itemArray.items[i]._name != "nothing")
                    return itemArray.items[i];
            }
        }

        return temp;
    }

    public CItem GetItemInfo(int itemCode)  
    {
        CItem temp = new CItem();
        if (itemCode == -1)
            return temp;

        switch (itemCode % 1000 / 100)
        {
            case 0: //소비형템은 아예 랜덤으로 줘야함
                switch (itemCode / 1000)
                {
                    case 0:
                        return FindItem(ref _consumeCommonItemArray, 0); 
                    case 1:
                        return FindItem(ref _consumeSpecialItemArray, 0);
                    case 2:
                        return FindItem(ref _consumeUniqueItemArray, 0);
                    case 3:
                        return FindItem(ref _consumeMysteryItemArray, 0);
                    case 4:
                        return FindItem(ref _consumeShopItemArray, 0);
                }
                break;
            case 1: //장착형
                switch (itemCode / 1000)
                {
                    case 0:
                        return FindItem(ref _equipCommonItemArray, 1);
                    case 1:
                        return FindItem(ref _equipSpecialItemArray, 1);
                    case 2:
                        return FindItem(ref _equipUniqueItemArray, 1);
                    case 3:
                        return FindItem(ref _equipMysteryItemArray, 1);
                    case 4:
                        return FindItem(ref _equipShopItemArray, 1);
                }
                break;
        }
        return temp;
    }

    /*
 * 등급 기준
 * 흔함 0, 특별 1, 희귀 2, 신비 3, 상점 4, 천번대
 * 타입 기준
 * 소비형 0, 장착형 1 백번대
 * 아이템 번호 엑셀에 서술할 것.
 */

    void SetItemRandom()
    {
        int CCCnt = 0; //ConsumeCommon
        int CSpecialCnt = 0, CUCnt = 0, CMCnt = 0, CShopCnt = 0, ECCnt = 0, ESpecialCnt = 0, EUCnt = 0, EMCnt = 0, EShopCnt = 0;

        SortItem(ref CCCnt, ref CSpecialCnt, ref CUCnt, ref CMCnt, ref CShopCnt, ref ECCnt, ref ESpecialCnt, ref EUCnt, ref EMCnt, ref EShopCnt);

        //소비 흔함 특별 희귀 신비 상점
        ShuffleItemArray(CCCnt, ref _consumeCommonItemArray);
        ShuffleItemArray(CSpecialCnt, ref _consumeSpecialItemArray);
        ShuffleItemArray(CUCnt, ref _consumeUniqueItemArray);
        ShuffleItemArray(CMCnt, ref _consumeMysteryItemArray);
        ShuffleItemArray(CShopCnt, ref _consumeShopItemArray);

        //장착 흔함 특별 희귀 신비 상점
        ShuffleItemArray(ECCnt, ref _equipCommonItemArray);
        ShuffleItemArray(ESpecialCnt, ref _equipSpecialItemArray);
        ShuffleItemArray(EUCnt, ref _equipUniqueItemArray);
        ShuffleItemArray(EMCnt, ref _equipMysteryItemArray);
        ShuffleItemArray(EShopCnt, ref _equipShopItemArray);
    }

    void ShuffleItemArray(int arrayCount, ref CItemArray itemArray)
    {
        CItem temp;
        int random;
        System.Random r = new System.Random();

        for (int i = 0; i < arrayCount; i++)
        {
            random = r.Next() % arrayCount;
            temp = itemArray.items[i];
            itemArray.items[i] = itemArray.items[random];
            itemArray.items[random] = temp;
        }
    }

    void SortItem(ref int CCCnt, ref int CSpecialCnt, ref int CUCnt, ref int CMCnt, ref int CShopCnt, ref int ECCnt, ref int ESpecialCnt, ref int EUCnt, ref int EMCnt, ref int EShopCnt)
    {

        for (int i = 0; i < CConstants.MAX_ITEM_IN_GAME; i++)
        {
            int itemCode = _itemArray.items[i]._itemCode;

            if (itemCode == -1) //아무것도 없을때 예외처리
                continue;

            switch (itemCode % 1000 / 100)
            {
                case 0: //소비형
                    switch (itemCode / 1000)
                    {
                        case 0:
                            _consumeCommonItemArray.items[CCCnt] = _itemArray.items[i];
                            CCCnt++;
                            break;
                        case 1:
                            _consumeSpecialItemArray.items[CSpecialCnt] = _itemArray.items[i];
                            CSpecialCnt++;
                            break;
                        case 2:
                            _consumeUniqueItemArray.items[CUCnt] = _itemArray.items[i];
                            CUCnt++;
                            break;
                        case 3:
                            _consumeMysteryItemArray.items[CMCnt] = _itemArray.items[i];
                            CMCnt++;
                            break;
                        case 4:
                            _consumeShopItemArray.items[CShopCnt] = _itemArray.items[i];
                            CShopCnt++;
                            break;
                    }
                    break;
                case 1: //장착형
                    switch (itemCode / 1000)
                    {
                        case 0:
                            _equipCommonItemArray.items[ECCnt] = _itemArray.items[i];
                            ECCnt++;
                            break;
                        case 1:
                            _equipSpecialItemArray.items[ESpecialCnt] = _itemArray.items[i];
                            ESpecialCnt++;
                            break;
                        case 2:
                            _equipUniqueItemArray.items[EUCnt] = _itemArray.items[i];
                            EUCnt++;
                            break;
                        case 3:
                            _equipMysteryItemArray.items[EMCnt] = _itemArray.items[i];
                            EMCnt++;
                            break;
                        case 4:
                            _equipShopItemArray.items[EShopCnt] = _itemArray.items[i];
                            EShopCnt++;
                            break;
                    }
                    break;
            }
        }
    }
}

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
