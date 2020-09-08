public class CPlayerPara : CharacterPara
{
    public string _name;
    public int _curExp { get; set; }
    public int _expToNextLevel { get; set; }
    public int _money { get; set; }

    public override void InitPara()
    {
        _maxHp = 1000;
        _curHp = _maxHp;
        _attackMin = 50;
        _attackMax = 80;
        _defense = 30;
        _eLevel = 0;
        _eType = EElementType.none;
        _money = 0;
        _isAnotherAction = false;
        _isStunned = false;
        _isDead = false;
        //CUIManager.instance.UpdatePlayerUI(this);
    }

    protected override void UpdateAfterReceiveAttack()
    {
        base.UpdateAfterReceiveAttack();
        //CUIManager.instance.UpdatePlayerUI(this);
    }
}