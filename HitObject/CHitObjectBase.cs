﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CHitObjectBase : MonoBehaviour
{
    [Tooltip("충돌 시 적용될 효과")]
    public List<CUseEffectHandle> useEffects;

    [Tooltip("시작 시, 한번 재생되는 오디오 소스")]
    public AudioSource AudioSource;

    [Tooltip("시작 시간 애니메이션이랑 사운드에 사용")]
    public float StartTime = 1.0f;

    [Tooltip("끝 시간. 애니메이션이랑 사운드에 사용")]
    public float StopTime = 3.0f;

    [Tooltip("얼마나 오래 갈건가?")]
    public float Duration = 2.0f;
    private float realDuration;

    [Tooltip("폭발중심 생성 힘량")]
    public float ForceAmount;

    [Tooltip("힘의 반경")]
    public float ForceRadius;

    [Tooltip("쏘냐?")]
    public bool IsProjectile;

    [Tooltip("수동으로 쓰는 파티클")]
    public ParticleSystem[] ManualParticleSystems;

    private float startTimeMultiplier;
    private float startTimeIncrement;

    private float stopTimeMultiplier;
    private float stopTimeIncrement;

    private int _recentCollisionInstanceID = 0;

    public bool IsInit { get; set; }
    private bool bIsStartParticle = false;

    private IEnumerator CleanupEverythingCoRoutine()
    {
        // 2 extra seconds just to make sure animation and graphics have finished ending
        yield return new WaitForSeconds(StopTime + 2.0f);

        gameObject.SetActive(false);
    }

    protected virtual void StartParticleSystems()
    {
        foreach (ParticleSystem p in gameObject.GetComponentsInChildren<ParticleSystem>())
        {
            if (ManualParticleSystems == null || ManualParticleSystems.Length == 0 ||
                System.Array.IndexOf(ManualParticleSystems, p) < 0)
            {
                if (p.main.startDelay.constant == 0.0f)
                {
                    // wait until next frame because the transform may change
                    var m = p.main;
                    var d = p.main.startDelay;
                    d.constant = 0.01f;
                    m.startDelay = d;
                }
                p.Clear();
                p.Simulate(p.main.duration);
                p.Play();
            }
        }
    }

    protected virtual void OnEnable()
    {
        Starting = true;
        IsInit = false;
        realDuration = Duration;
    }

    protected virtual void Start()
    {
        // precalculate so we can multiply instead of divide every frame
        stopTimeMultiplier = 1.0f / StopTime;
        startTimeMultiplier = 1.0f / StartTime;

        // if this effect has an explosion force, apply that now
        //CreateExplosion(gameObject.transform.position, ForceRadius, ForceAmount);
    }

    protected virtual void Update()
    {
        if (IsInit && !bIsStartParticle)
        {
            if (AudioSource != null)
            {
                AudioSource.Play();
            }

            // start any particle system that is not in the list of manual start particle systems
            StartParticleSystems();

            bIsStartParticle = true;
        }

        // reduce the duration
        realDuration -= Time.deltaTime;
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
        else if (realDuration <= 0.0f)
        {
            // time to stop, no duration left
            Stop();
        }
    }

    private void OnDisable()
    {
        Stopping = false;
        IsInit = false;
        bIsStartParticle = false;
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

        Debug.Log("Stopping");
        // cleanup particle systems
        foreach (ParticleSystem p in gameObject.GetComponentsInChildren<ParticleSystem>())
        {
            p.Stop();
        }

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

    public void SetObjectLayer(int layer)
    {
        gameObject.layer = layer;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (IsInit && !IsTriggeredRecently(other))
        {
            GetUseEffect(other);
        }
    }

    protected bool IsTriggeredRecently(Collider other)
    {
        int recentCollisionID = other.gameObject.GetInstanceID();
        if (_recentCollisionInstanceID == recentCollisionID)
        {
            return true;
        }
        return false;
    }

    protected void GetUseEffect(Collider other)
    {
        var cPara = other.GetComponent<CharacterPara>();
        if (cPara != null)
        {
            foreach (var effect in useEffects)
            {
                effect.TakeUseEffect(cPara);
            }
        }
    }
}
