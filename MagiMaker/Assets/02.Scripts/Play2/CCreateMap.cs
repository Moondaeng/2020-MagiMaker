using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// 해야할 것 정리 : 1.방이 실제로 씬에서 생기게 하기, 2. 입력 받을때마다 뒷 방 생기는거 포탈 이동시로 변경하기 3. 일반 방인 경우 포탈 준비, 일반 방 아닌 경우 방 클리어 될때까지 포탈 삭제
// 4. 멀티기준 플레이어 모두 올라와야 넘어갈수 있게하기(이건 구조만 유도)
public class CCreateMap : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        CCreateStage map = new CCreateStage(1);
        map.CreateStage();
    }

    // Update is called once per frame
    void Update()
    {

    }

    class CConstant
    {
        #region MaxMinStage
        public static int ROOM_PER_STAGE = 12;              //시작 + 랜덤 + 보스
        public static int MAX_ROAD = 3;                     //갈림길 최대
        public static int MIN_ROAD = 1;                     //갈림길 최소
        public static int MAX_SKILL_ELITE_PER_STAGE = 2;    //스테이지당 최대 스킬 엘리트
        public static int MAX_EVENT_PER_STAGE = 15;         //스테이지당 최대 이벤트 방
        public static int MIN_SHOP_PER_STAGE = 1;           //스티이지당 최소 상점
        #endregion

        #region StageProbability
        public static int NORMAL_PROBABILITY = 30;  //일반 방 출현 확률 30퍼
        public static int EVENT_PROBABLILITY = 30;  //이벤트 방
        public static int SKILL_PROBABILITY = 15;   //스킬 엘리트
        public static int ITEM_PROBABILITY = 15;    //아이템 엘리트
        public static int SHOP__PROBABILITY = 10;   //상점
        #endregion
    }

    public enum ERoomType
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

    class CCreateStage
    {
        private int _stageNumber;
        private int _skillEliteCount;
        private int _eventCount;
        private int _shopCount;
        private bool _noRoomFlag;  //트루일 경우 방이 없음
        private ERoomType _roomTypeFlag;
        //private CRoom[,] roomArr;

        //public CCreateMap()
        //{
        //    roomArr = new CRoom[CConstant.ROOM_PER_STAGE, CConstant.MAX_ROAD];
        //    for (int i = 0; i < CConstant.ROOM_PER_STAGE; i++)
        //        for (int j = 0; j < CConstant.MAX_ROAD; j++)
        //            roomArr[i, j] = new CRoom();
        //}


        public CCreateStage(int stageNumber)
        {
            _stageNumber = stageNumber;
            _skillEliteCount = 0;
            _eventCount = 0;
            _shopCount = 0;
            _noRoomFlag = true;
            _roomTypeFlag = ERoomType._empty;
        }

        public void CreateStage()
        {
            int roomCount = 0; //방의 개수가 12개가 넘어가면 멈춰줄 변수
            int randomRoad = 0; //랜덤한 갈림길 개수
            int selectRoomType = 0; //랜덤으로 방 종류 뽑기
            int roadCount;  //갈림길 숫자
            System.Random rand = new System.Random();
            CRoom[,] roomArr = new CRoom[CConstant.ROOM_PER_STAGE, CConstant.MAX_ROAD];
            for (int i = 0; i < CConstant.ROOM_PER_STAGE; i++)
                for (int j = 0; j < CConstant.MAX_ROAD; j++)
                    roomArr[i, j] = new CRoom();


            roomArr[0, 0].RoomType = ERoomType._start;
            roomArr[11, 0].RoomType = ERoomType._boss;
            CreateRoom(roomArr, roomCount); //시작방 생성
            PrintStage(roomArr, roomCount);
            roomCount++;

            while (roomCount <= CConstant.ROOM_PER_STAGE - 2) //보스방 시작방 제외한 10개방 생성
            {
                randomRoad = rand.Next(CConstant.MAX_ROAD - 1);
                _noRoomFlag = true;
                roadCount = 0;
                for (int j = CConstant.MAX_ROAD; j > randomRoad; randomRoad++, roadCount++)
                {
                    //최소 조건
                    if (_shopCount < CConstant.MIN_SHOP_PER_STAGE && roomCount == 10 && roadCount == 0) //상점이 한번도 안나왔고 마지막 방일 때 첫번째 갈림길에는 무조건 상점방
                    {
                        //Console.WriteLine("in first if shop"); //debug
                        roomArr[roomCount, roadCount].RoomType = ERoomType._shop;
                        _shopCount++;
                        RoomFlagCtrl(ERoomType._shop);
                        continue;
                    }
                    if (_noRoomFlag == true && j - randomRoad == 1) //조건들에 걸려 방이 아예 안나오는 경우 방지
                    {
                        //Console.WriteLine("in second if normal"); //debug
                        roomArr[roomCount, roadCount].RoomType = ERoomType._normal;
                        RoomFlagCtrl(ERoomType._normal);
                        continue;
                    }

                    selectRoomType = rand.Next(100);
                    //Console.Write(" selectRoomType " + selectRoomType); //debug
                    if (selectRoomType < CConstant.SKILL_PROBABILITY - PreventOverlap(ERoomType._skillElite)
                        && _skillEliteCount < 2 && roomCount > (((CConstant.ROOM_PER_STAGE - 2) / 2) - 1))                                                                                                //스킬 엘리트
                    {
                        //Console.WriteLine("in 3 if skillelite"); //debug
                        roomArr[roomCount, roadCount].RoomType = ERoomType._skillElite;
                        _skillEliteCount++;
                        RoomFlagCtrl(ERoomType._skillElite);
                    }

                    else if (selectRoomType >= CConstant.SKILL_PROBABILITY
                        && selectRoomType < CConstant.SKILL_PROBABILITY + CConstant.SHOP__PROBABILITY - PreventOverlap(ERoomType._shop))                // 상점
                    {
                        //Console.WriteLine("in 4 if shop"); //debug
                        roomArr[roomCount, roadCount].RoomType = ERoomType._shop;
                        _shopCount++;
                        RoomFlagCtrl(ERoomType._shop);
                    }

                    else if (selectRoomType >= CConstant.SKILL_PROBABILITY + CConstant.SHOP__PROBABILITY
                        && selectRoomType < CConstant.SKILL_PROBABILITY + CConstant.SHOP__PROBABILITY + CConstant.NORMAL_PROBABILITY - PreventOverlap(ERoomType._normal))//일반 방
                    {
                        //Console.WriteLine("in 5 if normal"); //debug
                        roomArr[roomCount, roadCount].RoomType = ERoomType._normal;
                        RoomFlagCtrl(ERoomType._normal);
                    }

                    else if (selectRoomType >= CConstant.SKILL_PROBABILITY + CConstant.SHOP__PROBABILITY + CConstant.NORMAL_PROBABILITY
                        && selectRoomType < CConstant.SKILL_PROBABILITY + CConstant.SHOP__PROBABILITY + CConstant.NORMAL_PROBABILITY + CConstant.EVENT_PROBABLILITY - PreventOverlap(ERoomType._event)
                        && _eventCount < 15)                                                                                                    //이벤트
                    {
                        //Console.WriteLine("in 6 if event"); //debug
                        roomArr[roomCount, roadCount].RoomType = ERoomType._event;
                        _eventCount++;
                        RoomFlagCtrl(ERoomType._event);
                    }

                    else if (selectRoomType >= CConstant.SKILL_PROBABILITY + CConstant.SHOP__PROBABILITY + CConstant.NORMAL_PROBABILITY + CConstant.EVENT_PROBABLILITY
                        && selectRoomType < CConstant.SKILL_PROBABILITY + CConstant.SHOP__PROBABILITY + CConstant.NORMAL_PROBABILITY + CConstant.EVENT_PROBABLILITY + CConstant.ITEM_PROBABILITY - PreventOverlap(ERoomType._itemElite)) //아이템 엘리트
                    {
                        //Console.WriteLine("in 7 if itemlite"); //debug
                        roomArr[roomCount, roadCount].RoomType = ERoomType._itemElite;
                        RoomFlagCtrl(ERoomType._itemElite);
                    }
                }
                CreateRoom(roomArr, roomCount);
                Console.ReadLine(); //유니티에선 주인공 캐릭터 포탈 이동 시
                //RoomFlagCtrl(ERoomType roomType)l; 주인공 캐릭터가 선택한 룸타입을 설정
                PrintStage(roomArr, roomCount);
                roomCount++;
            }
            //Console.WriteLine("befor create boss" + roomCount); //debug
            CreateRoom(roomArr, roomCount); //보스방 생성
            PrintStage(roomArr, roomCount);
            ProbabilityMeasurement(roomArr);
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
                        return CConstant.EVENT_PROBABLILITY / 2;

                    case ERoomType._itemElite:
                        return CConstant.ITEM_PROBABILITY / 2;

                    case ERoomType._normal:
                        return CConstant.NORMAL_PROBABILITY / 2;

                    case ERoomType._shop:
                        return CConstant.SHOP__PROBABILITY / 2;

                    case ERoomType._skillElite:
                        return CConstant.SKILL_PROBABILITY / 2;
                }
            }

            return 0;
        }
        private void CreateRoom(CRoom[,] roomArr, int roomCount)
        {
            int roadCount = 0;
            while (roadCount < CConstant.MAX_ROAD && roomArr[roomCount, roadCount].RoomType != ERoomType._empty) //갈림길 최대 개수보다 많거나 갈림길이 더 없을 경우 ㅌㅌ
            {
                roadCount++;
                break;
            }
        }

        //확률 측정용 실제론 주석처리 또는 삭제
        private void ProbabilityMeasurement(CRoom[,] roomArr)
        {
            double normal = 0;
            double skill = 0;
            double _event = 0;
            double item = 0;
            double shop = 0;
            double sum = 0;

            for (int i = 0; i < CConstant.MAX_ROAD; i++)
            {
                for (int j = 0; j < CConstant.ROOM_PER_STAGE; j++)
                {
                    switch (roomArr[j, i].RoomType)
                    {
                        case ERoomType._event:
                            _event++;
                            break;
                        case ERoomType._itemElite:
                            item++;
                            break;
                        case ERoomType._normal:
                            normal++;
                            break;
                        case ERoomType._shop:
                            shop++;
                            break;
                        case ERoomType._skillElite:
                            skill++;
                            break;
                    }
                }
            }

            sum = _event + normal + skill + item + shop;

            Console.WriteLine("normal = " + normal + "normal = " + normal / sum * 100);
            Console.WriteLine("_event = " + _event + "_event = " + _event / sum * 100);
            Console.WriteLine("skill = " + skill + "skill = " + skill / sum * 100);
            Console.WriteLine("item = " + item + "item = " + item / sum * 100);
            Console.WriteLine("shop = " + shop + "shop = " + shop / sum * 100);
        }

        private void DestroyRoom()
        {

        }

        private void PrintStage(CRoom[,] roomArr, int roomCount)
        {
            int roadCount = 0;

            while (roadCount < CConstant.MAX_ROAD) //갈림길 최대 개수보다 많거나 갈림길이 더 없을 경우 ㅌㅌ
            {
                //Console.WriteLine("roomCount " + roomCount + " ); //debug
                if (roomArr[roomCount, roadCount].RoomType != ERoomType._empty)
                    Console.Write(roomCount + "th room" + roomArr[roomCount, roadCount].RoomType + " ");
                roadCount++;
            }
            Console.WriteLine();
        }
    }
}
