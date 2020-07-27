using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRespawn : MonoBehaviour
{
    List<Transform> spawnPos = new List<Transform>();
    GameObject[] monsters;

    [SerializeField]
    private GameObject summonMagic;
    private GameObject destroySummonMagic;
    public GameObject monPrefab;
    public int spawnNumber = 1;
    public float respawnDelay = 3f;

    int deadMonsters = 0;
    void Start()
    {
        MakeSpawnPos();
    }
    void MakeSpawnPos()
    {
        foreach (Transform pos in transform)
        {
            if (pos.tag == "Respawn")
            {
                spawnPos.Add(pos);
            }
        }
        if (spawnNumber > spawnPos.Count)
        {
            spawnNumber = spawnPos.Count;
        }

        monsters = new GameObject[spawnNumber];
        destroySummonMagic = Instantiate(summonMagic, transform.position + new Vector3(0, 0.5f, 0), summonMagic.transform.rotation);
        MakeMonsters();

    }

    //프리팹으로 부터 몬스터를 만들어 관리하는 함수
    void MakeMonsters()
    {
        for (int i = 0; i < spawnNumber; i++)
        {
            GameObject mon = Instantiate(monPrefab, spawnPos[i].position, Quaternion.identity) as GameObject;
            mon.GetComponent<CEnemyFSM>().SetRespawn(gameObject, i, spawnPos[i].position);
            mon.SetActive(false);
            monsters[i] = mon;
            CManager.instance.AddNewMonsters(mon);
        }
    }

    public void RemoveMonster(int spawnID)
    {
        deadMonsters++;
        monsters[spawnID].SetActive(false);
        print(spawnID + "monster was killed");
        // 리스폰 트리거

        if (deadMonsters == monsters.Length)
        {
            StartCoroutine(InitMonsters());
            deadMonsters = 0;
        }
        
    }

    IEnumerator InitMonsters()
    {
        yield return new WaitForSeconds(respawnDelay);

        GetComponent<SphereCollider>().enabled = true;
    }


    void SpawnMonster()
    {
        for (int i = 0; i < monsters.Length; i++)
        {
            monsters[i].SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            SpawnMonster();
            GetComponent<SphereCollider>().enabled = false;
        }
    }
    void Update()
    {

    }
}