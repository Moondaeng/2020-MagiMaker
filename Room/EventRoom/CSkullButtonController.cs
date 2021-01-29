using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CSkullButtonController : MonoBehaviour
{
    GameObject _skull;
    GameObject _popUp;
    private int _choose;
    private void Start()
    {
        _skull = GameObject.Find("Skull");
        _popUp = gameObject;
        _choose = 0; //기본 0

        _popUp.transform.GetChild(0).GetComponent<Image>().color = Color.white;
        _popUp.transform.GetChild(1).GetComponent<Image>().color = Color.grey;
        _popUp.transform.GetChild(2).GetComponent<Image>().color = Color.grey;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
            if (_choose < 2)
                _choose += 1;

        if (Input.GetKeyDown(KeyCode.UpArrow))
            if (_choose > 0)
                _choose -= 1;

        switch(_choose)
        {
            case 0:
                _popUp.transform.GetChild(0).GetComponent<Image>().color = Color.white;
                _popUp.transform.GetChild(1).GetComponent<Image>().color = Color.grey;
                _popUp.transform.GetChild(2).GetComponent<Image>().color = Color.grey;
                break;
            case 1:
                _popUp.transform.GetChild(0).GetComponent<Image>().color = Color.grey;
                _popUp.transform.GetChild(1).GetComponent<Image>().color = Color.white;
                _popUp.transform.GetChild(2).GetComponent<Image>().color = Color.grey;
                break;
            case 2:
                _popUp.transform.GetChild(0).GetComponent<Image>().color = Color.grey;
                _popUp.transform.GetChild(1).GetComponent<Image>().color = Color.grey;
                _popUp.transform.GetChild(2).GetComponent<Image>().color = Color.white;
                break;
        }

        if (Input.GetKeyDown(KeyCode.Return))
            switch (_choose)
            {
                case 0:
                    ClickRandomItem();
                    break;
                case 1:
                    ClickRandomMinorElement();
                    break;
                case 2:
                    ClickCancel();
                    break;
            }
}
    public void ClickRandomItem()
    {
        GameObject item = CItemDropTable.instance.DropRandomItem(CCreateMap.instance.GetStageNumber());
        item = Instantiate(item, _skull.transform.position, _skull.transform.rotation);
        item.SetActive(true);

        Debug.Log("Lose Max HP");
        _popUp.SetActive(false);
        CGlobal.useNPC = false;
        Destroy(_skull);
    }
    
    public void ClickRandomMinorElement()
    {
        Debug.Log("Get Element!");
        Debug.Log("Lose Max HP");
        _popUp.SetActive(false);
        CGlobal.useNPC = false;
        Destroy(_skull);
    }

    public void ClickCancel()
    {
        _popUp.SetActive(false);
        CGlobal.useNPC = false;
    }
}
