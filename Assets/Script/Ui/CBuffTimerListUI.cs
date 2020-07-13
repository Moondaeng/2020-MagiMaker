using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CBuffTimerListUI : CTimerListUiManager
{
    // 추적해서 그릴 타이머 등록
    // 이후 등록한 대상의 타이머를 따라 그림
    public new void RegisterTimer(GameObject timerOwner)
    {
        _timer = timerOwner.GetComponent<CBuffTimer>();
        _timer.TimerStart += Register;
        _timer.TimerEnd += Deregister;
    }

    public override void DeregisterTimer(GameObject timerOwner)
    {
        _timer = timerOwner.GetComponent<CBuffTimer>();
        _timer.TimerStart -= Register;
        _timer.TimerEnd -= Deregister;
    }

    protected override Sprite GetImageByRegisterNumber(int registeredNumber)
    {
        if (!CBuffList.BuffUIList.TryGetValue(registeredNumber, out string spritePath))
        {
            return base.GetImageByRegisterNumber(registeredNumber);
        }
        else
            return Resources.Load<Sprite>(spritePath);
    }
}
