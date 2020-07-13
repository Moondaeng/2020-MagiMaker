using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBuffSkill : MonoBehaviour
{
    public GameObject DefenceBuff;
    
    private static CLogComponent _log;

    private void Awake()
    {
        _log = new CLogComponent(ELogType.Skill);
    }

    public void DefenceUp(GameObject user, Vector3 targetPos)
    {
        MakeBuffRoundPrototype(user, targetPos, DefenceBuff);
    }

    // user를 중심으로 원형 버프 생성
    private void MakeBuffRoundPrototype(GameObject user, Vector3 targetPos, GameObject BuffModel)
    {
        // 회전 설정
        var userPos = user.transform.position;
        var objectivePos = targetPos - userPos;
        Quaternion lookRotation = Quaternion.LookRotation(objectivePos);

        // 버프 생성
        var roundBuff = Instantiate(BuffModel, userPos, lookRotation);
        roundBuff.tag = user.tag;
    }
}
