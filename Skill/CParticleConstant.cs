using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopingAudioSource
{
    public AudioSource AudioSource { get; private set; }
    public float TargetVolume { get; private set; }

    private float startMultiplier;
    private float stopMultiplier;
    private float currentMultiplier;

    public LoopingAudioSource(MonoBehaviour script, AudioSource audioSource, float startMultiplier, float stopMultiplier)
    {
        AudioSource = audioSource;
        if (audioSource != null)
        {
            AudioSource.loop = true;
            AudioSource.volume = 0.0f;
            AudioSource.Stop();
        }

        TargetVolume = 1.0f;

        this.startMultiplier = currentMultiplier = startMultiplier;
        this.stopMultiplier = stopMultiplier;
    }

    public void Play()
    {
        Play(TargetVolume);
    }

    public void Play(float targetVolume)
    {
        if (AudioSource != null && !AudioSource.isPlaying)
        {
            AudioSource.volume = 0.0f;
            AudioSource.Play();
            currentMultiplier = startMultiplier;
        }
        TargetVolume = targetVolume;
    }

    public void Stop()
    {
        if (AudioSource != null && AudioSource.isPlaying)
        {
            TargetVolume = 0.0f;
            currentMultiplier = stopMultiplier;
        }
    }

    public void Update()
    {
        if (AudioSource != null && AudioSource.isPlaying &&
            (AudioSource.volume = Mathf.Lerp(AudioSource.volume, TargetVolume, Time.deltaTime / currentMultiplier)) == 0.0f)
        {
            AudioSource.Stop();
        }
    }
}

public class CParticleConstant : CParitcleSkillBase
{
    [HideInInspector]
    public LoopingAudioSource LoopingAudioSource;

    [Tooltip("looping하는 오디오 소스")]
    [SerializeField] protected AudioSource _audioSource;

    [Tooltip("시작 시간 애니메이션이랑 사운드에 사용")]
    [SerializeField] protected float _startTime = 1f;

    [Tooltip("끝 시간. 애니메이션이랑 사운드에 사용")]
    [SerializeField] protected float _stopTime = 3f;

    [Tooltip("사정 거리")]
    [SerializeField] public float _distance = 5f;

    [Tooltip("공격 간격")]
    [SerializeField] public float _attackTickTime = 1f;

    [Tooltip("폭발 파티클")]
    [SerializeField] private ParticleSystem _explosion = null;

    [Tooltip("유저가 사용 시 스태프에서 나갈거냐?")]
    [SerializeField] private bool _staffAnchor;

    private CCntl _myControl;
    public enum ColliderType
    {
        Sphere,
        Capsule,
        Box
    }
    
    public enum VectorDirection
    {
        x,
        y,
        z
    }

    [System.Serializable]
    public struct ColliderSettings
    {
        [Tooltip("Sphere면 건드리지마")]public ColliderType type;
        [Tooltip("박스:콜라이더 포지션 값에 추가하는 보정값\n캡슐: 위 꼭짓점 위치 보정값")] public float arg1;
        [Tooltip("박스: 콜라이더의 사이즈 값\n캡슐: 밑 꼭짓점 위치 보정값")] public float arg2;
        [Tooltip("캡슐만 사용 : 캡슐의 방향을 지시함.")] public VectorDirection arg3;
    }
    private float _startTimeMultiplier;
    private float _startTimeIncrement;

    private float _stopTimeMultiplier;
    private float _stopTimeIncrement;

    private List<GameObject> InTriggerUnit;
    public ColliderSettings ColliderSet;

    Vector3 forward;
    Vector3 up;

    IEnumerator TickDamage()
    {
        int Tick = (int)(_duration / _attackTickTime);
        for (int i = 0; i < Tick; i++)
        {
            yield return new WaitForSeconds(_attackTickTime);
            CreateExplosion(transform.position, _forceRadius, _forceAmount);
        }
    }

    private void Awake()
    {
        Starting = true;
        LoopingAudioSource = new LoopingAudioSource(this, _audioSource, _startTime, _stopTime);
    }
    
