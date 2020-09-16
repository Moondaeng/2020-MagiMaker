using UnityEngine;
using System.Collections;

public class CPlayerPara : CharacterPara
{
    public string _name;
    public bool _invincibility;
    public bool _invincibilityChecker;
    BoxCollider _col;
    [SerializeField] public Renderer _obj;
    Color _originColor;

    public override void InitPara()
    {
        _col = GetComponent<BoxCollider>();
        _maxHp = 1000;
        _curHp = _maxHp;
        _attackMin = 50;
        _attackMax = 80;
        _defense = 30;
        _eLevel = 0;
        _eType = EElementType.none;
        _isAnotherAction = false;
        _isStunned = false;
        _isDead = false;
        _invincibility = false;
        _originColor = _obj.material.color;
        //CUIManager.instance.UpdatePlayerUI(this);
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

    public void OffInvincibility()
    {
        _invincibility = false;
        StopCoroutine(PowerOverwhelming());
        _obj.material.color = _originColor;
        _col.enabled = true;
    }

    public void OnInvincibility()
    {
        if (_invincibilityChecker)
        {
            _col.enabled = false;
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
}