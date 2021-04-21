using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CDefinedUseEffect : CUseEffectHandle
{
    enum EDefinedEffect
    {
        Wet,
        Burn,
        Curse
    }

    private EDefinedEffect effect;

    public override void TakeUseEffect(CharacterPara cPara)
    {
        //cPara.TakeUseEffect(GetUseEffectFromEnum(effect));
    }

    //private CUseEffect GetUseEffectFromEnum(EDefinedEffect effect)
    //{
        
    //}
}
