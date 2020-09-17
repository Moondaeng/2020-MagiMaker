using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CNonParticleSkillBase : MonoBehaviour
{
    [Tooltip("시작 시, 한번 재생되는 오디오 소스")]
    public AudioSource AudioSource;

    [Tooltip("시작 시간 애니메이션이랑 사운드에 사용")]
    public float StartTime = 1.0f;

    [Tooltip("끝 시간. 애니메이션이랑 사운드에 사용")]
    public float StopTime = 3.0f;

    [Tooltip("얼마나 오래 갈건가?")]
    public float Duration = 2.0f;

    [Tooltip("폭발중심 생성 힘량")]
    public float ForceAmount;

    [Tooltip("힘의 반경")]
    public float ForceRadius;
    
    private float startTimeMultiplier;
    private float startTimeIncrement;

    private float stopTimeMultiplier;
    private float stopTimeIncrement;

    private IEnumerator CleanupEverythingCoRoutine()
    {
        // 2 extra seconds just to make sure animation and graphics have finished ending
        yield return new WaitForSeconds(StopTime + 2.0f);

        GameObject.Destroy(gameObject);
    }

    protected virtual void Awake()
    {
        Starting = true;
        int skillLayer = LayerMask.NameToLayer("SkillLayer");
        if (skillLayer >= 0 && skillLayer < 32)
            UnityEngine.Physics.IgnoreLayerCollision(skillLayer, skillLayer);
    }

    protected virtual void Start()
    {
        if (AudioSource != null)
        {
            AudioSource.Play();
        }

        CreateExplosion(gameObject.transform.position, ForceRadius, ForceAmount);
        
        ICollisionHandler handler = (this as ICollisionHandler);
        if (handler != null)
        {
            CMonsterCollision collisionForwarder = GetComponentInChildren<CMonsterCollision>();
            if (collisionForwarder != null)
            {
                collisionForwarder.CollisionHandler = handler;
            }
        }
    }

    protected virtual void Update()
    {
        // reduce the duration
        Duration -= Time.deltaTime;
        if (Stopping)
        {
            // increase the stop time
            stopTimeIncrement += Time.deltaTime;
            if (stopTimeIncrement < StopTime)
            {
                StopPercent = stopTimeIncrement * stopTimeMultiplier;
            }
        }
        else if (Starting)
        {
            // increase the start time
            startTimeIncrement += Time.deltaTime;
            if (startTimeIncrement < StartTime)
            {
                StartPercent = startTimeIncrement * startTimeMultiplier;
            }
            else
            {
                Starting = false;
            }
        }
        else if (Duration <= 0.0f)
        {
            // time to stop, no duration left
            Stop();
        }
    }

    public static void CreateExplosion(Vector3 pos, float radius, float force)
    {
        if (force <= 0.0f || radius <= 0.0f)
        {
            return;
        }

        // find all colliders and add explosive force
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
        if (Stopping)
        {
            return;
        }
        Stopping = true;

        //// cleanup particle systems
        //foreach (ParticleSystem p in gameObject.GetComponentsInChildren<ParticleSystem>())
        //{
        //    p.Stop();
        //}

        StartCoroutine(CleanupEverythingCoRoutine());
    }
    public bool Starting
    {
        get;
        private set;
    }

    public float StartPercent
    {
        get;
        private set;
    }

    public bool Stopping
    {
        get;
        private set;
    }

    public float StopPercent
    {
        get;
        private set;
    }
}
