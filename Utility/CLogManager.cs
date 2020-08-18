using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ELogSelect
{
    [SerializeField]
    public ELogType eLogType;

    [SerializeField]
    public bool select;
}

public enum ELogType
{
    System,
    Skill,
    Character,
<<<<<<< HEAD
    Network
=======
    Network,
    UI,
    Ctrl,
    Buff,
    State
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
}

/*
 * 로그 영역 관리 클래스
 * Scene의 Log 클래스로 들어가서 필요한 것만 체크할 것
 * 내부 구현은 천천히 개선 예정
 */
public class CLogManager : MonoBehaviour
{
    //[SerializeField]
    //[ArrayElementTitle("eLogType")]
    //private ELogSelect[] onLogs;

    public bool onSystem;
    public bool onSkill;
    public bool onCharacter;
    public bool onNetwork;
<<<<<<< HEAD
=======
    public bool onUI;
    public bool onCtrl;
    public bool onBuff;
    public bool onState;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3

    public void Log(ELogType logType, object message)
    {
        switch(logType)
        {
            case ELogType.System:
                if(onSystem) Debug.Log(message);
                break;
            case ELogType.Skill:
                if (onSkill) Debug.Log(message);
                break;
            case ELogType.Character:
                if (onCharacter) Debug.Log(message);
                break;
            case ELogType.Network:
                if (onNetwork) Debug.Log(message);
                break;
<<<<<<< HEAD
=======
            case ELogType.UI:
                if (onUI) Debug.Log(message);
                break;
            case ELogType.Ctrl:
                if (onCtrl) Debug.Log(message);
                break;
            case ELogType.Buff:
                if (onBuff) Debug.Log(message);
                break;
            case ELogType.State:
                if (onState) Debug.Log(message);
                break;
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
        }
    }

    public void Log(ELogType logType, string message, params object[] args)
    {
        switch (logType)
        {
            case ELogType.System:
                if (onSystem) Debug.LogFormat(message, args);
                break;
            case ELogType.Skill:
                if (onSkill) Debug.LogFormat(message, args);
                break;
            case ELogType.Character:
                if (onCharacter) Debug.LogFormat(message, args);
                break;
            case ELogType.Network:
                if (onNetwork) Debug.LogFormat(message, args);
                break;
<<<<<<< HEAD
=======
            case ELogType.UI:
                if (onUI) Debug.LogFormat(message, args);
                break;
            case ELogType.Ctrl:
                if (onCtrl) Debug.LogFormat(message, args);
                break;
            case ELogType.Buff:
                if (onBuff) Debug.LogFormat(message, args);
                break;
            case ELogType.State:
                if (onState) Debug.LogFormat(message, args);
                break;
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
        };
    }
}
