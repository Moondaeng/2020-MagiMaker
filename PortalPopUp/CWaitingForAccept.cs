using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CWaitingForAccept : MonoBehaviour
{
    private const int MAX_PLAYER_COUNT = 4;
    private int _playerCount;

    public enum EAccept
    {
        _waiting,
        _accept,
        _cancle
    }
    private int _acceptCount = 0;
    private EAccept[] _playerAccepts;

    public GameObject _portal;
    public GameObject _waitingForOtherPlayer;
    public GameObject _portalPopUp;
    public GameObject PortalAccept;
    public static CWaitingForAccept instance;

    public bool isVoteEnable = true;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        _playerCount = CPlayerCommand.instance.ActivatedPlayersCount;
        _playerAccepts = new EAccept[_playerCount];

        Debug.Log("playerCount " + _playerCount);
        SetPlayerSelect();
        ResetPortalUseSelect();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_waitingForOtherPlayer.activeSelf)
        {
            return;
        }

        // 승낙, 거절 버튼
        if (Input.GetKeyDown(KeyCode.T))
        {
            //CPortalManager.instance.MoveToNextRoom();
            SetPortalUseSelect(CPlayerCommand.instance.ControlCharacterID, EAccept._accept);
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            SetPortalUseSelect(CPlayerCommand.instance.ControlCharacterID, EAccept._cancle);
        }
    }

    public void SetPlayerSelect() //현재 플레이어의 숫자에 따라 팝업의 체크표시 개수 변경
    {
        for (int i = _playerCount; i < MAX_PLAYER_COUNT; i++)
        {
            _waitingForOtherPlayer.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void SetActivePortalPopup(bool value)
    {
        PortalAccept.SetActive(value);
        _waitingForOtherPlayer.SetActive(value);
    }

    public void SetPortalUseSelect(int playerNumber, EAccept opinion)
    {
        //if (opinion != EAccept._waiting && _playerAccepts[playerNumber] != EAccept._waiting)
        //{
        //    return;
        //}
        _playerAccepts[playerNumber] = opinion;
        LoadImage(playerNumber, opinion);

        if (opinion == EAccept._accept)
        {
            _acceptCount++;
            // 싱글 / 멀티 플레이용 확인
            //Network.CNetworkEvent.instance.PortalVoteEvent?.Invoke(0);
            //if (CPlayerCommand.instance.ActivatedPlayersCount <= _acceptCount)
            if (CClientInfo.JoinRoom.IsHost)
            {
                Debug.Log("Go Next Room");
                CPortalManager portalManager = GameObject.Find("PortalManager").GetComponent<CPortalManager>();
                SetActivePortalPopup(false);
                CPortalManager.instance.EnterNextRoom();
                //portalManager.MoveToNextRoom();

                CPlayerCommand.instance.Teleport(0, new Vector3(0, 1, 0));
                CPlayerCommand.instance.Teleport(1, new Vector3(0, 1, 4));
                CPlayerCommand.instance.Teleport(2, new Vector3(4, 1, 0));
                CPlayerCommand.instance.Teleport(3, new Vector3(4, 1, 4));

                //ResetPortalUseSelect();
            }
        }
        else if (opinion == EAccept._cancle)
        {
            // 취소 처리하고 몇 초 있다가 복구
            Invoke("CancelPortal", 3.0f);
            //Network.CNetworkEvent.instance.PortalVoteEvent?.Invoke(0);
        }
    }

    private void DisableVote()
    {

    }

    private void LoadImage(int childNumber, EAccept opinion)
    {
        Image image = _waitingForOtherPlayer.transform.GetChild(childNumber).GetComponent<Image>();
        switch (opinion)
        {
            case EAccept._waiting:
                image.sprite = Resources.Load<Sprite>("PortalAccept_wait") as Sprite;
                break;
            case EAccept._accept:
                image.sprite = Resources.Load<Sprite>("T_12_ok_") as Sprite;
                break;
            case EAccept._cancle:
                image.sprite = Resources.Load<Sprite>("T_11_no_") as Sprite;
                break;
        }
    }

    private void CancelPortal()
    {
        ResetPortalUseSelect();
        SetActivePortalPopup(false);
    }

    private void ResetPortalUseSelect()
    {
        _acceptCount = 0;
        for (int i = 0; i < _playerCount; i++)
        {
            SetPortalUseSelect(i, EAccept._waiting);
        }
    }
}