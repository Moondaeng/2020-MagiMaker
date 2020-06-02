using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

public class JsonConvert : MonoBehaviour
{
    public static JsonConvert instance;
    
    struct Monster
    {
        public int _maxHp;
        public int _curHp;
        public int _attackMin;
        public int _attackMax;
        public int _defense;
        public int _eLevel;
        public CharacterPara.EElementType _eType;
        public int _rewardMoney;
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    string ObjectToJson(object obj)
    {
        return JsonUtility.ToJson(obj);
    }

    T JsonToOject<T>(string jsonData)
    {
        return JsonUtility.FromJson<T>(jsonData);
    }

    T LoadJsonFile<T>(string loadPath, string fileName)
    {
        FileStream fileStream = new FileStream(string.Format("{0}/{1}.json", loadPath, fileName), FileMode.Open);
        byte[] data = new byte[fileStream.Length];
        fileStream.Read(data, 0, data.Length);
        fileStream.Close();
        string jsonData = Encoding.UTF8.GetString(data);
        return JsonUtility.FromJson<T>(jsonData);
    }

    string namingStatFile(string name)
    {
        string returnName = name + "Stat";
        return returnName;
    }

    public void loadToMonster(string name, CharacterPara mon)
    {
        string StatName;
        StatName = namingStatFile(name);
        JsonToObject jtc = LoadJsonFile<JsonToObject>(Application.dataPath, StatName);
        
        mon._maxHp = jtc._maxHp;
        mon._curHp = mon._maxHp;
        mon._attackMin = jtc._attackMin;
        mon._attackMax = jtc._attackMax;
        mon._defense = jtc._defense;
        mon._eLevel = jtc._eLevel;
        mon._eType = jtc._eType;
        mon._rewardMoney = jtc._rewardMoney;
    }
}
