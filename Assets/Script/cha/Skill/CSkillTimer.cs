using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSkillTimer : MonoBehaviour
{
    public delegate void Callback();

    class CCooldown
    {
        public float current;
        public float max;
        public Callback notify;
        public bool isNotifyed;
    }

    private static CLogComponent _logger;

    private CSkillUIManager skillUIManager;

    private List<CCooldown> _cooldownList = new List<CCooldown>();
    private const float _updateTime = 0.1f;
    private int _updateThreshold;
    private int _updateCount;

    private void Awake()
    {
        _logger = new CLogComponent(ELogType.Skill);
        skillUIManager = GameObject.Find("SkillScript").GetComponent<CSkillUIManager>();
        _updateThreshold = (int)(_updateTime / Time.fixedDeltaTime);
        _updateCount = 0;
    }

    private void FixedUpdate()
    {
        _updateCount++;

        // 업데이트 작동 횟수 조절
        if(_updateCount % _updateThreshold != 1)
        {
            return;
        }

        // 모든 관찰 대상 상태 갱신
        for (int registerNumber = 0; registerNumber < _cooldownList.Count; registerNumber++)
        {
            var cooldown = _cooldownList[registerNumber];
            if (cooldown.current > 0)
            {
                TimerRun(registerNumber, Time.fixedDeltaTime * _updateThreshold);
            }
            else
            {
                TimerEnd(registerNumber);
            }
        }
        skillUIManager.Draw();
    }

    // 타이머 시간 설정 및 쿨타임 그리기 활성화
    public void TimerStart(int registeredNumber)
    {
        var cooldown = _cooldownList[registeredNumber];

        cooldown.current = cooldown.max;
        cooldown.isNotifyed = false;

        skillUIManager.CooldownEnable(registeredNumber);
    }

    private void TimerRun(int registeredNumber, float ranTime)
    {
        var cooldown = _cooldownList[registeredNumber];

        cooldown.current -= ranTime;
    }

    // 타이머 종료. 쿨타임 완료 알림 및 쿨타임 그리기 비활성화 
    private void TimerEnd(int registeredNumber)
    {
        var cooldown = _cooldownList[registeredNumber];

        if (cooldown.isNotifyed)
        {
            return;
        }

        cooldown.notify();
        cooldown.isNotifyed = true;

        skillUIManager.CooldownDisable(registeredNumber);
    }

    public int RegisterSkill(float cooldownMax, Callback cooldownNotify)
    {
        var newSkill = new CCooldown
        {
            current = 0,
            max = cooldownMax,
            notify = cooldownNotify,
            isNotifyed = false
        };

        _cooldownList.Add(newSkill);

        _logger.Log("skill registerd : {0}", _cooldownList.Count);

        return _cooldownList.Count - 1;
    }

    public float GetCurrentCooldown(int registeredNumber)
    {
        return _cooldownList[registeredNumber].current;
    }

    public float GetMaxCooldown(int registeredNumber)
    {
        return _cooldownList[registeredNumber].max;
    }

    // 등록 번호들 중 가장 쿨타임이 적게 남은 번호를 순차 리스트 범위에서 찾는다
    public int FindMinimumCooldown(int startPosition, int searchSize)
    {
        int minimumNumber = -1;
        
        for(int number = startPosition; number < startPosition + searchSize; number++)
        {
            if(minimumNumber == -1 || _cooldownList[number].current < _cooldownList[minimumNumber].current)
            {
                minimumNumber = number;
            }
        }

        return minimumNumber;
    }

    // 등록 번호들 중 가장 쿨타임이 적게 남은 번호를 순차 리스트 범위에서 찾는다
    public int FindMinimumCooldown(List<int> comboList)
    {
        int minimumNumber = -1;

        foreach(var skill in comboList)
        {
            if(minimumNumber == -1 || _cooldownList[skill].current < _cooldownList[minimumNumber].current)
            {
                minimumNumber = skill;
            }
        }

        return minimumNumber;
    }
}
