using System.Collections.Generic;
using UnityEngine;

public class CItemDropTable : MonoBehaviour
{
    [System.Serializable]
    public class DropItemGradeChanceList
    {
        [Range(0,1)] public float Normal;
        [Range(0,1)] public float Special;
        [Range(0,1)] public float Rare;
        [Range(0,1)] public float Unique;
    }

    [SerializeField]
    private DropItemGradeChanceList[] DropItemChanceInStages = new DropItemGradeChanceList[6];

    [SerializeField] private List<GameObject> _normalItemObjectList;
    [SerializeField] private List<GameObject> _specialItemObjectList;
    [SerializeField] private List<GameObject> _rareItemObjectList;
    [SerializeField] private List<GameObject> _uniqueItemObjectList;
    [SerializeField] private List<GameObject> _shopItemObjectList;
    [SerializeField] private List<GameObject> _eventItemObjectList;

    public GameObject DropRandomItem(int stage)
    {
        Debug.Log("Drop Random Item");
        float chanceSum = 0f;
        float[] chanceList = new float[4];
        chanceSum += DropItemChanceInStages[stage].Normal;
        chanceList[0] = chanceSum;
        chanceSum += DropItemChanceInStages[stage].Special;
        chanceList[1] = chanceSum;
        chanceSum += DropItemChanceInStages[stage].Rare;
        chanceList[2] = chanceSum;
        chanceSum += DropItemChanceInStages[stage].Unique;
        chanceList[3] = chanceSum;

        // 랜덤한 리스트 선택
        List<GameObject> itemList = null;
        float randomChance = UnityEngine.Random.Range(0f, 1f);
        if(randomChance < chanceList[0])
        {
            itemList = _normalItemObjectList;
        }
        else if (randomChance < chanceList[1])
        {
            itemList = _specialItemObjectList;
        }
        else if (randomChance < chanceList[2])
        {
            itemList = _rareItemObjectList;
        }
        else if (randomChance < chanceList[3])
        {
            itemList = _uniqueItemObjectList;
        }

        // 확률 합이 1이 안 넘을 경우, 아이템을 드랍하지 않음
        if(itemList == null)
        {
            return null;
        }
        else
        {
            return FindRandomItemInList(itemList);
            // 드랍 테이블이 빈 경우 처리 방안 필요
        }
    }

    private GameObject FindRandomItemInList(List<GameObject> itemList)
    {
        if(itemList.Count == 0)
        {
            Debug.Log("Item List is empty");
            return null;
        }

        int randInt = UnityEngine.Random.Range(0, itemList.Count);
        if(randInt == itemList.Count)
        {
            randInt--;
        }

        var item = itemList[randInt];
        itemList.RemoveAt(randInt);
        
        return item;
    }

    public GameObject FindItemByItemCode(int itemCode)
    {
        return null;
    }
}
