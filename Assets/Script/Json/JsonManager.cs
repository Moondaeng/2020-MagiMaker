using UnityEngine;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
using System;

[Serializable]
public class JsonToObject
{
    public static JsonToObject instance;
    public int maxHp;
    public int attackMin;
    public int attackMax; 
    public int defense;
    public int eLevel;
    public CharacterPara.EElementType eType;
    public int rewardMoney;
    
    public JsonToObject()
    {

    }

    public JsonToObject(bool isSet)
    {
        if (isSet)
        {
            maxHp = 100;
            attackMin = 10;
            attackMax = 20;
            defense = 1;
            eLevel = 0;
            eType = CharacterPara.EElementType.none;
            rewardMoney = 10;
        }
    }

    public void Print()
    {
        Debug.Log("maxHp = " + maxHp);
        Debug.Log("attackMin = " + attackMin);
        Debug.Log("attackMax = " + attackMax);
        Debug.Log("defense = " + defense);
        Debug.Log("eLevel = " + eLevel);
        Debug.Log("eType = " + eType);
        Debug.Log("rewardMoney = " + rewardMoney);
    }
}