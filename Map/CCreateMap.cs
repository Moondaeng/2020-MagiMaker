using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
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
    public Queue<GameObject> normalRoomQueue = new Queue<GameObject>();
    public Queue<GameObject> eventRoomQueue = new Queue<GameObject>();
    public Queue<GameObject> skillEliteRoomQueue = new Queue<GameObject>();
    public Queue<GameObject> itemEliteRoomQueue = new Queue<GameObject>();
    #endregion

    #region memberVar
    private int _stageNumber;
    private int _skillEliteCount;
    private int _eventCount;
    private int _shopCount;
    private bool _noRoomFlag;  //트루일 경우 방이 없음
    private CGlobal.ERoomType _roomTypeFlag;
    private int _roomCount;
    private CRoom[,] _roomArr;
    private LinkedList<GameObject> _rooms;
    private LinkedListNode<GameObject> _tempRoomNode;
    #endregion

    private void Start()
    {
        startRoom = Resources.Load("Room/StartRoom0") as GameObject;
        bossRoom = Resources.Load("Room/BossRoom0") as GameObject;
        shopRoom = Resources.Load("Room/ShopRoom0") as GameObject;

        _portals = new List<CPortal>();

        if (instance == null)
            instance = this;

        _stageNumber = 0;
        _skillEliteCount = 0;
        _eventCount = 0;
        _shopCount = 0;
        _noRoomFlag = true;
        _roomTypeFlag = CGlobal.ERoomType._empty;
        _roomCount = 0; //방의 개수가 12개가 넘어가면 멈춰줄 변수

        _roomArr = new CRoom[CConstants.ROOM_PER_STAGE, CConstants.MAX_ROAD];
        for (int i = 0; i < CConstants.ROOM_PER_STAGE; i++)
            for (int j = 0; j < CConstants.MAX_ROAD; j++)
                _roomArr[i, j] = new CRoom();

        _rooms = new LinkedList<GameObject>();

        RandomRoomEnqueue();
        CreateStage();
    }

    public void AddPortal()
    {
        GameObject[] portalMom = GameObject.FindGameObjectsWithTag("PORTAL_MOM");

        foreach (GameObject ob in portalMom)
            _portals.Add(ob.transform.FindChild("Portal").GetComponent<CPortal>());
    }

    public void RemovePortal()
    {
        _portals.RemoveRange(0, _portals.Count);
    }

    public void NotifyPortal()
    { 
        for (int i = 0; i < _portals.Count; i++)
            _portals[i].OpenNClosePortal();
    }

    public int GetStageNumber()
    {
        return _stageNumber;
    }

    public CRoom[,] GetRooms()
    {
        return _roomArr;
    }

    public void RandomRoomEnqueue()  //스테이지의 종류에 따라 랜덤한 스테이지로 문자열을 만들어 반환함, random이 트루면 랜덤한 맵 리턴.
    {
        //일반 방
        string randomRoom = "Room/NormalRoom" + _stageNumber + "_";
        int j, max = 0;
        bool isSame;

        while (Resources.Load(randomRoom + max.ToString()) != null) //랜덤맵의 개수 확인
            max++;
        int[] check = new int[max];
        for (int i = 0; i < max; i++) //중복없는 랜덤을 만들기 위해 중복확인용 배열 생성
            check[i] = -1;

        for (int i = 0; i < max; i++)
        {
            while (true)
            {
                isSame = false;
                j = Random.Range(0, max);


                for (int a = 0; a < max; a++)  //중복확인
                {
                    if (check[a] == j)
                    {
                        isSame = true;
                        break;
                    }
                }

                if (!isSame) //중복 없을 경우 루프문 탈출
                {
                    check[i] = j;
                    normalRoomQueue.Enqueue(Resources.Load<GameObject>(randomRoom + j.ToString()));
                    break;
                }
            }
        }

        //스킬엘리트 방
        randomRoom = "Room/SkillEliteRoom" + _stageNumber + "_";
        max = 0;

        while (Resources.Load(randomRoom + max.ToString()) != null) //랜덤맵의 개수 확인
            max++;

        check = new int[max];
        for (int i = 0; i < max; i++) //중복없는 랜덤을 만들기 위해 중복확인용 배열 생성
            check[i] = -1;

        for (int i = 0; i < max; i++)
        {
            while (true)
            {
                isSame = false;
                j = Random.Range(0, max);

                for (int a = 0; a < max; a++)  //중복확인
                {
                    if (check[a] == j)
                    {
                        isSame = true;
                        break;
                    }
                }

                if (!isSame) //중복 없을 경우 루프문 탈출
                {
                    check[i] = j;
                    skillEliteRoomQueue.Enqueue(Resources.Load<GameObject>(randomRoom + j.ToString()));
                    break;
                }
            }
        }

        //아이템엘리트 방
        randomRoom = "Room/ItemEliteRoom" + _stageNumber + "_";
        max = 0;

        while (Resources.Load(randomRoom + max.ToString()) != null) //랜덤맵의 개수 확인
            max++;
        check = new int[max];
        for (int i = 0; i < max; i++) //중복없는 랜덤을 만들기 위해 중복확인용 배열 생성
            check[i] = -1;

        for (int i = 0; i < max; i++)
        {
            while (true)
            {
                isSame = false;
                j = Random.Range(0, max);

                for (int a = 0; a < max; a++)  //중복확인
                {
                    if (check[a] == j)
                    {
                        isSame = true;
                        break;
                    }
                }

                if (!isSame) //중복 없을 경우 루프문 탈출
                {
                    check[i] = j;
                    itemEliteRoomQueue.Enqueue(Resources.Load<GameObject>(randomRoom + j.ToString()));
                    break;
                }
            }
        }

        //이벤트 방
        randomRoom = "Room/EventRoom" + _stageNumber + "_";
        max = 0;

        while (Resources.Load(randomRoom + max.ToString()) != null) //랜덤맵의 개수 확인
            max++;
        check = new int[max];
        for (int i = 0; i < max; i++) //중복없는 랜덤을 만들기 위해 중복확인용 배열 생성
            check[i] = -1;

        for (int i = 0; i < max; i++)
        {
            while (true)
            {
                isSame = false;
                j = Random.Range(0, max);

                for (int a = 0; a < max; a++)  //중복확인
                {
                    if (check[a] == j)
                    {
                        isSame = true;
                        break;
                    }
                }

                if (!isSame) //중복 없을 경우 루프문 탈출
                {
                    check[i] = j;
                    eventRoomQueue.Enqueue(Resources.Load<GameObject>(randomRoom + j.ToString()));
                    break;
                }
            }
        }

    }

    public void CreateStage()
    {
        if (_roomCount >= CConstants.ROOM_PER_STAGE) //방 12개 전부 생성 시 대기
            return;

        int randomRoad; //랜덤한 갈림길 개수
        int selectRoomType; //랜덤으로 방 종류 뽑기
        int roadCount;  //갈림길 숫자

        if (_roomCount == 0)
        {
            _roomArr[0, 0].RoomType = CGlobal.ERoomType._start;
            InstantiateRoom(_roomArr[_roomCount, 0].RoomType); //시작방 생성
        }

        if (_roomCount == CConstants.ROOM_PER_STAGE - 1)
        {
            _roomArr[CConstants.ROOM_PER_STAGE - 1, 0].RoomType = CGlobal.ERoomType._boss; //보스방 따로 넣기
            return;
        }


        randomRoad = Random.Range(0, CConstants.MAX_ROAD - 1);
        _noRoomFlag = true;
        roadCount = 0;
        selectRoomType = 123; //랜덤시드값
        for (int j = CConstants.MAX_ROAD; j > randomRoad; randomRoad++, roadCount++)
        {
            //최소 조건
            if (_shopCount < CConstants.MIN_SHOP_PER_STAGE && _roomCount == 10 && roadCount == 0) //상점이 한번도 안나왔고 마지막 방일 때 첫번째 갈림길에는 무조건 상점방
            {
                _roomArr[_roomCount, roadCount].RoomType = CGlobal.ERoomType._shop;
                _shopCount++;
                RoomFlagCtrl(CGlobal.ERoomType._shop);
                continue;
            }
            if (_noRoomFlag == true && j - randomRoad == 1) //조건들에 걸려 방이 아예 안나오는 경우 방지
            {
                _roomArr[_roomCount, roadCount].RoomType = CGlobal.ERoomType._normal;
                RoomFlagCtrl(CGlobal.ERoomType._normal);
                continue;
            }

            selectRoomType = ((selectRoomType * Random.Range(0, 100)) + Random.Range(0, 50)) % 100;

            if (selectRoomType < CConstants.SKILL_PROBABILITY
                && _skillEliteCount < 2 && _roomCount > (((CConstants.ROOM_PER_STAGE - 2) / 2) - 1))                                                                                                //스킬 엘리트
            {
                _roomArr[_roomCount, roadCount].RoomType = CGlobal.ERoomType._skillElite;
                _skillEliteCount++;
                RoomFlagCtrl(CGlobal.ERoomType._skillElite);
                continue;
            }

            if (selectRoomType >= CConstants.SKILL_PROBABILITY
                && selectRoomType < CConstants.SKILL_PROBABILITY + CConstants.SHOP__PROBABILITY)             // 상점
            {
                _roomArr[_roomCount, roadCount].RoomType = CGlobal.ERoomType._shop;
                _shopCount++;
                RoomFlagCtrl(CGlobal.ERoomType._shop);
                continue;
            }

            if (selectRoomType >= CConstants.SKILL_PROBABILITY + CConstants.SHOP__PROBABILITY
                && selectRoomType < CConstants.SKILL_PROBABILITY + CConstants.SHOP__PROBABILITY + CConstants.NORMAL_PROBABILITY)//일반 방
            {
                _roomArr[_roomCount, roadCount].RoomType = CGlobal.ERoomType._normal;
                RoomFlagCtrl(CGlobal.ERoomType._normal);
                continue;
            }

            if (selectRoomType >= CConstants.SKILL_PROBABILITY + CConstants.SHOP__PROBABILITY + CConstants.NORMAL_PROBABILITY
                && selectRoomType < CConstants.SKILL_PROBABILITY + CConstants.SHOP__PROBABILITY + CConstants.NORMAL_PROBABILITY + CConstants.EVENT_PROBABLILITY
                && _eventCount < 15)                                                                                                    //이벤트
            {
                _roomArr[_roomCount, roadCount].RoomType = CGlobal.ERoomType._event;
                _eventCount++;
                RoomFlagCtrl(CGlobal.ERoomType._event);
                continue;
            }

            if (selectRoomType >= CConstants.SKILL_PROBABILITY + CConstants.SHOP__PROBABILITY + CConstants.NORMAL_PROBABILITY + CConstants.EVENT_PROBABLILITY
                && selectRoomType < CConstants.SKILL_PROBABILITY + CConstants.SHOP__PROBABILITY + CConstants.NORMAL_PROBABILITY + CConstants.EVENT_PROBABLILITY + CConstants.ITEM_PROBABILITY) //아이템 엘리트
            {
                _roomArr[_roomCount, roadCount].RoomType = CGlobal.ERoomType._itemElite;
                RoomFlagCtrl(CGlobal.ERoomType._itemElite);
                continue;
            }
        }

        MakePortalText(_roomCount);
    }

    public void RoomFlagCtrl(CGlobal.ERoomType roomType)
    {
        _noRoomFlag = false;
        switch (roomType)
        {
            case CGlobal.ERoomType._event:
                _roomTypeFlag = CGlobal.ERoomType._event;
                break;
            case CGlobal.ERoomType._itemElite:
                _roomTypeFlag = CGlobal.ERoomType._itemElite;
                break;
            case CGlobal.ERoomType._normal:
                _roomTypeFlag = CGlobal.ERoomType._normal;
                break;
            case CGlobal.ERoomType._shop:
                _roomTypeFlag = CGlobal.ERoomType._shop;
                break;
            case CGlobal.ERoomType._skillElite:
                _roomTypeFlag = CGlobal.ERoomType._skillElite;
                break;
        }
    }

    public void CreateRoom(CRoom[,] roomArr, int roomCount, int roadCount)
    {
        //debug용 보고싶은 맵 있으면 여기다 가져다 두면 됨.
        if (roomCount == 1)
        {
            GameObject temproom = Resources.Load("Room/ShopRoom0") as GameObject;
            Object.Instantiate(temproom, temproom.transform.position, temproom.transform.rotation);
            _roomCount++;
            AddPortal();
            NotifyPortal();
            return;
        }
        //이것도 마찬가지로 디버그용
        //if (roomCount == 2)
        //{
        //    GameObject temproom = Resources.Load("Room/EventRoom0_1") as GameObject;
        //    Object.Instantiate(temproom, temproom.transform.position, temproom.transform.rotation);
        //    _roomCount++;
        //    AddPortal();
        //    NotifyPortal();
        //    return;
        //}

        InstantiateRoom(roomArr[roomCount, roadCount].RoomType);
    }

    private void InstantiateRoom(CGlobal.ERoomType roomType)
    {
        GameObject tempRoom;

        switch (roomType)
        {
            case CGlobal.ERoomType._start:
                tempRoom = Object.Instantiate(startRoom, startRoom.transform.position, startRoom.transform.rotation);
                _rooms.AddLast(tempRoom);
                break;

            case CGlobal.ERoomType._boss:
                tempRoom = Object.Instantiate(bossRoom, bossRoom.transform.position, bossRoom.transform.rotation);
                _rooms.AddLast(tempRoom);
                break;

            case CGlobal.ERoomType._event:
                tempRoom = eventRoomQueue.Dequeue();
                tempRoom = Object.Instantiate(tempRoom, tempRoom.transform.position, tempRoom.transform.rotation);
                _rooms.AddLast(tempRoom);
                break;

            case CGlobal.ERoomType._itemElite:
                tempRoom = itemEliteRoomQueue.Dequeue();
                tempRoom = Object.Instantiate(tempRoom, tempRoom.transform.position, tempRoom.transform.rotation);
                _rooms.AddLast(tempRoom);
                break;

            case CGlobal.ERoomType._normal:
                tempRoom = normalRoomQueue.Dequeue();
                tempRoom = Object.Instantiate(tempRoom, tempRoom.transform.position, tempRoom.transform.rotation);
                _rooms.AddLast(tempRoom);
                break;

            case CGlobal.ERoomType._shop:
                tempRoom = Object.Instantiate(shopRoom, shopRoom.transform.position, shopRoom.transform.rotation);
                _rooms.AddLast(tempRoom);
                break;

            case CGlobal.ERoomType._skillElite:
                tempRoom = skillEliteRoomQueue.Dequeue();
                tempRoom = Object.Instantiate(tempRoom, tempRoom.transform.position, tempRoom.transform.rotation);
                _rooms.AddLast(tempRoom);
                break;
            case CGlobal.ERoomType._empty:
                Debug.Log("Can't create empty room error");
                return;
        }

        AddPortal();
        NotifyPortal();
        _roomCount++;
    }

    public void DestroyRoom()
    {
        RemovePortal();
        _tempRoomNode = _rooms.First;
        Object.Destroy(_tempRoomNode.Value);

        //debug 용 태그로 방지우기
        //Object.Destroy(GameObject.FindGameObjectWithTag("DeleteRoom"));
        _rooms.RemoveFirst();
        return;
    }

    public void MakePortalText(int roomCount) //포탈 위에 다음 방이 어떤 방인지 알려주는 텍스트 생성, 빈 방인 경우 포탈 삭제
    {
        GameObject[] portalMom = GameObject.FindGameObjectsWithTag("PORTAL_MOM");

        foreach (GameObject ob in portalMom)
        {
            Transform portal = ob.transform.Find("Portal");
            Transform text = ob.transform.Find("PortalText").Find("Text");

            switch (portal.tag)
            {
                case "LEFT_PORTAL":
                    text.GetComponent<TextMeshProUGUI>().text = _roomArr[roomCount, 0].RoomType.ToString().Substring(1);
                    if (_roomArr[roomCount, 0].RoomType == CGlobal.ERoomType._empty)
                        GameObject.Destroy(ob);
                    break;
                case "PORTAL":
                    text.GetComponent<TextMeshProUGUI>().text = _roomArr[roomCount, 1].RoomType.ToString().Substring(1);
                    if (_roomArr[roomCount, 1].RoomType == CGlobal.ERoomType._empty)
                        GameObject.Destroy(ob);
                    break;
                case "RIGHT_PORTAL":
                    text.GetComponent<TextMeshProUGUI>().text = _roomArr[roomCount, 2].RoomType.ToString().Substring(1);
                    if (_roomArr[roomCount, 2].RoomType == CGlobal.ERoomType._empty)
                        GameObject.Destroy(ob);
                    break;
            }
        }
    }

    public int getRoomCount()
    {
        return _roomCount;
    }
}