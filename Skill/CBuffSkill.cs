using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBuffSkill : MonoBehaviour
{
    public GameObject AttackBuff;
    public GameObject DefenceStackBuff;

    public void AttackUp(GameObject user, Vector3 targetPos)
    {
        MakeBuffRoundPrototype(user, targetPos, AttackBuff);
    }

    public void DefenceUp(GameObject user, Vector3 targetPos)
    {
        MakeBuffRoundPrototype(user, targetPos, DefenceStackBuff);
    }

    // user를 중심으로 원형 버프 생성
    private void MakeBuffRoundPrototype(GameObject user, Vector3 targetPos, GameObject BuffModel)
    {
        // 버프 생성
        var roundBuff = Instantiate(BuffModel, user.transform.position, Quaternion.identity);
        roundBuff.tag = user.tag;
    }
}
