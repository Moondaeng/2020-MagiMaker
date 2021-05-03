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

    public enum ERoomType
    {
        _start,
        _normal,
        _event,
        _elite,
        _shop,
        _boss,
        _empty
    }

    #region room&portal
    protected GameObject startRoom;
    protected GameObject bossRoom;
    protected GameObject shopRoom;

    protected List<CPortal> _portals = new List<CPortal>();
    #endregion

    #region roomQueue
    public static readonly int ROOM_QUEUE_COUNTS = 3;
    public static readonly int VARIABLE_ROOMS_COUNT_IN_STAGE = CConstants.ROOM_PER_STAGE - 2;

    public Queue<GameObject> normalRoomQueue = new Queue<GameObject>();
    public Queue<GameObject> eventRoomQueue = new Queue<GameObject>();
    public Queue<GameObject> eliteRoomQueue = new Queue<GameObject>();
    //0노말 1이벤트 2엘리트
    protected Dictionary<ERoomType, Queue<GameObject>> _roomQueueDict = new Dictionary<ERoomType, Queue<GameObject>>();

    public int[,] randomRoomArray = new int[ROOM_QUEUE_COUNTS, VARIABLE_ROOMS_COUNT_IN_STAGE]; //패킷 보내기 위해 임의로 사이즈 고정, 노말방이든, 이벤트 방이든 같은 종류 개수 10개 넘어가면 수정필요
    #endregion

    #region memberVar
    public int StageNumber { get; protected set; } = 0;
    public ERoomType UserSelectRoom { get; protected set; } = ERoomType._empty;

    protected int _eliteCount = 0;
    protected int _eventCount = 0;
    protected int _shopCount = 0;
    protected int _roomCount = 0;
    protected ERoomType[] nextRoomTypeArr = new ERoomType[CConstants.MAX_ROAD];
    protected GameObject _currentRoom;
    protected int _portalMomCount = 0; //포탈맘 사용할때 태그를 이용해서 오브젝트를 받아오는데, 이 때 사라져야할 전방 포탈들도 가져와서 전 방 포탈들을 따로 하드코딩으로 제외하기 위한 변수
    #endregion

    #region Debug용 변수
    [Header("For Debug")]
    [SerializeField] protected List<GameObject> _explicitRoomList = new List<GameObject>();
    #endregion

    #region Init Function
    protected void Awake()
    {
        if (instance == null)
            instance = this;

        // 임시 초기화용
        nextRoomTypeArr[0] = ERoomType._normal;
        nextRoomTypeArr[1] = ERoomType._event;
        nextRoomTypeArr[2] = ERoomType._elite;

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                randomRoomArray[i, j] = -1;
            }
        }

        _roomQueueDict.Add(ERoomType._normal, normalRoomQueue);
        _roomQueueDict.Add(ERoomType._event, eventRoomQueue);
        _roomQueueDict.Add(ERoomType._elite, eliteRoomQueue);
        if (CClientInfo.JoinRoom.IsHost)
        {
            RandomRoomEnqueue(StageNumber);
        }

        LoadSpecialRoom();
    }

    protected virtual void LoadSpecialRoom()
    {
        startRoom = Resources.Load("Room/0/StartRoom0") as GameObject;
        bossRoom = Resources.Load("Room/0/BossRoom0") as GameObject;
        shopRoom = Resources.Load("Room/0/ShopRoom0") as GameObject;
    }

    protected void Start()
    {
        CCreateMap.instance.InstantiateRoom((int)CCreateMap.ERoomType._start, 0, new int[] { 1, 2, 3 });
    }

    protected void RandomRoomEnqueue(int stageNumber)  //스테이지의 종류에 따라 랜덤한 스테이지로 문자열을 만들어 반환함, random이 트루면 랜덤한 맵 리턴.
    {
        Debug.Log("Random Room Enqueue");

        string[] roomName = { "Room/" + stageNumber + "/NormalRoom", "Room/" + stageNumber + "/EventRoom", "Room/" + stageNumber + "/EliteRoom" };

        for (int i = 0; i < _roomQueueDict.Count; i++)
        {
            GameObject[] rooms = Resources.LoadAll<GameObject>(roomName[i]);
            int count = 0;
            int rand;
            int j = 0;

            while (count < rooms.Length)
            {
                rand = Random.Range(0, rooms.Length);
                if (rooms[rand] != null)
                {
                    _roomQueueDict[(ERoomType)i + 1].Enqueue(rooms[rand]);
                    randomRoomArray[i, j++] = rand;
                    rooms[rand] = null;
                    count++;
                }
            }
        }
    }
    #endregion

    #region Portal 관련
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

    public void NotifyPortal()
    {
        foreach (CPortal portal in _portals)
        {
            if (portal != null)
                portal.OpenNClosePortal();
        }
    }

    public void MakePortalText(ERoomType leftRoomType, ERoomType middleRoomType, ERoomType rightRoomType)
    {
        GameObject[] portalMom = GameObject.FindGameObjectsWithTag("PORTAL_MOM");

        for (int i = _portalMomCount; i < portalMom.Length; i++)
        {
            Transform portal = portalMom[i].transform.Find("Portal");
            Transform text = portalMom[i].transform.Find("PortalText").Find("Text");

            switch (portal.tag)
            {
                case "LEFT_PORTAL":
                    text.GetComponent<TextMeshProUGUI>().text = leftRoomType.ToString().Substring(1);
                    portalMom[i].SetActive(leftRoomType == ERoomType._empty ? false : true);
                    break;
                case "PORTAL":
                    text.GetComponent<TextMeshProUGUI>().text = middleRoomType.ToString().Substring(1);
                    portalMom[i].SetActive(middleRoomType == ERoomType._empty ? false : true);
                    break;
                case "RIGHT_PORTAL":
                    text.GetComponent<TextMeshProUGUI>().text = rightRoomType.ToString().Substring(1);
                    portalMom[i].SetActive(rightRoomType == ERoomType._empty ? false : true);
                    break;
            }
        }

        CGlobal.isClear = false; //포탈을 사용해서 새로운 방으로 왔으므로 방은 클리어되지 않은 상태
        NotifyPortal(); //플래그를 이용한 옵저버 패턴, 포탈 삭제하기
    }

    public void MakePortalText()
    {
        MakePortalText(nextRoomTypeArr[0], nextRoomTypeArr[1], nextRoomTypeArr[2]);
    }
    #endregion

    #region 방 생성 관련
    public int[] CreateNextRoomsInfo()
    {
        // 방 12개 전부 생성 시 대기
        if (_roomCount + 1 >= CConstants.ROOM_PER_STAGE) return null;

        MakeNextRoomTypeInfos(_roomCount + 1);
        return new int[] {
            (int)nextRoomTypeArr[0],  
            (int)nextRoomTypeArr[1],  
            (int)nextRoomTypeArr[2],  
        };
    }

    public System.Tuple<int, int, int[]> CreateNextRoomInfo(int roadNumber)
    {
        ERoomType selectedRoomType = nextRoomTypeArr[roadNumber];

        return new System.Tuple<int, int, int[]>(
            (int)selectedRoomType,
            GetRoomNumber(GetNextRoom(selectedRoomType)),
            CreateNextRoomsInfo()
            );
    }

    public virtual void CreateStage()
    {
        if (!CClientInfo.JoinRoom.IsHost)
        {
            Debug.Log("Guest can't create room type info");
            return;
        }

        if (_roomCount >= CConstants.ROOM_PER_STAGE) //방 12개 전부 생성 시 대기
            return;

        MakeNextRoomTypeInfos(_roomCount);
    }

    protected virtual void MakeNextRoomTypeInfos(int roomCount)
    {
        int randomRoad; //랜덤한 갈림길 개수
        int selectRoomType; //랜덤으로 방 종류 뽑기
        int roadCount;  //갈림길 숫자

        if (roomCount == CConstants.ROOM_PER_STAGE - 1)
        {
            nextRoomTypeArr[0] = ERoomType._boss; //보스방 따로 넣기
            return;
        }

        randomRoad = Random.Range(0, CConstants.MAX_ROAD - 1);
        roadCount = 0;
        selectRoomType = 123; //랜덤시드값
        for (int j = CConstants.MAX_ROAD; j > randomRoad; randomRoad++, roadCount++)
        {
            //최소 조건
            if (_shopCount < CConstants.MIN_SHOP_PER_STAGE && roomCount == 10 && roadCount == 0) //상점이 한번도 안나왔고 마지막 방일 때 첫번째 갈림길에는 무조건 상점방
            {
                nextRoomTypeArr[roadCount] = ERoomType._shop;
                _shopCount++;
                continue;
            }

            selectRoomType = ((selectRoomType * Random.Range(0, 100)) + Random.Range(0, 50)) % 100;

            int probability = CConstants.ELITE_PROBABILITY;
            if (selectRoomType < probability
                && _eliteCount < 2 && roomCount > (((CConstants.ROOM_PER_STAGE - 2) / 2) - 1)) //엘리트                                                                                    
            {
                nextRoomTypeArr[roadCount] = ERoomType._elite;
                _eliteCount++;
                continue;
            }

            probability += CConstants.EVENT_PROBABLILITY; //이벤트 방
            if (selectRoomType < probability && _eventCount < 15)
            {
                nextRoomTypeArr[roadCount] = ERoomType._event;
                _eventCount++;
                continue;
            }

            probability += CConstants.NORMAL_PROBABILITY;
            if (selectRoomType < probability) //일반 방
            {
                nextRoomTypeArr[roadCount] = ERoomType._normal;
                continue;
            }

            probability += CConstants.SHOP__PROBABILITY;
            if (selectRoomType < probability) // 상점
            {
                nextRoomTypeArr[roadCount] = ERoomType._shop;
                _shopCount++;
                continue;
            }
        }
    }

    public void RoomFlagCtrl(ERoomType roomType)
    {
        if (roomType == ERoomType._event ||
            roomType == ERoomType._elite ||
            roomType == ERoomType._normal || 
            roomType == ERoomType._shop)
        {
            UserSelectRoom = roomType;
        }
    }

    public void InstantiateRoom(int currentRoomType, int currentRoomNumber, int[] nextRoomTypeInfos)
    {
        GameObject loadRoomOrigin;
        if (IsExplicitRoom(_roomCount))
        {
            loadRoomOrigin = GetExplicitRoomInList(_roomCount);
        }
        else
        {
            loadRoomOrigin = LoadRoom(StageNumber, (ERoomType)currentRoomType, currentRoomNumber);
        }
        Debug.Log($"loaded room : {loadRoomOrigin.name}");
        var loadRoom = Instantiate(loadRoomOrigin);
        _currentRoom = loadRoom;

        AddPortal();
        MakePortalText((ERoomType)nextRoomTypeInfos[0], (ERoomType)nextRoomTypeInfos[1], (ERoomType)nextRoomTypeInfos[2]);
        _roomCount++;
    }

    public void DestroyRoom()
    {
        RemovePortal();
        Debug.Log("DestroyRoom : " + _currentRoom.name);
        Destroy(_currentRoom);

        //debug 용 태그로 방지우기
        //Object.Destroy(GameObject.FindGameObjectWithTag("DeleteRoom"));
        _currentRoom = null;
        return;
    }

    protected GameObject GetNextRoom(ERoomType roomType)
    {
        switch (roomType)
        {
            case ERoomType._start: return startRoom;
            case ERoomType._boss: return bossRoom;
            case ERoomType._event: return eventRoomQueue.Dequeue();
            case ERoomType._elite: return eliteRoomQueue.Dequeue();
            case ERoomType._normal: return normalRoomQueue.Dequeue();
            case ERoomType._shop: return shopRoom;
            case ERoomType._empty: return null;
            default: return null;
        }
    }

    protected int GetRoomNumber(GameObject room)
    {
        return int.Parse(room.name.Substring(System.Text.RegularExpressions.Regex.Match(room.name, "[0-9]").Index));
    }

    protected void InstantiateRoom(ERoomType roomType)
    {
        GameObject tempRoom = GetNextRoom(roomType);

        tempRoom = Object.Instantiate(tempRoom, tempRoom.transform.position, tempRoom.transform.rotation);
        _currentRoom = tempRoom;

        AddPortal();
        NotifyPortal();
        _roomCount++;
    }

    protected GameObject LoadRoom(int stageNumber, ERoomType currentRoomType, int roomNumber)
    {
        switch (currentRoomType)
        {
            case ERoomType._start:
                return startRoom;
            case ERoomType._normal:
                return Resources.Load($"Room/{stageNumber}/NormalRoom/NormalRoom{roomNumber}") as GameObject;
            case ERoomType._event:
                return Resources.Load($"Room/{stageNumber}/EventRoom/EventRoom{roomNumber}") as GameObject;
            case ERoomType._elite:
                return Resources.Load($"Room/{stageNumber}/EliteRoom/EliteRoom{roomNumber}") as GameObject;
            case ERoomType._shop:
                return shopRoom;
            case ERoomType._boss:
                return bossRoom;
            case ERoomType._empty:
                Debug.Log("Can't load empty room");
                return null;
            default:
                return null;
        }
    }
    #endregion

    #region 스테이지 관련
    /// <summary>
    /// 다음 스테이지로 넘어가면서 기존 스테이지 정보를 초기화하는 함수
    /// </summary>
    protected void InitNextStageInfo()
    {
        StageNumber++;
        _eliteCount = 0;
        _eventCount = 0;
        _shopCount = 0;
        _roomCount = 0;


    }

    protected void LoadStageRooms()
    {

    }
    #endregion

    #region Debug
    public void PrintCurrentRoomInfo()
    {
        Debug.Log($"currentRoom = {_currentRoom.name}");
        Debug.Log($"next Room = {nextRoomTypeArr[0]} {nextRoomTypeArr[1]} {nextRoomTypeArr[2]}");
    }

    protected bool IsExplicitRoom(int roomCount)
    {
        return roomCount < _explicitRoomList.Count && _explicitRoomList[roomCount] != null;
    }

    protected GameObject GetExplicitRoomInList(int elementNumber)
    {
        return _explicitRoomList[elementNumber];
    }
    #endregion
}