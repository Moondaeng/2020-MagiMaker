using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class CJsonMaker : MonoBehaviour
{
    public enum EElementType
    {
        none = -1, fire = 0, wind = 1, water = 2, earth = 3
    }
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
    
    public CJsonMaker()
    {

    }

    public CJsonMaker(bool isSet)
    {
        if (isSet)
        {
            name = "Skeleton";
            maxHp = 50;
            curHp = maxHp;
            attackMin = 5;
            attackMax = 10;
            defense = 1;
            eLevel = 0;
            eType = EElementType.earth;
            rewardMoney = Random.Range(10, 31);
            isStunned = false;
            isDead = false;
        }
    }
    
    string ObjectToJson (object obj)
    {
        return JsonUtility.ToJson(obj);
    }

    T JsonToObJect<T>(string jsonData)
    {
        return JsonUtility.FromJson<T>(jsonData);
    }

    void CreateJsonFile(string createPath, string fileName, string jsonData)
    {
        FileStream fileStream = new FileStream(string.Format("{0}/{1}.json", createPath, fileName), FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(jsonData);
        fileStream.Write(data, 0, data.Length);
        fileStream.Close();
    }

    T LoadJsonFile<T>(string loadPath, string fileName)
    {
        FileStream filestream = new FileStream(string.Format("{0}/{1}.json", loadPath, fileName), FileMode.Open);
        byte[] data = new byte[filestream.Length];
        filestream.Read(data, 0, data.Length);
        filestream.Close();
        string jsonData = Encoding.UTF8.GetString(data);
        return JsonUtility.FromJson<T>(jsonData);
    }

    // Start is called before the first frame update
    void Start()
    {
        //CJsonMaker jtc = new CJsonMaker(false);
        //string jsonData = ObjectToJson(jtc);
        //CreateJsonFile(Application.dataPath, name, jsonData);
        var jtc2 = LoadJsonFile<CJsonMaker>(Application.dataPath, "Stat");
        Debug.Log(jtc2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
