using UnityEngine;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
using System;

[Serializable]
public class JsonToObject
{
    public static JsonToObject instance;
    public int _maxHp;
    public int _attackMin;
    public int _attackMax; 
    public int _defense;
    public int _eLevel;
    public int _rewardMoney;
    
    public JsonToObject()
    {

    }

    public JsonToObject(bool isSet)
    {
        if (isSet)
        {
            _maxHp = 100;
            _attackMin = 10;
            _attackMax = 20;
            _defense = 1;
            _eLevel = 0;
            _rewardMoney = 10;
        }
    }

    public void Print()
    {
        Debug.Log("maxHp = " + _maxHp);
        Debug.Log("attackMin = " + _attackMin);
        Debug.Log("attackMax = " + _attackMax);
        Debug.Log("defense = " + _defense);
        Debug.Log("eLevel = " + _eLevel);
        Debug.Log("rewardMoney = " + _rewardMoney);
    }
}