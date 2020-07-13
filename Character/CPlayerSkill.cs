using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 플레이어 캐릭터의 스킬을 관리하는 클래스
 * 
 */
public class CPlayerSkill : CCharacterSkill
{
    protected List<CSkillFormat> _comboSkillList;

    protected override void Awake()
    {
        base.Awake();
        _comboSkillList = new List<CSkillFormat>();
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        // 스킬은 나중에 플레이어 관련 클래스에서 처리하도록 변경
        // 기본 스킬 포맷 추가
        _skillList.Add(new CSkillFormat(0, 1.0f, gameObject));
        _skillList.Add(new CSkillFormat(1, 7.0f, gameObject));
        _skillList.Add(new CSkillFormat(2, 9.0f, gameObject));
        _skillList.Add(new CSkillFormat(3, 10.0f, gameObject));

        // 기본 스킬 등록
        _skillList[0].RegisterSkill(_projectileSkill.Fireball);
        _skillList[1].RegisterSkill(_buffSkill.DefenceUp);
        
        // 콤보 스킬 포맷 등록
        for (int i = 0; i < 36; i++)
        {
            _comboSkillList.Add(new CSkillFormat(4 + i, 10.5f + 1.0f * i, gameObject));
        }
    }

    public bool? UseComboSkillToPosition(int skillNumber, Vector3 targetPos)
    {
        return _comboSkillList?[skillNumber].Use(targetPos);
    }

    // 아이템에 의한 스킬 대체
    public void ReplaceSkill(int skillNumber)
    {

    }
}
