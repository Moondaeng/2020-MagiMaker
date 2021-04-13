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
        foreach(var effectWithChance in randomEffect.Effects)
        {
            sb.Append($"{effectWithChance.persantChance}% 확률로 " + 
                CUseEffectExplain.CreateUseEffectText(effectWithChance.effect));
        }
        return sb.ToString();
    }
}

public class CRandomUseEffect : CUseEffectHandle
{
    [System.Serializable]
    public class UseEffectWithChance
    {
        public CUseEffect effect;
        public int persantChance;
    }

    // Effects를 통해 접근하면 effects를 get으로 접근함에도 불구하고 하위 멤버를 수정할 수 있으니 주의 필요
    public List<UseEffectWithChance> Effects
    {
        get
        {
            return effects;
        }
    }

    [SerializeField]
    private List<UseEffectWithChance> effects;

    public override void TakeUseEffect(CharacterPara cPara)
    {
        int selectEffect = SelectRandomEffect();
        if (selectEffect == -1)
        {
            return;
        }
        cPara.TakeUseEffect(effects[selectEffect].effect);
    }

    public override void EnhanceEffectByStat(CharacterPara cPara)
    {
        Debug.Log("RandomUseEffect's EnhanceEffectByStat isn't updated");
    }

    private int SelectRandomEffect()
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
        int chanceSum = 0;
        List<int> chanceSumList = new List<int>(effects.Count);
        foreach (var effect in effects)
        {
            chanceSum += effect.persantChance;
            chanceSumList.Add(chanceSum);
        }

        // 임의 효과 선택
        int randomChance = UnityEngine.Random.Range(0, 100);
        Debug.Log($"random number is {randomChance}");
        int idx = 0;
        while (idx < chanceSumList.Count - 1 && randomChance >= chanceSumList[idx])
        {
            idx++;
        }

        Debug.Log($"select {idx} effect");
        return idx;
    }
}
