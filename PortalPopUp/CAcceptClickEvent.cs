using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CAcceptClickEvent : MonoBehaviour
{
//<<<<<</*<*/ HEAD
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
//=======
//    public GameObject portalPopup;
//    public Button AcceptBtn;
//    public Button CancelBtn;
//    private CPlayerCommand _playerCommand;
//    private CWaitingForAccept _waiting;

//    private void Awake()
//    {
//        _playerCommand = GameObject.Find("GameManager").GetComponent<CPlayerCommand>();
//        _waiting = portalPopup.GetComponent<CWaitingForAccept>();
//        AcceptBtn.onClick.AddListener(ClickAccept);
//        CancelBtn.onClick.AddListener(ClickCancel);
//    }

//    public void ClickAccept()
//    {
//        Debug.Log("Click Accept");
//        _waiting.SetPlayerAccept(_playerCommand.ControlCharacterId, CWaitingForAccept.EAccept._accept);
//        InactiveButton();
//    }

//    public void ClickCancel()
//    {
//        Debug.Log("Click Cancel");
//        _waiting.SetPlayerAccept(_playerCommand.ControlCharacterId, CWaitingForAccept.EAccept._cancel);
//        InactiveButton();
//    }

//    private void InactiveButton()
//    {
//        AcceptBtn.interactable = false;
//        CancelBtn.interactable = false;
//>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
    }
}
