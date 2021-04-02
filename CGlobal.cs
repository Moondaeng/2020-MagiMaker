﻿using UnityEngine;

public class CGlobal : MonoBehaviour
{
    public static bool isClear = false; //CCreateMap에서 포탈 생성 조건
    public static bool isEvent = false; //갑작스레 몬스터가 생겨서 포탈 이동이 제한된다거나 할 때의 플래그
    public static bool popUpCancel = false; //팝업 끌지말지 결정용 플래그
    public static bool isTutorial = true; //튜토리얼 맵 생성
    public static bool isHost = true; //현재 플레이어가 호스트인지 확인하는 변수. 임시로 만든 변수다.

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
}
