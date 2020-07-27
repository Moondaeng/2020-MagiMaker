using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CAcceptClickEvent : MonoBehaviour
{
    //public GameObject _waitingForOtherPlayer;

    public void ClickAccept()
    {
        //GameObject portalPopUp = GameObject.Find("PortalPopUp");
        //_waitingForOtherPlayer = portalPopUp.transform.FindChild("WaitingForOtherPlayer").gameObject;
        CWaitingForAccept.instance._waitingForOtherPlayer.SetActive(true);
        CWaitingForAccept.instance._player1Accept = CWaitingForAccept.EAccept._accept;
        Image image = CWaitingForAccept.instance._waitingForOtherPlayer.transform.GetChild(0).GetComponent<Image>();
        image.sprite = Resources.Load<Sprite>("T_12_ok_") as Sprite;
        CWaitingForAccept.instance._portalPopUp.SendMessage("TestAccept");
        CWaitingForAccept.instance._portalPopUp.transform.FindChild("PortalAccept").gameObject.SetActive(false);
    }
}
