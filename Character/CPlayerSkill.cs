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
        // 콤보 스킬 포맷 등록
        for (int i = 0; i < 42; i++)
        {
            _skillList.Add(new CSkillFormat(i, 0.5f + 0.5f * i, gameObject));
        }

        // 기본 스킬 등록
        _skillList[0].RegisterSkill(_projectileSkill.LightArrow);
        _skillList[1].RegisterSkill(_projectileSkill.Fireball);
        _skillList[2].RegisterSkill(_buffSkill.DefenceUp);
        
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
