using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCreateStage
{
    #region room&portal
    public GameObject portal = Resources.Load("Portal/PortalMom") as GameObject;
    public GameObject leftPortal = Resources.Load("Portal/LeftPortalMom") as GameObject;
    public GameObject rightPortal = Resources.Load("Portal/RightPortalMom") as GameObject;
    public GameObject startRoom = Resources.Load("Room/StartRoom1") as GameObject;
    public GameObject bossRoom = Resources.Load("Room/BossRoom1") as GameObject;
    public GameObject itemEliteRoom = Resources.Load("Room/ItemEliteRoom1_0") as GameObject;
    public GameObject skillEliteRoom = Resources.Load("Room/SkillEliteRoom1_0") as GameObject;
    public GameObject eventRoom = Resources.Load("Room/EventRoom1_0") as GameObject;
    public GameObject shopRoom = Resources.Load("Room/ShopRoom1") as GameObject;
    public GameObject normalRoom = Resources.Load("Room/NormalRoom1_0") as GameObject;
    private GameObject tempRoom;
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
    public CCreateStage(int stageNumber)
    {
        _stageNumber = stageNumber;
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
                System.Random r = new System.Random();
                j = r.Next() % max;

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
                System.Random r = new System.Random();
                j = r.Next() % max;

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
                System.Random r = new System.Random();
                j = r.Next() % max;

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
                System.Random r = new System.Random();
                j = r.Next() % max;

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

        int randomRoad = 0; //랜덤한 갈림길 개수
        int selectRoomType = 0; //랜덤으로 방 종류 뽑기
        int roadCount;  //갈림길 숫자

        System.Random rand = new System.Random();

        if (_roomCount == 0)
        {
            _roomArr[0, 0].RoomType = CGlobal.ERoomType._start;
            CreateRoom(_roomArr, _roomCount); //시작방 생성
            _roomCount++;
        }

        if (_roomCount == CConstants.ROOM_PER_STAGE - 1)
        {
            _roomArr[CConstants.ROOM_PER_STAGE - 1, 0].RoomType = CGlobal.ERoomType._boss;
            CreateRoom(_roomArr, _roomCount); //보스방 생성
            _roomCount++;
            return;
        }


        randomRoad = rand.Next(CConstants.MAX_ROAD - 1);
        _noRoomFlag = true;
        roadCount = 0;
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

            selectRoomType = rand.Next(100);
            if (selectRoomType < CConstants.SKILL_PROBABILITY - PreventOverlap(CGlobal.ERoomType._skillElite)
                && _skillEliteCount < 2 && _roomCount > (((CConstants.ROOM_PER_STAGE - 2) / 2) - 1))                                                                                                //스킬 엘리트
            {
                _roomArr[_roomCount, roadCount].RoomType = CGlobal.ERoomType._skillElite;
                _skillEliteCount++;
                RoomFlagCtrl(CGlobal.ERoomType._skillElite);
            }

            else if (selectRoomType >= CConstants.SKILL_PROBABILITY
                && selectRoomType < CConstants.SKILL_PROBABILITY + CConstants.SHOP__PROBABILITY - PreventOverlap(CGlobal.ERoomType._shop))                // 상점
            {
                _roomArr[_roomCount, roadCount].RoomType = CGlobal.ERoomType._shop;
                _shopCount++;
                RoomFlagCtrl(CGlobal.ERoomType._shop);
            }

            else if (selectRoomType >= CConstants.SKILL_PROBABILITY + CConstants.SHOP__PROBABILITY
                && selectRoomType < CConstants.SKILL_PROBABILITY + CConstants.SHOP__PROBABILITY + CConstants.NORMAL_PROBABILITY - PreventOverlap(CGlobal.ERoomType._normal))//일반 방
            {
                _roomArr[_roomCount, roadCount].RoomType = CGlobal.ERoomType._normal;
                RoomFlagCtrl(CGlobal.ERoomType._normal);
            }

            else if (selectRoomType >= CConstants.SKILL_PROBABILITY + CConstants.SHOP__PROBABILITY + CConstants.NORMAL_PROBABILITY
                && selectRoomType < CConstants.SKILL_PROBABILITY + CConstants.SHOP__PROBABILITY + CConstants.NORMAL_PROBABILITY + CConstants.EVENT_PROBABLILITY - PreventOverlap(CGlobal.ERoomType._event)
                && _eventCount < 15)                                                                                                    //이벤트
            {
                _roomArr[_roomCount, roadCount].RoomType = CGlobal.ERoomType._event;
                _eventCount++;
                RoomFlagCtrl(CGlobal.ERoomType._event);
            }

            else if (selectRoomType >= CConstants.SKILL_PROBABILITY + CConstants.SHOP__PROBABILITY + CConstants.NORMAL_PROBABILITY + CConstants.EVENT_PROBABLILITY
                && selectRoomType < CConstants.SKILL_PROBABILITY + CConstants.SHOP__PROBABILITY + CConstants.NORMAL_PROBABILITY + CConstants.EVENT_PROBABLILITY + CConstants.ITEM_PROBABILITY - PreventOverlap(CGlobal.ERoomType._itemElite)) //아이템 엘리트
            {
                _roomArr[_roomCount, roadCount].RoomType = CGlobal.ERoomType._itemElite;
                RoomFlagCtrl(CGlobal.ERoomType._itemElite);
            }
        }
        //RoomFlagCtrl(ERoomType roomType)l; 주인공 캐릭터가 선택한 룸타입을 설정
    }

    private void RoomFlagCtrl(CGlobal.ERoomType roomType)
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

    private int PreventOverlap(CGlobal.ERoomType roomType)
    {
        if (roomType == _roomTypeFlag)
        {
            switch (roomType)
            {
                case CGlobal.ERoomType._event:
                    return CConstants.EVENT_PROBABLILITY / 2;

                case CGlobal.ERoomType._itemElite:
                    return CConstants.ITEM_PROBABILITY / 2;

                case CGlobal.ERoomType._normal:
                    return CConstants.NORMAL_PROBABILITY / 2;

                case CGlobal.ERoomType._shop:
                    return CConstants.SHOP__PROBABILITY / 2;

                case CGlobal.ERoomType._skillElite:
                    return CConstants.SKILL_PROBABILITY / 2;
            }
        }

        return 0;
    }
    public void CreateRoom(CRoom[,] roomArr, int roomCount)
    {
        for (int roadCount = 0; roadCount < CConstants.MAX_ROAD; roadCount++)
        {
            if (roomArr[roomCount, roadCount].RoomType != CGlobal.ERoomType._empty) //빈 방이 아닐 경우
            {
                InstantiateRoom(roomArr[roomCount, roadCount].RoomType, roomCount, roadCount);
            }
        }
        _roomCount++;
    }

    private void InstantiateRoom(CGlobal.ERoomType roomType, int roomCount, int roadCount)
    {
        switch (roomType)
        {
            case CGlobal.ERoomType._start:
                if (roadCount == 1 || roadCount == 2) //시작방은 하나만!
                    return;

                tempRoom = Object.Instantiate(startRoom, new Vector3(0, 0, 0), Quaternion.Euler(new Vector3(90, 0, 0)));
                _rooms.AddLast(tempRoom);
                break;

            case CGlobal.ERoomType._boss:
                if (roadCount == 1 || roadCount == 2) //보스방도 하나만!
                    return;

                tempRoom = Object.Instantiate(bossRoom, new Vector3((roadCount - 1) * CConstants.ROOM_DISTANCE_X, 0, roomCount * CConstants.ROOM_DISTANCE_Z), Quaternion.Euler(new Vector3(90, 0, 0)));
                _rooms.AddLast(tempRoom);
                break;

            case CGlobal.ERoomType._event:
                tempRoom = Object.Instantiate(eventRoom, new Vector3((roadCount - 1) * CConstants.ROOM_DISTANCE_X, 0, roomCount * CConstants.ROOM_DISTANCE_Z), Quaternion.Euler(new Vector3(90, 0, 0)));
                _rooms.AddLast(tempRoom);
                break;

            case CGlobal.ERoomType._itemElite:
                tempRoom = Object.Instantiate(itemEliteRoom, new Vector3((roadCount - 1) * CConstants.ROOM_DISTANCE_X, 0, roomCount * CConstants.ROOM_DISTANCE_Z), Quaternion.Euler(new Vector3(90, 0, 0)));
                _rooms.AddLast(tempRoom);
                break;

            case CGlobal.ERoomType._normal:
                tempRoom = Object.Instantiate(normalRoomQueue.Dequeue(), new Vector3((roadCount - 1) * CConstants.ROOM_DISTANCE_X, 0, roomCount * CConstants.ROOM_DISTANCE_Z), Quaternion.Euler(new Vector3(90, 0, 0)));
                _rooms.AddLast(tempRoom);
                break;

            case CGlobal.ERoomType._shop:
                tempRoom = Object.Instantiate(shopRoom, new Vector3((roadCount - 1) * CConstants.ROOM_DISTANCE_X, 0, roomCount * CConstants.ROOM_DISTANCE_Z), Quaternion.Euler(new Vector3(90, 0, 0)));
                _rooms.AddLast(tempRoom);
                break;

            case CGlobal.ERoomType._skillElite:
                tempRoom = Object.Instantiate(skillEliteRoom, new Vector3((roadCount - 1) * CConstants.ROOM_DISTANCE_X, 0, roomCount * CConstants.ROOM_DISTANCE_Z), Quaternion.Euler(new Vector3(90, 0, 0)));
                _rooms.AddLast(tempRoom);
                break;
        }
    }

    private void DestroyRoom()
    {
        int tempRoomCount = _roomCount - 2;
        int existRoomCount = 0;
        int secondExistRoomCount = 0; //포탈 삭제를 위한 지금 삭제해야할 다음 방 개수

        for (int i = 0; i < CConstants.MAX_ROAD; i++)
        {
            if (_roomArr[tempRoomCount, i].RoomType != CGlobal.ERoomType._empty)
                existRoomCount++;
            if (_roomArr[tempRoomCount + 1, i].RoomType != CGlobal.ERoomType._empty)
                secondExistRoomCount++;
        }

        if (tempRoomCount == 0) //시작 방일 경우, 보스방도 추후에 고려해야함.
        {
            _tempRoomNode = _rooms.First;
            Object.Destroy(_tempRoomNode.Value);
            _rooms.RemoveFirst();
            return;
        }

        for (int i = 0; i < existRoomCount; i++)
        {
            _tempRoomNode = _rooms.First;
            Object.Destroy(_tempRoomNode.Value);
            _rooms.RemoveFirst();
        }
    }

    public int getRoomCount()
    {
        return _roomCount;
    }

    public void CtrlPortal() //포탈 오브젝트 클리어 조건 만족시 true 아닐 경우 false
    {
        GameObject[] leftPortal = GameObject.FindGameObjectsWithTag("LEFT_PORTAL");
        GameObject[] rightPortal = GameObject.FindGameObjectsWithTag("RIGHT_PORTAL");
        GameObject[] Portal = GameObject.FindGameObjectsWithTag("PORTAL");

        if (CGlobal.isClear == false) //클리어 아직 안됬을 경우
        {
            if (CGlobal.isPortalActive == true)
            {
                foreach (GameObject ob in leftPortal)
                    ob.transform.Find("Portal").gameObject.SetActive(false);
                foreach (GameObject ob in rightPortal)
                    ob.transform.Find("Portal").gameObject.SetActive(false);
                foreach (GameObject ob in Portal)
                    ob.transform.Find("Portal").gameObject.SetActive(false);

                CGlobal.isPortalActive = false;
            }
            return;
        }

        //클리어 됬을 경우
        foreach (GameObject ob in leftPortal)
            ob.transform.FindChild("Portal").gameObject.SetActive(true);
        foreach (GameObject ob in rightPortal)
            ob.transform.FindChild("Portal").gameObject.SetActive(true);
        foreach (GameObject ob in Portal)
            ob.transform.FindChild("Portal").gameObject.SetActive(true);
        CGlobal.isPortalActive = true;

    }
}
