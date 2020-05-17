using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRespawn : MonoBehaviour
{
    List<Transform> _spawnPos = new List<Transform>();
    GameObject[] _monsters;

    [SerializeField]
 //   private GameObject destroySummonMagic;
    public GameObject _monPrefab;
    public int _spawnNumber = 1;
    public float _respawnDelay = 3f;

    int _deadMonsters = 0;
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
                _spawnPos.Add(pos);
            }
        }
        if (_spawnNumber > _spawnPos.Count)
        {
            _spawnNumber = _spawnPos.Count;
        }

        _monsters = new GameObject[_spawnNumber];
 //       destroySummonMagic = Instantiate(summonMagic, transform.position + new Vector3(0, 0.5f, 0), summonMagic.transform.rotation);
        MakeMonsters();

    }

    //프리팹으로 부터 몬스터를 만들어 관리하는 함수
    void MakeMonsters()
    {
        for (int i = 0; i < _spawnNumber; i++)
        {
            GameObject mon = Instantiate(_monPrefab, _spawnPos[i].position, Quaternion.identity) as GameObject;
            mon.GetComponent<CEnemyPara>().SetRespawn(gameObject, i, _spawnPos[i].position);
            mon.SetActive(false);
            _monsters[i] = mon;
            CManager.instance.AddNewMonsters(mon);
        }
    }

    public void RemoveMonster(int spawnID)
    {
        _deadMonsters++;
        Debug.Log(_deadMonsters);
        _monsters[spawnID].SetActive(false);
        //print(spawnID + " monster was killed");
        // 리스폰 트리거

        if (_deadMonsters == _monsters.Length)
        {
            StartCoroutine(InitMonsters());
            _deadMonsters = 0;
        }
        
    }

    IEnumerator InitMonsters()
    {
        yield return new WaitForSeconds(_respawnDelay);
        GetComponent<SphereCollider>().enabled = true;
    }


    void SpawnMonster()
    {
        for (int i = 0; i < _monsters.Length; i++)
        {
            _monsters[i].GetComponent<CEnemyPara>().respawnAgain();
            _monsters[i].SetActive(true);
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
}