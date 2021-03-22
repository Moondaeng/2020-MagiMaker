using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGlobal : MonoBehaviour
{
    public static bool isPlay = false; // 메뉴창에서 플레이 중에 메뉴창으로 들어왔는지 체크하는 플래그
    public static bool isClear = true; //CCreateMap에서 포탈 생성 조건
    public static bool isPortalActive = true; //노말방 포탈 액티브를 위한 포탈 작동 확인 플래그
    public static int roomCount = 0;
    public static bool isEvent = false; //갑작스레 몬스터가 생겨서 포탈 이동이 제한된다거나 할 때의 플래그

    public static int _idleState = Animator.StringToHash("Base Layer.Idle");
    public static int _standState = Animator.StringToHash("Base Layer.MovingSub.Stand");
    public static int _walkState = Animator.StringToHash("Base Layer.MovingSub.Walk");
    public static int _runState = Animator.StringToHash("Base Layer.MovingSub.Run");
    public static int _attackState1 = Animator.StringToHash("Base Layer.AttackSub.Attack1");
    public static int _attackState2 = Animator.StringToHash("Base Layer.AttackSub.Attack2");
    public static int _waitState = Animator.StringToHash("Base Layer.AttackSub.AttackWait");
    public static int _skillState1 = Animator.StringToHash("Base Layer.AnySub.Skill1");
    public static int _skillState2 = Animator.StringToHash("Base Layer.AnySub.Skill2");
    public static int _skillState3 = Animator.StringToHash("Base Layer.AnySub.Skill3");
    public static int _skillWaitState1 = Animator.StringToHash("Base Layer.AnySub.SkillWait1");
    public static int _skillWaitState2 = Animator.StringToHash("Base Layer.AnySub.SkillWait2");
    public static int _skillWaitState3 = Animator.StringToHash("Base Layer.AnySub.SkillWait3");
    public static int _gethitState = Animator.StringToHash("Base Layer.AnySub.GetHit");
    public static int _deadState = Animator.StringToHash("Base Layer.AnySub.Dead");

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
