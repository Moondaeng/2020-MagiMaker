using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading;
using System.Collections;
using System;
using System.Text;
using System.Collections.Generic;

public class CLobby : MonoBehaviour
{
    public enum MessageCode
    {
        RoomCreateAccept = 250,
        RoomJoinAccept = 251,
        RoomListAccept = 252,
        RoomJoinFail = 254,
        Test = 655,
    }

    public bool debug; // debugMode
    public GameObject roomPrefeb; // inspector에서 프리펩 추가
    public Transform roomCreatePos;
    public int timeout = 10;
    public Text errorHandlingDisplay;

    public Transform RoomListTransform;
    public Button RefreshBtn;
    public Button CreateBtn;

    public Button quitMessageButton;
    public Button QuitBtn;

    public int TotalRoomCount { get; private set; }

    [SerializeField] GameObject _debugPanel;
    [SerializeField] Button _debugAddRoomBtn;
    [SerializeField] TMPro.TMP_Text _debugUserNameText;
    [SerializeField] TMPro.TMP_Text _debugClearCountText;

    private int joinRoomNumber;
    private Network.CTcpClient _tcpManager;
    private bool handling;
    private int requestResult;

    private class Room
    {
        public readonly int rid;
        public readonly int ucnt;
        public readonly string hname;

        public Room(int rid, int ucnt, string hname)
        {
            this.rid = rid;
            this.ucnt = ucnt;
            this.hname = hname;
        }
    }

    List<Room> rooms = new List<Room>();

    void Start()
    {
        if (debug)
        {
            CClientInfo.ThisUser = new CClientInfo.User(1, "test", 5);
        }

        _tcpManager = Network.CTcpClient.instance;
        _tcpManager.SetPacketInterpret(PacketInterpret);
        RefreshRoom();

        if (RefreshBtn != null)
        {
            RefreshBtn.onClick.AddListener(RefreshRoom);
        }
        if (CreateBtn != null)
        {
            CreateBtn.onClick.AddListener(CreateRoomHandler);
        }
        if (QuitBtn != null)
        {
            QuitBtn.onClick.AddListener(QuitLobby);
        }

        if (!CClientInfo.IsDebugMode)
        {
            _debugPanel.SetActive(false);
        }
        else
        {
            _debugAddRoomBtn.onClick.AddListener(AddFakeRoom);
            UpdateUserInfo();
        }

        Debug.Log("UID : " + CClientInfo.ThisUser.uid);
    }

    // 리프레시 버튼을 눌렀을 때 방 리스트를 갱신
    public void RefreshRoom()
    {
        if (debug)
        {
            DeleteAllRoomToListView();

            rooms.Clear();

            return;
        }

        var message = Network.CPacketFactory.CreateRoomListRequest();

        _tcpManager.Send(message.data);
    }

    // 방 생성
    public void CreateRoomHandler()
    {
        if (debug)
        {
            rooms.Add(new Room(-1, 1, "test"));
            Debug.LogFormat("Create Room : {0}", rooms.Count);

            RenewRoomListView();

            return;
        }

        var message = Network.CPacketFactory.CreateRoomCreateRequest(CClientInfo.ThisUser.id);

        _tcpManager.Send(message.data);
    }

    // 방에 들어가기
    public void JoinRoomRequest(int rid)
    {
        joinRoomNumber = rid;

        var message = Network.CPacketFactory.CreateRoomJoinRequest(rid, CClientInfo.ThisUser.id);

        _tcpManager.Send(message.data);
    }

    // 로비 나가기
    private void QuitLobby()
    {
        if (debug)
        {
            Application.Quit();
            return;
        }

        _tcpManager.SendShutdown();
        _tcpManager.DeletePacketInterpret();
        SceneManager.LoadScene("Start");
    }

    // 패킷 해석
    private void PacketInterpret(byte[] data)
    {
        // 헤더 읽기
        Network.CPacket packet = new Network.CPacket(data);
        packet.ReadHeader(out byte payloadSize, out short messageType);
        Debug.LogFormat("Lobby Header : payloadSize = {0}, messageType = {1}", payloadSize, messageType);

        switch ((int)messageType)
        {
            case (int)MessageCode.RoomCreateAccept:
                InterpretCreateRoom(packet);
                break;
            case (int)MessageCode.RoomJoinAccept:
                InterpretJoinRoom(packet, (int)payloadSize);
                break;
            case (int)MessageCode.RoomListAccept:
                InterpretRenewRoomList(packet, (int)payloadSize);
                break;
            case (int)MessageCode.RoomJoinFail:
                ErrorHandling("방 접속에 실패했습니다");
                break;
            default:
                ErrorHandling("오류가 발생했습니다. 개발자에게 문의하세요.");
                break;
        }
    }

    // 내 방을 만들기
    private void InterpretCreateRoom(Network.CPacket packet)
    {
        Debug.Log("room Create");
        int rid = packet.ReadInt32();

        CClientInfo.JoinRoom.CreateRoom(rid);
        CClientInfo.ThisUser.SlotNumber = 0;

        _tcpManager.DeletePacketInterpret();
        SceneManager.LoadScene("Room");
    }

