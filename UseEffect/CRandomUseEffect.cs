using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

public static class CRandomUseEffectExplain
{
    public static string CreateRandomUseEffectText(CRandomUseEffect randomEffect)
    {
        if (randomEffect == null)
        {
            return "";
        }

        StringBuilder sb = new StringBuilder();
        
        return sb.ToString();
    }
}

public class CRandomUseEffect : CUseEffectHandle
{
    [System.Serializable]
    private class UseEffectWithChance
    {
        public CUseEffect effect;
        public float chance;
    }

    [SerializeField]
    private List<UseEffectWithChance> effects;

    public override void TakeUseEffect(CharacterPara cPara)
    {
        cPara.TakeUseEffect(effects[SelectRandomEffect()].effect);
    }

    public int SelectRandomEffect()
    {
        if (effects.Count == 0)
        {
            Debug.Log("No Use Effect");
            return -1;
        }
        else if (effects.Count == 1)
        {
            return 0;
        }

        Debug.Log("Get Random Effect");
        float chanceSum = 0f;
        List<float> chanceSumList = new List<float>(effects.Count);
        foreach (var effect in effects)
        {
            chanceSum += effect.chance;
            chanceSumList.Add(chanceSum);
        }
        Debug.Log("set chance");

        // 임의 효과 선택
        float randomChance = UnityEngine.Random.Range(0f, 1f);
        int idx = 0;
        while (idx < chanceSumList.Count - 1 && randomChance >= chanceSumList[idx])
        {
            idx++;
        }

        return idx;
    }
}
