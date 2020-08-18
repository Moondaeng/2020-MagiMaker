using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 범용 타이머 클래스
 * 링크드 리스트로 타이머 대상들을 관리
 */
<<<<<<< HEAD
public class CTimer : MonoBehaviour
{
    public delegate void Callback();
=======
public class CTimer<T, U> : MonoBehaviour
{
    public delegate void Callback(U callbackData = default);
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
    public delegate void TimerCallback(int registerdNumber);

    /*
     * 타이머 대상
     * 타이머에 등록한 대상은 시간이 지나면 notify를 부르고 타이머 목록에서 사라진다
     */
    protected class CObserved
    {
<<<<<<< HEAD
        public int registerNumber;
        public float current;
        public float max;
        public Callback notify;
=======
        public int key;
        public float current;
        public float max;
        public T data;
        public Callback startCallback;
        public Callback endCallback;
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
    }

    protected const float _updateTime = 0.1f;
    protected int _updateThreshold;
    protected int _updateCount;
<<<<<<< HEAD

=======
    
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
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
<<<<<<< HEAD
            // 시간이 남아있다면 시간 돌아가게
            // 시간이 끝났다면 알림 호출 후 삭제
=======
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
            if(observedObject.Value.current > 0)
            {
                observedObject.Value.current -= Time.fixedDeltaTime * _updateThreshold;
                observedObject = observedObject.Next;
            }
            else
            {
<<<<<<< HEAD
                // 제거
                observedObject.Value.notify();
                TimerEnd?.Invoke(observedObject.Value.registerNumber);
                var remove = observedObject;
                observedObject = observedObject.Next;
                observeList.Remove(remove);
=======
                var remove = observedObject;
                observedObject = observedObject.Next;
                Remove(remove);
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
            }
        }
    }

    // 타이머에 등록
    // 번호가 등록되어 있는 경우, 시간에 맞춰 갱신
    // 콜백이 다른 경우 문제가 생길 수 있음. 버프 같은 경우는 조심해야 함
<<<<<<< HEAD
    public void Register(int regNum, float time, Callback callback)
=======
    protected void Register(int regNum, float time, Callback startFunc, Callback endFunc, T initData)
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
    {
        // 이미 있는 번호의 경우 갱신만 하고 생성은 하지 않음
        if(FindByRegisterNumber(regNum) != null)
        {
<<<<<<< HEAD
            Renew(regNum, time);
=======
            Renew(regNum, time, initData);
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
            return;
        }

        CObserved observed = new CObserved()
        {
<<<<<<< HEAD
            registerNumber = regNum,
            max = time,
            current = time,
            notify = callback
        };
        TimerStart?.Invoke(regNum);
        observeList.AddLast(observed);
    }

    public float GetCurrentCooldown(int registeredNumber)
    {
        var observed = FindByRegisterNumber(registeredNumber);
        if (observed == null)
        {
            return 0;
        }
        else
        {
            return observed.Value.current;
        }
    }

    public float GetMaxCooldown(int registeredNumber)
    {
        var observed = FindByRegisterNumber(registeredNumber);
        if (observed == null)
        {
            return -1;
        }
        else
        {
            return observed.Value.max;
=======
            key = regNum,
            max = time,
            current = time,
            data = initData,
            startCallback = startFunc,
            endCallback = endFunc
        };
        TimerStart?.Invoke(regNum);
        var observeNode = observeList.AddLast(observed);
        ExcuteStartCallback(observeNode);
    }

    protected void Register(int regNum, float time, Callback endFunc, T initData)
    {
        Register(regNum, time, null, endFunc, initData);
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
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
        }
    }

    // 타이머 갱신
    // 기존 타이머 설정 시간에 맞춰 갱신
    protected void Renew(int regNum)
    {
        var observed = FindByRegisterNumber(regNum);
<<<<<<< HEAD
        if (observed == null)
        {
            return;
        }
        // 시간 갱신
        else
        {
            observed.Value.current = observed.Value.max;
        }
    }

    // 타이머 갱신
    // 새로운 타이머 설정 시간에 맞춰 갱신
    protected void Renew(int regNum, float time)
    {
        var observed = FindByRegisterNumber(regNum);
        if (observed == null)
        {
            return;
        }
        // 시간 갱신
        else
        {
            observed.Value.current = time;
            observed.Value.max = time;
        }
=======
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

    protected virtual void Remove(LinkedListNode<CObserved> cObservedNode)
    {
        ExcuteEndCallback(cObservedNode);
        TimerEnd?.Invoke(cObservedNode.Value.key);
        observeList.Remove(cObservedNode);
    }
    
    protected virtual void ExcuteStartCallback(LinkedListNode<CObserved> cobservedNode)
    {
        cobservedNode.Value.startCallback?.Invoke();
    }

    protected virtual void ExcuteEndCallback(LinkedListNode<CObserved> cobservedNode)
    {
        cobservedNode.Value.endCallback?.Invoke();
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
    }

    // 등록된 번호가 있는지 순회 탐색
    protected LinkedListNode<CObserved> FindByRegisterNumber(int regNum)
    {
<<<<<<< HEAD
        //for(var f = observeList.First; f != null; f = f.Next)
        //{
        //    if (f.Value.registerNumber == regNum) return f;
        //}
        var find = observeList.First;
        while(find != null)
        {
            // 탐색
            if(find.Value.registerNumber == regNum)
            {
                return find;
            }
            find = find.Next;
        }
        return find;
=======
        for (var find = observeList.First; find != null; find = find.Next)
            if (find.Value.key == regNum) return find;
        return null;
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
    }
}
