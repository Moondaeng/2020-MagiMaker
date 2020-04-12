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
        public int maxHp;
        public int curHp;
        public int attackMin;
        public int attackMax;
        public int defense;
        public int eLevel;
        public CharacterPara.EElementType eType;
        public int rewardMoney;
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
        
        mon.maxHp = jtc.maxHp;
        mon.curHp = mon.maxHp;
        mon.attackMin = jtc.attackMin;
        mon.attackMax = jtc.attackMax;
        mon.defense = jtc.defense;
        mon.eLevel = jtc.eLevel;
        mon.eType = jtc.eType;
        mon.rewardMoney = jtc.rewardMoney;
    }
}
