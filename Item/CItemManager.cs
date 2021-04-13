using System;
using System.Collections.Generic;
using UnityEngine;

/*
 * 아이템 프리팹으로부터 아이템 리스트를 만들고 관리하는 클래스
 */
[DisallowMultipleComponent]
public class CItemManager : MonoBehaviour
{
    public static CItemManager instance = null;

    public enum EItemGrade
    {
        Normal, Special, Rare, Unique, Shop, Event
    }

    public enum EPlayerType
    {
        player1, player2, player3, player4
    }

    private static readonly int DROPABLE_ITEM_GRADE = (int)EItemGrade.Unique;

    [Serializable]
    private class DropItemChanceInStage
    {
        [Range(0, 100)]
        public float[] chances = new float[DROPABLE_ITEM_GRADE + 1];
    }

    private static readonly int dropForce = 300;

    [SerializeField]
    private DropItemChanceInStage[] DropItemChanceInStages = new DropItemChanceInStage[CConstants.MAX_STAGE_IN_GAME];

    private List<GameObject>[] _equipObjectLists = new List<GameObject>[Enum.GetValues(typeof(EItemGrade)).Length];
    private List<GameObject>[] _consumableObjectLists = new List<GameObject>[Enum.GetValues(typeof(EItemGrade)).Length];

    private Dictionary<int, GameObject> _itemDict = new Dictionary<int, GameObject>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        for (int i = 0; i < Enum.GetValues(typeof(EItemGrade)).Length; i++)
        {
            _consumableObjectLists[i] = new List<GameObject>();
            _equipObjectLists[i] = new List<GameObject>();
        }
    }

    void Start()
    {
        //DefinedUseEffectSetting();

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
            // 게임 이전에 받아오기
            int playerType = (item.GetComponent<CItemComponent>().Item.ItemCode / 10000);
            AddItem(item, playerType);
        }
    }

    #region Item Drop Table
    public void ClearDropList()
    {
        foreach (var list in _equipObjectLists)
        {
            list.Clear();
        }

        foreach (var list in _consumableObjectLists)
        {
            list.Clear();
        }
    }

    /// <summary>
    /// dropPos 위치에 item이 드랍되도록 연출한다
    /// </summary>
    /// <param name="item"></param>
    public static void SetItemToDropState(GameObject item, Vector3 dropPos)
    {
        item.SetActive(true);
        item.transform.position = dropPos;
        item.GetComponent<Rigidbody>().velocity = Vector3.zero;
        item.GetComponent<Rigidbody>().AddForce(Vector3.up * dropForce);
    }

    public GameObject DropRandomItem(int stage, int itemType)
    {
        Debug.Log("Drop Random Item");
        float chanceSum = 0f;
        float[] chances = DropItemChanceInStages[stage].chances;
        float[] accumulateChances = new float[chances.Length];
        for (int i = 0; i < chances.Length; i++)
        {
            chanceSum += chances[i];
            accumulateChances[i] = chanceSum;
        }

        // 랜덤한 리스트 선택
        float randomChance = UnityEngine.Random.Range(0, 100f);
        //Debug.Log($"random number is {randomChance}");
        for (int idx = 0; idx < accumulateChances.Length; idx++)
        {
            if (randomChance < accumulateChances[idx])
            {
                return PopRandomItemByGrade((EItemGrade)idx, itemType);
            }
        }
        // 확률 합이 1이 안 넘을 경우, 아이템을 드랍하지 않음
        // 드랍 테이블이 빈 경우 처리 방안 필요
        Debug.Log("not drop item");
        return null;
    }

    public GameObject PopRandomItemByGrade(EItemGrade itemGrade, int itemType)
    {
        List<GameObject> itemList;

        if (itemType == 0)
            itemList = _consumableObjectLists[(int)itemGrade];
        else if (itemType == 1)
            itemList = _equipObjectLists[(int)itemGrade];
        else
        {
            Debug.Log("itemType error");
            return null;
        }

        if (itemList.Count == 0)
        {
            Debug.Log($"{itemGrade} Item List is empty");
            return null;
        }

        return PopRandomItemInList(itemList, itemType);
    }

    private GameObject PopRandomItemInList(List<GameObject> list, int itemType)
    {
        int randInt = UnityEngine.Random.Range(0, list.Count);
        var item = list[randInt];

        if (itemType == 1)
            list.RemoveAt(randInt);

        return item;
    }

    public GameObject DropConsumable()
    {
        List<GameObject> itemList = _consumableObjectLists[(int)EItemGrade.Normal];

        if (itemList.Count == 0)
        {
            Debug.Log("Consumable Item List is empty");
            return null;
        }

        return FindRandomItemInList(itemList);
    }

    /// <summary>
    /// ItemCompoenent를 가지고 있는 item 추가
    /// </summary>
    /// <param name="item"></param>
    private void AddItem(GameObject item, int playerType)
    {
        if (item.GetComponent<CItemComponent>() == null)
        {
            Debug.Log("Error - item hasn't CItemComponent");
            return;
        }

        int itemCode = item.GetComponent<CItemComponent>().Item.ItemCode / 100;
        int itemType = itemCode % 10;
        int itemGrade = (itemCode / 10) % 10;
        int itemPlayerType = itemCode / 100;

        // 소비 아이템
        if (itemType == 0)
        {
            _consumableObjectLists[itemGrade].Add(item);
        }
        // 장비 아이템
        else if (itemType == 1)
        {
            if (itemGrade < (int)EItemGrade.Rare)
                _equipObjectLists[itemGrade].Add(item);
            else
            {
                if (playerType == itemPlayerType)
                    _equipObjectLists[itemGrade].Add(item);
            }
        }
        // 기타 케이스는 오류 간주
        else
        {
            Debug.Log($"Can't Add Item By Item Code : {itemCode}");
        }
    }

    public GameObject PopItemByItemCode(int itemCode)
    {
        itemCode /= 100;
        int itemType = itemCode % 10;
        int itemGrade = itemCode / 10;
        GameObject item = null;

        item = GetItemObject(itemCode);
        // 소비 아이템
        if (itemType == 0)
        {
            _consumableObjectLists[itemGrade].Remove(item);
        }
        // 장비 아이템
        else if (itemType == 1)
        {
            _equipObjectLists[itemGrade].Remove(item);
        }
        // 기타 케이스는 오류 간주
        {
            Debug.Log("Can't Pop Item By Item Code");
        }

        return item;
    }

    private GameObject FindRandomItemInList(List<GameObject> list)
    {
        int randInt = UnityEngine.Random.Range(0, list.Count);
        Debug.Log($"find item{randInt}");
        return list[randInt];
    }
    #endregion

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
            Debug.LogWarning($"{item.name}'s UseEffect is omission");
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
        bool isFind = _itemDict.TryGetValue(itemCode, out var item);
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