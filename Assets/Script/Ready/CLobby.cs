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
        RoomCreateAccept = 210,
        RoomJoinAccept = 211,
        RoomListAccept = 212,
        RoomCountAccept = 213,
        RoomJoinFail = 214,
    }

    public bool debug; // debugMode
    public GameObject roomPrefeb; // inspector에서 프리펩 추가
    public Transform roomCreatePos;
    public int timeout = 10;
    public Text errorHandlingDisplay;

    public Transform canvas;
    public Button RefreshBtn;
    public Button CreateBtn;
    public Button QuitBtn;

    public int TotalRoomCount { get; private set; }

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
        if(debug)
        {
            CClientInfo.ThisUser = new CClientInfo.User(1, "test", 5);
        }

        var obj = GameObject.Find("Network");
        if (obj)
        {
            _tcpManager = obj.GetComponent<Network.CTcpClient>();
            _tcpManager.SetPacketInterpret(PacketInterpret);
            RefreshRoom();
        }
        //tcpClient = (Network.CTcpClient)FindObjectOfType(typeof(Network.CTcpClient));

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

        Debug.Log("UID : " + CClientInfo.ThisUser.uid);
    }

    // 리프레시 버튼을 눌렀을 때 방 리스트를 갱신
    public void RefreshRoom()
    {
        if(debug)
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
        if(debug)
        {
            rooms.Add(new Room(-1, 1, "test"));
            Debug.LogFormat("Create Room : {0}", rooms.Count);

            RenewRoomListView();

            return;
        }

        var message =  Network.CPacketFactory.CreateRoomCreateRequest(CClientInfo.ThisUser.id);

        _tcpManager.Send(message.data);
    }

    // 방에 들어가기
    public void JoinRoomRequest(int rid)
    {
        if(debug)
        {
            int[] slot = new int[4] { 0, -1, 0, -1 };
            var others = new List<CClientInfo.User> {
                new CClientInfo.User(0, "abc", 10, 0),
                new CClientInfo.User(0, "kim", 15, 2),
                null
            };
            CClientInfo.ThisRoom = new CClientInfo.JoinRoom(150, slot, others, false);
            CClientInfo.ThisUser.Slot = 1;
            //CClientInfo.ThisUser.State = 0;

            SceneManager.LoadScene("Room");
            return;
        }
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
            case (int)MessageCode.RoomCountAccept:
                ErrorHandling("아이디 혹은 비밀번호가 일치하지 않습니다.");
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

        CClientInfo.ThisRoom = new CClientInfo.JoinRoom(rid, new int[4] { 0, -1, -1, -1 }, new List<CClientInfo.User>(), true);
        CClientInfo.ThisUser.Slot = 0;
        
        _tcpManager.DeletePacketInterpret();
        SceneManager.LoadScene("Room");
    }

    // 방 목록 갱신
    private void InterpretRenewRoomList(Network.CPacket packet, int payloadSize)
    {
        Debug.Log("Renew Room");

        DeleteAllRoomToListView();
        rooms.Clear();

        int roomCount = payloadSize / 24;

        if (payloadSize % 24 != 0)
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
        CClientInfo.ThisRoom = CClientInfo.JoinRoom.GetRoomInfo(joinRoomNumber, packet, payloadSize);

        _tcpManager.DeletePacketInterpret();
        SceneManager.LoadScene("Room");
    }

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
        for (int i = 0; i < rooms.Count; i++)
        {
            string name = "ReadyRoom" + i;
            Debug.Log(name);
            Destroy(GameObject.Find(name));
        }
    }

    // 현재 오브젝트(룸 리스트) 위치에 새로운 룸 인스턴스를 추가한다
    public void AddRoomToListView(int rid, string hostName, int rCnt, int roomNumber)
    {
        // 방 추가
        int roomSize = 10;
        var instance = Instantiate(roomPrefeb);
        instance.transform.SetParent(canvas, false);
        var newPosition = roomCreatePos.position + Vector3.down * roomSize * roomNumber;
        instance.transform.position = newPosition;
        instance.name = "ReadyRoom" + roomNumber;

        // 방 정보 기입
        Text roomName = instance.transform.Find("Room Info Group/Room Name").GetComponent<Text>();
        Text roomCnt = instance.transform.Find("Room Info Group/Room Count").GetComponent<Text>();
        roomName.text = hostName + "'s room";
        roomCnt.text = rCnt + "/4";
        Button roomEnter = instance.transform.Find("Enter").GetComponent<Button>();
        roomEnter.onClick.AddListener(() => JoinRoomRequest(rid));
    }

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
}