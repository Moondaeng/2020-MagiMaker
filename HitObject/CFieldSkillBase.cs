using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CFieldSkillBase : CHitObjectBase
{
    [SerializeField] private float _actionPeriod;
    private static readonly int EXPECT_FRAMERATE = 60;
    private int operateStack = 0;

    protected override void Awake()
    {
        base.Awake();
        // 0으로 초기화되서 무한 호출되는 경우 방지
        if (_actionPeriod == 0)
        {
            _actionPeriod = 1f;
        }
    }

    private void FixedUpdate()
    {
        operateStack = (operateStack + 1) % GetFramePerActionPeriod();
    }

    private int GetFramePerActionPeriod()
    {
        return (int)(EXPECT_FRAMERATE * _actionPeriod);
    }

    private void OnTriggerStay(Collider other)
    {
        if (operateStack == 0 && !IsTriggeredRecently(other))
        {
            GetUseEffect(other);
        }
    }

    // 데미지 2번 줄 필요 없음
    protected override void OnTriggerEnter(Collider other)
    {
        return;
    }
}
