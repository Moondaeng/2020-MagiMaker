using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
 * 게임 이벤트 관리 컴포넌트
 * 
 */
public class CGameEvent : MonoBehaviour
{
    public event EventHandler<Tuple<float, float, float, float>> PlayerMoveStartEvent;
    public event EventHandler<Tuple<float, float>> PlayerMoveStopEvent;
    public event EventHandler<Tuple<float, float>> PlayerAttackEvent;
    public UnityEvent<Tuple<float, float, float, float>> uPlayerMoveStartEvent;

    public void PlayerMoveStop(Tuple<float, float> e) => PlayerMoveStopEvent?.Invoke(this, e);

}
