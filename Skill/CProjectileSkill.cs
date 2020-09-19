using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 투사체형 발사 스킬군
// 스킬 제작 경과에 따라 다른 스킬군과 코드 합체할 수도 있음
public class CProjectileSkill : MonoBehaviour
{
    public GameObject _fireballObject;
    public GameObject _lightArrowObject;
    private GameObject _prefabObject;
    private CParitcleSkillBase _prefabScript;

    private static CLogComponent _log;

    private void Awake()
    {
        _log = new CLogComponent(ELogType.Skill);
    }

    // 스킬 포맷에 넣을 스킬
    public void Fireball(GameObject user, Vector3 targetPos)
    {
        MakeProjectilePrototype(user, targetPos, _fireballObject, true, false);
    }

    public void LightArrow(GameObject user, Vector3 targetPos)
    {
        MakeProjectilePrototype(user, targetPos, _lightArrowObject, true, false);
    }

    // 투사체 발사 스킬 원형
    private void MakeProjectilePrototype(GameObject user, Vector3 targetPos, 
        GameObject ProjectileModel, bool IsParticleSystem, bool IsChaseEnemy)
    {
        Vector3 startPos;
        float yRot = transform.rotation.eulerAngles.y;
        var forward = user.transform.forward;
        var right = user.transform.right;
        var up = user.transform.up;
        var forwardY = Quaternion.Euler(0.0f, yRot, 0.0f) * forward;
        Quaternion rotation = Quaternion.identity;

        // 회전 설정
        var userPos = user.transform.position + forward + up;
        var objectivePos = targetPos - userPos;
        Quaternion lookRotation = Quaternion.LookRotation(objectivePos);

        if (IsParticleSystem)
        {
            _prefabObject = GameObject.Instantiate(ProjectileModel);
            _prefabScript = _prefabObject.GetComponent<CParticleConstant>();

            if (_prefabScript == null)
            {
                _prefabScript = _prefabObject.GetComponent<CParitcleSkillBase>();
                if (_prefabScript._isProjectile)
                {
                    rotation = user.transform.rotation;
                    startPos = user.transform.position + forward * 2f + up * 1.7f;
                }
                else
                {
                    startPos = user.transform.position + (forwardY * 10.0f);
                }
            }
            else
            {
                startPos = transform.position + (forwardY * 5.0f);
                rotation = transform.rotation;
                startPos.y = 0.0f;
            }
            _prefabObject.transform.position = startPos;
            _prefabObject.transform.rotation = rotation;

            var projectileScript = _prefabObject.GetComponent<CParticleProjectile>();
            if (IsChaseEnemy)
            {
                projectileScript._direction = objectivePos;
            }
        }
        else
        {
            // 투사체 생성
            var projectile = Instantiate(ProjectileModel, userPos, lookRotation);
            //projectile.SendMessage("SetTargetPos", targetPos);
            projectile.tag = user.tag;
        }
        // 공격력 등 필요한 정보 넣기
        _prefabScript._attackPower = user.GetComponent<CharacterPara>()._attackMax;
        // 원소 관련 정보
    }
}