    // 방 목록 갱신
    private void InterpretRenewRoomList(Network.CPacket packet, int payloadSize)
    {
        Debug.Log("Renew Room");

        rooms.Clear();

        int roomCount = payloadSize / CClientInfo.User.USER_DATA_SIZE;

        if (payloadSize % CClientInfo.User.USER_DATA_SIZE != 0)
        {
            Debug.Log("Room Renew Error!");
        }

        Debug.LogFormat("Room Count : {0}", roomCount);

        for (int i = 0; i < roomCount; i++)
        {
            int rid = packet.ReadInt32();
            int ucount = packet.ReadInt32();
            string hname = packet.ReadString(16);

            Debug.Log("Room id = " + rid + " " + hname);

            rooms.Add(new Room(rid, ucount, hname));
        }

        RenewRoomListView();
    }

    // 방 들어가기
    private void InterpretJoinRoom(Network.CPacket packet, int payloadSize)
    {
        CClientInfo.JoinRoom.JoinToRoom(joinRoomNumber, packet, payloadSize);

        _tcpManager.DeletePacketInterpret();
        SceneManager.LoadScene("Room");
    }

    #region UI
    // room 리스트를 읽고 현재 scrollView의 방 정보를 갱신한다
    public void RenewRoomListView()
    {
        DeleteAllRoomToListView();

        for (int i = 0; i < rooms.Count; i++)
        {
            var roomInfo = rooms[i];
            AddRoomToListView(roomInfo.rid, roomInfo.hname, roomInfo.ucnt, i);
        }
    }

    private void DeleteAllRoomToListView()
    {
        for (int i = 0; i < RoomListTransform.childCount; i++)
        {
            RoomListTransform.GetChild(i).gameObject.SetActive(false);
        }
    }

    // 현재 오브젝트(룸 리스트) 위치에 새로운 룸 인스턴스를 추가한다
    private void AddRoomToListView(int rid, string hostName, int rCnt, int roomNumber)
    {
        var roomInstance = AddRoomInstance();

        // 방 정보 기입
        Text roomName = roomInstance.transform.Find("Room Info Group/Room Name").GetComponent<Text>();
        Text roomCnt = roomInstance.transform.Find("Room Info Group/Room Count").GetComponent<Text>();
        roomName.text = hostName + "'s room";
        roomCnt.text = rCnt + "/4";
        Button roomEnter = roomInstance.transform.Find("Enter").GetComponent<Button>();
        roomEnter.onClick.AddListener(() => JoinRoomRequest(rid));
        roomEnter.onClick.AddListener(() => DisableButton(roomNumber));
    }

    private GameObject AddRoomInstance()
    {
        for (int i = 0; i < RoomListTransform.childCount; i++)
        {
            GameObject notUsedRoomInstance;
            if (!(notUsedRoomInstance = RoomListTransform.GetChild(i).gameObject).activeSelf)
            {
                notUsedRoomInstance.SetActive(true);
                notUsedRoomInstance.transform.Find("Enter").GetComponent<Button>().interactable = true;
                return notUsedRoomInstance;
            }
        }

        var roomInstance = Instantiate(roomPrefeb);
        var roomTransform = roomInstance.GetComponent<RectTransform>();
        var newPivot = new Vector2(0.5f, 1);
        roomInstance.GetComponent<RectTransform>().pivot = newPivot;
        var newLocalPosition = Vector3.down * roomTransform.rect.height * RoomListTransform.childCount;
        roomInstance.transform.localPosition = newLocalPosition;
        roomInstance.name = "ReadyRoom" + RoomListTransform.childCount;
        roomInstance.transform.SetParent(RoomListTransform, false);
        AutoRoomListViewer(roomTransform.rect.height);
        return roomInstance;
    }

    private void DisableButton(int roomNumber)
    {
        RoomListTransform.Find($"ReadyRoom" + roomNumber).Find("Enter").GetComponent<Button>().interactable = false;
    }

    private void AutoRoomListViewer(float roomInstanceHeight)
    {
        if (RoomListTransform.childCount >= 8)
        {
            var rectTransform = RoomListTransform.GetComponent<RectTransform>();
            var newSize = rectTransform.sizeDelta;
            newSize.y = RoomListTransform.childCount * roomInstanceHeight;
            rectTransform.sizeDelta = newSize;
        }
    }
    #endregion

    private void ErrorHandling(string errorMsg, string observer)
    {
        StopCoroutine(observer);

        errorHandlingDisplay.text = errorMsg;

        handling = false;
        requestResult = 0;
    }

    // 에러 텍스트 갱신
    private void ErrorHandling(string errorMsg)
    {
        errorHandlingDisplay.text = errorMsg;
    }

    #region Debug
    private void AddFakeRoom()
    {
        AddRoomToListView(5, "Fake", 3, RoomListTransform.childCount);
    }

    private void UpdateUserInfo()
    {
        _debugUserNameText.text = CClientInfo.ThisUser.id;
        _debugClearCountText.text = CClientInfo.ThisUser.clear.ToString();
    }
    #endregion
}