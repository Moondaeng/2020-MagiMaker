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
    protected static readonly int NOT_SELECTED = -1;

    [System.Serializable]
    protected class SkillFormat
    {
        [Tooltip("스킬 사용 시 나가는 오브젝트")]
        public GameObject skillObject;
        [Tooltip("스킬 쿨다운")]
        public float cooldown;
        [Tooltip("스킬 사용 시 취할 모션 번호")]
        public int actionNumber;
        [Tooltip("비축 가능한 스택")]
        public int maxStack;
        [Tooltip("스킬 썸네일")]
        public Sprite thumbnail;

        private int currentStack;

        public SkillFormat()
        {
            maxStack = 1;
            currentStack = 1;
        }

        public bool Use(Vector3 targetPos)
        {
            if (0 >= currentStack)
            {
                return false;
            }
            else
            {
                currentStack--;
                return true;
            }
        }

        public void EndCooldown(int notUsed)
        {
            currentStack++;
            if (currentStack > maxStack)
            {
                currentStack = maxStack;
            }
        }
    }

    [SerializeField]
    protected List<SkillFormat> _skillList = new List<SkillFormat>();

    public int SelectedSkillNum
    {
        get { return _selectedSkillNum; }
        protected set
        {
            _selectedSkillNum = value;
            skillSelectEvent?.Invoke(_selectedSkillNum);
        }
    }

    protected int _selectedSkillNum = 0;

    public SkillSelectEvent skillSelectEvent = new SkillSelectEvent();
    public SkillUseEvent skillUseEvent = new SkillUseEvent();

    protected virtual void Awake()
    {
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
        UseSkillToPosition(SelectedSkillNum, targetPos);
    }

    public virtual void UseSkillToPosition(int skillNum, Vector3 targetPos)
    {
        if (skillNum == NOT_SELECTED)
        {
            Debug.Log("Skill Not Selected");
            return;
        }

        if (_skillList[skillNum].Use(targetPos))
        {
            GetComponent<CSkillTimer>().Register(skillNum, _skillList[skillNum].cooldown, _skillList[skillNum].EndCooldown);
            // CCntl의 행동 코드
            skillUseEvent?.Invoke(skillNum, targetPos);
            CreateSkillObject(_skillList[skillNum].skillObject, targetPos);
        }
        SelectedSkillNum = 0;
    }

    public Sprite GetSkillThumbnail(int skillNumber)
    {
        return _skillList[skillNumber].thumbnail;
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
        var skillObj = CSkillObjectPool.instance.GetAvailableSkillObject(SkillPrefab);
        InitToSkillObject(skillObj, targetPos);
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
        skillObj.GetComponent<CUseEffectHandle>().EnhanceEffectByStat(userStat);
        hitObjectBase.IsInit = true;
    }

    protected virtual void CallSkillUseEvent(int skillIndex, Vector3 targetPos)
    {
        skillUseEvent.Invoke(skillIndex, targetPos);
        CreateSkillObject(_skillList[_selectedSkillNum].skillObject, targetPos);
    }
}