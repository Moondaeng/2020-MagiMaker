using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CBuffPara
{
    public float AttackCoef;
    public float DefenceCoef;
    public float MoveSpeedCoef;
    public float AttackSpeedCoef;

    protected readonly CBuffTimer timer;

    public CBuffPara(CBuffTimer myTimer)
    {
        AttackCoef = 1.0f;
        DefenceCoef = 1.0f;
        MoveSpeedCoef = 1.0f;
        AttackSpeedCoef = 1.0f;

        timer = myTimer;
    }

    // 버프 스킬군
    public virtual void BuffAttack(float time, float buffScale)
    {
        timer.Register(CBuffList.AttackBuff, time,
            (int notUsed) => StartBuffAttack(buffScale),
            (int notUsed) => EndBuffAttack(buffScale));
    }

    public void BuffDefence(float time, float buffScale)
    {
        timer.Register(CBuffList.DefenceBuff, time,
            (int notUsed) => StartBuffDefence(buffScale),
            (int notUsed) => EndBuffDefence(buffScale));
    }

    public void BuffAttackStack(float time, float stackBuffScale, int stack)
    {
        timer.Register(CBuffList.AttackBuff, time, stack,
            (int buffStack) => StartBuffAttackStack(stackBuffScale, buffStack),
            (int buffStack) => EndBuffAttackStack(stackBuffScale, buffStack));
    }

    public void BuffDefenceStack(float time, float stackBuffScale, int stack)
    {
        timer.Register(CBuffList.DefenceBuff, time, stack,
            (int buffStack) => StartBuffDefenceStack(stackBuffScale, buffStack),
            (int buffStack) => EndBuffDefenceStack(stackBuffScale, buffStack));
    }

    protected void StartBuffAttack(float buffScale) => AttackCoef *= buffScale;
    protected void EndBuffAttack(float buffScale) => AttackCoef /= buffScale;
    protected void StartBuffDefence(float buffScale) => DefenceCoef *= buffScale;
    protected void EndBuffDefence(float buffScale) => DefenceCoef /= buffScale;

    protected void StartBuffAttackStack(float stackBuffScale, int stack)
    {
        float buffScale = 1.0f + stackBuffScale * stack;
        AttackCoef *= buffScale;
    }
    protected void EndBuffAttackStack(float stackBuffScale, int stack)
    {
        float buffScale = 1.0f + stackBuffScale * stack;
        AttackCoef /= buffScale;
    }
    protected void StartBuffDefenceStack(float stackBuffScale, int stack)
    {
        float buffScale = 1.0f + stackBuffScale * stack;
        DefenceCoef *= buffScale;
    }
    protected void EndBuffDefenceStack(float stackBuffScale, int stack)
    {
        float buffScale = 1.0f + stackBuffScale * stack;
        DefenceCoef /= buffScale;
    }
}
