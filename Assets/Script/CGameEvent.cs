using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
 * 게임 이벤트 관리 컴포넌트
 * 
 */
namespace NEvent
{
    [System.Serializable]
    public class MoveStart : UnityEvent<Vector3, Vector3> { }
    [System.Serializable]
    public class MoveStop : UnityEvent<Vector3> { }
}

public class CGameEvent : MonoBehaviour
{
    public NEvent.MoveStart PlayerMoveStartEvent;
    public NEvent.MoveStop PlayerMoveStopEvent;
    public UnityEvent<Vector3> PlayerAttackEvent;

    private Network.CTcpClient _tcpManager;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        _tcpManager = GameObject.Find("Network")?.GetComponent<Network.CTcpClient>();

        if(!_tcpManager.IsConnect)
        {
            // 연결되면 패킷 받을거 설정
        }
    }

    public void PlayerMoveStart(Vector3 a, Vector3 b) => PlayerMoveStartEvent?.Invoke(a, b);
    public void PlayerMoveStop(Vector3 pos) => PlayerMoveStopEvent?.Invoke(pos);
    public void PlayerAttack(Vector3 pos) => PlayerAttackEvent?.Invoke(pos);
}
