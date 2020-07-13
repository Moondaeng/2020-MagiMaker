using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 스킬을 등록해서 실행하는 포맷
public class CSkillFormat
{
    private static CLogComponent _logger = new CLogComponent(ELogType.Skill);
    public delegate void RetentionSkill(GameObject user, Vector3 targetPos);

    private GameObject _userObject;
    private CSkillTimer _timer;
    private RetentionSkill _usingSkill;
    private bool _isCooldown;
    private int _timerRegisterNumber;
    private float _cooldown;

    public CSkillFormat(int registerNumber, float cooldown, GameObject user)
    {
        _userObject = user;
        _timer = _userObject.GetComponent<CSkillTimer>();
        _isCooldown = false;
        _timerRegisterNumber = registerNumber;
        _cooldown = cooldown;
    }

    public void RegisterSkill(RetentionSkill register)
    {
        _usingSkill = register;
    }

    public bool Use(Vector3 targetPos)
    {
        if(_isCooldown)
        {
            // 실행 거부
            _logger.Log("Skill is Cooldown!");
            return false;
        }
        else
        {
            _isCooldown = true;
            // 스킬 실행
            _usingSkill?.Invoke(_userObject, targetPos);
            _timer.Register(_timerRegisterNumber, _cooldown, EndCooldown);
            return true;
        }
    }

    private void EndCooldown() =>_isCooldown = false;
}
