using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CReadyRoom : MonoBehaviour
{
    public enum MessageCode
    {
        GameStartAccept = 311,
        GuestQuitRoom = 312,
        HostQuitRoom = 313,
        NoticeUserQuit = 320,
        NewGuest = 321
    }

    public bool nonNetwork;
    public bool debugFlag;
    public Transform Canvas;
    public GameObject playerSlotPrefeb;
    public Transform UserPos;

    public Button startBtn;
    public Button leaveBtn;
    public Button AddBtn;
    
    private int currentUserIndex;
    private Network.CTcpClient _tcpManager;

    private void Awake()
    {
        if (AddBtn != null)
        {
            AddBtn.onClick.AddListener(() => AddGuest("new", 3, 15));
            if (debugFlag)
            {
                AddBtn.gameObject.SetActive(true);
            }
        }
    }

    void Start()
    {
        Debug.Log("In Ready Room");
        if (nonNetwork)
        {
            CClientInfo.ThisUser = new CClientInfo.User(1, "test", 1);

            int[] slot = new int[4] { 0, -1, 0, -1 };
            var others = new List<CClientInfo.User> {
                new CClientInfo.User(0, "abc", 10, 0),
                new CClientInfo.User(0, "kim", 15, 2)
            };
            CClientInfo.ThisRoom = new CClientInfo.JoinRoom(150, slot, others, false);
            CClientInfo.ThisUser.Slot = 1;
        }
        else
        {
            _tcpManager = GameObject.Find("Network").GetComponent<Network.CTcpClient>();
            if (_tcpManager != null)
                _tcpManager.SetPacketInterpret(PacketInterpret);
        }

        if (CClientInfo.ThisRoom.IsHost)
        {
            Debug.Log("I'm Host");
            // 방 생성 시
            var u = CClientInfo.ThisUser;
            AddPlayerToListView(u.Slot, u.id, u.clear);
        }
        else
        {
            Debug.Log("I'm not Host");
            // 방에 접속할 경우
            UpdateRoomListView();
        }

        if (startBtn != null)
        {
            if (!CClientInfo.ThisRoom.IsHost)
                startBtn.interactable = false;
            startBtn.onClick.AddListener(GameStartRequest);
        }
        if (leaveBtn != null)
        {
            //leaveBtn.onClick.AddListener(QuitRoomRequest);
            leaveBtn.onClick.AddListener(QuitRoomRequest);
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

        var message = Network.CPacketFactory.CreateGameStartPacket(CClientInfo.ThisRoom.RoomID);

        _tcpManager.Send(message.data);
    }

    public void QuitRoomRequest()
    {
        if (nonNetwork)
        {
            QuitRoom();
            return;
        }

        Network.CPacket message;
        if (CClientInfo.ThisRoom.IsHost)
        {
            message = Network.CPacketFactory.CreateHostQuitPacket(CClientInfo.ThisRoom.RoomID);
        }
        else
        {
            message = Network.CPacketFactory.CreateGuestQuitPacket(CClientInfo.ThisRoom.RoomID, CClientInfo.ThisUser.Slot);
        }
        _tcpManager.Send(message.data);
    }

    // for debug
    // 상대가 나가게 만듦
    public void BanRequest(int slotNum)
    {
        if(nonNetwork)
        {
            CClientInfo.JoinRoom.DeleteUser(slotNum);

            Debug.Log($"Delete User {slotNum}");

            UpdateRoomListView();

            return;
        }

        var message = Network.CPacketFactory.CreateBanRequest(slotNum);

        _tcpManager.Send(message.data);
    }

    // for debug
    // 손님 강제 추가
    public void AddGuestRequest()
    {
        var message = Network.CPacketFactory.CreateAddGuestRequest();

        _tcpManager.Send(message.data);
    }

    // 현재 오브젝트(룸 리스트) 위치에 새로운 룸 인스턴스를 추가한다
    public void AddPlayerToListView(int slotNum, string id, int clear)
    {
        var instance = Instantiate(playerSlotPrefeb);
        instance.transform.SetParent(Canvas, false);
        var newPosition = UserPos.localPosition + Vector3.down * instance.GetComponent<RectTransform>().sizeDelta.y * slotNum;
        instance.transform.localPosition = newPosition;
        instance.name = "User" + slotNum;
        
        Text playerIdText = instance.transform.Find("PlayerID").GetComponent<Text>();
        playerIdText.text = id;
        Text playerClearCountText = instance.transform.Find("ClearCount").GetComponent<Text>();
        playerClearCountText.text = clear.ToString();
        if(debugFlag)
        {
            var ban = instance.transform.Find("BanBtn");
            ban.gameObject.SetActive(true);
            var banBtn = ban.GetComponent<Button>();
            banBtn.onClick.AddListener(() => BanRequest(slotNum));
        }
    }

    private void UpdateRoomListView()
    {
        // 이전 UI를 지우는 작업
        ClearRoomListView();

        // 새 UI 그리는 작업
        var thisUser = CClientInfo.ThisUser;
        AddPlayerToListView(thisUser.Slot, thisUser.id, thisUser.clear);
        foreach (var user in CClientInfo.ThisRoom.Others)
        {
            AddPlayerToListView(user.Slot, user.id, user.clear);
        }
    }

    private void ClearRoomListView()
    {
        for(int slot = 0; slot < 4; slot++)
        {
            DeletePlayerToListView(slot);
        }
    }

    public void DeletePlayerToListView(int slotNum)
    {
        string userName = "User" + slotNum;
        var userUi = GameObject.Find(userName);
        if (userUi != null)
        {
            Destroy(userUi);
            Debug.Log($"delete user {slotNum} ui");
        }
    }

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
        if(nonNetwork)
        {
            Application.Quit();
            return;
        }

        // 로비로 전환 요청
        var message = Network.CPacketFactory.CreateReturnLobbyPacket();
        _tcpManager.Send(message.data);

        _tcpManager.DeletePacketInterpret();
        SceneManager.LoadScene("Lobby");
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
        CClientInfo.JoinRoom.UpdateRoom(slot, new CClientInfo.User(id, clear, slot));

        UpdateRoomListView();
    }

    // 인원이 빠져나감 알림
    private void DeleteQuitUser(Network.CPacket packet)
    {
        int slotNum = packet.ReadInt32();

        CClientInfo.JoinRoom.DeleteUser(slotNum);

        Debug.Log($"Delete User {slotNum}");
        
        UpdateRoomListView();
    }
}
