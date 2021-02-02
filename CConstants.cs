using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CConstants
{
    #region MAX Room
    public const int MAX_ELITE = MAX_ROOM_PER_ROAD / 2;                         // 엘리트 최대 개수
    public const int MAX_ELITE_PER_ROAD = (MAX_ROOM_PER_ROAD - 2) / 2;          // 길마다 포진할 수 있는 엘리트의 최대 개수(총 방 수에서 보스방 스타팅 제외하고 절반)
    public const int MAX_ROOM_PER_ROAD = 8;                                     // 길 당 방 개수
    public const int MAX_CROSSROAD_PER_ROAD = MAX_ROOM_PER_ROAD / 4;                     // 갈림길 개수
    public const int MAX_ROUTE = 3;                                             // 길 개수
    public const int SECOND_HALF_START = MAX_ROOM_PER_ROAD / 2;                 // 엘리트가 나오는 후반부 방의 시작 번호(배열 상에서 조정없이 사용할 수 있도록 방8개 기준 0123 4567
    public const int MAX_EVENT_PER_ROAD = (MAX_ROOM_PER_ROAD / 2) + 1;          // 길 당 이벤트 최대 개수
    public const int MAX_EVENT = ((MAX_ROOM_PER_ROAD - 2) * 3 - MAX_ELITE) / 2; // 이벤트 최대 개수(첫방, 보스방 제외 모든방을 합쳐 엘리트 개수를 뺀 나머지를 1/2한 개수)
    #endregion

    #region Distance & Position
    //포탈의 위치
    public const int PORTAL_POSITION_Z = 48; 
    public const int PORTAL_POSITION_X = 48;
    public const float PORTAL_POSITION_Y = 0.01f;
    //중앙 포탈 1번째길 2번째길 3번째길
    public const int MID_PORTAL_1 = -130;
    public const int MID_PORTAL_2 = 0;
    public const int MID_PORTAL_3 = 130;
    //왼쪽 포탈 2,3 길
    public const int LEFT_PORTAL_2 = -48;
    public const int LEFT_PORTAL_3 = 82;
    //오른쪽 포탈 1,2 길
    public const int RIGHT_PORTAL_1 = -82;
    public const int RIGHT_PORTAL_2 = 48;
    //포탈로 이동한 거리
    public const int PORTAL_DISTANCE_X = 130;
    public const int PORTAL_DISTANCE_Z = 38;
    //방과 방사이의 거리
    public const int ROOM_DISTANCE_Z = 135;
    public const int ROOM_DISTANCE_X = MID_PORTAL_3;
    #endregion

    #region Max Min Stage
    public const int ROOM_PER_STAGE = 12;              //시작 + 랜덤 + 보스
    public const int MAX_ROAD = 3;                     //갈림길 최대
    public const int MIN_ROAD = 1;                     //갈림길 최소
    public const int MAX_SKILL_ELITE_PER_STAGE = 2;    //스테이지당 최대 스킬 엘리트
    public const int MAX_EVENT_PER_STAGE = 15;         //스테이지당 최대 이벤트 방
    public const int MIN_SHOP_PER_STAGE = 1;           //스티이지당 최소 상점
    #endregion

    #region Stage Probability
    public const int NORMAL_PROBABILITY = 30;  //일반 방 출현 확률 30퍼
    public const int EVENT_PROBABLILITY = 30;  //이벤트 방
    public const int SKILL_PROBABILITY = 15;   //스킬 엘리트
    public const int ITEM_PROBABILITY = 15;    //아이템 엘리트
    public const int SHOP__PROBABILITY = 10;   //상점
    #endregion

    #region Shop Purchase Item
    public const int MAX_COMMON_EQUIPMENT_SHOP = 2;
    public const int MAX_SPECIAL_EQUIPMENT_SHOP = 1;
    public const int MAX_RARE_AND_MYSTERY_EQUIPMENT_SHOP = 1;
    public const int MAX_SHOP_EQUIPMENT_SHOP = 2;
    public const int MAX_CONSUMABLE_SHOP = 2;
    #endregion

    public const int MAX_ITEM_IN_GAME = 100;

    #region Item Drop Percentage
    public const int COMMON = 40;
    public const int SPECIAL = 70;
    public const int UNIQUE = 90;
    public const int MYSTERY = 100;
    #endregion

    #region Number of Slot
    public const int SLOT_ROW = 2;  //CStroeController에서 커서 위치 이동에 필요한 상수들
    public const int SLOT_COLOUMN = 4;
    #endregion //CStoreController에서 커서 이동에 필요한 상수들

    public const int EVENT_ROLLING_STONE_DAMAGE = 100;

    public static int EQUIP_ITEM_TYPE = 1;
    public static int CONSUM_ITEM_TYPE = 0;
}