using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSkillTimer : CTimer
{
    private static CLogComponent _logger;

    protected new void Awake()
    {
        base.Awake();
        _logger = new CLogComponent(ELogType.Skill);
    }
    
    // 현재 쿨다운 추가
    // 타이머 상에 존재하는 번호만 적용됨
    public void AddCooldown(int registeredNumber, float addTime)
    {
        var timer = FindByRegisterNumber(registeredNumber);
        if (timer == null) return;

        timer.Value.current += addTime;
    }

    // 현재 쿨다운 감소
    // 타이머 상에 존재하는 번호만 적용됨
    public void SubtractCooldown(int registeredNumber, float subTime)
    {
        var timer = FindByRegisterNumber(registeredNumber);
        if (timer == null) return;
        
        timer.Value.current += subTime;
    }

    // 등록 번호들 중 가장 쿨타임이 적게 남은 번호를 순차 리스트 범위에서 찾는다
    // 만약 타이머에 없는 번호인 경우 쿨다운 중에 있지 않은 상태이므로 해당 번호를 리턴한다
    public int FindMinimumCooldown(List<int> comboList)
    {
        int minimumNumber = -1;
        LinkedListNode<CObserved> minimumCooldown = null;

        foreach(var skill in comboList)
        {
            var cooldown = FindByRegisterNumber(skill);
            if (cooldown == null)
            {
                Debug.LogFormat("minimum = {0}", skill);
                return skill;
            }
            else
            {
                if (minimumCooldown == null || cooldown.Value.current < minimumCooldown.Value.current)
                {
                    minimumCooldown = cooldown;
                    minimumNumber = skill;
                }
            }
        }
        Debug.LogFormat("final minimum = {0}", minimumNumber);

        return minimumNumber;
    }
}
