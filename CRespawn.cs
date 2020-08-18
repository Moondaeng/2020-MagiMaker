using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRespawn : MonoBehaviour
{
<<<<<<< HEAD
    List<Transform> spawnPos = new List<Transform>();
    GameObject[] monsters;

    [SerializeField]
    private GameObject summonMagic;
    private GameObject destroySummonMagic;
    public GameObject monPrefab;
    public int spawnNumber = 1;
    public float respawnDelay = 3f;

    int deadMonsters = 0;
=======
    List<Transform> _spawnPos = new List<Transform>();
    GameObject[] _monsters;

    [SerializeField]
    //   private GameObject destroySummonMagic;
    public List<GameObject> _monPrefab = new List<GameObject>();
    public List<int> _numberOfSpawn = new List<int>();
    public int _spawnNumber;
    public float _respawnDelay = 3f;

    int _deadMonsters = 0;
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
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
<<<<<<< HEAD
                spawnPos.Add(pos);
            }
        }
        if (spawnNumber > spawnPos.Count)
        {
            spawnNumber = spawnPos.Count;
        }

        monsters = new GameObject[spawnNumber];
        destroySummonMagic = Instantiate(summonMagic, transform.position + new Vector3(0, 0.5f, 0), summonMagic.transform.rotation);
=======
                _spawnPos.Add(pos);
            }
        }
        if (_spawnNumber > _spawnPos.Count)
        {
            _spawnNumber = _spawnPos.Count;
        }

        _monsters = new GameObject[_spawnNumber];
        //       destroySummonMagic = Instantiate(summonMagic, transform.position + new Vector3(0, 0.5f, 0), summonMagic.transform.rotation);
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
        MakeMonsters();

    }

    //프리팹으로 부터 몬스터를 만들어 관리하는 함수
    void MakeMonsters()
    {
<<<<<<< HEAD
        for (int i = 0; i < spawnNumber; i++)
        {
            GameObject mon = Instantiate(monPrefab, spawnPos[i].position, Quaternion.identity) as GameObject;
            mon.GetComponent<CEnemyFSM>().SetRespawn(gameObject, i, spawnPos[i].position);
            mon.SetActive(false);
            monsters[i] = mon;
            CManager.instance.AddNewMonsters(mon);
=======
        int spawnId = 0;
        for (int i = 0; i < _numberOfSpawn.Count; i++)
        {
            for (int j = 0; j < _numberOfSpawn[i]; j++)
            {
                //GameObject mon = Instantiate(_monPrefab[i], _spawnPos[j].position, Quaternion.identity) as GameObject;
                //mon.GetComponent<CEnemyPara>().SetRespawn(gameObject, j, _spawnPos[j].position);
                GameObject mon = Instantiate(_monPrefab[i], _spawnPos[spawnId].position, Quaternion.identity) as GameObject;
                mon.GetComponent<CEnemyPara>().SetRespawn(gameObject, spawnId, _spawnPos[spawnId].position);
                mon.SetActive(false);
                //_monsters[spawnId] = mon;
                _monsters[spawnId] = mon;
                CManager.instance.AddNewMonsters(mon);
                spawnId++;
            }
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
        }
    }

    public void RemoveMonster(int spawnID)
    {
<<<<<<< HEAD
        deadMonsters++;
        monsters[spawnID].SetActive(false);
        print(spawnID + "monster was killed");
        // 리스폰 트리거

        if (deadMonsters == monsters.Length)
        {
            StartCoroutine(InitMonsters());
            deadMonsters = 0;
        }
        
=======
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

>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
    }

    IEnumerator InitMonsters()
    {
<<<<<<< HEAD
        yield return new WaitForSeconds(respawnDelay);

=======
        yield return new WaitForSeconds(_respawnDelay);
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
        GetComponent<SphereCollider>().enabled = true;
    }


    void SpawnMonster()
    {
<<<<<<< HEAD
        for (int i = 0; i < monsters.Length; i++)
        {
            monsters[i].SetActive(true);
=======
        for (int i = 0; i < _monsters.Length; i++)
        {
            _monsters[i].GetComponent<CEnemyPara>().respawnAgain();
            _monsters[i].SetActive(true);
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
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
<<<<<<< HEAD
    void Update()
    {

    }
=======
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
}