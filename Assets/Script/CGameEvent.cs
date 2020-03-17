using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGameEvent : MonoBehaviour
{
    public event EventHandler<Tuple<float, float, float, float>> PlayerMoveStartEvent;
    public event EventHandler<Tuple<float, float>> PlayerMoveStopEvent;
    public event EventHandler<Tuple<float, float>> PlayerAttackEvent;

    public void PlayerMoveStop(Tuple<float, float> e) => PlayerMoveStopEvent?.Invoke(this, e);


}
