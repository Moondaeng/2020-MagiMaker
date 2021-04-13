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

    public int SelectedSkillNum
    {
        get { return _selectedSkillNum; }
        protected set
        {
            _selectedSkillNum = value;
            skillSelectEvent?.Invoke(_selectedSkillNum);
        }
    }

    protected int _selectedSkillNum;

    public SkillSelectEvent skillSelectEvent = new SkillSelectEvent();
    public SkillUseEvent skillUseEvent = new SkillUseEvent();

    protected virtual void Awake()
    {
        _selectedSkillNum = 0;

        if (_skillList != null)
        {
            for (int i = 0; i < _skillList.Count; i++)
            {
                _skillList[i].InitRegisteredNumber(i);
                _skillList[i].InitSkillUser(gameObject);
            }
        }
        else
        {
            _skillList = new List<CSkillFormat>();
        }
    }

    /// <summary>
    /// 스킬 선택
    /// </summary>
    /// <param name="index"></param>
    public virtual void SkillSelect(int index)
    {
        if (index < 0 || index >= _skillList.Count)
        {
            Debug.Log("Skill Select Error");
            return;
        }

        SelectedSkillNum = index + 1;
    }

    public virtual void UseSkillToPosition(Vector3 targetPos)
    {
        if (_selectedSkillNum == -1)
        {
            Debug.Log("Skill Not Selected");
            return;
        }

        if (_skillList[_selectedSkillNum].Use(targetPos))
        {
            // CCntl의 행동 코드
            skillUseEvent?.Invoke(_selectedSkillNum, targetPos);
            CreateSkillObject(_skillList[_selectedSkillNum].skillObject, targetPos);
        }
        SelectedSkillNum = 0;
    }

    public virtual void UseSkillToPosition(int skillNum, Vector3 targetPos)
    {
        if (skillNum == -1)
        {
            Debug.Log("Skill Not Selected");
            return;
        }

        if (_skillList[skillNum].Use(targetPos))
        {
            // CCntl의 행동 코드
            skillUseEvent?.Invoke(skillNum, targetPos);
            CreateSkillObject(_skillList[skillNum].skillObject, targetPos);
        }
        SelectedSkillNum = 0;
    }

    // 스킬 오브젝트 생성
    protected void CreateSkillObject(GameObject SkillPrefab, Vector3 targetPos)
    {
        if (SkillPrefab == null)
        {
            Debug.Log("Skill Object not setting");
            return;
        }

        // 오브젝트 생성
        //var skillObj = CSkillObjectPool.instance.GetAvailableSkillObject(SkillPrefab);
        //InitToSkillObject(skillObj, targetPos);
    }

    /// <summary>
    /// 대상에 맞는 스킬 레이어로 변환한다
    /// </summary>
    /// <returns></returns>
    protected virtual int TranslateLayerCharacterToSkill(bool isAttack)
    {
        return isAttack == true ? LayerMask.NameToLayer("PlayerSkill") : LayerMask.NameToLayer("MonsterSkill");
    }

    protected void InitToSkillObject(GameObject skillObj, Vector3 targetPos)
    {
        skillObj.SetActive(true);
        skillObj.tag = gameObject.tag;

        var hitObjectBase = skillObj.GetComponent<CHitObjectBase>();
        if (hitObjectBase is CProjectileBase)
        {
            var userPos = transform.position;
            var objectivePos = targetPos - userPos;
            Quaternion lookRotation = Quaternion.LookRotation(objectivePos);
            skillObj.transform.position = userPos + Vector3.up;
            skillObj.transform.rotation = lookRotation;
            hitObjectBase.SetObjectLayer(TranslateLayerCharacterToSkill(true));
        }
        else if (hitObjectBase is CBuffBase)
        {
            hitObjectBase.SetObjectLayer(TranslateLayerCharacterToSkill(false));
            skillObj.transform.position = targetPos;
        }
        else if (hitObjectBase is CFieldSkillBase)
        {
            hitObjectBase.SetObjectLayer(TranslateLayerCharacterToSkill(true));
            skillObj.transform.position = targetPos;
        }
        else if (hitObjectBase is CCornSkillBase)
        {
            var userPos = transform.position;
            var objectivePos = targetPos - userPos;
            Quaternion lookRotation = Quaternion.LookRotation(objectivePos);
            skillObj.transform.position = userPos + Vector3.up;
            skillObj.transform.rotation = lookRotation;
            hitObjectBase.SetObjectLayer(TranslateLayerCharacterToSkill(true));
            skillObj.transform.SetParent(transform);
        }
        // 유저 스탯에 비례해 스킬 발사
        var userStat = GetComponent<CharacterPara>();
        //skillObj.GetComponent<CUseEffectHandle>().EnhanceEffectByStat(userStat);
        hitObjectBase.IsInit = true;
    }

    protected virtual void CallSkillUseEvent(int skillIndex, Vector3 targetPos)
    {
        skillUseEvent.Invoke(skillIndex, targetPos);
        CreateSkillObject(_skillList[_selectedSkillNum].skillObject, targetPos);
    }
}