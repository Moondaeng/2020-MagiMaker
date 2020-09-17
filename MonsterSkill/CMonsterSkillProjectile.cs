using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void CMonsterProjectileCollisionDelegate(CMonsterSkillProjectile script, Vector3 pos);

/// <summary>
/// This script handles a projectile such as a fire ball
/// </summary>
public class CMonsterSkillProjectile : CMonsterSkillBase, ICollisionHandler
{
    [Tooltip("충돌 및 물리에 사용할 충돌체 개체")]
    public GameObject ProjectileColliderObject;

    [Tooltip("충돌시 재생할 사운드")]
    public AudioSource ProjectileCollisionSound;

    [Tooltip("충돌시 재생할 파티클 시스템")]
    public ParticleSystem ProjectileExplosionParticleSystem;

    [Tooltip("충돌시 폭발 반경")]
    public float ProjectileExplosionRadius = 50.0f;

    [Tooltip("충돌시 폭발의 힘")]
    public float ProjectileExplosionForce = 50.0f;

    [Tooltip("관통여부.")]
    public bool isPenetrate;

    [Tooltip("사전에 발사하는 애니메이션 있는 경우 선택적 지연")]
    public float ProjectileColliderDelay = 0.0f;

    [Tooltip("콜라이더 움직이는 속도")]
    public float ProjectileColliderSpeed = 450.0f;

    [Tooltip("충돌체가 갈 방향")]
    public Vector3 ProjectileDirection = Vector3.forward;

    [Tooltip("충돌체가 충돌 할 수있는 레이어.")]
    public LayerMask ProjectileCollisionLayers = Physics.AllLayers;

    [Tooltip("충돌시 파괴 할 입자 시스템.")]
    public ParticleSystem[] ProjectileDestroyParticleSystemsOnCollision;

    [HideInInspector]
    public CMonsterProjectileCollisionDelegate CollisionDelegate;

    private bool collided;

    private IEnumerator SendCollisionAfterDelay()
    {
        yield return new WaitForSeconds(ProjectileColliderDelay);

        Vector3 dir = ProjectileDirection * ProjectileColliderSpeed;
        dir = ProjectileColliderObject.transform.rotation * dir;
        ProjectileColliderObject.GetComponent<Rigidbody>().velocity = dir;
    }

    protected override void Start()
    {
        base.Start();

        StartCoroutine(SendCollisionAfterDelay());
    }

    public override void Stop()
    {
        base.Stop();
        ProjectileColliderSpeed = 0f;
    }

    public void HandleCollision(GameObject obj, Collision c)
    {
        if (collided)
        {
            // already collided, don't do anything
            return;
        }

        // stop the projectile
        collided = true;
        Stop();

        // destroy particle systems after a slight delay
        if (ProjectileDestroyParticleSystemsOnCollision != null)
        {
            foreach (ParticleSystem p in ProjectileDestroyParticleSystemsOnCollision)
            {
                GameObject.Destroy(p, 0.1f);
            }
        }

        // play collision sound
        if (ProjectileCollisionSound != null)
        {
            ProjectileCollisionSound.Play();
        }

        // if we have contacts, play the collision particle system and call the delegate
        if (isPenetrate && c.contacts.Length != 0)
        {
            ProjectileExplosionParticleSystem.transform.position = c.contacts[0].point;
            ProjectileExplosionParticleSystem.Play();
            CMonsterSkillBase.CreateExplosion(c.contacts[0].point, ProjectileExplosionRadius, ProjectileExplosionForce);
            if (CollisionDelegate != null)
            {
                CollisionDelegate(this, c.contacts[0].point);
            }
        } 
    }
}
