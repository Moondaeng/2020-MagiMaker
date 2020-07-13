using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Console;

// 해야할 것 정리 : // 4. 멀티기준 플레이어 모두 올라와야 넘어갈수 있게하기(이건 구조만 유도)
// 8. 다음스테이지 생성시 보스방 제거 알고리즘 생각해야함
public class CCreateMap : MonoBehaviour
{
    CCreateStage map;
    private int _createStageNumber;
    private void Start()
    {
        _createStageNumber = 1;
        map = new CCreateStage(_createStageNumber);
    }

    private void Update()
    {
        map.CreateStage();
        if (map.getRoomCount() >= CConstants.ROOM_PER_STAGE && CGlobal.usePortal == true) //다음스테이지 준비
        {
            map = new CCreateStage(_createStageNumber);
            _createStageNumber++;
        }
        map.CtrlPortal(); //스테이지 클리어 하는 즉시 포탈 true로 바꿔줌
    }

    class CCreateStage
    {
        #region room&portal
        public GameObject portal = Resources.Load("Portal/PortalMom") as GameObject;
        public GameObject leftPortal = Resources.Load("Portal/LeftPortalMom") as GameObject;
        public GameObject rightPortal = Resources.Load("Portal/RightPortalMom") as GameObject;
        public GameObject startRoom = Resources.Load("Room/StartRoom") as GameObject;
        public GameObject bossRoom = Resources.Load("Room/BossRoom") as GameObject;
        public GameObject itemEliteRoom = Resources.Load("Room/ItemEliteRoom") as GameObject;
        public GameObject skillEliteRoom = Resources.Load("Room/SkillEliteRoom") as GameObject;
        public GameObject eventRoom = Resources.Load("Room/EventRoom") as GameObject;
        public GameObject shopRoom = Resources.Load("Room/ShopRoom") as GameObject;
        public GameObject normalRoom = Resources.Load("Room/NormalRoom") as GameObject;
        private GameObject tempRoom;
        #endregion

        #region memberVar
        private int _stageNumber;
        private int _skillEliteCount;
        private int _eventCount;
        private int _shopCount;
        private bool _noRoomFlag;  //트루일 경우 방이 없음
        private ERoomType _roomTypeFlag;
        private int _roomCount;
        private CRoom[,] _roomArr;
        private LinkedList<GameObject> _rooms;
        private LinkedListNode<GameObject> _tempRoomNode;
        private LinkedList<GameObject> _portals;
        #endregion
        public CCreateStage(int stageNumber)
        {
            _stageNumber = stageNumber;
            _skillEliteCount = 0;
            _eventCount = 0;
            _shopCount = 0;
            _noRoomFlag = true;
            _roomTypeFlag = ERoomType._empty;
            _roomCount = 0; //방의 개수가 12개가 넘어가면 멈춰줄 변수
            CGlobal.usePortal = true; //시작방과 동시에 다른 방도 만들어질 수 있게 하기 위함

            _roomArr = new CRoom[CConstants.ROOM_PER_STAGE, CConstants.MAX_ROAD];
            for (int i = 0; i < CConstants.ROOM_PER_STAGE; i++)
                for (int j = 0; j < CConstants.MAX_ROAD; j++)
                    _roomArr[i, j] = new CRoom();

            _rooms = new LinkedList<GameObject>();
            _portals = new LinkedList<GameObject>();
        }

