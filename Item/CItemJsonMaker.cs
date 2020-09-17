using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

//json 생성용 스크립트 생성 후 주석처리!
public class CItemJsonMaker : MonoBehaviour
{

    string ObjectToJson(object obj)
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

    void MakeItemForJson()
    {
        //json으로 만들 아이템들
        CItemArray jtc = new CItemArray();
        //흔함
        //jtc.items[0] = new CItem("허름한 가죽 장갑", 0100, null, 10, 0, 0, 0, 0, 0, 0, 0, 0, null, 0);
        //jtc.items[1] = new CItem("빛바랜 적색 수정구", 0101, null, 0, 10, 0, 0, 0, 0, 0, 0, 0, null, 0);
        //jtc.items[2] = new CItem("허름한 가죽 갑옷", 0102, null, 0, 0, 50, 3, 0, 0, 0, 0, 0, null, 0);
        //jtc.items[3] = new CItem("빛바랜 녹색 수정구", 0103, null, 0, 0, 100, 0, 0, 0, 0, 0, 0, null, 0);
        //jtc.items[4] = new CItem("빛바랜 청색 수정구", 0104, null, 0, 0, 0, 10, 0, 0, 0, 0, 0, null, 0);
        //jtc.items[5] = new CItem("낡아빠진 클로", 0105, null, 5, 8, 0, 0, 0, 0, 0, 0, 0, null, 0);
        //jtc.items[6] = new CItem("허름한 가죽 신발", 0106, null, 0, 0, 0, 0, 10, 0, 0, 0, 0, null, 0);

        ////특별
        //jtc.items[7] = new CItem("가죽 장갑", 1100, null, 15, 0, 0, 0, 0, 0, 0, 0, 0, null, 0);
        //jtc.items[8] = new CItem("적색 수정구", 1101, null, 0, 15, 0, 0, 0, 0, 0, 0, 0, null, 0);
        //jtc.items[9] = new CItem("가죽 갑옷", 1102, null, 0, 0, 100, 7, 0, 0, 0, 0, 0, null, 0);
        //jtc.items[10] = new CItem("녹색 수정구", 1103, null, 0, 0, 200, 0, 0, 0, 0, 0, 0, null, 0);
        //jtc.items[11] = new CItem("청색 수정구", 1104, null, 0, 0, 0, 15, 0, 0, 0, 0, 0, null, 0);
        //jtc.items[12] = new CItem("클로", 1105, null, 8, 12, 0, 0, 0, 0, 0, 0, 0, null, 0);
        //jtc.items[13] = new CItem("가죽 신발", 1106, null, 0, 0, 0, 0, 15, 0, 0, 0, 0, null, 0);

        ////희귀
        //jtc.items[14] = new CItem("날렵한 가죽 장갑", 2100, null, 20, 0, 0, 0, 0, 0, 0, 0, 0, null, 0);
        //jtc.items[15] = new CItem("빛나는 적색 수정구", 2101, null, 0, 20, 0, 0, 0, 0, 0, 0, 0, null, 0);
        //jtc.items[16] = new CItem("견고한 가죽 갑옷", 2102, null, 0, 0, 150, 12, 0, 0, 0, 0, 0, null, 0);
        //jtc.items[17] = new CItem("빛나는 녹색 수정구", 2103, null, 0, 0, 300, 0, 0, 0, 0, 0, 0, null, 0);
        //jtc.items[18] = new CItem("빛나는 청색 수정구", 2104, null, 0, 0, 0, 20, 0, 0, 0, 0, 0, null, 0);
        //jtc.items[19] = new CItem("날이선 클로", 2105, null, 10, 15, 0, 0, 0, 0, 0, 0, 0, null, 0);
        //jtc.items[20] = new CItem("날렵한 가죽 신발", 2106, null, 0, 0, 0, 0, 20, 0, 0, 0, 0, null, 0);


        //json 만드는 과정
        string jsonData = ObjectToJson(jtc);
        CreateJsonFile(Application.dataPath, "Items", jsonData);
        var jtc2 = LoadJsonFile<CItemArray>(Application.dataPath, "Items");

        for(int i = 0; i < jtc2.items.Length; i++)
        {
            jtc2.items[i].Print();
        }
    }

    void Start()
    {
        //한번 생성한 후 주석 처리
        //MakeItemForJson();
    }
}
