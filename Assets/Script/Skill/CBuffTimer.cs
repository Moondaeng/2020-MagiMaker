using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 버프 및 디버프 시간을 관리하는 타이머 클래스
 * 필요에 따라 다양한 기능 추가
 */
public class CBuffTimer : CTimer
{
    // 시간에 상관없이 해당 버프 취소
    // 버프 효과 해제, 디버프 효과 해제에 이용 가능
    public void CancelBuff(int registeredNum)
    {
        var cancel = FindByRegisterNumber(registeredNum);

        if(cancel == null) return;

        cancel.Value.notify();
        TimerEnd?.Invoke(cancel.Value.registerNumber);
        var remove = cancel;
        cancel = cancel.Next;
        observeList.Remove(cancel);
    }
}