        public void CreateStage()
        {
            if (CGlobal.usePortal)  //포탈을 사용하지 않은 경우 포탈 사용할때 까지 대기
            {
                CGlobal.usePortal = false;

                if (_roomCount > 0) //시작하자마자 방 제거 방지
                    DestroyRoom();
            }
            else
                return;

            if (_roomCount >= CConstants.ROOM_PER_STAGE) //방 12개 전부 생성 시 대기
                return;

            int randomRoad = 0; //랜덤한 갈림길 개수
            int selectRoomType = 0; //랜덤으로 방 종류 뽑기
            int roadCount;  //갈림길 숫자

            System.Random rand = new System.Random();

            if (_roomCount == 0)
            {
                _roomArr[0, 0].RoomType = ERoomType._start;
                CreateRoom(_roomArr, _roomCount); //시작방 생성
                _roomCount++;
            }

            if (_roomCount == CConstants.ROOM_PER_STAGE - 1)
            {
                _roomArr[CConstants.ROOM_PER_STAGE - 1, 0].RoomType = ERoomType._boss;
                CreateRoom(_roomArr, _roomCount); //보스방 생성
                _roomCount++;
                return;
            }

            Debug.Log("in CreateStage function!"); //debug


            randomRoad = rand.Next(CConstants.MAX_ROAD - 1);
            _noRoomFlag = true;
            roadCount = 0;
            for (int j = CConstants.MAX_ROAD; j > randomRoad; randomRoad++, roadCount++)
            {
                //최소 조건
                if (_shopCount < CConstants.MIN_SHOP_PER_STAGE && _roomCount == 10 && roadCount == 0) //상점이 한번도 안나왔고 마지막 방일 때 첫번째 갈림길에는 무조건 상점방
                {
                    //Console.WriteLine("in first if shop"); //debug
                    _roomArr[_roomCount, roadCount].RoomType = ERoomType._shop;
                    _shopCount++;
                    RoomFlagCtrl(ERoomType._shop);
                    continue;
                }
                if (_noRoomFlag == true && j - randomRoad == 1) //조건들에 걸려 방이 아예 안나오는 경우 방지
                {
                    //Console.WriteLine("in second if normal"); //debug
                    _roomArr[_roomCount, roadCount].RoomType = ERoomType._normal;
                    RoomFlagCtrl(ERoomType._normal);
                    continue;
                }

                selectRoomType = rand.Next(100);
                //Console.Write(" selectRoomType " + selectRoomType); //debug
                if (selectRoomType < CConstants.SKILL_PROBABILITY - PreventOverlap(ERoomType._skillElite)
                    && _skillEliteCount < 2 && _roomCount > (((CConstants.ROOM_PER_STAGE - 2) / 2) - 1))                                                                                                //스킬 엘리트
                {
                    //Console.WriteLine("in 3 if skillelite"); //debug
                    _roomArr[_roomCount, roadCount].RoomType = ERoomType._skillElite;
                    _skillEliteCount++;
                    RoomFlagCtrl(ERoomType._skillElite);
                }

                else if (selectRoomType >= CConstants.SKILL_PROBABILITY
                    && selectRoomType < CConstants.SKILL_PROBABILITY + CConstants.SHOP__PROBABILITY - PreventOverlap(ERoomType._shop))                // 상점
                {
                    //Console.WriteLine("in 4 if shop"); //debug
                    _roomArr[_roomCount, roadCount].RoomType = ERoomType._shop;
                    _shopCount++;
                    RoomFlagCtrl(ERoomType._shop);
                }

                else if (selectRoomType >= CConstants.SKILL_PROBABILITY + CConstants.SHOP__PROBABILITY
                    && selectRoomType < CConstants.SKILL_PROBABILITY + CConstants.SHOP__PROBABILITY + CConstants.NORMAL_PROBABILITY - PreventOverlap(ERoomType._normal))//일반 방
                {
                    //Console.WriteLine("in 5 if normal"); //debug
                    _roomArr[_roomCount, roadCount].RoomType = ERoomType._normal;
                    RoomFlagCtrl(ERoomType._normal);
                }

                else if (selectRoomType >= CConstants.SKILL_PROBABILITY + CConstants.SHOP__PROBABILITY + CConstants.NORMAL_PROBABILITY
                    && selectRoomType < CConstants.SKILL_PROBABILITY + CConstants.SHOP__PROBABILITY + CConstants.NORMAL_PROBABILITY + CConstants.EVENT_PROBABLILITY - PreventOverlap(ERoomType._event)
                    && _eventCount < 15)                                                                                                    //이벤트
                {
                    //Console.WriteLine("in 6 if event"); //debug
                    _roomArr[_roomCount, roadCount].RoomType = ERoomType._event;
                    _eventCount++;
                    RoomFlagCtrl(ERoomType._event);
                }

                else if (selectRoomType >= CConstants.SKILL_PROBABILITY + CConstants.SHOP__PROBABILITY + CConstants.NORMAL_PROBABILITY + CConstants.EVENT_PROBABLILITY
                    && selectRoomType < CConstants.SKILL_PROBABILITY + CConstants.SHOP__PROBABILITY + CConstants.NORMAL_PROBABILITY + CConstants.EVENT_PROBABLILITY + CConstants.ITEM_PROBABILITY - PreventOverlap(ERoomType._itemElite)) //아이템 엘리트
                {
                    //Console.WriteLine("in 7 if itemlite"); //debug
                    _roomArr[_roomCount, roadCount].RoomType = ERoomType._itemElite;
                    RoomFlagCtrl(ERoomType._itemElite);
                }
            }
            CreateRoom(_roomArr, _roomCount);
            CreatePortal();
            _roomCount++;

            //debug
            for (int i = 0; i < CConstants.ROOM_PER_STAGE; i++)
            {
                for (int j = 0; j < CConstants.MAX_ROAD; j++)
                    Debug.Log(_roomArr[i, j].RoomType);
                Debug.Log("\n");
            }
            //RoomFlagCtrl(ERoomType roomType)l; 주인공 캐릭터가 선택한 룸타입을 설정
        }

