using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * CLogManager 간편화 컴포넌트 클래스
 * 관련 클래스에 추가하고 기존 로그처럼 사용하면 편리하게 사용 가능
 * Monobehavior 상속 클래스에선 Start에서 new를 통해 할당
 * ex) 네트워크 영역 로그를 사용한다고 가정
 * private static CLogComponent = new CLogComponent(ELogType.Network);
 * ...
 * _logger.Log(message); -> 기존 로그처럼 사용
 */
public class CLogComponent
{
    protected static CLogManager _logManager = GameObject.Find("Log").GetComponent<CLogManager>();
    public ELogType logType;

    public CLogComponent(ELogType type)
    {
        logType = type;
    }

    public void Log(object message)
    {
        _logManager.Log(logType, message);
    }

    public void Log(string message, params object[] args)
    {
        _logManager.Log(logType, message, args);
    }
}