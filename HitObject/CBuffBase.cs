﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBuffBase : MonoBehaviour
{
    // 아군을 돕는 행위 관련 모든 경우들
    // 힐, 버프 등
    public enum BuffType
    {
        fastHeal, attackBuff, defenceBuff
    }

    // 공격에 해당하는 경우들에 필요한 정보들 구조체
    // 데미지 - 데미지 계수 / 스턴 - 시간 / 슬로우 - 슬로우량, 시간 등
    // 필요에 따라 추가하기
    [System.Serializable]
    public struct BuffArgument
    {
        public BuffType type;
        public float arg1;
        public float arg2;
    }

    public List<BuffArgument> BuffArgumentList;

    private float lifeTime = 0.3f;

    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        Invoke("Death", lifeTime);
    }

    // 자동 소멸
    private void Death()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.tag == "Player" || gameObject.tag == "Allies")
        {
            if (other.CompareTag("Player") || other.CompareTag("Allies"))
            {
                // 이벤트 처리 : 네트워크한테 충돌 알림

                // 버프 관련 함수
                var aliesPara = other.GetComponent<CharacterPara>();
                if (aliesPara != null)
                {
                    foreach (var buffArg in BuffArgumentList)
                    {
                        switch (buffArg.type)
                        {
                            case BuffType.fastHeal:
                                break;
                            case BuffType.attackBuff:
                                aliesPara.buffParameter.BuffAttack(buffArg.arg1, buffArg.arg2);
                                break;
                            case BuffType.defenceBuff:
                                aliesPara.buffParameter.BuffDefenceStack(buffArg.arg1, buffArg.arg2, 1);
                                break;
                        }
                    }
                }
            }
        }
        else if (gameObject.tag == "Monster")
        {
            if (other.CompareTag("Player") || other.CompareTag("Allies"))
            {
            }
        }
    }
}