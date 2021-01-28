using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 모든 투사체형 공격 클래스
 * 
 */
public class CProjectileBase : CHitObjectBase
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

    public Vector3 TargetPos;

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

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        //Debug.Log($"Hit {collision.gameObject.name}");

        // destroy particle systems after a slight delay
        if (ProjectileDestroyParticleSystemsOnCollision != null)
        {
            foreach (ParticleSystem p in ProjectileDestroyParticleSystemsOnCollision)
            {
                GameObject.Destroy(p, 0.1f);
            }
        }

        //ProjectileExplosionParticleSystem.transform.position = c.contacts[0].point;
        ProjectileExplosionParticleSystem.Play();
        CHitObjectBase.CreateExplosion(gameObject.transform.position, ProjectileExplosionRadius, ProjectileExplosionForce);

        Destroy(gameObject, 0.5f);
    }

    //protected override void OnCollisionEnter(Collision collision)
    //{
    //    base.OnCollisionEnter(collision);
    //    Debug.Log($"Hit {collision.gameObject.name}");

    //    // destroy particle systems after a slight delay
    //    if (ProjectileDestroyParticleSystemsOnCollision != null)
    //    {
    //        foreach (ParticleSystem p in ProjectileDestroyParticleSystemsOnCollision)
    //        {
    //            GameObject.Destroy(p, 0.1f);
    //        }
    //    }

    //    //ProjectileExplosionParticleSystem.transform.position = c.contacts[0].point;
    //    ProjectileExplosionParticleSystem.Play();
    //    CHitObjectBase.CreateExplosion(gameObject.transform.position, ProjectileExplosionRadius, ProjectileExplosionForce);

    //    Destroy(gameObject, 0.5f);
    //}
}
