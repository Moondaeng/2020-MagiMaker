using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 투사체형 발사 스킬군
// 스킬 제작 경과에 따라 다른 스킬군과 코드 합체할 수도 있음
public class CGolemSkill : MonoBehaviour
{
    public GameObject _golemNeedleObject;
    private static CLogComponent _log;

    private void Awake()
    {
        _log = new CLogComponent(ELogType.Skill);
    }
    
    // 스킬 포맷에 넣을 스킬
    public void StoneNeedle(GameObject user, Vector3 targetPos)
    {
        MakeSkillPrototype(user, targetPos, _golemNeedleObject);
    }

    // 스킬 원형
    private void MakeSkillPrototype(GameObject user, Vector3 targetPos, GameObject HitScanModel)
    {
        //// 회전 설정
        var userPos = user.transform.position;

        // 투사체 생성
        var hitScan = Instantiate(HitScanModel, userPos-new Vector3(0f, -8f, 0f), Quaternion.identity);
        hitScan.SendMessage("SetTargetPos", targetPos);
        hitScan.tag = user.tag;

        //유저 스탯에 비례해 스킬 발사
        var userStat = user.GetComponent<CharacterPara>();
        var hitScanBase = hitScan.GetComponent<CHitScanBase>();
        // 공격력 등 필요한 정보 넣기
        hitScanBase.userAttackPower = userStat.RandomAttackDamage();
        // 원소 관련 정보
    }
}