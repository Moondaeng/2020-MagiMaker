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

    private float _startTimeMultiplier;
    private float _startTimeIncrement;

    private float _stopTimeMultiplier;
    private float _stopTimeIncrement;

    private void Awake()
    {
        Starting = true;
        LoopingAudioSource = new LoopingAudioSource(this, _audioSource, _startTime, _stopTime);
    }

    protected override void Start()
    {
        if (_audioSource != null)
        {
            _audioSource.Play();
        }
        _stopTimeMultiplier = 1.0f / _stopTime;
        _startTimeMultiplier = 1.0f / _startTime;
        base.Start();

        LoopingAudioSource.Play();
    }

    protected override void Update()
    {
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
