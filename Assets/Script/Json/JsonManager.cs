using System.Collections.Generic;
using UnityEngine;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
using System.IO;
using System;

public class JsonManager : MonoBehaviour
{
    public static JsonManager instance;
    public enum EElementType
    {
        none = -1, fire = 0, wind = 1, water = 2, earth = 3
    }
    struct MonsterPara
    {
        public string name;
        public int maxHp;
        public int curHp;
        public int attackMin;
        public int attackMax; 
        public int defense;
        public int eLevel;
        public EElementType eType;
        public bool isStunned;
        public bool isDead;
        public int rewardMoney;
    }

    //Dictionary<string, MonsterPara> dicMon =
    //    new Dictionary<string, MonsterPara>();
    string json;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    static void ReadMonsterJson(string jsonStr, string monsterName)
    {
        //        JObject json = JObject.Parse(jsonStr);
        //        MonsterPara monster = JsonConvert.DeserializeObject<MonsterPara>(json["Monster"].ToString());
        /*
        foreach (var data in json["Monster"])
        {
            JProperty jProperty = data.ToObject<JProperty>();
            
        }

        Debug.Log(monster);
                */
    }

    // Start is called before the first frame update
    void Start()
    {
        {
            StreamReader r = new StreamReader("Stat.json");
            try
            {
                json = r.ReadToEnd();
                
                //ReadMonsterJson(json, name);
            }
            finally
            {
                if (r != null)
                {
                    ((IDisposable)r).Dispose();
                }
            }
        }
        Debug.Log(json);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
