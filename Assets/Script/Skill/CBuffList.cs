using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 버프 목록 관리
 * 해당 버프에 맞는 스킬, UI 저장
 */
public static class CBuffList
{
    public const int Stun           = 101;
    public const int Slow           = 102;
    public const int Snare          = 103;
    public const int DotDamage      = 104;

    public const int AttackBuff     = 201;
    public const int DefenceBuff    = 202;

    public static Dictionary<int, string> BuffUIList = new Dictionary<int, string>()
    {
        {AttackBuff, "RPG icons/64X64/Sword_1"},
        {DefenceBuff, "RPG icons/64X64/Shield"}
    };
}
