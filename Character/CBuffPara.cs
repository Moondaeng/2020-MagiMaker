using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBuffPara
{
    public float AttackCoef;
    public float DefenceCoef;
    public float MoveSpeedCoef;
    public float AttackSpeedCoef;

    public CBuffTimer timer;

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
        StartBuffAttack(buffScale);
        timer.Register(CBuffList.AttackBuff, time, () => EndBuffAttack(buffScale));
    }

    public void BuffDefence(float time, float buffScale)
    {
        StartBuffDefence(buffScale);
        timer.Register(CBuffList.DefenceBuff, time, () => EndBuffDefence(buffScale));
    }

    public void BuffAttackStack(float time, float stackBuffScale, int stack)
    {
        StartBuffAttackStack(time, stackBuffScale, stack);
    }
    
    protected void StartBuffAttack(float buffScale) => AttackCoef *= buffScale;
    protected void EndBuffAttack(float buffScale) => AttackCoef /= buffScale;
    protected void StartBuffDefence(float buffScale) => DefenceCoef *= buffScale;
    protected void EndBuffDefence(float buffScale) => DefenceCoef /= buffScale;

    protected void StartBuffAttackStack(float time, float stackBuffScale, int stack)
    {
        float buffScale = 1.0f + stackBuffScale * stack;
        AttackCoef *= stackBuffScale;
        timer.Register(CBuffList.DefenceBuff, time, () => EndBuffAttackStack(time, stackBuffScale, stack), stack);
    }
    protected void EndBuffAttackStack(float time, float stackBuffScale, int stack)
    {
        float buffScale = 1.0f + stackBuffScale * stack;
        AttackCoef /= stackBuffScale;

        if (stack == 0)
        {
            return;
        }

        StartBuffAttackStack(time, stackBuffScale, stack-1);
    }
}
