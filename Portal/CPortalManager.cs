using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CPortalManager : DestroyableSingleton<CPortalManager>
{
    private const int MAX_PLAYER_COUNT = 4;

    public enum EPortalVote
    {
        Waiting,
        Accept,
        Cancel,
    }

    public class EnterNextRoomEvent : UnityEvent<int, int, int[]> { }


    private int _playerCount;
    private int _acceptCount = 0;
    private EPortalVote[] _playerAccepts;

    public string SelectedPortalStr;
    public GameObject PortalPopup;

    private GameObject[] player;
    private GameObject PortalAcceptParent;
    private GameObject FadeController;

    public static CPortalManager instance;

    public EnterNextRoomEvent EnterNextRoom = new EnterNextRoomEvent();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        player = GameObject.FindGameObjectsWithTag("Player");
        PortalAcceptParent = GameObject.Find("PortalPopUp");
        FadeController = GameObject.Find("FadeController");


        _playerCount = CPlayerCommand.instance.ActivatedPlayersCount;
        _playerAccepts = new EPortalVote[_playerCount];

        Debug.Log("playerCount " + _playerCount);
        SetPlayerSelect();
        ResetPortalUseSelect();
    }

    #region 기존 WaitingForAccept 내용
    void Update()
    {
        if (!PortalPopup.activeSelf)
        {
            return;
        }

        // 승낙, 거절 버튼
        if (Input.GetKeyDown(KeyCode.T))
        {
            //CPortalManager.instance.MoveToNextRoom();
            SetPortalUseSelect(CPlayerCommand.instance.ControlCharacterID, EPortalVote.Accept);
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            SetPortalUseSelect(CPlayerCommand.instance.ControlCharacterID, EPortalVote.Cancel);
        }
    }

    public void SetPlayerSelect() //현재 플레이어의 숫자에 따라 팝업의 체크표시 개수 변경
    {
        var waitingForOtherPlayer = PortalPopup.transform.Find("WaitingForOtherPlayer");
        for (int i = _playerCount; i < MAX_PLAYER_COUNT; i++)
        {
            waitingForOtherPlayer.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void SetActivePortalPopup(bool value)
    {
        PortalPopup.SetActive(value);
    }

    public void SetPortalUseSelect(int playerNumber, EPortalVote opinion)
    {
        //if (opinion != EAccept._waiting && _playerAccepts[playerNumber] != EAccept._waiting)
        //{
        //    return;
        //}
        _playerAccepts[playerNumber] = opinion;
        //LoadImage(playerNumber, opinion);

        if (opinion == EPortalVote.Accept)
        {
            _acceptCount++;
            // 싱글 / 멀티 플레이용 확인
            //Network.CNetworkEvent.instance.PortalVoteEvent?.Invoke(0);
            //if (CPlayerCommand.instance.ActivatedPlayersCount <= _acceptCount)
            if (CClientInfo.JoinRoom.IsHost)
            {
                Debug.Log("Go Next Room");
                PortalPopup.SetActive(false);
                var info = CCreateMap.instance.CreateNextRoomInfo(InvertSelectedPortalTagToInt(SelectedPortalStr));
                Debug.Log($"{info.Item1} {info.Item2} {info.Item3[0]}, {info.Item3[1]}, {info.Item3[2]}");
                EnterNextRoom?.Invoke(info.Item1, info.Item2, info.Item3);

                //MoveToNextRoom();

                CPlayerCommand.instance.Teleport(0, new Vector3(0, 1, 0));
                CPlayerCommand.instance.Teleport(1, new Vector3(0, 1, 4));
                CPlayerCommand.instance.Teleport(2, new Vector3(4, 1, 0));
                CPlayerCommand.instance.Teleport(3, new Vector3(4, 1, 4));

                //ResetPortalUseSelect();
            }
        }
        else if (opinion == EPortalVote.Cancel)
        {
            // 취소 처리하고 몇 초 있다가 복구
            Invoke("CancelPortal", 3.0f);
            //Network.CNetworkEvent.instance.PortalVoteEvent?.Invoke(0);
        }
    }

    private void DisableVote()
    {

    }

    //private void LoadImage(int childNumber, EPortalVote opinion)
    //{
    //    Image image = _waitingForOtherPlayer.transform.GetChild(childNumber).GetComponent<Image>();
    //    switch (opinion)
    //    {
    //        case EPortalVote.Waiting:
    //            image.sprite = Resources.Load<Sprite>("PortalAccept_wait") as Sprite;
    //            break;
    //        case EPortalVote.Accept:
    //            image.sprite = Resources.Load<Sprite>("T_12_ok_") as Sprite;
    //            break;
    //        case EPortalVote.Cancel:
    //            image.sprite = Resources.Load<Sprite>("T_11_no_") as Sprite;
    //            break;
    //    }
    //}

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
            SetPortalUseSelect(i, EPortalVote.Waiting);
        }
    }
    #endregion

    private int InvertSelectedPortalTagToInt(string portalTag)
    {
        switch (portalTag)
        {
            case "LEFT_PORTAL": return 0;
            case "PORTAL": return 1;
            case "RIGHT_PORTAL": return 2;
            default: return -1;
        }
    }

    public void MoveToNextRoom(int enteringRoomType, int enteringRoomNumber, int[] nextRoomTypeInfos)
    {
        FadeController.transform.Find("FadeCanvas").gameObject.SetActive(true);
        CFadeInOut.instance.PlayFadeFlow(); //다음 방 넘어갈 때, 페이드 아웃 방 생성 이후 페이드 인
        StartCoroutine(RefreshWorld(enteringRoomType, enteringRoomNumber, nextRoomTypeInfos));
    }

    public IEnumerator RefreshWorld(int enteringRoomType, int enteringRoomNumber, int[] nextRoomTypeInfos)
    {
        yield return new WaitForSeconds(1.0f);

        CCreateMap.instance.DestroyRoom();//오브젝트 삭제

        yield return new WaitForSeconds(1.0f); //삭제 후 잠시 대기(삭제되는 오브젝트 참조하는 경우가 생겼음)

        CCreateMap.instance.InstantiateRoom(enteringRoomType, enteringRoomNumber, nextRoomTypeInfos);
        CCreateMap.instance.RoomFlagCtrl((CCreateMap.ERoomType)enteringRoomType);

        // 방 이동에 따른 캐릭터 위치 이동
        CPlayerCommand.instance.Teleport(0, new Vector3(0, 1, 0));
        CPlayerCommand.instance.Teleport(1, new Vector3(0, 1, 4));
        CPlayerCommand.instance.Teleport(2, new Vector3(4, 1, 0));
        CPlayerCommand.instance.Teleport(3, new Vector3(4, 1, 4));
    }

    //public IEnumerator RefreshWorld()
    //{
    //    yield return new WaitForSeconds(1.0f);

    //    CCreateMap.instance.DestroyRoom();//오브젝트 삭제

    //    yield return new WaitForSeconds(1.0f); //삭제 후 잠시 대기(삭제되는 오브젝트 참조하는 경우가 생겼음)

    //    //방 배치
    //    int whichPortalSelect = InvertSelectedPortalStringToInt(SelectedPortalStr);

    //    Debug.Log($"whitchPortal = {whichPortalSelect}");

    //    CCreateMap.instance.CreateRoom(CCreateMap.instance.Rooms, CCreateMap.instance.getRoomCount(), whichPortalSelect); //유저가 플레이 할 방 생성
    //    CCreateMap.instance.RoomFlagCtrl(CCreateMap.instance.Rooms[CCreateMap.instance.getRoomCount(), whichPortalSelect]); //유저가 선택한 방 타입 저장

    //    // 방 이동에 따른 캐릭터 위치 이동
    //    CPlayerCommand.instance.Teleport(0, new Vector3(0, 1, 0));
    //    CPlayerCommand.instance.Teleport(1, new Vector3(0, 1, 4));
    //    CPlayerCommand.instance.Teleport(2, new Vector3(4, 1, 0));
    //    CPlayerCommand.instance.Teleport(3, new Vector3(4, 1, 4));

    //    CCreateMap.instance.CreateStage(); //현재 방 이후의 방들 맵으로 생성
    //}
}