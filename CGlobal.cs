using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGlobal : MonoBehaviour
{
    public static bool isPlay = false; // 메뉴창에서 플레이 중에 메뉴창으로 들어왔는지 체크하는 플래그
    public static bool isClear = true; //CCreateMap에서 포탈 생성 조건
    public static bool isPortalActive = true; //노말방 포탈 액티브를 위한 포탈 작동 확인 플래그
    public static int roomCount = 0;

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
}
