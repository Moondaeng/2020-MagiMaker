using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CWaitingForAccept : MonoBehaviour
{
    private const int MAX_PLAYER_COUNT = 4;

    public enum EAccept
    {
        _waiting,
        _accept,
        _cancle
    }
    private int _acceptCount = 0;
    private EAccept[] _playerAccepts = new EAccept[MAX_PLAYER_COUNT];

    public GameObject _portal;
    public GameObject _waitingForOtherPlayer;
    public GameObject _portalPopUp;
    public GameObject PortalAccept;
    public static CWaitingForAccept instance;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        ResetPortalUseSelect();
    }

    // Update is called once per frame
    void Update()
    {
        // 승낙, 거절 버튼
        if (Input.GetKeyDown(KeyCode.T))
        {
            SetPortalUseSelect(CPlayerCommand.instance.ControlCharacterId, EAccept._accept);
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            SetPortalUseSelect(CPlayerCommand.instance.ControlCharacterId, EAccept._cancle);
        }
    }

    public void SetActivePortalPopup(bool value)
    {
        PortalAccept.SetActive(value);
        _waitingForOtherPlayer.SetActive(value);
    }

    public void SetPortalUseSelect(int playerNumber, EAccept opinion)
    {
        if (opinion != EAccept._waiting && _playerAccepts[playerNumber] != EAccept._waiting)
        {
            return;
        }
        _playerAccepts[playerNumber] = opinion;
        LoadImage(playerNumber, opinion);

        if(opinion == EAccept._accept)
        {
            _acceptCount++;
            if(Network.CTcpClient.instance != null)
            {
                var packet = Network.CPacketFactory.CreatePortalVote(0);
                Network.CTcpClient.instance.Send(packet.data);
            }
            // 싱글 / 멀티 플레이용 확인
            if (CPlayerCommand.instance.activePlayersCount <= _acceptCount)
            {
                Debug.Log("Go Next Room");
                CPortalManager portalManager = GameObject.Find("PortalManager").GetComponent<CPortalManager>();
                SetActivePortalPopup(false);
                portalManager.MoveToNextRoom();

                ResetPortalUseSelect();
            }
        }
        else if(opinion == EAccept._cancle)
        {
            // 취소 처리하고 몇 초 있다가 복구
            Invoke("CancelPortal", 3.0f);
            if (Network.CTcpClient.instance != null)
            {
                var packet = Network.CPacketFactory.CreatePortalVote(1);
                Network.CTcpClient.instance.Send(packet.data);
            }
        }
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
        for(int i = 0; i < MAX_PLAYER_COUNT; i++)
        {
            SetPortalUseSelect(i, EAccept._waiting);
        }
    }

    //IEnumerator TestAccept()
    //{
    //    _player2Accept = EAccept._accept;
    //    Image image = _waitingForOtherPlayer.transform.GetChild(1).GetComponent<Image>();
    //    image.sprite = Resources.Load<Sprite>("T_12_ok_") as Sprite;

    //    _player3Accept = EAccept._accept;
    //    image = _waitingForOtherPlayer.transform.GetChild(2).GetComponent<Image>();
    //    image.sprite = Resources.Load<Sprite>("T_12_ok_") as Sprite;

    //    _player4Accept = EAccept._accept;
    //    image = _waitingForOtherPlayer.transform.GetChild(3).GetComponent<Image>();
    //    image.sprite = Resources.Load<Sprite>("T_12_ok_") as Sprite;
    //    yield return null;
    //}
}
