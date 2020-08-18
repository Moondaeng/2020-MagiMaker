using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 모든 캐릭터의 스킬과 스킬 관련 정보를 저장하는 컴포넌트
 * 아이템 추가에 따라 데미지 증폭, 쿨다운 감소 등을 여기서 관리
 */
<<<<<<< HEAD
public class CCharacterSkill : MonoBehaviour
{
    protected List<CSkillFormat> _skillList;

    protected void Awake()
=======
 [RequireComponent(typeof(CSkillTimer))]
public class CCharacterSkill : MonoBehaviour
{
    protected List<CSkillFormat> _skillList;
    protected CProjectileSkill _projectileSkill;
    protected CBuffSkill _buffSkill;

    protected virtual void Awake()
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
    {
        _skillList = new List<CSkillFormat>();
    }

<<<<<<< HEAD
    public void UseSkillToPosition(int skillNumber, Vector3 targetPos)
    {
        _skillList?[skillNumber].Use(targetPos);
=======
    protected virtual void Start()
    {
        _projectileSkill = GameObject.Find("SkillScript").GetComponent<CProjectileSkill>();
        _buffSkill = GameObject.Find("SkillScript").GetComponent<CBuffSkill>();
    }
    
    // 스킬 배우기
    public void LearnSkill(int skillNumber)
    {

    }

    public bool? UseSkillToPosition(int skillNumber, Vector3 targetPos)
    {
        return _skillList?[skillNumber].Use(targetPos);
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
    }
}
