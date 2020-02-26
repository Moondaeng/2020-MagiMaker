﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CConstants
{
    public const int MAX_ELITE = MAX_ROOM_PER_ROAD / 2;                         // 엘리트 최대 개수
    public const int MAX_ELITE_PER_ROAD = (MAX_ROOM_PER_ROAD - 2) / 2;          // 길마다 포진할 수 있는 엘리트의 최대 개수(총 방 수에서 보스방 스타팅 제외하고 절반)
    public const int MAX_ROOM_PER_ROAD = 8;                                     // 길 당 방 개수
    public const int MAX_CROSSROAD_PER_ROAD = MAX_ROOM_PER_ROAD / 4;                     // 갈림길 개수
    public const int MAX_ROUTE = 3;                                             // 길 개수
    public const int SECOND_HALF_START = MAX_ROOM_PER_ROAD / 2;                 // 엘리트가 나오는 후반부 방의 시작 번호(배열 상에서 조정없이 사용할 수 있도록 방8개 기준 0123 4567
    public const int MAX_EVENT_PER_ROAD = (MAX_ROOM_PER_ROAD / 2) + 1;          // 길 당 이벤트 최대 개수
    public const int MAX_EVENT = ((MAX_ROOM_PER_ROAD - 2) * 3 - MAX_ELITE) / 2; // 이벤트 최대 개수(첫방, 보스방 제외 모든방을 합쳐 엘리트 개수를 뺀 나머지를 1/2한 개수)

    //포탈의 위치
    public const int PORTAL_POSITION_Z = 48;
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
    public const int PORTAL_DISTANCE_X = 82;
    public const int PORTAL_DISTANCE_Z = 38;
    //방과 방사이의 거리
    public const int ROOM_DISTANCE_Z = 135;
    public const int ROOM_DISTANCE_X = MID_PORTAL_3;
}
public class Constants : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
