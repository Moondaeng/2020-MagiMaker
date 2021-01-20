using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BuffStack
{
    public int MaxStack;
    public int currentStack;

    public BuffStack(int maxStack, int increaseStack)
    {
        MaxStack = maxStack;
        currentStack = increaseStack;
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

        cancel.Value.endCallback(cancel.Value.data.currentStack);
        TimerEnd?.Invoke(cancel.Value.key);
        observeList.Remove(cancel);
    }

    public void Register(int regNum, float time, int maxStack, int increaseStack, Callback startFunc, Callback endFunc)
    {
        Register(regNum, time, startFunc, endFunc, new BuffStack(maxStack, increaseStack));
    }

    // 스택 없는 버프 추가
    public void Register(int regNum, float time, Callback startFunc, Callback endFunc)
    {
        Register(regNum, time, startFunc, endFunc, new BuffStack(1, 1));
    }

    public bool IsStackableBuff(int registeredNumber)
    {
        var observed = FindByRegisterNumber(registeredNumber);
        if(observed == null)
        {
            return false;
        }
        else
        {
            if(observed.Value.data.MaxStack == 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    public int GetBuffStack(int registeredNumber)
    {
        var observed = FindByRegisterNumber(registeredNumber);
        return observed == null ? -1 : observed.Value.data.currentStack;
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
        observedObject.Value.startCallback?.Invoke(observedObject.Value.data.currentStack);
    }

    protected override void ExcuteEndCallback(LinkedListNode<CObserved> observedObject)
    {
        observedObject.Value.endCallback?.Invoke(observedObject.Value.data.currentStack);
    }

    protected override void Remove(LinkedListNode<CObserved> cObservedNode)
    {
        if (cObservedNode.Value.data.MaxStack == 1)
        {
            base.Remove(cObservedNode);
        }
        else
        {
            if (cObservedNode.Value.data.currentStack <= 1)
            {
                base.Remove(cObservedNode);
            }
            else
            {
                ExcuteEndCallback(cObservedNode);
                cObservedNode.Value.data.currentStack--;
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
        if (observed.Value.data.MaxStack >= 1)
        {
            ExcuteEndCallback(observed);
            int addedStack = observed.Value.data.currentStack += updateData.currentStack;
            observed.Value.data.currentStack = 
                addedStack > observed.Value.data.MaxStack 
                ? observed.Value.data.MaxStack
                : addedStack;
            Debug.LogFormat("buff stack : {0}", observed.Value.data.currentStack);
            ExcuteStartCallback(observed);
        }
    }
}
