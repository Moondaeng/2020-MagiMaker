using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CAcceptClickEvent : MonoBehaviour
{
    public GameObject _waitingForOtherPlayer;

    public GameObject portalPopup;
    public Button AcceptBtn;
    public Button CancelBtn;
    private CPlayerCommand _playerCommand;
    private CWaitingForAccept _waiting;

    private void Awake()
    {
        //_playerCommand = CPlayerCommand.instance.GetComponent<CPlayerCommand>();
        //_waiting = portalPopup.GetComponent<CWaitingForAccept>();
    }

    // 버튼을 사용하지 않으므로 obsolete
    //public void ClickAccept()
    //{
    //    Debug.Log("Click Accept");
    //    _waiting.SetPlayerAccept(_playerCommand.ControlCharacterId, CWaitingForAccept.EAccept._accept);
    //    InactiveButton();
    //}

    //public void ClickAccept()
    //{
    //    //GameObject portalPopUp = GameObject.Find("PortalPopUp");
    //    //_waitingForOtherPlayer = portalPopUp.transform.FindChild("WaitingForOtherPlayer").gameObject;
    //    CWaitingForAccept.instance._waitingForOtherPlayer.SetActive(true);
    //    CWaitingForAccept.instance._player1Accept = CWaitingForAccept.EAccept._accept;
    //    Image image = CWaitingForAccept.instance._waitingForOtherPlayer.transform.GetChild(0).GetComponent<Image>();
    //    image.sprite = Resources.Load<Sprite>("T_12_ok_") as Sprite;
    //    CWaitingForAccept.instance._portalPopUp.SendMessage("TestAccept");
    //    CWaitingForAccept.instance._portalPopUp.transform.Find("PortalAccept").gameObject.SetActive(false);
    //}

    //public void ClickCancel()
    //{
    //    Debug.Log("Click Cancel");
    //    _waiting.SetPlayerAccept(_playerCommand.ControlCharacterId, CWaitingForAccept.EAccept._cancel);
    //    InactiveButton();
    //}

    //private void InactiveButton()
    //{
    //    AcceptBtn.interactable = false;
    //    CancelBtn.interactable = false;
    //}
}
