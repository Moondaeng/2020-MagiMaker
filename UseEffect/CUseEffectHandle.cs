using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CUseEffectHandleExplain
{
    public static string CreateUseEffectListText(List<CUseEffectHandle> effectList)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var effect in effectList)
        {
            sb.Append(CreateUseEffectText(effect));
        }
        return sb.ToString();
    }

    private static string CreateUseEffectText(CUseEffectHandle effect)
    {
        if (effect == null)
        {
            return "";
        }

        if (effect is CUseEffect)
        {
            return CUseEffectExplain.CreateUseEffectText(effect as CUseEffect);
        }
        else if (effect is CRandomUseEffect)
        {
            return CRandomUseEffectExplain.CreateRandomUseEffectText(effect as CRandomUseEffect);
        }
        else if (effect is CMakeObjectUseEffect)
        {
            Debug.LogWarning("Warning : no return value");
            return "";
        }
        else
        {
            Debug.LogWarning("Warning : undefined effect type");
            return "";
        }
    }
}

public abstract class CUseEffectHandle : MonoBehaviour
{
    public abstract void TakeUseEffect(CharacterPara cPara);
    public abstract void EnhanceEffectByStat(CharacterPara userStatus);
}
