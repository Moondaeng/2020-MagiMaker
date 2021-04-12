using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static System.Console;

// 해야할 것 정리 : 포탈 생성 삭제하고 포탈 사용 시 페이드 아웃하면서 맵에 잇는 오브젝트들 삭제하고 새로운 맵 불러오고 플레이어 위치 조정
// _rooms는 방을 관리하는 링크드 리스트지만 변경사항으로 실제로 존재하는 방은 1개만 계속 유지되기에 추후 수정필요. 링크드 리스트는 필요없다.
public class CCreateMap : MonoBehaviour
{
    static public CCreateMap instance = null;

    #region room&portal
    private GameObject startRoom;
    private GameObject bossRoom;
    private GameObject shopRoom;

    private List<CPortal> _portals;
    #endregion

    #region roomQueue
    private Dictionary<CGlobal.ERoomType, Queue<GameObject>> _roomQueueDict = new Dictionary<CGlobal.ERoomType, Queue<GameObject>>(); //0노말 1이벤트 2엘리트
    public Queue<GameObject> normalRoomQueue = new Queue<GameObject>();
    public Queue<GameObject> eventRoomQueue = new Queue<GameObject>();
    public Queue<GameObject> eliteRoomQueue = new Queue<GameObject>();
    public int[,] randomRoomArray = new int[3, 10]; //패킷 보내기 위해 임의로 사이즈 고정, 노말방이든, 이벤트 방이든 같은 종류 개수 10개 넘어가면 수정필요
    #endregion

    #region memberVar
    public int _stageNumber;
    private int _eliteCount;
    private int _eventCount;
    private int _shopCount;
    private CGlobal.ERoomType _userSelectRoom;
    public int _roomCount;
    public CRoom[,] _roomArr;
    private LinkedList<GameObject> _rooms;
    private LinkedListNode<GameObject> _tempRoomNode;
    private int _portalMomCount; //포탈맘 사용할때 태그를 이용해서 오브젝트를 받아오는데, 이 때 사라져야할 전방 포탈들도 가져와서 전 방 포탈들을 따로 하드코딩으로 제외하기 위한 변수
    #endregion

    #region Debug용 변수
    [SerializeField] private List<GameObject> _explicitRoomList = new List<GameObject>();
    #endregion

    #region 스테이지 이벤트
    public class CreateRoomEvent : UnityEngine.Events.UnityEvent<CRoom[,]> { }
    public CreateRoomEvent CreateRooms = new CreateRoomEvent();
    #endregion

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        startRoom = Resources.Load("Room/0/StartRoom0") as GameObject;
        bossRoom = Resources.Load("Room/0/BossRoom0") as GameObject;
        shopRoom = Resources.Load("Room/0/ShopRoom0") as GameObject;

        _portals = new List<CPortal>();

        _stageNumber = 0;
        _eliteCount = 0;
        _eventCount = 0;
        _shopCount = 0;
        _userSelectRoom = CGlobal.ERoomType._empty;
        _roomCount = 0; //방의 개수가 12개가 넘어가면 멈춰줄 변수
        _portalMomCount = 0;

        _roomArr = new CRoom[CConstants.ROOM_PER_STAGE, CConstants.MAX_ROAD];
        for (int i = 0; i < CConstants.ROOM_PER_STAGE; i++)
            for (int j = 0; j < CConstants.MAX_ROAD; j++)
                _roomArr[i, j] = new CRoom();

