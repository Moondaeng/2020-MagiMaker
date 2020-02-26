using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Console;

public class CStage
{
    private int _stageNum;                                                                  // 현재 스테이지의 층수 정보
    private CRoom[,] _room = new CRoom[CConstants.MAX_ROUTE, CConstants.MAX_ROOM_PER_ROAD]; // 방을 요소로 갖는 2차원 배열
    private int _generatedElite;                                                            // 엘리트 생성시 마다 ++ 엘리트 최대 개수보다 커지면 안됨
    private int _generatedEvent;
    public CStage(int stageNum)
    {
        _stageNum = stageNum;
        _generatedElite = 0;
        _generatedEvent = 0;

        for (int i = 0; i < CConstants.MAX_ROUTE; i++)
            for (int j = 0; j < CConstants.MAX_ROOM_PER_ROAD; j++)
                _room[i, j] = new CRoom();

    }

    public int GetStageNum()
    {
        return _stageNum;
    }

    public CRoom GetRoom(int i, int j)
    {
        return _room[i, j];
    }

    public void SetRoomType() // 룸 타입 설정
    {
        for (int i = 0; i < CConstants.MAX_ROUTE; i++)
        {
            for (int j = 0; j < CConstants.MAX_ROOM_PER_ROAD; j++)              // 모든 방 NORMAL타입으로 초기화
                _room[i, j].Type = ERoomType.Normal;
            _room[i, 0].Type = ERoomType.Start;                                 // 첫 방은 시작
            _room[i, CConstants.MAX_ROOM_PER_ROAD - 1].Type = ERoomType.Boss;   // 마지막 방은 보스
            SetElite(i);                                                        // 엘리트 몹 배치
            SetCrossRoad(i); //갈림길 배치
        }
        SetShop();                                                              //상점 맵 배치
        SetEvent();                                                             // 이벤트 맵 배치
    }

    private void SetCrossRoad(int routeNum) // 갈림길 배치
    {
        int roomNum;

        //if (routeNum == CConstants.MAX_ROUTE - 1) 3번째 길 안쓰려고 했던건데 그럴필요 없어보임
        //    return;

        for (int i = 0; i < CConstants.MAX_CROSSROAD_PER_ROAD;)
        {
            System.Random r = new System.Random();
            roomNum = r.Next() % 7; 
            if (roomNum == 0) //시작점이면 ㅌㅌ
                continue;

            if (_room[routeNum, roomNum].CheckCrossRoad == false)
            {
                _room[routeNum, roomNum].CheckCrossRoad = true;
                i++;
            }

        }
    }
    private void SetElite(int routeNum) // 엘리트 타입 배치
    {
        System.Random r = new System.Random();
        int eliteCount = r.Next(); //생성할 엘리트 개수
        int roomNum;               //엘리트 배치할 방 번호 

        if (_generatedElite == CConstants.MAX_ELITE)                                         // 생성된 엘리트가 최대치 인 경우
            return;
        else if (CConstants.MAX_ELITE - _generatedElite >= CConstants.MAX_ELITE_PER_ROAD)    // 남은 엘리트 개수가 3개 이상이면 최대 3개 생성 가능
            eliteCount %= (CConstants.MAX_ELITE_PER_ROAD + 1);
        else if (_generatedElite <= CConstants.MAX_ELITE)                                    // 남은 엘리트 개수가 3개 미만이지만 4개를 초과하지 않은 경우 1개,2개
            eliteCount %= (CConstants.MAX_ELITE - _generatedElite + 1);

        if (routeNum == 0 && eliteCount == 0) // 첫 루트의 경우 엘리트 최소 1개
            eliteCount = 1;

        if (routeNum == 2)                    // 3번째 루트의 경우 남은 엘리트 전부
            eliteCount = CConstants.MAX_ELITE - _generatedElite;

        for (int i = 0; i < eliteCount;)                                //엘리트 배치 시 조건
        {
            roomNum = (r.Next() % CConstants.MAX_ELITE_PER_ROAD + CConstants.SECOND_HALF_START);

            if (_room[routeNum, roomNum].Type == ERoomType.Elite)       //이미 엘리트가 배치 되어있을경우 ㅌㅌ            
                continue;
            else                                                        //그 외의 경우 엘리트 배치
            {
                _room[routeNum, roomNum].Type = ERoomType.Elite;
                _generatedElite++;
                i++;
            }
        }
    }