        private void RoomFlagCtrl(ERoomType roomType)
        {
            _noRoomFlag = false;
            switch (roomType)
            {
                case ERoomType._event:
                    _roomTypeFlag = ERoomType._event;
                    break;
                case ERoomType._itemElite:
                    _roomTypeFlag = ERoomType._itemElite;
                    break;
                case ERoomType._normal:
                    _roomTypeFlag = ERoomType._normal;
                    break;
                case ERoomType._shop:
                    _roomTypeFlag = ERoomType._shop;
                    break;
                case ERoomType._skillElite:
                    _roomTypeFlag = ERoomType._skillElite;
                    break;
            }
        }

        private int PreventOverlap(ERoomType roomType)
        {
            if (roomType == _roomTypeFlag)
            {
                switch (roomType)
                {
                    case ERoomType._event:
                        return CConstants.EVENT_PROBABLILITY / 2;

                    case ERoomType._itemElite:
                        return CConstants.ITEM_PROBABILITY / 2;

                    case ERoomType._normal:
                        return CConstants.NORMAL_PROBABILITY / 2;

                    case ERoomType._shop:
                        return CConstants.SHOP__PROBABILITY / 2;

                    case ERoomType._skillElite:
                        return CConstants.SKILL_PROBABILITY / 2;
                }
            }

            return 0;
        }
        private void CreateRoom(CRoom[,] roomArr, int roomCount)
        {
            for (int roadCount = 0; roadCount < CConstants.MAX_ROAD; roadCount++)
            {
                if (roomArr[roomCount, roadCount].RoomType != ERoomType._empty) //빈 방이 아닐 경우
                {
                    InstantiateRoom(roomArr[roomCount, roadCount].RoomType, roomCount, roadCount);
                }
            }
        }

        private void InstantiateRoom(ERoomType roomType, int roomCount, int roadCount)
        {
            switch (roomType)
            {
                case ERoomType._start:
                    if (roadCount == 1 || roadCount == 2) //시작방은 하나만!
                        return;

                    tempRoom = Instantiate(startRoom, new Vector3((roadCount - 1) * CConstants.ROOM_DISTANCE_X, 0, 0), Quaternion.Euler(new Vector3(90, 0, 0)));
                    _rooms.AddLast(tempRoom);
                    break;

                case ERoomType._boss:
                    if (roadCount == 1 || roadCount == 2) //보스방도 하나만!
                        return;

                    tempRoom = Instantiate(bossRoom, new Vector3((roadCount - 1) * CConstants.ROOM_DISTANCE_X, 0, roomCount * CConstants.ROOM_DISTANCE_Z), Quaternion.Euler(new Vector3(90, 0, 0)));
                    _rooms.AddLast(tempRoom);
                    break;

                case ERoomType._event:
                    tempRoom = Instantiate(eventRoom, new Vector3((roadCount - 1) * CConstants.ROOM_DISTANCE_X, 0, roomCount * CConstants.ROOM_DISTANCE_Z), Quaternion.Euler(new Vector3(90, 0, 0)));
                    _rooms.AddLast(tempRoom);
                    break;

                case ERoomType._itemElite:
                    tempRoom = Instantiate(itemEliteRoom, new Vector3((roadCount - 1) * CConstants.ROOM_DISTANCE_X, 0, roomCount * CConstants.ROOM_DISTANCE_Z), Quaternion.Euler(new Vector3(90, 0, 0)));
                    _rooms.AddLast(tempRoom);
                    break;

                case ERoomType._normal:
                    tempRoom = Instantiate(normalRoom, new Vector3((roadCount - 1) * CConstants.ROOM_DISTANCE_X, 0, roomCount * CConstants.ROOM_DISTANCE_Z), Quaternion.Euler(new Vector3(90, 0, 0)));
                    _rooms.AddLast(tempRoom);
                    break;

                case ERoomType._shop:
                    tempRoom = Instantiate(shopRoom, new Vector3((roadCount - 1) * CConstants.ROOM_DISTANCE_X, 0, roomCount * CConstants.ROOM_DISTANCE_Z), Quaternion.Euler(new Vector3(90, 0, 0)));
                    _rooms.AddLast(tempRoom);
                    break;

                case ERoomType._skillElite:
                    tempRoom = Instantiate(skillEliteRoom, new Vector3((roadCount - 1) * CConstants.ROOM_DISTANCE_X, 0, roomCount * CConstants.ROOM_DISTANCE_Z), Quaternion.Euler(new Vector3(90, 0, 0)));
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
                if (_roomArr[tempRoomCount, i].RoomType != ERoomType._empty)
                    existRoomCount++;
                if (_roomArr[tempRoomCount + 1, i].RoomType != ERoomType._empty)
                    secondExistRoomCount++;
            }

            if (tempRoomCount == 0) //시작 방일 경우, 보스방도 추후에 고려해야함.
            {
                _tempRoomNode = _rooms.First;
                Destroy(_tempRoomNode.Value);
                _rooms.RemoveFirst();

                for (int j = 0; j < secondExistRoomCount; j++)
                {
                    _tempRoomNode = _portals.First;
                    Destroy(_tempRoomNode.Value);
                    _portals.RemoveFirst();
                }
                return;
            }

            for (int i = 0; i < existRoomCount; i++)
            {
                _tempRoomNode = _rooms.First;
                Destroy(_tempRoomNode.Value);
                _rooms.RemoveFirst();

                for(int j = 0; j < secondExistRoomCount; j++)
                {
                    _tempRoomNode = _portals.First;
                    Destroy(_tempRoomNode.Value);
                    _portals.RemoveFirst();
                }
            }
        }

