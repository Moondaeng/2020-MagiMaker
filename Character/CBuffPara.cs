/*
 * 능력치 버프 및 디버프 관리 클래스(지속 힐 / 데미지는 미포함)
 */
[System.Serializable]
public class CBuffPara
{
    public enum EBuffType
    {
        Attack,
        Defence,
        MoveSpeed,
        AttackSpeed
    }
    
    public float AttackCoef { get; private set; }
    public float DefenceCoef { get; private set; }
    public float MoveSpeedCoef { get; private set; }
    public float AttackSpeedCoef { get; private set; }

    public float AttackDebuffCoef { get; private set; }
    public float DefenceDebuffCoef { get; private set; }
    public float MoveSpeedDebuffCoef { get; private set; }
    public float AttackSpeedDebuffCoef { get; private set; }

    protected readonly CBuffTimer timer;

    public CBuffPara(CBuffTimer myTimer)
    {
        AttackCoef = 1.0f;
        DefenceCoef = 1.0f;
        MoveSpeedCoef = 1.0f;
        AttackSpeedCoef = 1.0f;

        AttackDebuffCoef = 1.0f;
        DefenceDebuffCoef = 1.0f;
        MoveSpeedDebuffCoef = 1.0f;
        AttackSpeedDebuffCoef = 1.0f;

        timer = myTimer;
    }

    #region command buff
    public void Buff(int BuffID, float time, float buffScale)
    {
        switch(BuffID)
        {
            case CBuffList.AttackBuff:
                timer.Register(CBuffList.AttackBuff, time,
                    (int notUsed) => StartBuffAttack(buffScale),
                    (int notUsed) => EndBuffAttack(buffScale));
                break;
            case CBuffList.DefenceBuff:
                timer.Register(CBuffList.DefenceBuff, time,
                    (int notUsed) => StartBuffDefence(buffScale),
                    (int notUsed) => EndBuffDefence(buffScale));
                break;
            default:
                break;
        }
    }

    public void BuffAttack(float time, float buffScale)
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
    #endregion
    #region implement buff
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
    #endregion
}
