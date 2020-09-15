using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 스킬을 등록해서 실행하는 포맷
public class CSkillFormat
{
    public delegate void RetentionSkill(GameObject user, Vector3 targetPos);

    private GameObject _userObject;
    private CSkillTimer _timer;
    private RetentionSkill _usingSkill;

    private int _currentStack;
    public int MaxStack { private set; get; }

    private int _timerRegisterNumber;
    private float _cooldown;

    public CSkillFormat(int registerNumber, float cooldown, GameObject user)
    {
        MaxStack = 1;
        _currentStack = 1;
        _userObject = user;
        _timer = _userObject.GetComponent<CSkillTimer>();
        _timerRegisterNumber = registerNumber;
        _cooldown = cooldown;
    }

    public CSkillFormat(int maxStack, int registerNumber, float cooldown, GameObject user)
    {
        MaxStack = maxStack;
        _currentStack = MaxStack;
        _userObject = user;
        _timer = _userObject.GetComponent<CSkillTimer>();
        _timerRegisterNumber = registerNumber;
        _cooldown = cooldown;
    }

    public void RegisterSkill(RetentionSkill register)
    {
        _usingSkill = register;
    }

    public bool Use(Vector3 targetPos)
    {
        if (0 >= _currentStack)
        {
            // 실행 거부
            Debug.Log($"Skill {_timerRegisterNumber} is Cooldown");
            return false;
        }
        else
        {
            _currentStack--;
            // 스킬 실행
            _usingSkill?.Invoke(_userObject, targetPos);
            _timer.Register(_timerRegisterNumber, _cooldown, EndCooldown);
            return true;
        }
    }

    private void EndCooldown(int notUsed)
    {
        _currentStack++;
        if(_currentStack > MaxStack)
        {
            _currentStack = MaxStack;
        }
    }
}
