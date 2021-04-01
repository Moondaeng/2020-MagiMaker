using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CObtainElementEffect : CUseEffectHandle
{
    public bool isMainElement;
    public CPlayerSkill.ESkillElement elementType;

    public override void EnhanceEffectByStat(CharacterPara userStatus)
    {
        // 강화되지 않음
        return;
    }

    public override void TakeUseEffect(CharacterPara cPara)
    {
        // 원소 획득
        if (!(cPara is CPlayerPara))
        {
            Debug.Log($"{cPara.gameObject.name} can't Get Element");
            return;
        }

        CElementObtainViewer.instance.gameObject.SetActive(true);
        CElementObtainViewer.instance.OpenViewer(cPara.GetComponent<CPlayerSkill>(), isMainElement, elementType);
    }
}