        public int getRoomCount()
        {
            return _roomCount;
        }

        public void CreatePortal()
        {
            int roomCount = _roomCount - 1;
            for(int i = 0; i < CConstants.MAX_ROAD; i++)
            {
                if(_roomArr[_roomCount, i].RoomType != ERoomType._empty) //앞의 방이 empty가 아니면 뒷방에다가 포탈 생성!
                {
                    for(int j = 0; j < CConstants.MAX_ROAD; j++)
                    {
                        if(_roomArr[roomCount, j].RoomType != ERoomType._empty) //뒷 방 empty 아닌 경우에만 포탈 생성
                        {
                            // j는 0~2번째 방의위치, i는 좌중우 포탈위치
                            switch (i-1)
                            {
                                case -1:
                                    _portals.AddLast(Instantiate(leftPortal, new Vector3((j - 1) * CConstants.ROOM_DISTANCE_X + (i - 1) * CConstants.PORTAL_POSITION_X, 0
                                , (roomCount * CConstants.ROOM_DISTANCE_Z) + CConstants.PORTAL_POSITION_Z), Quaternion.Euler(new Vector3())));
                                    break;
                                case 0:
                                    _portals.AddLast(Instantiate(portal, new Vector3((j - 1) * CConstants.ROOM_DISTANCE_X + (i - 1) * CConstants.PORTAL_POSITION_X, 0
                                , (roomCount * CConstants.ROOM_DISTANCE_Z) + CConstants.PORTAL_POSITION_Z), Quaternion.Euler(new Vector3())));
                                    break;
                                case 1:
                                    _portals.AddLast(Instantiate(rightPortal, new Vector3((j - 1) * CConstants.ROOM_DISTANCE_X + (i - 1) * CConstants.PORTAL_POSITION_X, 0
                                , (roomCount * CConstants.ROOM_DISTANCE_Z) + CConstants.PORTAL_POSITION_Z), Quaternion.Euler(new Vector3())));
                                    break;
                            }
                        }
                    }
                }
            }
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
                ob.transform.Find("Portal").gameObject.SetActive(true);
            foreach (GameObject ob in rightPortal)
                ob.transform.Find("Portal").gameObject.SetActive(true);
            foreach (GameObject ob in Portal)
                ob.transform.Find("Portal").gameObject.SetActive(true);
            CGlobal.isPortalActive = true;

        }
    }
}

    public enum ERoomType //첫번째 씬에 ERoomType코드가 있음 추후에 수정
    {
        _start,
        _normal,
        _event,
        _skillElite,
        _itemElite,
        _shop,
        _boss,
        _empty
    }
    class CRoom
    {
        public CRoom()
        {
            _roomType = ERoomType._empty;
        }

        private ERoomType _roomType;
        public ERoomType RoomType
        {
            get { return _roomType; }
            set { _roomType = value; }
        }

        public override string ToString()
        {
            return _roomType.ToString();
        }
    }