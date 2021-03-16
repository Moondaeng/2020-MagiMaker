using UnityEngine;

public class CGlobal : MonoBehaviour
{
    public static bool isClear = false; //CCreateMap에서 포탈 생성 조건
    public static bool isEvent = false; //갑작스레 몬스터가 생겨서 포탈 이동이 제한된다거나 할 때의 플래그
    public static bool popUpCancel = false; //팝업 끌지말지 결정용 플래그
    public static bool isTutorial = true; //튜토리얼 맵 생성

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
