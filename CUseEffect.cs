using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
