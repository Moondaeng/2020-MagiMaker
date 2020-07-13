using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CAcceptClickEvent : MonoBehaviour
{
    public GameObject portalPopup;
    public Button AcceptBtn;
    public Button CancelBtn;
    private CPlayerCommand _playerCommand;
    private CWaitingForAccept _waiting;

    private void Awake()
    {
        _playerCommand = GameObject.Find("GameManager").GetComponent<CPlayerCommand>();
        _waiting = portalPopup.GetComponent<CWaitingForAccept>();
        AcceptBtn.onClick.AddListener(ClickAccept);
        CancelBtn.onClick.AddListener(ClickCancel);
    }

    public void ClickAccept()
    {
        Debug.Log("Click Accept");
        _waiting.SetPlayerAccept(_playerCommand.ControlCharacterId, CWaitingForAccept.EAccept._accept);
        InactiveButton();
    }

    public void ClickCancel()
    {
        Debug.Log("Click Cancel");
        _waiting.SetPlayerAccept(_playerCommand.ControlCharacterId, CWaitingForAccept.EAccept._cancel);
        InactiveButton();
    }

    private void InactiveButton()
    {
        AcceptBtn.interactable = false;
        CancelBtn.interactable = false;
    }
}