        for(int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                randomRoomArray[i, j] = -1;
            }
        }

        _rooms = new LinkedList<GameObject>();

        if (CClientInfo.JoinRoom.IsHost)
        {
            RandomRoomEnqueue(_stageNumber);
        }
        Debug.Log("Create Start Room");

        // CreateStartRoom
        if (_roomCount == 0)
        {
            _roomArr[0, 0].RoomType = CGlobal.ERoomType._start;
            InstantiateRoom(_roomArr[_roomCount, 0].RoomType); //시작방 생성
            //MakePortalText(_roomCount, _roomArr);
        }
    }

    public void SendRoomArr()
    {
        // debug code
        //for (int i = 0; i < CConstants.ROOM_PER_STAGE; i++)
        //{
        //    string roomData = $"rooms row {i} : ";
        //    for (int j = 0; j < CConstants.MAX_ROAD; j++)
        //    {
        //        roomData += (int)_roomArr[i, j].RoomType;
        //    }
        //    Debug.Log(roomData);
        //}

        CreateRooms?.Invoke(_roomArr);
    }

    public void ReceiveRoomArr(CRoom[,] roomArr)
    {
        Debug.Log("Receive Room Arr");
        _roomArr = roomArr;
        MakePortalText(_roomCount, _roomArr);
    }

    public void ReceiveRoomArr(int[,] intRoomArr)
    {
        Debug.Log("Receive Room Arr");
        for (int i = 0; i < CConstants.ROOM_PER_STAGE; i++)
        {
            for (int j = 0; j < CConstants.MAX_ROAD; j++)
            {
                _roomArr[i, j].RoomType = (CGlobal.ERoomType)intRoomArr[i, j];
            }
        }
        MakePortalText(_roomCount, _roomArr);
    }

    public void CreateRoom(CRoom[,] roomArr, int roomCount, int roadCount)
    {
        Debug.Log("Create Room");
        //debug용 보고싶은 맵 있으면 여기다 가져다 두면 됨.
        if (_roomCount < _explicitRoomList.Count && CreateExplicitRoomInList(_roomCount))
        {
            return;
        }

        InstantiateRoom(roomArr[roomCount, roadCount].RoomType);
    }

    public void AddPortal()
    {

        GameObject[] portalMom = GameObject.FindGameObjectsWithTag("PORTAL_MOM");

        for (int i = _portalMomCount; i < portalMom.Length; i++)
            _portals.Add(portalMom[i].transform.Find("Portal").GetComponent<CPortal>());
    }

    public void RemovePortal()
    {
        _portals.Clear();
    }

    public CGlobal.ERoomType userSelectRoom()
    {
        return _userSelectRoom;
    }

    public void NotifyPortal()
    {
        foreach (CPortal portal in _portals)
        {
            if (portal != null)
                portal.OpenNClosePortal();
        }
    }

    public int GetStageNumber()
    {
        return _stageNumber;
    }

    public CRoom[,] GetRooms()
    {
        return _roomArr;
    }

    public void RandomRoomEnqueue(int stageNumber)  //스테이지의 종류에 따라 랜덤한 스테이지로 문자열을 만들어 반환함, random이 트루면 랜덤한 맵 리턴.
    {
        Debug.Log("Random Room Enqueue");

        string[] roomName = { "Room/" + stageNumber + "/NormalRoom", "Room/" + stageNumber + "/EventRoom", "Room/" + stageNumber + "/EliteRoom" };
        _roomQueueDict.Add(CGlobal.ERoomType._normal, normalRoomQueue);
        _roomQueueDict.Add(CGlobal.ERoomType._event, eventRoomQueue);
        _roomQueueDict.Add(CGlobal.ERoomType._elite, eliteRoomQueue);

        for (int i = 0; i < _roomQueueDict.Count; i++)
        {
            GameObject[] rooms = Resources.LoadAll<GameObject>(roomName[i]);
            foreach (var room in rooms)
            {
                Debug.Log(room.name);
            }
            int count = 0;
            int rand;
            int j = 0;

            while (count < rooms.Length)
            {
                rand = Random.Range(0, rooms.Length);
                if (rooms[rand] != null)
                {
                    _roomQueueDict[(CGlobal.ERoomType)i + 1].Enqueue(rooms[rand]);
                    randomRoomArray[i, j++] = rand;
                    rooms[rand] = null;
                    count++;
                }
            }
        }
    }

    public void NonHostRoomEnqueue(int[,] randomRoom ,int stageNumber)
    {
        string[] roomName = { "Room/" + stageNumber + "/NormalRoom/NormalRoom", "Room/" + stageNumber + "/EventRoom/EventRoom", "Room/" + stageNumber + "/EliteRoom/EliteRoom" };
        _roomQueueDict.Add(CGlobal.ERoomType._normal, normalRoomQueue);
        _roomQueueDict.Add(CGlobal.ERoomType._event, eventRoomQueue);
        _roomQueueDict.Add(CGlobal.ERoomType._elite, eliteRoomQueue);

        for (int i = 0; i < _roomQueueDict.Count; i++)
        {
            int j = 0;
            while(randomRoom.GetLength(i) > j && randomRoom[i, j] != -1)
            {
                GameObject room = Resources.Load(roomName[i] + randomRoom[i,j].ToString()) as GameObject;
                _roomQueueDict[(CGlobal.ERoomType)i + 1].Enqueue(room);
                j++;
            }
        }
    }

    public void CreateStageVerTuto()
    {
        int randomRoad; //랜덤한 갈림길 개수
        int selectRoomType; //랜덤으로 방 종류 뽑기
        int roadCount;  //갈림길 숫자

        if (_roomCount > 5) //방 전부 생성됬을경우 대기
            return;

        if (_roomCount == 0)
        {
            if (_explicitRoomList.Count != 0)
            {
                CreateExplicitRoomInList(0);
            }
            else
            {
                _roomArr[0, 0].RoomType = CGlobal.ERoomType._start;
                InstantiateRoom(_roomArr[_roomCount, 0].RoomType); //시작방 생성
            }
        }

        if (_roomCount == 1)
        {
            _roomArr[1, 0].RoomType = CGlobal.ERoomType._elite; //일반방 넣기
        }

        if (_roomCount == 2)
        {
            _roomArr[2, 0].RoomType = CGlobal.ERoomType._event; //이벤트방 넣기
        }

        if (_roomCount == 3)
        {
            _roomArr[3, 0].RoomType = CGlobal.ERoomType._normal; //아이템 엘리트방 넣기
        }

        if (_roomCount == 4)
        {
            _roomArr[4, 0].RoomType = CGlobal.ERoomType._shop; //상점방 넣기
        }

        if (_roomCount == 6 - 1)
        {
            _roomArr[6 - 1, 0].RoomType = CGlobal.ERoomType._boss; //보스방 따로 넣기
        }

        MakePortalText(_roomCount, _roomArr);
        SendRoomArr();
    }

    public void CreateStage()
    {
        if (!CClientInfo.JoinRoom.IsHost)
        {
            return;
        }

        Debug.Log("Host Create Stage");

        if (CGlobal.isTutorial)  //튜토리얼만 임의로 생성
        {
            CreateStageVerTuto();
            return;
        }

        if (_roomCount >= CConstants.ROOM_PER_STAGE) //방 12개 전부 생성 시 대기
            return;

        int randomRoad; //랜덤한 갈림길 개수
        int selectRoomType; //랜덤으로 방 종류 뽑기
        int roadCount;  //갈림길 숫자

        if (_roomCount == CConstants.ROOM_PER_STAGE - 1)
        {
            _roomArr[CConstants.ROOM_PER_STAGE - 1, 0].RoomType = CGlobal.ERoomType._boss; //보스방 따로 넣기
            return;
        }

        Debug.Log("return?");

        randomRoad = Random.Range(0, CConstants.MAX_ROAD - 1);
        roadCount = 0;
        selectRoomType = 123; //랜덤시드값
        for (int j = CConstants.MAX_ROAD; j > randomRoad; randomRoad++, roadCount++)
        {
            //최소 조건
            if (_shopCount < CConstants.MIN_SHOP_PER_STAGE && _roomCount == 10 && roadCount == 0) //상점이 한번도 안나왔고 마지막 방일 때 첫번째 갈림길에는 무조건 상점방
            {
                _roomArr[_roomCount, roadCount].RoomType = CGlobal.ERoomType._shop;
                _shopCount++;
                continue;
            }

            selectRoomType = ((selectRoomType * Random.Range(0, 100)) + Random.Range(0, 50)) % 100;

            int probability = CConstants.ELITE_PROBABILITY;
            if (selectRoomType < probability
                && _eliteCount < 2 && _roomCount > (((CConstants.ROOM_PER_STAGE - 2) / 2) - 1)) //엘리트                                                                                    
            {
                _roomArr[_roomCount, roadCount].RoomType = CGlobal.ERoomType._elite;
                _eliteCount++;
            }

            probability += CConstants.EVENT_PROBABLILITY; //이벤트 방
            if (selectRoomType < probability && _eventCount < 15)                                                                                         
            {
                _roomArr[_roomCount, roadCount].RoomType = CGlobal.ERoomType._event;
                _eventCount++;
            }

            probability += CConstants.NORMAL_PROBABILITY;
            if (selectRoomType < probability) //일반 방
            {
                _roomArr[_roomCount, roadCount].RoomType = CGlobal.ERoomType._normal;
            }

            probability += CConstants.SHOP__PROBABILITY;
            if (selectRoomType < probability) // 상점
            {
                _roomArr[_roomCount, roadCount].RoomType = CGlobal.ERoomType._shop;
                _shopCount++;
            }
        }

        SendRoomArr();
    }

    public void RoomFlagCtrl(CGlobal.ERoomType roomType)
    {
        switch (roomType)
        {
            case CGlobal.ERoomType._event:
                _userSelectRoom = CGlobal.ERoomType._event;
                break;
            case CGlobal.ERoomType._elite:
                _userSelectRoom = CGlobal.ERoomType._elite;
                break;
            case CGlobal.ERoomType._normal:
                _userSelectRoom = CGlobal.ERoomType._normal;
                break;
            case CGlobal.ERoomType._shop:
                _userSelectRoom = CGlobal.ERoomType._shop;
                break;
        }
    }

    private void InstantiateRoom(CGlobal.ERoomType roomType)
    {
        GameObject tempRoom = null;

        switch (roomType)
        {
            case CGlobal.ERoomType._start:
                Debug.Log("start room");
                tempRoom = startRoom;
                break;

            case CGlobal.ERoomType._boss:
                Debug.Log("boss room");
                tempRoom = bossRoom;
                break;

            case CGlobal.ERoomType._event:
                Debug.Log("event room");
                tempRoom = eventRoomQueue.Dequeue();
                break;

            case CGlobal.ERoomType._elite:
                Debug.Log("elite room");
                tempRoom = eliteRoomQueue.Dequeue();
                break;

            case CGlobal.ERoomType._normal:
                Debug.Log("normal room");
                tempRoom = normalRoomQueue.Dequeue();
                break;

            case CGlobal.ERoomType._shop:
                tempRoom = shopRoom;
                break;

            case CGlobal.ERoomType._empty:
                Debug.Log("Can't create empty room error");
                return;
        }

        tempRoom = Object.Instantiate(tempRoom, tempRoom.transform.position, tempRoom.transform.rotation);
        _rooms.AddLast(tempRoom);

        AddPortal();
        NotifyPortal();
        _roomCount++;
    }

    public void DestroyRoom()
    {
        RemovePortal();
        _tempRoomNode = _rooms.First;
        Debug.Log("DestroyRoom : " + _tempRoomNode.Value);
        Object.Destroy(_tempRoomNode.Value);

        //debug 용 태그로 방지우기
        //Object.Destroy(GameObject.FindGameObjectWithTag("DeleteRoom"));
        _rooms.RemoveFirst();
        return;
    }

    public void MakePortalText(int roomCount, CRoom[,] roomArr) //포탈 위에 다음 방이 어떤 방인지 알려주는 텍스트 생성, 빈 방인 경우 포탈 삭제
    {
        GameObject[] portalMom = GameObject.FindGameObjectsWithTag("PORTAL_MOM");

        for (int i = _portalMomCount; i < portalMom.Length; i++)
        {
            Transform portal = portalMom[i].transform.Find("Portal");
            Transform text = portalMom[i].transform.Find("PortalText").Find("Text");

            switch (portal.tag)
            {
                case "LEFT_PORTAL":
                    text.GetComponent<TextMeshProUGUI>().text = roomArr[roomCount, 0].RoomType.ToString().Substring(1);
                    if (roomArr[roomCount, 0].RoomType == CGlobal.ERoomType._empty)
                        portalMom[i].SetActive(false);
                    break;
                case "PORTAL":
                    text.GetComponent<TextMeshProUGUI>().text = roomArr[roomCount, 1].RoomType.ToString().Substring(1);
                    if (roomArr[roomCount, 1].RoomType == CGlobal.ERoomType._empty)
                        portalMom[i].SetActive(false);
                    break;
                case "RIGHT_PORTAL":
                    text.GetComponent<TextMeshProUGUI>().text = roomArr[roomCount, 2].RoomType.ToString().Substring(1);
                    if (roomArr[roomCount, 2].RoomType == CGlobal.ERoomType._empty)
                        portalMom[i].SetActive(false);
                    break;
            }
        }

        CGlobal.isClear = false; //포탈을 사용해서 새로운 방으로 왔으므로 방은 클리어되지 않은 상태
        NotifyPortal(); //플래그를 이용한 옵저버 패턴, 포탈 삭제하기
    }

    public int getRoomCount()
    {
        return _roomCount;
    }

    #region Debug
    private bool CreateExplicitRoomInList(int elementNumber)
    {
        if (_explicitRoomList[elementNumber] == null)
        {
            Debug.Log("can't create Explicit Room");
            return false;
        }

        var roomOrigin = _explicitRoomList[elementNumber];
        var copy = Instantiate(roomOrigin, roomOrigin.transform.position, roomOrigin.transform.rotation);
        _rooms.AddLast(copy);
        _roomCount++;
        AddPortal();
        NotifyPortal();
        return true;
    }
    #endregion
}