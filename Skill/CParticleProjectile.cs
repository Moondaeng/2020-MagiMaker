using System.Collections;
using UnityEngine;

public delegate void CProjectileCollisionDelegate(CParticleProjectile script, Vector3 pos);

public class CParticleProjectile : CParitcleSkillBase, ICollisionHandler
{
    #region parameters
    [Tooltip("충돌 및 물리에 사용할 충돌체 개체")]
    [SerializeField] private GameObject _projectileColliderObject;

    [Tooltip("충돌시 재생할 사운드")]
    [SerializeField] private AudioSource _projectileCollisionSound;

    [Tooltip("충돌시 재생할 파티클 시스템")]
    [SerializeField] private ParticleSystem _explosion;

    [Tooltip("충돌시 폭발 반경")]
    [SerializeField] private float _explosionRadius = 50.0f;

    [Tooltip("충돌시 폭발의 힘")]
    [SerializeField] private float _explosionForce = 50.0f;

    [Tooltip("콜라이더 움직이는 속도")]
    [SerializeField] private float _colliderSpeed = 450.0f;

    [Tooltip("충돌체가 갈 방향")]
    [SerializeField] public Vector3 _direction = Vector3.forward;



    [Tooltip("충돌시 파괴 할 파티클 시스템.")]
    public ParticleSystem[] _destroyParticleonCollision;

    [HideInInspector]
    public CProjectileCollisionDelegate CollisionDelegate;

    private bool collided;
    #endregion
   
    public override void Stop()
    {
        base.Stop();
        _colliderSpeed = 0f;
    }

    protected override void Update()
    {
        Vector3 dir = _direction * _colliderSpeed;
        dir = _projectileColliderObject.transform.rotation * dir;
        _projectileColliderObject.GetComponent<Rigidbody>().velocity = dir;
        base.Update();
    }

    public void HandleCollision(GameObject obj, Collision c)
    {
        if (collided) return;
        
        collided = true;
        Stop();
        
        if (_destroyParticleonCollision != null)
        {
            foreach (ParticleSystem p in _destroyParticleonCollision)
            {
                GameObject.Destroy(p, 0.1f);
            }
        }
        
        if (_projectileCollisionSound != null)
        {
            _projectileCollisionSound.Play();
        }
        
        if (c.contacts.Length != 0)
        {
            _explosion.transform.position = c.contacts[0].point;
            _explosion.Play();
            CParitcleSkillBase.CreateExplosion(c.contacts[0].point, _explosionRadius, _explosionForce);

            // 몬스터 타격 관련 함수
            CEnemyPara enemyPara = c.gameObject.GetComponent<CEnemyPara>();
            CPlayerPara playerPara = c.gameObject.GetComponent<CPlayerPara>();

            foreach (var attackArg in AttackArguments)
            {
                switch (attackArg.type)
                {
                    case AttackType.damage:
                        if (enemyPara != null)
                        {
                            enemyPara.SetEnemyAttack(_attackPower * attackArg.arg1);
                        }   
                        else if (playerPara != null)
                        {
                            playerPara.SetEnemyAttack(_attackPower * attackArg.arg1);
                        }
                        break;
                    case AttackType.stun:
                        Debug.Log("stun");
                        break;
                }
            }
            if (CollisionDelegate != null)
            {
                CollisionDelegate(this, c.contacts[0].point);
            }
        }
    }
}