    private void SetEvent() // 이벤트 타입 배치
    {
        System.Random r = new System.Random();
        int routeNum;
        int roomNum;

        while (_generatedEvent != CConstants.MAX_EVENT)              //랜덤으로 방을 조사하여 Normal 타입 방을 Event 방으로 바꿈 Event 방의 개수 7개(상점의 개수에 따라 변동가능), Normal 방의 개수 7개
        {
            routeNum = r.Next() % CConstants.MAX_ROUTE;
            roomNum = r.Next() % CConstants.MAX_ROOM_PER_ROAD;

            if (_room[routeNum, roomNum].Type == ERoomType.Normal)
            {
                _room[routeNum, roomNum].Type = ERoomType.Event;
                _generatedEvent++;
            }
            else if (_room[routeNum, roomNum].Type == ERoomType.Shop)
                _generatedEvent++;
        }
    }

    private void SetShop()
    {
        System.Random r = new System.Random();
        int shopCount = r.Next() % CConstants.MAX_ROUTE;
        int flag = 0, route1 = 1, route2 = 2, route3 = 4;
        int routeNum;
        int roomNum;

        while (shopCount != CConstants.MAX_ROUTE) // 스테이지 통틀어 최소1개 같은루트에 2개 들어가지않게, 최대 개수의 경우 모든 루트에 1개씩 3개
        {
            routeNum = r.Next() % CConstants.MAX_ROUTE;
            roomNum = r.Next() % CConstants.MAX_ROOM_PER_ROAD;

            if (routeNum == 0 && (route1 & flag) != route1)
                if (_room[routeNum, roomNum].Type == ERoomType.Normal)
                {
                    _room[routeNum, roomNum].Type = ERoomType.Shop;
                    flag += route1;
                    shopCount++;
                }

            if (routeNum == 1 && (route2 & flag) != route2)
                if (_room[routeNum, roomNum].Type == ERoomType.Normal)
                {
                    _room[routeNum, roomNum].Type = ERoomType.Shop;
                    flag += route2;
                    shopCount++;
                }

            if (routeNum == 2 && (route3 & flag) != route3)
                if (_room[routeNum, roomNum].Type == ERoomType.Normal)
                {
                    _room[routeNum, roomNum].Type = ERoomType.Shop;
                    flag += route3;
                    shopCount++;
                }
        }
    }

    public void PrintRoom()  //디버그용 
    {
        for (int i = 0; i < CConstants.MAX_ROUTE; i++)
        {
            for (int j = 0; j < CConstants.MAX_ROOM_PER_ROAD; j++)
                Write("{0} ", _room[i, j].Type);
            WriteLine();
            for (int j = 0; j < CConstants.MAX_ROOM_PER_ROAD; j++)
            {
                if (_room[i, j].CheckCrossRoad == true)
                    Write("   /  ");
                else
                    Write("      ");
            }
            WriteLine();
        }
    }
}

public class CRoom
{
    private ERoomType _type;        // 방의 종류 정보
    private bool _checkCrossRoad;   // 갈림길 유무 정보

    public ERoomType Type
    {
        get { return _type; }
        set { _type = value; }
    }

    public bool CheckCrossRoad
    {
        get { return _checkCrossRoad; }
        set { _checkCrossRoad = value; }
    }
}

public enum ERoomType // 방의 종류
{
    Start,
    Boss,
    Normal,
    Elite,
    Event,
    Shop
}
public class CreateMap : MonoBehaviour
{
    public GameObject startRoom;
    public GameObject bossRoom;
    public GameObject eliteRoom;
    public GameObject eventRoom;
    public GameObject shopRoom;
    public GameObject normalRoom;
    public GameObject portal;
    // Start is called before the first frame update
    void Start()
    {
        CStage stage = new CStage(1);

        stage.SetRoomType();
        stage.PrintRoom();

        if(CreateStage(stage) < 0)
        {
            Debug.Log("CreateStage error\n");
            return;
        }

        stage = new CStage(2);

        stage.SetRoomType();
        stage.PrintRoom();

        stage = new CStage(3);

        stage.SetRoomType();
        stage.PrintRoom();
    }
    //테스트용 코드

