﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 스킬을 등록해서 실행하는 포맷
public class CSkillFacade
{
    private static CLogComponent _logger = new CLogComponent(ELogType.Skill);
    public delegate void RetentionSkill(GameObject user, Vector3 targetPos);

    public CSkillTimer _timer;

    private GameObject _userObject;
    private RetentionSkill _usingSkill;
    public bool _isCooldown;
    private int _timerRegisterNumber;
    private float _cooldown;

    public CSkillFacade(int registerNumber, float cooldown, GameObject user)
    {
        _userObject = user;
        _timer = _userObject.GetComponent<CSkillTimer>();
        _isCooldown = false;
        _timerRegisterNumber = registerNumber;
        _cooldown = cooldown;
    }

    // 인터페이스 변경 필요 - Unit이 모두 Timer를 가지므로 자기 자신의 Timer를 추적하게 만들어야 함
    public CSkillFacade(int registerNumber, float cooldown)
    {
        _timer = GameObject.Find("SkillScript").GetComponent<CSkillTimer>();
        _userObject = GameObject.Find("Player");
        _isCooldown = false;
        _timerRegisterNumber = registerNumber;
        _cooldown = cooldown;
    }

    public void RegisterSkill(RetentionSkill register)
    {
        _usingSkill = register;
    }

    public void Use(Vector3 targetPos)
    {
        if(_isCooldown)
        {
            // 실행 거부
            _logger.Log("Skill is Cooldown!");
        }
        else
        {
            _isCooldown = true;
            // 스킬 실행
            _usingSkill(_userObject, targetPos);
            _timer.Register(_timerRegisterNumber, _cooldown, EndCooldown);
        }
    }

    private void EndCooldown() =>_isCooldown = false;
}
