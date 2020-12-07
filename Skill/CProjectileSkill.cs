using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 투사체형 발사 스킬군
// 스킬 제작 경과에 따라 다른 스킬군과 코드 합체할 수도 있음
public class CProjectileSkill : MonoBehaviour
{
    public GameObject fireballObject;

    private static CLogComponent _log;

    private void Awake()
    {
        _log = new CLogComponent(ELogType.Skill);
    }

    // 스킬 포맷에 넣을 스킬
    public void Fireball(GameObject user, Vector3 targetPos)
    {
        MakeProjectilePrototype(user, targetPos, fireballObject);
    }

    // 투사체 발사 스킬 원형
    private void MakeProjectilePrototype(GameObject user, Vector3 targetPos, GameObject ProjectileModel)
    {
        // 회전 설정
        var userPos = user.transform.position;
        var objectivePos = targetPos - userPos;
        Quaternion lookRotation = Quaternion.LookRotation(objectivePos);

        // 투사체 생성
        var projectile = Instantiate(ProjectileModel, userPos + Vector3.up, lookRotation);
        projectile.SendMessage("SetTargetPos", targetPos);
        projectile.layer = LayerMask.NameToLayer("PlayerSkill");
        projectile.tag = user.tag;

        // 유저 스탯에 비례해 스킬 발사
        var userStat = user.GetComponent<CharacterPara>();
        //var projectileBase = projectile.GetComponent<CProjectileBase>();
        // 공격력 등 필요한 정보 넣기
        //projectileBase.userAttackPower = userStat._attackMax;
        // 원소 관련 정보
    }
}