    int CreateStage(CStage stage)
    {
        for (int i = 0; i < CConstants.MAX_ROUTE; i++)
        {
            for (int j = 0; j < CConstants.MAX_ROOM_PER_ROAD; j++)
            {
                switch (stage.GetRoom(i, j).Type)
                {
                    case ERoomType.Start:
                        CreateStartRoom(i, j, stage);
                        break;
                    case ERoomType.Boss:
                        CreateBossRoom(i, j, stage);
                        break;
                    case ERoomType.Elite:
                        CreateEliteRoom(i, j, stage);
                        break;
                    case ERoomType.Event:
                        CreateEventRoom(i, j, stage);
                        break;
                    case ERoomType.Shop:
                        CreateShopRoom(i, j, stage);
                        break;
                    case ERoomType.Normal:
                        CreateNormalRoom(i, j, stage);
                        break;                  
                }

                CreateCrossRoad(i, j, stage);
            }
        }
        return 1;
    }

    void CreateStartRoom(int i, int j, CStage stage)
    {
        if (i > 0) //1회만 생성하기 위함
            return;
        Instantiate(startRoom, new Vector3(0, 0, 0), Quaternion.Euler(new Vector3(90, 0, 0)));
    }
    void CreateBossRoom(int i, int j, CStage stage)
    {
        if (i > 0)
            return;
        Instantiate(bossRoom, new Vector3(0, 0, j * CConstants.ROOM_DISTANCE_Z), Quaternion.Euler(new Vector3(90, 0, 0)));
    }

    void CreateEliteRoom(int i, int j, CStage stage)
    {
        Instantiate(eliteRoom, new Vector3((i-1)*CConstants.ROOM_DISTANCE_X, 0, j * CConstants.ROOM_DISTANCE_Z), Quaternion.Euler(new Vector3(90, 0, 0)));
    }

    void CreateEventRoom(int i, int j, CStage stage)
    {
        Instantiate(eventRoom, new Vector3((i - 1) * CConstants.ROOM_DISTANCE_X, 0, j * CConstants.ROOM_DISTANCE_Z), Quaternion.Euler(new Vector3(90, 0, 0)));
    }

    void CreateShopRoom(int i, int j, CStage stage)
    {
        Instantiate(shopRoom, new Vector3((i - 1) * CConstants.ROOM_DISTANCE_X, 0, j * CConstants.ROOM_DISTANCE_Z), Quaternion.Euler(new Vector3(90, 0, 0)));
    }

    void CreateNormalRoom(int i, int j, CStage stage)
    {
        Instantiate(normalRoom, new Vector3((i - 1) * CConstants.ROOM_DISTANCE_X, 0, j * CConstants.ROOM_DISTANCE_Z), Quaternion.Euler(new Vector3(90, 0, 0)));
    }

    void CreatePortal(int i, int j, bool left)
    {
        if(left == true)
        {         
            GameObject tempPortal = Instantiate(portal, new Vector3(CConstants.LEFT_PORTAL_2+((i-1)*CConstants.ROOM_DISTANCE_X), CConstants.PORTAL_POSITION_Y, j * CConstants.ROOM_DISTANCE_Z + CConstants.PORTAL_POSITION_Z), Quaternion.Euler(new Vector3(0, 0, 0)));
            tempPortal.tag = "LEFT_PORTAL";
        }
        else
        {
            GameObject tempPortal = Instantiate(portal, new Vector3(CConstants.RIGHT_PORTAL_1 + (i * CConstants.ROOM_DISTANCE_X), CConstants.PORTAL_POSITION_Y, j * CConstants.ROOM_DISTANCE_Z + CConstants.PORTAL_POSITION_Z), Quaternion.Euler(new Vector3(0, 0, 0)));
            tempPortal.tag = "RIGHT_PORTAL";
        }
    }
    
    void CreateCrossRoad(int i, int j, CStage stage)
    {
        if (stage.GetRoom(i, j).CheckCrossRoad == true)
        {
            switch(i)
            {
                case 0:
                    CreatePortal(i, j, false);
                    break;
                case 1:
                    System.Random r = new System.Random();
                    if (r.Next() % 2 == 0) //왼쪽
                    {
                        CreatePortal(i, j, true);
                    }
                    else //오른쪽
                    {
                        CreatePortal(i, j, false);
                    }
                    break;
                case 2:
                    CreatePortal(i, j, true);
                    break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
