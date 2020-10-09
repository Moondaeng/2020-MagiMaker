using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class CUseEffectExplain
{
    public static readonly Dictionary<CUseEffect.DamageType, string> DamageTypeExplainDict
            = new Dictionary<CUseEffect.DamageType, string>
        {
            {CUseEffect.DamageType.damage, "데미지"},
            {CUseEffect.DamageType.heal, "힐"},
        };

    public static readonly Dictionary<CUseEffect.CCType, string> CCTypeExplainDict
            = new Dictionary<CUseEffect.CCType, string>
        {
            {CUseEffect.CCType.slow, "느려짐"},
            {CUseEffect.CCType.stun, "기절"},
        };

    public static readonly Dictionary<CUseEffect.BuffType, string> BuffTypeExplainDict
            = new Dictionary<CUseEffect.BuffType, string>
        {
            {CUseEffect.BuffType.attackBuff, "공격력 증가"},
            {CUseEffect.BuffType.defenceBuff, "방어력 증가"},
        };

    public static string CreateUseEffectText(CUseEffect useEffect)
    {
        StringBuilder sb = new StringBuilder();
        foreach(var damage in useEffect.DamageEffectList)
        {
            if(!DamageTypeExplainDict.TryGetValue(damage.type, out string text))
            {
                text = damage.type.ToString();
                Debug.Log($"Warning : {damage.type.GetType().ToString()}'s {text} explain isn't setting");
            }
            sb.AppendLine("즉시 " + damage.startDamage + "만큼 " + text + "주고" + damage.dotPeriod + "동안 초당" + damage.dotDamage + "만큼 " + text);
        }
        foreach (var cc in useEffect.CCEffectList)
        {
            if (!CCTypeExplainDict.TryGetValue(cc.type, out string text))
            {
                text = cc.type.ToString();
                Debug.Log($"Warning : {cc.type.GetType().ToString()}'s {text} explain isn't setting");
            }
            sb.AppendLine(cc.time + "초 " + text);
        }
        foreach (var buff in useEffect.BuffEffectList)
        {
            if (!BuffTypeExplainDict.TryGetValue(buff.type, out string text))
            {
                text = buff.type.ToString();
                Debug.Log($"Warning : {buff.type.GetType().ToString()}'s {text} explain isn't setting");
            }
            sb.AppendLine(buff.time + "초 동안 " + buff.effectPersent + "% " + text);
        }
        return sb.ToString();
    }

    public static string CreateUseEffectListText(List<UseEffectWithChance> useEffects)
    {
        StringBuilder sb = new StringBuilder();
        
        return sb.ToString();
    }
}

/*
 * 효과 정의 클래스
 */
[System.Serializable]
public class CUseEffect
{
    // 공격에 해당하는 모든 경우들
    // 데미지, 스턴 등
    public enum DamageType
    {
        damage, heal
    }

    // 공격에 해당하는 경우들에 필요한 정보들 구조체
    // 데미지 - 데미지 계수 / 스턴 - 시간 / 슬로우 - 슬로우량, 시간 등
    // 필요에 따라 추가하기

    [System.Serializable]
    public struct DamageEffect
    {
        public DamageType type;
        [Tooltip("즉발로 입히는 데미지량")]
        public int startDamage;
        [Tooltip("초당 데미지(DoT)량")]
        public int dotDamage;
        [Tooltip("DoT 데미지 지속시간")]
        public float dotPeriod;
    }

    public enum CCType
    {
        stun, slow
    }

    [System.Serializable]
    public struct CCEffect
    {
        public CCType type;
        [Tooltip("지속 시간")]
        public float time;
        [Tooltip("효과 퍼센트")]
        public int effectPersent;
    }

    // 아군을 돕는 행위 관련 모든 경우들
    // 힐, 버프 등
    public enum BuffType
    {
        attackBuff, defenceBuff
    }

    // 공격에 해당하는 경우들에 필요한 정보들 구조체
    // 데미지 - 데미지 계수 / 스턴 - 시간 / 슬로우 - 슬로우량, 시간 등
    // 필요에 따라 추가하기
    [System.Serializable]
    public struct BuffEffect
    {
        public BuffType type;
        [Tooltip("지속 시간")]
        public float time;
        [Tooltip("효과 퍼센트")]
        public float effectPersent;
    }

    [SerializeField]
    public List<DamageEffect> DamageEffectList;
    [SerializeField]
    public List<CCEffect> CCEffectList;
    [SerializeField]
    public List<BuffEffect> BuffEffectList;
}

/// <summary>
/// 확률적으로 효과 사용
/// </summary>
[System.Serializable]
public class UseEffectWithChance
{
    [Tooltip("사용 효과")]
    public CUseEffect useEffect;
    [Tooltip("사용 시 생성하는 오브젝트(투사체 등)")]
    public GameObject useEffectObject;
    [Tooltip("효과 발동 확률")]
    [Range(0f, 1f)] public float Chance;
}
