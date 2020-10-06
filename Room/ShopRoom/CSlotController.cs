using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CSlotController : MonoBehaviour
{
    [System.NonSerialized] public GameObject item;
    public void SetItem(GameObject item)
    {
        this.item = item;

        Debug.Log("bug");

        if(item == null)
        {
            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).gameObject.SetActive(false);
        }   
        else
        {
            if(item.GetComponent<CEquipComponent>() != null)
            {
                gameObject.transform.FindChild("Item").GetComponent<Image>().sprite = 
                    item.GetComponent<CEquipComponent>().Item.ItemImage;

                int itemcode = item.GetComponent<CEquipComponent>().Item.ItemCode;
                itemcode = 100 + itemcode / 10;
                int price = Random.Range(itemcode - 50, itemcode);
                gameObject.transform.FindChild("Price").GetComponent<TMPro.TextMeshProUGUI>().text = price.ToString();
            }
            else if(item.GetComponent<CConsumableComponent>() != null)
            {
                gameObject.transform.FindChild("Item").GetComponent<Image>().sprite =
                    item.GetComponent<CConsumableComponent>().Item.ItemImage;

                int itemcode = item.GetComponent<CConsumableComponent>().Item.ItemCode;
                itemcode = 100 + itemcode / 10;
                int price = Random.Range(itemcode - 50, itemcode) / 2;
                gameObject.transform.FindChild("Price").GetComponent<TMPro.TextMeshProUGUI>().text = price.ToString();
            }
        }
    }
}
