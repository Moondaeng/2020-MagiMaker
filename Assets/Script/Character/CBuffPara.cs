using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBuffPara
{
    public float attackCoef;
    public float DefenceCoef;
    public float MoveSpeedCoef;
    public float AttackSpeedCoef;

    public CSkillTimer timer;

    public CBuffPara(CSkillTimer myTimer)
    {
        attackCoef = 1.0f;
        DefenceCoef = 1.0f;
        MoveSpeedCoef = 1.0f;
        AttackSpeedCoef = 1.0f;

        timer = myTimer;
    }

    // 버프 스킬군
    // 버프가 걸린 상태에서 또 걸리면 중첩되지 않도록 캔슬해야함
    public virtual void BuffAttack(float time, float buffScale)
    {
        StartBuffAttack(buffScale);
        // 버프 효과 끝내기 수치 넣을 땐 필요에 따라 커링을 사용
        timer.Register(CBuffList.AttackBuff, time, () => EndBuffAttack(buffScale));
    }

    public void BuffDefence(float time, float buffScale)
    {
        StartBuffDefence(buffScale);
        timer.Register(CBuffList.DefenceBuff, time, () => EndBuffDefence(buffScale));
    }
    
    protected void StartBuffAttack(float buffScale) => attackCoef *= buffScale;
    protected void EndBuffAttack(float buffScale) => attackCoef /= buffScale;
    protected void StartBuffDefence(float buffScale) => DefenceCoef *= buffScale;
    protected void EndBuffDefence(float buffScale) => DefenceCoef /= buffScale;
}
