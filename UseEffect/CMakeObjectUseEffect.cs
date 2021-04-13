using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMakeObjectUseEffect : CUseEffectHandle
{
    public GameObject shootObject;

    public override void TakeUseEffect(CharacterPara cPara)
    {
        throw new System.NotImplementedException();
    }

    public override void EnhanceEffectByStat(CharacterPara cPara)
    {
        Debug.Log("MakeObjectUseEffect's EnhanceEffectByStat isn't updated");
    }
}
