using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 범용 타이머 클래스
 * 링크드 리스트로 타이머 대상들을 관리
 */
public class CTimer<T> : MonoBehaviour
{
    public delegate void Callback();
    public delegate void TimerCallback(int registerdNumber);

    /*
     * 타이머 대상
     * 타이머에 등록한 대상은 시간이 지나면 notify를 부르고 타이머 목록에서 사라진다
     */
    protected class CObserved
    {
        public int key;
        public float current;
        public float max;
        public T data;
        public Callback notify;
    }

    protected const float _updateTime = 0.1f;
    protected int _updateThreshold;
    protected int _updateCount;
    
    protected LinkedList<CObserved> observeList = new LinkedList<CObserved>();

    // 타이머에 추가적으로 요청할게 있으면 사용하는 대리자
    // ex) 타이머 UI 그리기
    public TimerCallback TimerStart;
    public TimerCallback TimerEnd;

    protected void Awake()
    {
        _updateThreshold = (int)(_updateTime / Time.fixedDeltaTime);
        _updateCount = 0;
    }

    protected void FixedUpdate()
    {
        _updateCount++;

        // 업데이트 작동 횟수 조절
        if (_updateCount % _updateThreshold != 1)
        {
            return;
        }

        var observedObject = observeList.First;
        // 모든 관찰 대상 상태 갱신
        while(observedObject != null)
        {
            if(observedObject.Value.current > 0)
            {
                observedObject.Value.current -= Time.fixedDeltaTime * _updateThreshold;
                observedObject = observedObject.Next;
            }
            else
            {
                observedObject.Value.notify();
                TimerEnd?.Invoke(observedObject.Value.key);
                var remove = observedObject;
                observedObject = observedObject.Next;
                Remove(remove);
            }
        }
    }

    // 타이머에 등록
    // 번호가 등록되어 있는 경우, 시간에 맞춰 갱신
    // 콜백이 다른 경우 문제가 생길 수 있음. 버프 같은 경우는 조심해야 함
    protected void Register(int regNum, float time, Callback callback, T initData)
    {
        // 이미 있는 번호의 경우 갱신만 하고 생성은 하지 않음
        if(FindByRegisterNumber(regNum) != null)
        {
            Renew(regNum, time, initData);
            return;
        }

        CObserved observed = new CObserved()
        {
            key = regNum,
            max = time,
            current = time,
            data = initData,
            notify = callback
        };
        TimerStart?.Invoke(regNum);
        observeList.AddLast(observed);
    }

    // 타이머 갱신
    // 새로운 타이머 설정 시간에 맞춰 갱신
    protected virtual void Renew(int regNum, float updataTime, T updateData = default)
    {
        var observed = FindByRegisterNumber(regNum);
        if (observed != null)
        {
            observed.Value.current = updataTime;
            observed.Value.max = updataTime;
        }
    }

    // 타이머 갱신
    // 기존 타이머 설정 시간에 맞춰 갱신
    protected void Renew(int regNum)
    {
        var observed = FindByRegisterNumber(regNum);
        if (observed != null) observed.Value.current = observed.Value.max;
    }

    public float GetCurrentCooldown(int registeredNumber)
    {
        var observed = FindByRegisterNumber(registeredNumber);
        return observed == null ? 0 : observed.Value.current;
    }

    public float GetMaxCooldown(int registeredNumber)
    {
        var observed = FindByRegisterNumber(registeredNumber);
        return observed == null ? -1 : observed.Value.max;
    }

    // 등록된 번호가 있는지 순회 탐색
    protected LinkedListNode<CObserved> FindByRegisterNumber(int regNum)
    {
        for (var find = observeList.First; find != null; find = find.Next)
            if (find.Value.key == regNum) return find;
        return null;
        //var find = observeList.First;
        //while(find != null)
        //{
        //    // 탐색
        //    if(find.Value.registerNumber == regNum)
        //    {
        //        return find;
        //    }
        //    find = find.Next;
        //}
        //return find;
    }

    protected void Remove(LinkedListNode<CObserved> cObservedNode)
    {
        observeList.Remove(cObservedNode);
    }
}
