using DigitalRuby.PyroParticles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBuffBase : CHitObjectBase
{
    [HideInInspector]
    public LoopingAudioSource LoopingAudioSource;

    protected override void Awake()
    {
        base.Awake();

        // constant effect, so set the duration really high and add an infinite looping sound
        LoopingAudioSource = new LoopingAudioSource(this, AudioSource, StartTime, StopTime);
        Duration = 1;
    }

    protected override void Update()
    {
        base.Update();

        LoopingAudioSource.Update();
    }

    protected override void Start()
    {
        base.Start();

        LoopingAudioSource.Play();
    }

    public override void Stop()
    {
        LoopingAudioSource.Stop();

        base.Stop();
    }
}
