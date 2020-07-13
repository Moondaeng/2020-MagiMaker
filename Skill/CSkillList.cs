using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CSkillList
{
    public const int comboSkill1 = 4;
    public const int comboSkill2 = 5;
    public const int comboSkill3 = 6;
    public const int comboSkill4 = 7;
    public const int comboSkill5 = 8;
    public const int comboSkill6 = 9;
    public const int comboSkill7 = 10;
    public const int comboSkill8 = 11;
    public const int comboSkill9 = 12;

    public static Dictionary<int, string> SkillList = new Dictionary<int, string>()
    {
        {comboSkill1, "Clean Vector Icons/T_3_magic_fire_"},
        {comboSkill2, "Clean Vector Icons/T_2_lighthing_"},
        {comboSkill3, "Clean Vector Icons/T_9_kunai_"},
        {comboSkill4, "Clean Vector Icons/T_10_hand_fire_"},
        {comboSkill5, "Clean Vector Icons/T_9_cloud_down_"},
        {comboSkill6, "Clean Vector Icons/T_12_spit_"},
        {comboSkill7, "Clean Vector Icons/T_6_cloud_"},
        {comboSkill8, "Clean Vector Icons/T_4_eye_bleed_"},
        {comboSkill9, "Clean Vector Icons/T_1_scroll_"},
    };
}
