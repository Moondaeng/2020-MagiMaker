using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGlobal : MonoBehaviour
{
    public static bool isPlay = false; // 메뉴창에서 플레이 중에 메뉴창으로 들어왔는지 체크하는 플래그
    public static bool isClear = false; //CCreateMap에서 포탈 생성 조건
    public static bool isPortalActive = true; //노말방 포탈 액티브를 위한 포탈 작동 확인 플래그
    public static int roomCount = 0;
    public static bool isEvent = false; //갑작스레 몬스터가 생겨서 포탈 이동이 제한된다거나 할 때의 플래그
    public static bool useNPC = false; //상점 사용중일때 무브 방지용 플래그
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

    static public T CopyComponent<T>(T original, GameObject destination) where T : Component
    {
        System.Type type = original.GetType();
        Component copy = destination.AddComponent(type);
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(original));
        }
        return copy as T;
    }
}
