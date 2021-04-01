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
            int playerType = (item.GetComponent<CItemComponent>().Item.ItemCode / 10000);
            CItemDropTable.instance.AddItem(item, playerType);
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