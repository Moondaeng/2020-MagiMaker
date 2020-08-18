using UnityEngine;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
using System;

[Serializable]
public class JsonToObject
{
    public static JsonToObject instance;
<<<<<<< HEAD
    public int maxHp;
    public int attackMin;
    public int attackMax; 
    public int defense;
    public int eLevel;
    public CharacterPara.EElementType eType;
    public int rewardMoney;
=======
    public int _maxHp;
    public int _attackMin;
    public int _attackMax; 
    public int _defense;
    public int _eLevel;
    public CharacterPara.EElementType _eType;
    public int _rewardMoney;
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
    
    public JsonToObject()
    {

    }

    public JsonToObject(bool isSet)
    {
        if (isSet)
        {
<<<<<<< HEAD
            maxHp = 100;
            attackMin = 10;
            attackMax = 20;
            defense = 1;
            eLevel = 0;
            eType = CharacterPara.EElementType.none;
            rewardMoney = 10;
=======
            _maxHp = 100;
            _attackMin = 10;
            _attackMax = 20;
            _defense = 1;
            _eLevel = 0;
            _eType = CharacterPara.EElementType.none;
            _rewardMoney = 10;
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
        }
    }

    public void Print()
    {
<<<<<<< HEAD
        Debug.Log("maxHp = " + maxHp);
        Debug.Log("attackMin = " + attackMin);
        Debug.Log("attackMax = " + attackMax);
        Debug.Log("defense = " + defense);
        Debug.Log("eLevel = " + eLevel);
        Debug.Log("eType = " + eType);
        Debug.Log("rewardMoney = " + rewardMoney);
=======
        Debug.Log("maxHp = " + _maxHp);
        Debug.Log("attackMin = " + _attackMin);
        Debug.Log("attackMax = " + _attackMax);
        Debug.Log("defense = " + _defense);
        Debug.Log("eLevel = " + _eLevel);
        Debug.Log("eType = " + _eType);
        Debug.Log("rewardMoney = " + _rewardMoney);
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
    }
}