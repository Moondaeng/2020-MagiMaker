using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSkillFacade
{
    public CSkillTimer _timer;
    private CSkillUIManager _skillUIManager;

    public bool _isCooldown;
    private int _timerRegisterNumber;

    public CSkillFacade(float cooldown)
    {
        _skillUIManager = GameObject.Find("SkillScript").GetComponent<CSkillUIManager>();
        _timer = GameObject.Find("SkillScript").GetComponent<CSkillTimer>();
        _isCooldown = false;
        _timerRegisterNumber = _timer.RegisterSkill(cooldown, EndCooldown);
    }

    // 스킬 등록
    public CSkillFacade(float cooldown, CSkillUIManager.EUIName eUIName)
    {
        _skillUIManager = GameObject.Find("SkillScript").GetComponent<CSkillUIManager>();
        _timer = GameObject.Find("SkillScript").GetComponent<CSkillTimer>();
        _isCooldown = false;
        _timerRegisterNumber = _timer.RegisterSkill(cooldown, EndCooldown);
        Debug.LogFormat("register number = {0}", _timerRegisterNumber);
        _skillUIManager.Preempt(eUIName, _timerRegisterNumber);
    }

    public void Use()
    {
        if(_isCooldown)
        {
            // 실행 거부
            Debug.Log("Skill is Cooldown!");
        }
        else
        {
            _isCooldown = true;
            // 스킬 실행
            _timer.TimerStart(_timerRegisterNumber);
        }
    }

    private void EndCooldown()
    {
        _isCooldown = false;
    }
}
