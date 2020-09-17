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
public class CBuffTimer : CTimer<BuffStack, int>
{
    // 시간에 상관없이 해당 버프 취소
    // 버프 효과 해제, 디버프 효과 해제에 이용 가능
    public void CancelBuff(int registeredNum)
    {
        var cancel = FindByRegisterNumber(registeredNum);

        if(cancel == null) return;

        cancel.Value.endCallback(cancel.Value.data.stack);
        TimerEnd?.Invoke(cancel.Value.key);
        observeList.Remove(cancel);
    }

    public void Register(int regNum, float time, int stack, Callback startFunc, Callback endFunc)
    {
        Register(regNum, time, startFunc, endFunc, new BuffStack(true, stack));
    }

    // 스택 없는 버프 추가
    public void Register(int regNum, float time, Callback startFunc, Callback endFunc)
    {
        Register(regNum, time, startFunc, endFunc, new BuffStack(false, 1));
    }

    public bool? IsStackableBuff(int registeredNumber)
    {
        var observed = FindByRegisterNumber(registeredNumber);
        return observed?.Value.data.stackable;
    }

    public int GetBuffStack(int registeredNumber)
    {
        var observed = FindByRegisterNumber(registeredNumber);
        return observed == null ? -1 : observed.Value.data.stack;
    }

    public List<int> GetRegisterNumberList()
    {
        List<int> regNumList = new List<int>();
        for (var find = observeList.First; find != null; find = find.Next)
        {
            regNumList.Add(find.Value.key);
        }
        //regNumList.Add();
        return regNumList;
    }

    protected override void ExcuteStartCallback(LinkedListNode<CObserved> observedObject)
    {
        observedObject.Value.startCallback?.Invoke(observedObject.Value.data.stack);
    }

    protected override void ExcuteEndCallback(LinkedListNode<CObserved> observedObject)
    {
        observedObject.Value.endCallback?.Invoke(observedObject.Value.data.stack);
    }

    protected override void Remove(LinkedListNode<CObserved> cObservedNode)
    {
        if (cObservedNode.Value.data.stackable == false)
        {
            base.Remove(cObservedNode);
        }
        else
        {
            if (cObservedNode.Value.data.stack <= 0)
            {
                Debug.LogFormat("Buff Timer Error - stack is {0}", cObservedNode.Value.data.stack);
            }

            if (cObservedNode.Value.data.stack <= 1)
            {
                base.Remove(cObservedNode);
            }
            else
            {
                ExcuteEndCallback(cObservedNode);
                cObservedNode.Value.data.stack--;
                cObservedNode.Value.current = cObservedNode.Value.max;
                ExcuteStartCallback(cObservedNode);
            }
        }
    }

    // 버프 갱신
    // 시간 초기화 외에도 버프를 새로 갱신한다
    protected override void Renew(int regNum, float updataTime, BuffStack updateData)
    {
        var observed = FindByRegisterNumber(regNum);
        if (observed == null)
        {
            return;
        }
        
        observed.Value.current = updataTime;
        observed.Value.max = updataTime;
        if (observed.Value.data.stackable)
        {
            ExcuteEndCallback(observed);
            observed.Value.data.stack += updateData.stack;
            Debug.LogFormat("buff stack : {0}", observed.Value.data.stack);
            ExcuteStartCallback(observed);
        }
    }
}
