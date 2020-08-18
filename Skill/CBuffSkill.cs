using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBuffSkill : MonoBehaviour
{
<<<<<<< HEAD
    public GameObject DefenceBuff;
    
    private static CLogComponent _log;

    private void Awake()
    {
        _log = new CLogComponent(ELogType.Skill);
=======
    public GameObject AttackBuff;
    public GameObject DefenceStackBuff;

    public void AttackUp(GameObject user, Vector3 targetPos)
    {
        MakeBuffRoundPrototype(user, targetPos, AttackBuff);
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
    }

    public void DefenceUp(GameObject user, Vector3 targetPos)
    {
<<<<<<< HEAD
        MakeBuffRoundPrototype(user, targetPos, DefenceBuff);
=======
        MakeBuffRoundPrototype(user, targetPos, DefenceStackBuff);
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
    }

    // user를 중심으로 원형 버프 생성
    private void MakeBuffRoundPrototype(GameObject user, Vector3 targetPos, GameObject BuffModel)
    {
<<<<<<< HEAD
        // 회전 설정
        var userPos = user.transform.position;
        var objectivePos = targetPos - userPos;
        Quaternion lookRotation = Quaternion.LookRotation(objectivePos);

        // 버프 생성
        var roundBuff = Instantiate(BuffModel, userPos, lookRotation);
=======
        // 버프 생성
        var roundBuff = Instantiate(BuffModel, user.transform.position, Quaternion.identity);
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
        roundBuff.tag = user.tag;
    }
}
