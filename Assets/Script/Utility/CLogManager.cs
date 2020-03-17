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
    Network
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
        };
    }
}
