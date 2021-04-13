using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSkullButtonController : MonoBehaviour
{
    GameObject _skull;
    GameObject _popUp;
    private void Start()
    {
        _skull = GameObject.Find("Skull");
        _popUp = gameObject.transform.parent.gameObject;
    }
    public void ClickRandomItem()
    {
        Debug.Log("Get Item!");
        Debug.Log("Lose Max HP");
        _popUp.SetActive(false);
        Destroy(_skull);
    }
    
    public void ClickRandomMinorElement()
    {
        Debug.Log("Get Element!");
        Debug.Log("Lose Max HP");
        _popUp.SetActive(false);
        Destroy(_skull);
    }

    public void ClickCancel()
    {
        _popUp.SetActive(false);
    }
}
