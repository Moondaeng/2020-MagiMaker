using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 범용 타이머 클래스
 * 링크드 리스트로 타이머 대상들을 관리
 */
public class CTimer : MonoBehaviour
{
    public delegate void Callback();
    public delegate void TimerCallback(int registerdNumber);

    /*
     * 타이머 대상
     * 타이머에 등록한 대상은 시간이 지나면 notify를 부르고 타이머 목록에서 사라진다
     */
    protected class CObserved
    {
        public int registerNumber;
        public float current;
        public float max;
        public Callback notify;
    }

    protected const float _updateTime = 0.1f;
    protected int _updateThreshold;
    protected int _updateCount;

    protected LinkedList<CObserved> observeList = new LinkedList<CObserved>();

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
            // 시간이 남아있다면 시간 돌아가게
            // 시간이 끝났다면 알림 호출 후 삭제
            if(observedObject.Value.current > 0)
            {
                observedObject.Value.current -= Time.fixedDeltaTime * _updateThreshold;
                observedObject = observedObject.Next;
            }
            else
            {
                // 제거
                observedObject.Value.notify();
                TimerEnd(observedObject.Value.registerNumber);
                var remove = observedObject;
                observedObject = observedObject.Next;
                observeList.Remove(remove);
            }
        }
    }

    // 타이머에 등록
    // 번호가 등록되어 있는 경우, 시간에 맞춰 갱신
    public void Register(int regNum, float time, Callback callback)
    {
        CObserved observed = new CObserved()
        {
            registerNumber = regNum,
            max = time,
            current = time,
            notify = callback
        };
        TimerStart(regNum);
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
        }
    }

    // 타이머 갱신
    // 기존 타이머 설정 시간에 맞춰 갱신
    protected void Renew(int regNum)
    {
        var observed = FindByRegisterNumber(regNum);
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
    }

    // 등록된 번호가 있는지 순회 탐색
    protected LinkedListNode<CObserved> FindByRegisterNumber(int regNum)
    {
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
    }
}
