using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SkillSelectEvent : UnityEvent<int> { }
public class SkillUseEvent : UnityEvent<int, Vector3> { }

/*
 * 모든 캐릭터의 스킬과 스킬 관련 정보를 저장하는 컴포넌트
 * 아이템 추가에 따라 데미지 증폭, 쿨다운 감소 등을 여기서 관리
 */
[RequireComponent(typeof(CSkillTimer))]
public class CCharacterSkill : MonoBehaviour
{
    [SerializeField]
    protected List<CSkillFormat> _skillList;
    protected CProjectileSkill _projectileSkill;
    protected CBuffSkill _buffSkill;

    protected int _selectedSkillNum;

    public SkillSelectEvent skillSelectEvent = new SkillSelectEvent();
    public SkillUseEvent skillUseEvent = new SkillUseEvent();

    protected virtual void Awake()
    {
        _skillList = new List<CSkillFormat>();
        _selectedSkillNum = 0;

        for (int i = 0; i < _skillList.Count; i++)
        {
            _skillList[i].InitRegisteredNumber(i);
            _skillList[i].InitSkillUser(gameObject);
            _skillList[i].SetSkillUseEvent(i, skillUseEvent);
        }
    }

    protected virtual void Start()
    {
        _projectileSkill = GameObject.Find("SkillScript").GetComponent<CProjectileSkill>();
        _buffSkill = GameObject.Find("SkillScript").GetComponent<CBuffSkill>();
    }

    /// <summary>
    /// 스킬 선택
    /// </summary>
    /// <param name="index"></param>
    public virtual void SkillSelect(int index)
    {
        if(index < 0 || index >= _skillList.Count)
        {
            Debug.Log("Skill Select Error");
            return;
        }

        _selectedSkillNum = index + 1;
        skillSelectEvent.Invoke(_selectedSkillNum);
    }

    public virtual void UseSkillToPosition(Vector3 targetPos)
    {
        if (_selectedSkillNum == -1)
        {
            Debug.Log("Skill Not Selected");
            return;
        }

        _skillList[_selectedSkillNum].Use(targetPos);
        _selectedSkillNum = 0;
    }

    protected virtual void CallSkillUseEvent(int skillIndex, Vector3 targetPos)
    {
        skillUseEvent.Invoke(skillIndex, targetPos);
    }
}
