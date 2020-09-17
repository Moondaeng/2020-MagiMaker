using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CWaitingForAccept : MonoBehaviour
{
    public enum EAccept
    {
        _waiting,
        _accept,
        _cancle
    }
    public EAccept _player1Accept;
    public EAccept _player2Accept;
    public EAccept _player3Accept;
    public EAccept _player4Accept;
    [HideInInspector] public GameObject _portal;
    [HideInInspector] public GameObject _waitingForOtherPlayer;
    [HideInInspector] public GameObject _portalPopUp;
    public static CWaitingForAccept instance = null;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        _player1Accept = EAccept._waiting;
        _player2Accept = EAccept._waiting;
        _player3Accept = EAccept._waiting;
        _player4Accept = EAccept._waiting;
        _portalPopUp = GameObject.Find("PortalPopUp");
        _waitingForOtherPlayer = _portalPopUp.transform.Find("WaitingForOtherPlayer").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (_player1Accept == EAccept._accept && _player2Accept == EAccept._accept && _player3Accept == EAccept._accept && _player4Accept == EAccept._accept)
        {
            _waitingForOtherPlayer.SetActive(false);

            UsePortal usePortal = instance._portal.GetComponent<UsePortal>();
            _player1Accept = EAccept._waiting; // 디버깅용
            usePortal.MoveToNextRoom();
        }
    }

    IEnumerator TestAccept()
    {
        _player2Accept = EAccept._accept;
        Image image = _waitingForOtherPlayer.transform.GetChild(1).GetComponent<Image>();
        image.sprite = Resources.Load<Sprite>("T_12_ok_") as Sprite;

        _player3Accept = EAccept._accept;
        image = _waitingForOtherPlayer.transform.GetChild(2).GetComponent<Image>();
        image.sprite = Resources.Load<Sprite>("T_12_ok_") as Sprite;

        _player4Accept = EAccept._accept;
        image = _waitingForOtherPlayer.transform.GetChild(3).GetComponent<Image>();
        image.sprite = Resources.Load<Sprite>("T_12_ok_") as Sprite;
        yield return null;
    }
}