    protected override void Start()
    {
        _myControl = _skillUsingUser.GetComponent<CCntl>();
        _myControl.SkillExitEvent.AddListener(Stop);
        forward = _skillUsingUser.transform.forward;
        up = _skillUsingUser.transform.up;
        if (_audioSource != null)
        {
            _audioSource.Play();
        }
        _stopTimeMultiplier = 1.0f / _stopTime;
        _startTimeMultiplier = 1.0f / _startTime;
        base.Start();
        StartCoroutine(TickDamage());
        LoopingAudioSource.Play();
    }

    public override void CreateExplosion(Vector3 pos, float radius, float force)
    {
        Collider[] objects;
        if (force <= 0.0f || radius <= 0.0f)
        {
            return;
        }
        if (_explosion != null)
        {
            _explosion.Play();
        }

        int layermask = (1 << LayerMask.NameToLayer("Monster")) + (1 << LayerMask.NameToLayer("Player"));

        if (ColliderSet.type == ColliderType.Sphere)
        {
            objects = UnityEngine.Physics.OverlapSphere(pos, radius, layermask);

            CollisionMessage(objects);
        }
        else if (ColliderSet.type == ColliderType.Box)
        {
            objects = UnityEngine.Physics.OverlapBox(pos + Vector3.up * ColliderSet.arg1,
                   new Vector3(ColliderSet.arg2, ColliderSet.arg2, ColliderSet.arg2), _skillUsingUser.transform.rotation, layermask);
            CollisionMessage(objects);
        }
        else if (ColliderSet.type == ColliderType.Capsule)
        {
            if (ColliderSet.arg3 == VectorDirection.x)
            {
                objects = UnityEngine.Physics.OverlapCapsule(pos + Vector3.up * ColliderSet.arg1,
                pos - Vector3.up * ColliderSet.arg2, radius, layermask);
                CollisionMessage(objects);
            }

            else if (ColliderSet.arg3 == VectorDirection.y)
            {
                objects = UnityEngine.Physics.OverlapCapsule(pos + Vector3.up * ColliderSet.arg1,
                pos - Vector3.up * ColliderSet.arg2, radius, layermask);
                CollisionMessage(objects);
            }

            else if (ColliderSet.arg3 == VectorDirection.z)
            {
                objects = UnityEngine.Physics.OverlapCapsule(pos + Vector3.forward * ColliderSet.arg1,
                pos - Vector3.forward * ColliderSet.arg2, radius, layermask);
                CollisionMessage(objects);
            }

        }
    }

    public void CollisionMessage(Collider[] objects)
    {
        InTriggerUnit = new List<GameObject>();
        foreach (Collider h in objects)
        {
            Debug.Log(h);
            GameObject g = h.gameObject;
            bool SameExist = false;

            foreach (GameObject a in InTriggerUnit)
            {
                if (a == g)
                    SameExist = true;
            }

            if (SameExist)
                continue;
            else
            {
                InTriggerUnit.Add(g);
                if (g.tag == "Player")
                {
                    CPlayerPara p = g.GetComponent<CPlayerPara>();
                    foreach (AttackArgumentsList a in AttackArguments)
                    {
                        SwitchInType(a, p);
                    }
                }
                else if (g.tag == "Monster")
                {
                    CEnemyPara e = g.GetComponent<CEnemyPara>();
                    foreach (AttackArgumentsList a in AttackArguments)
                    {
                        SwitchInType(a, e);
                    }
                }
            }
        }
    }

    protected override void Update()
    {
        if (_staffAnchor)
        {
            transform.rotation = _skillUsingUser.transform.rotation;
            transform.position = _skillUsingUser.GetComponent<CCntl>().staff.transform.position;
        }
        if (Stopping)
        {
            // increase the stop time
            _stopTimeIncrement += Time.deltaTime;
            if (_stopTimeIncrement < _stopTime)
            {
                StopPercent = _stopTimeIncrement * _stopTimeMultiplier;
            }
        }
        else if (Starting)
        {
            // increase the start time
            _startTimeIncrement += Time.deltaTime;
            if (_startTimeIncrement < _startTime)
            {
                StartPercent = _startTimeIncrement * _startTimeMultiplier;
            }
            else
            {
                Starting = false;
            }
        }
        base.Update();

        LoopingAudioSource.Update();
    }


    public override void Stop()
    {
        if (Stopping)
        {
            return;
        }
        Stopping = true;

        LoopingAudioSource.Stop();

        base.Stop();
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
