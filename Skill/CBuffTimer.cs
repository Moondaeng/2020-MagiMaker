using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BuffStack
{
    public bool stackable;
    public int stack;

    public BuffStack(bool isStackable, int initStack)
    {
        stackable = isStackable;
        stack = initStack;
    }
}

/*
 * 버프 및 디버프 시간을 관리하는 타이머 클래스
 * 필요에 따라 다양한 기능 추가
 */
public class CBuffTimer : CTimer<BuffStack>
{
    // 시간에 상관없이 해당 버프 취소
    // 버프 효과 해제, 디버프 효과 해제에 이용 가능
    public void CancelBuff(int registeredNum)
    {
        var cancel = FindByRegisterNumber(registeredNum);

        if(cancel == null) return;

        cancel.Value.notify();
        TimerEnd?.Invoke(cancel.Value.key);
        observeList.Remove(cancel);
    }

    public void Register(int regNum, float time, Callback callback, int stack)
    {
        Register(regNum, time, callback, new BuffStack(true, stack));
    }

    // 스택 없는 버프 추가
    public void Register(int regNum, float time, Callback callback)
    {
        Register(regNum, time, callback, new BuffStack(false, 1));
    }

    public bool? IsStackableBuff(int registeredNumber)
    {
        var observed = FindByRegisterNumber(registeredNumber);
        return observed?.Value.data.stackable;
    }

    // 타이머 갱신
    // 새로운 타이머 설정 시간에 맞춰 갱신
    protected override void Renew(int regNum, float updataTime, BuffStack updateData)
    {
        var observed = FindByRegisterNumber(regNum);
        if (observed != null)
        {
            observed.Value.current = updataTime;
            observed.Value.max = updataTime;
            if(observed.Value.data.stackable)
            {
                observed.Value.data.stack += updateData.stack;
            }
        }
    }
}
