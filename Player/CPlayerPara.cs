using UnityEngine;
using System.Collections;

public class CPlayerPara : CharacterPara
{
    public string _name;
    public bool _invincibility;
    public bool _invincibilityChecker;
    public Animator _myAnimator;
    public float _runAnimationMultiply = 1f;
    public Renderer _obj;
    Color _originColor;

    [SerializeField]
    public CInventory Inventory;

    //debug용
    public static CPlayerPara instance = null;

    // ((캐릭터 공격력 * 공격력 증가) + 장비 공격력) * 버프로 올라가는 공격력 %
    public override int TotalAttackMin
    {
        get
        {
            Debug.Log("AttackMin = " + (int)((_attackMin + Inventory.EquipAtkIncreaseSize + _attackMin * Inventory.AtkPercentIncreaseSize / 100)
              * _buffCoef[(int)EBuffAbility.Attack] * _debuffCoef[(int)EBuffAbility.Attack]));

            return (int)((_attackMin + Inventory.EquipAtkIncreaseSize + _attackMin * Inventory.AtkPercentIncreaseSize / 100)
              * _buffCoef[(int)EBuffAbility.Attack] * _debuffCoef[(int)EBuffAbility.Attack]);
        }
    }
    public override int TotalAttackMax
    {
        get
        {
            return (int)((_attackMax + Inventory.EquipAtkIncreaseSize + _attackMax * Inventory.AtkPercentIncreaseSize / 100)
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

<<<<<<< HEAD:Character/CPlayerPara.cs
    public override void DamegedRegardDefence(int enemyAttack)
    {
        int damage = enemyAttack * 1000 / (950 + 10 * TotalDefenece);
        float damageF = damage * ((100 + Inventory.ReducedDmgReceived)/100);
        DamagedDisregardDefence((int)damageF);
=======
    public override int CurrentHp
    {
        protected set
        {
            base.CurrentHp = value;
            _myAnimator.SetInteger("Hp", CurrentHp);
        }
>>>>>>> origin/ZeroFe:Player/CPlayerPara.cs
    }

    protected override void Awake()
    {
        base.Awake();
        Inventory = new CInventory(gameObject);
<<<<<<< HEAD:Character/CPlayerPara.cs


        //debug용
        if (instance == null)
            instance = this;
=======
        
>>>>>>> origin/ZeroFe:Player/CPlayerPara.cs
    }

    public override void InitPara()
    {
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

    public override void DamegedRegardDefence(int enemyAttack)
    {
        int damage = enemyAttack * 1000 / (950 + 10 * TotalDefenece);
        float damageF = damage * ((100 + Inventory.ReducedDmgReceived) / 100);
        DamagedDisregardDefence((int)damageF);
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

    

    private void Update()
    {
        _myAnimator.SetFloat("RunMulti", _runAnimationMultiply);
    }

    #endregion
}