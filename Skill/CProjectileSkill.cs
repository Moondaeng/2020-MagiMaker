using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// 투사체형 발사 스킬군
// 스킬 제작 경과에 따라 다른 스킬군과 코드 합체할 수도 있음
public class CProjectileSkill : MonoBehaviour
{
    public List<GameObject> _spellObject;
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
        MakeProjectilePrototype(user, targetPos, _spellObject[0], true, false);
    }

    public void LightArrow(GameObject user, Vector3 targetPos)
    {
        MakeProjectilePrototype(user, targetPos, _spellObject[1], true, false);
    }

    public void LightOfPurification(GameObject user, Vector3 targetPos)
    {
        MakeProjectilePrototype(user, targetPos, _spellObject[2], true, false);
    }

    public void Flamethrower(GameObject user, Vector3 targetPos)
    {
        MakeProjectilePrototype(user, targetPos, _spellObject[3], true, false);
    }

    public void Rain(GameObject user, Vector3 targetPos)
    {
        MakeProjectilePrototype(user, targetPos, _spellObject[4], true, false);
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
                    if (_prefabScript.IsStartingPoint)
                    {
                        startPos = user.GetComponent<CCntl>()._skillHand._skillStartPoint;
                        _prefabObject.transform.position = startPos;
                    }
                    else
                    {
                        startPos = user.transform.position + forward * 2f + up * 1.7f;
                        _prefabObject.transform.position = startPos;
                    }
                    //Debug.Log("요기다1");
                }
                else
                {
                    startPos = user.transform.position + (forwardY * 10.0f);
                    _prefabObject.transform.position = startPos;
                    //Debug.Log("요기다2");
                }
            }
            else
            {
                if (Vector3.Distance(targetPos, user.transform.position) >= 5f || targetPos == Vector3.zero)
                {
                    if (ProjectileModel.name == "LightOfPurification")
                    {
                        startPos = user.transform.position + forward * 10 + up * 5;
                    }
                    else if (ProjectileModel.name == "Flamethrower")
                    {
                        startPos = user.transform.position + forward * 5f;
                    }
                    else startPos = user.transform.position + forward * 5;
                    _prefabObject.transform.position = startPos;
                    //Debug.Log(startPos);
                    //Debug.Log("요기다3");
                }
                else
                {
                    if (targetPos.y <= 1 && targetPos.y >= -1)
                    {
                        startPos = targetPos;
                        startPos.y = 2f;
                        _prefabObject.transform.position = startPos;
                    }
                    //Debug.Log("요기다4");
                }

            }
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
        _prefabScript._attackPower = (int) user.GetComponent<CharacterPara>().GetRandomAttack();
        _prefabScript._skillUsingUser = user;
        // 원소 관련 정보
    }
}