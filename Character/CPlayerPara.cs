using UnityEngine;
using System.Collections;

public class CPlayerPara : CharacterPara
{
    public string _name;
    public bool _invincibility;
    public bool _invincibilityChecker;
    public Animator _myAnimator;
    BoxCollider _col;
    [SerializeField] public Renderer _obj;
    Color _originColor;

    [SerializeField]
    public CInventory Inventory;

    // ((캐릭터 공격력 * 공격력 증가) + 장비 공격력) * 버프로 올라가는 공격력 %
    public override int TotalAttackMin
    {
        get
        {
            return (int)((_attackMin + Inventory.EquipAtkIncreaseSize)
              * _buffCoef[(int)EBuffAbility.Attack] * _debuffCoef[(int)EBuffAbility.Attack]);
        }
    }
    public override int TotalAttackMax
    {
        get
        {
            return (int)((_attackMax + Inventory.EquipAtkIncreaseSize)
              * _buffCoef[(int)EBuffAbility.Attack] * _debuffCoef[(int)EBuffAbility.Attack]);
        }
    }
    public override int TotalDefenece
    {
        get
        {
            return (int)(_defense + Inventory.DefIncreaseSize
              * _buffCoef[(int)EBuffAbility.Defence] * _debuffCoef[(int)EBuffAbility.Defence]);
        }
    }
    public override int TotalMaxHp
    {
        get { return (int)(_maxHp + Inventory.MaxHpIncreaseSize); }
    }
    public int TotalHpRegen
    {
        get { return (int)(Inventory.HpRegenIncreaseSize); }
    }

    public override int CurrentHp
    {
        protected set
        {
            base.CurrentHp = value;
            _myAnimator.SetInteger("Hp", CurrentHp);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        Inventory = new CInventory(gameObject);
    }

    public override void InitPara()
    {
        _col = GetComponent<BoxCollider>();
        _maxHp = 1000;
        _curHp = _maxHp;
        _attackMin = 50;
        _attackMax = 80;
        _defense = 30;
        _isAnotherAction = false;
        _isStunned = false;
        _isDead = false;
        _invincibility = false;
        //_originColor = _obj.material.color;
        _myAnimator = GetComponent<Animator>();
        _myAnimator.SetInteger("Hp", _curHp);
    }

    protected override void UpdateAfterReceiveAttack()
    {
        if (_invincibility) return;
        print(name + "'s HP: " + _curHp);

        if (_curHp <= 0)
        {
            _curHp = 0;
            _isDead = true;
            deadEvent.Invoke();
        }
    }

    #region 무적판정
    public void OffInvincibility()
    {
        _invincibility = false;
        StopCoroutine(PowerOverwhelming());
        _obj.material.color = _originColor;
        gameObject.layer = LayerMask.NameToLayer("DeadBody");
    }

    public void OnInvincibility()
    {
        if (_invincibilityChecker)
        {
            _invincibilityChecker = false;
            StartCoroutine(PowerOverwhelming());
        }
    }

    IEnumerator PowerOverwhelming()
    {
        while (_invincibility)
        {
            float flicker = Mathf.Abs(Mathf.Sin(Time.time * 10));
            _obj.material.color = _originColor * flicker;
            yield return null;
        }
    }

    #endregion
}