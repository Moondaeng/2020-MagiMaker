using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CParitcleSkillBase : MonoBehaviour
{
    #region properties
    [Tooltip("얼마나 오래 갈건가?")]
    [SerializeField] protected float _duration = 2.0f;

    [Tooltip("폭발중심 생성 힘량")]
    [SerializeField] protected float _forceAmount;

    [Tooltip("힘의 반경")]
    [SerializeField] protected float _forceRadius;

    [Tooltip("쏘냐?")]
    [SerializeField] public bool _isProjectile;

    [Tooltip("수동으로 쓰는 파티클")]
    [SerializeField] protected ParticleSystem[] _manualParticleSystems;
    
    [HideInInspector] public GameObject _skillUsingUser;

    [Tooltip("Staff에서 발사됨")]
    [SerializeField] public bool IsStartingStaff;
    public enum AttackType
    {
        damage, stun, KnockBack, Slow, Burn, Curse, Blind,
        DecreaseDefense, DecreaseAttack, Debuff
    }

    // 공격에 해당하는 경우들에 필요한 정보들 구조체
    // 데미지 - 데미지 계수 / 스턴 - 시간 / 슬로우 - 슬로우량, 시간 등
    // 필요에 따라 추가하기
    [System.Serializable]
    public struct AttackArgumentsList
    {
        public AttackType type;
        public int arg1;
    }

    public List<AttackArgumentsList> AttackArguments;
    public int _attackPower = 100;
    #endregion

    private IEnumerator CleanupEverythingCoRoutine()
    {
        // 2초 여유
        yield return new WaitForSeconds(2.0f);
        GameObject.Destroy(gameObject);
    }

    private void StartParticleSystems()
    {
        foreach (ParticleSystem p in gameObject.GetComponentsInChildren<ParticleSystem>())
        {
            if (_manualParticleSystems == null || _manualParticleSystems.Length == 0 ||
                System.Array.IndexOf(_manualParticleSystems, p) < 0)
            {
                if (p.main.startDelay.constant == 0.0f)
                {
                    var m = p.main;
                    var d = p.main.startDelay;
                    d.constant = 0.01f;
                    m.startDelay = d;
                }
                p.Play();
            }
        }
    }

    protected virtual void Start()
    {
        StartParticleSystems();
        
        ICollisionHandler handler = (this as ICollisionHandler);
        if (handler != null)
        {
            CCollision Checker = GetComponentInChildren<CCollision>();
            if (Checker != null)
            {
                Checker.CollisionHandler = handler;
            }
        }
    }

    protected virtual void Update()
    {
        _duration -= Time.deltaTime;

        if (_duration <= 0.0f)
        {
            Stop();
        }
    }

    public virtual void CreateExplosion(Vector3 pos, float radius, float force)
    {
        if (force <= 0.0f || radius <= 0.0f)
        {
            return;
        }
        
        Collider[] objects = UnityEngine.Physics.OverlapSphere(pos, radius);
        foreach (Collider h in objects)
        {
            Rigidbody r = h.GetComponent<Rigidbody>(); 
            if (r != null)
            {
                r.AddExplosionForce(force, pos, radius);
            }
        }
    }

    public virtual void Stop()
    {
        foreach (ParticleSystem p in gameObject.GetComponentsInChildren<ParticleSystem>())
        {
            p.Stop();
        }

        StartCoroutine(CleanupEverythingCoRoutine());
    }

    protected void SwitchInType(AttackArgumentsList a, CPlayerPara p)
    {
        switch (a.type)
        {
            #region Nonused
            case AttackType.damage:
                _attackPower = 1;
                p.DamagedDisregardDefence(_attackPower * a.arg1);
                Debug.Log(_attackPower * a.arg1);
                //if (_skillUsingUser.tag == "Player")
                //    return;
                break;
            case AttackType.stun:
                if (_skillUsingUser.tag == "Player")
                    return;
                break;
            case AttackType.KnockBack:
                if (_skillUsingUser.tag == "Player")
                    return;
                break;
            case AttackType.Slow:
                if (_skillUsingUser.tag == "Player")
                    return;
                break;
            case AttackType.Burn:
                if (_skillUsingUser.tag == "Player")
                    return;
                break;
            case AttackType.Curse:
                if (_skillUsingUser.tag == "Player")
                    return;
                break;
            case AttackType.Blind:
                if (_skillUsingUser.tag == "Player")
                    return;
                break;
            case AttackType.DecreaseDefense:
                if (_skillUsingUser.tag == "Player")
                    return;
                break;
            case AttackType.DecreaseAttack:
                if (_skillUsingUser.tag == "Player")
                    return;
                break;
            #endregion
            default:
                break;
        }
    }
}
