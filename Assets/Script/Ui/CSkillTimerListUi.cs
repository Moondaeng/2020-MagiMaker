using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSkillTimerListUi : CTimerListUiManager
{
    protected LinkedList<int> _excludeNumber = new LinkedList<int>();

    // 추적해서 그릴 타이머 등록
    // 이후 등록한 대상의 타이머를 따라 그림
    public new void RegisterTimer(GameObject timerOwner)
    {
        _timer = timerOwner.GetComponent<CSkillTimer>();
        _timer.TimerStart += Register;
        _timer.TimerEnd += Deregister;
    }

    public override void DeregisterTimer(GameObject timerOwner)
    {
        _timer = timerOwner.GetComponent<CSkillTimer>();
        _timer.TimerStart -= Register;
        _timer.TimerEnd -= Deregister;
    }

    // 쿨타임 그리기 제외 목록을 설정한다
    public void RegisterExcludeNumber(int registeredNumber)
    {
        _excludeNumber.AddFirst(registeredNumber);
    }

    // 쿨타임 그리기 제외 목록에서 제거한다
    public void DeregisterExcludeNumber(int registeredNumber)
    {
        _excludeNumber.Remove(registeredNumber);
    }

    protected override void Register(int registeredNumber)
    {
        if (IsExcludeNumber(registeredNumber))
            return;

        base.Register(registeredNumber);
    }

    // 그리기 제외 대상인지 확인
    protected bool IsExcludeNumber(int registeredNumber)
    {
        return _excludeNumber.Contains(registeredNumber);
    }
}
