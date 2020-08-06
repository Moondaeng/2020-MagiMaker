using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSkillTimer : CTimer<int, int>
{
    protected new void Awake()
    {
        base.Awake();
    }
    
    // 스킬 추가
    public void Register(int regNum, float time, Callback callback)
    {
        Register(regNum, time, callback, default);
    }

    // 해당 번호를 가진 observe 대상 시간 추가 / 감소
    // 등록된 번호가 없을 경우 모든 타이머에 동작
    // addTime에 음의 값을 넣으면 감소로 동작함
    public void AddCooldownExclude(int registeredNumber, float addTime)
    {
        var exclude = FindByRegisterNumber(registeredNumber);

        var timer = observeList.First;
        while(timer != null)
        {
            if(timer != exclude)
            {
                timer.Value.current += addTime;
            }
            timer = timer.Next;
        }
    }

    // 해당 번호를 가진 observe 대상 시간 추가 / 감소
    // 등록된 번호가 없을 경우 동작하지 않음
    // addTime에 음의 값을 넣으면 감소로 동작함
    public void AddCooldownOne(int registeredNumber, float addTime)
    {
        var timer = FindByRegisterNumber(registeredNumber);

        timer.Value.current += addTime;
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

        return minimumNumber;
    }
}
