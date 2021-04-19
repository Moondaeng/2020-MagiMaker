using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CReadyRoom : MonoBehaviour
{
    public enum MessageCode
    {
        GameStartAccept = 351,
        GuestQuitRoom = 352,
        HostQuitRoom = 353,
        NoticeUserQuit = 354,
        NewGuest = 355
    }

    private class DebugPanel
    {
        public TMPro.TMP_Text stateText;
        public Button addPlayerButton;
        public Button banPlayerButton;

        public DebugPanel() { }
    }

    private class UserInfoPanel
    {
        public GameObject panelObject;
        public TMPro.TMP_Text nameText;
        public TMPro.TMP_Text clearCountText;
    }

    public bool nonNetwork;
    public bool debugFlag;

    [SerializeField]
    private Transform _playerListTransform;
    private UserInfoPanel[] _userPanels = new UserInfoPanel[4];

    public Button startBtn;
    public Button leaveBtn;

    public GameObject quitMessagePanel;
    public Button quitMessageButton;

    #region Debug 관련 변수
    [SerializeField]
    private Transform _debugPanel;
    [SerializeField]
    private Button _forceStartButton;
    private DebugPanel[] _debugPlayerPanels = new DebugPanel[4];
    #endregion

    private Network.CTcpClient _tcpManager;

    private void Awake()
    {
        for (int i = 0; i < CClientInfo.JoinRoom.MAX_PLAYER_COUNT; i++)
        {
            var userPanel = _playerListTransform.Find($"UserPanel{i}");
            _userPanels[i] = new UserInfoPanel
            {
                panelObject = userPanel.gameObject,
                nameText = userPanel.Find("Name").GetComponent<TMPro.TMP_Text>(),
                clearCountText = userPanel.Find("ClearCountText").GetComponent<TMPro.TMP_Text>(),
            };

            var childPlayerPanel = _debugPanel.Find($"PlayerPanel{i}");
            _debugPlayerPanels[i] = new DebugPanel
            {
                stateText = childPlayerPanel.Find("StateText").GetComponent<TMPro.TMP_Text>(),
                addPlayerButton = childPlayerPanel.Find("AddButton").GetComponent<Button>(),
                banPlayerButton = childPlayerPanel.Find("BanButton").GetComponent<Button>(),
            };
        }
    }

    void Start()
    {
        if ((_tcpManager = Network.CTcpClient.instance) != null)
        {
            _tcpManager.SetPacketInterpret(PacketInterpret);
        }

        if (CClientInfo.JoinRoom.IsHost)
        {
            // 방 생성 시
            Debug.Log("I'm Host");
        }
        else
        {
            // 방에 접속할 경우
            Debug.Log("I'm not Host");
        }
        UpdateRoomListView();

        if (startBtn != null)
        {
            if (!CClientInfo.JoinRoom.IsHost)
                startBtn.interactable = false;
            startBtn.onClick.AddListener(GameStartRequest);
        }
        if (leaveBtn != null)
        {
            leaveBtn.onClick.AddListener(QuitRoomRequest);
        }
        if (CClientInfo.IsDebugMode)
        {
            if (_forceStartButton != null)
            {
                _forceStartButton.onClick.AddListener(ForceGameStart);
            }

            for (int i = 0; i < 4; i++)
            {
                UpdateDebugPlayerPanel(i);
                var slotNum = i;
                _debugPlayerPanels[i].addPlayerButton.onClick.AddListener(() => AddGuestRequest(slotNum));
                _debugPlayerPanels[i].banPlayerButton.onClick.AddListener(() => BanRequest(slotNum));
            }
        }
    }

    private void OnApplicationQuit()
    {
        // 순서 보장 필요
        if (_tcpManager != null && _tcpManager.IsConnect)
        {
            QuitRoomRequest();
            _tcpManager.SendShutdown();
        }
    }

    private void OnDisable()
    {
        Debug.Log("Exiting Room");
    }

    public void GameStartRequest()
    {
        if(nonNetwork)
        {
            GameStart(1);
        }

        var message = Network.CPacketFactory.CreateGameStartPacket(CClientInfo.JoinRoom.RoomID);

        _tcpManager.Send(message.data);
    }

    public void QuitRoomRequest()
    {
        Network.CPacket message;
        if (CClientInfo.JoinRoom.IsHost)
        {
            message = Network.CPacketFactory.CreateHostQuitPacket(CClientInfo.JoinRoom.RoomID);
        }
        else
        {
            message = Network.CPacketFactory.CreateGuestQuitPacket(CClientInfo.JoinRoom.RoomID, CClientInfo.ThisUser.SlotNumber);
        }
        _tcpManager.Send(message.data);
    }

    #region UI
    public void AddPlayerToListView(int slotNum, string id, int clear)
    {
        _userPanels[slotNum].panelObject.SetActive(true);
        _userPanels[slotNum].nameText.text = id;
        _userPanels[slotNum].clearCountText.text = clear.ToString();
    }

    private void UpdateRoomListView()
    {
        // 이전 UI를 지우는 작업
        ClearRoomListView();

        // 새 UI 그리는 작업
        var thisUser = CClientInfo.ThisUser;
        AddPlayerToListView(thisUser.SlotNumber, thisUser.id, thisUser.clear);
        foreach (var user in CClientInfo.JoinRoom.Others)
        {
            AddPlayerToListView(user.SlotNumber, user.id, user.clear);
        }
    }

    private void ClearRoomListView()
    {
        for (int i = 0; i < CClientInfo.JoinRoom.MAX_PLAYER_COUNT; i++)
        {
            _userPanels[i].panelObject.SetActive(false);
        }
    }

    public void DeletePlayerToListView(int slotNum)
    {
        _userPanels[slotNum].panelObject.SetActive(false);
    }
    #endregion

    private void PacketInterpret(byte[] data)
    {
        // 헤더 읽기
        Network.CPacket packet = new Network.CPacket(data);
        packet.ReadHeader(out byte payloadSize, out short messageType);
        Debug.LogFormat("ReadyRoom Header : payloadSize = {0}, messageType = {1}", payloadSize, messageType);

        switch ((int)messageType)
        {
            case (int)MessageCode.GameStartAccept:
                GameStart(packet.ReadInt32());
                break;
            case (int)MessageCode.GuestQuitRoom:
                QuitRoom();
                break;
            case (int)MessageCode.HostQuitRoom:
                QuitRoom();
                break;
            case (int)MessageCode.NoticeUserQuit:
                DeleteQuitUser(packet);
                break;
            case (int)MessageCode.NewGuest:
                InterpretAddGuest(packet);
                break;
            default:
                Debug.Log("오류가 발생했습니다. 개발자에게 문의하세요.");
                break;
        }
    }

    private void GameStart(int playerCount)
    {
        CClientInfo.PlayerCount = playerCount;
        SceneManager.LoadScene("Prototype");
    }

    private void QuitRoom()
    {
        ReturnLobbyRequest();
        SceneManager.LoadScene("Lobby");
    }

    private void HostQuitRoom()
    {
        ReturnLobbyRequest();
        quitMessagePanel.SetActive(true);
        quitMessageButton.onClick.AddListener(() => SceneManager.LoadScene("Lobby"));
    }

    private void ReturnLobbyRequest()
    {
        // 로비로 전환 요청
        var message = Network.CPacketFactory.CreateReturnLobbyPacket();
        _tcpManager.Send(message.data);

        _tcpManager.DeletePacketInterpret();
    }

    // 인원이 추가된 경우
    private void InterpretAddGuest(Network.CPacket packet)
    {
        string id = packet.ReadString(16);
        int slot = packet.ReadInt32();
        int clear = packet.ReadInt32();

        AddGuest(id, slot, clear);
    }

    // 인원이 추가된 경우
    private void AddGuest(string id, int slot, int clear)
    {
        CClientInfo.JoinRoom.AddUser(slot, new CClientInfo.User(id, clear, slot));

        UpdateRoomListView();
        UpdateDebugPlayerPanel(slot);
    }

    // 인원이 빠져나감 알림
    private void DeleteQuitUser(Network.CPacket packet)
    {
        int slotNum = packet.ReadInt32();

        CClientInfo.JoinRoom.DeleteUser(slotNum);

        Debug.Log($"Delete User {slotNum}");
        
        UpdateRoomListView();
        UpdateDebugPlayerPanel(slotNum);
    }

    #region Debug
    private void ForceGameStart()
    {
        var message = Network.CPacketFactory.CreateGameStartPacket(CClientInfo.JoinRoom.RoomID);

        _tcpManager.Send(message.data);
    }

    private void UpdateDebugPlayerPanel(int slotNumber)
    {
        _debugPlayerPanels[slotNumber].stateText.text 
            = CClientInfo.JoinRoom.Slots[slotNumber] == CClientInfo.JoinRoom.ESlotState.Open ? "비어있음" : "존재";
        _debugPlayerPanels[slotNumber].addPlayerButton.interactable 
            = CClientInfo.JoinRoom.Slots[slotNumber] == CClientInfo.JoinRoom.ESlotState.Open ? true : false;
        _debugPlayerPanels[slotNumber].banPlayerButton.interactable
            = CClientInfo.JoinRoom.Slots[slotNumber] == CClientInfo.JoinRoom.ESlotState.Open ? false : true;
    }

    private void AddGuestRequest(int slotNumber)
    {
        var message = Network.CPacketFactory.CreateAddGuestRequest(slotNumber);

        _tcpManager.Send(message.data);
    }

    private void BanRequest(int slotNum)
    {
        if (nonNetwork)
        {
            CClientInfo.JoinRoom.DeleteUser(slotNum);

            Debug.Log($"Delete User {slotNum}");

            UpdateRoomListView();

            return;
        }

        var message = Network.CPacketFactory.CreateBanRequest(slotNum);

        _tcpManager.Send(message.data);
    }
    #endregion
}
