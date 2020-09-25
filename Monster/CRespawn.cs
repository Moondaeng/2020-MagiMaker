using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRespawn : MonoBehaviour
{
    #region Properties
    List<GameObject> _monsters;
    [Tooltip("넣을 놈")]
    [SerializeField] GameObject[] _monPrefab;
    [Tooltip("리스폰인지 아닌지 체크")]
    [SerializeField] bool _isRespawn;
    [Tooltip("몇 마리?")]
    [SerializeField] int[] _spawnNumber;
    Transform[, ] _monsterPosition;
    int _sumOfSpawnNumber;
    int _maxOfSpawnNumber;
    float _respawnDelay = 3f;
    int _deadMonsters = 0;
    #endregion

    void Start()
    {
        SumSpawnNumber();
        ScaleMonsterTransform();
    }

    #region 스폰하는 몬스터 세팅
    void SumSpawnNumber()
    {
        _maxOfSpawnNumber = 0;
        for (int i = 0; i < _spawnNumber.Length; i++)
        {
            _sumOfSpawnNumber += _spawnNumber[i];
            if (_maxOfSpawnNumber < _spawnNumber[i])
            {
                _maxOfSpawnNumber = _spawnNumber[i];
            }
        }
    }

    void ScaleMonsterTransform()
    {
        int i = 0;
        _monsterPosition = new Transform[_spawnNumber.Length, _maxOfSpawnNumber];

        foreach (Transform pos in transform)
        {
            for (int j = 0; j < _spawnNumber[i]; j++)
            {
                _monsterPosition[i, j] = transform.GetChild(i).GetChild(j);
            }
            i++;
        }
        _monsters = new List<GameObject>();
        for (int j = 0; j < _spawnNumber.Length; j++)
        {
            MakeMonsters(_monPrefab[j], j);
        }
    }
    #endregion

    #region 몬스터 만들기
    //프리팹으로 부터 몬스터를 만들어 관리하는 함수
    void MakeMonsters(GameObject monster, int index)
    {
        int _monsterIndex;
        _monsterIndex = IndexMaker(index);
        for (int i = 0; i < _spawnNumber[index]; i++)
        {
            GameObject mon = Instantiate(monster, _monsterPosition[index, i].position, Quaternion.identity) as GameObject;
            mon.GetComponent<CEnemyPara>().SetRespawn(gameObject, _monsterIndex + i, _monsterPosition[index, i].position);
            mon.SetActive(false);
            _monsters.Add(mon);
            CManager.instance.AddNewMonsters(mon);
        }
    }

    int IndexMaker(int index)
    {
        int _sumOfIndex = 0;
        for (int i = 0; i < index; i++)
        {
            _sumOfIndex += _spawnNumber[i];
        }
        return _sumOfIndex;
    }
    #endregion

    public void RemoveMonster(int spawnID)
    {
        _deadMonsters++;
        //Debug.Log(_deadMonsters);
        _monsters[spawnID].SetActive(false);
        //print(spawnID + " monster was killed");
        // 리스폰 트리거
        //if (_deadMonsters == _monsters.Count)
        //{
        //    StartCoroutine(InitMonsters());
        //    _deadMonsters = 0;
        //    if (!_isRespawn)
        //    {
        //        CManager.instance.DestroyAllMonsters();
        //        Destroy(transform.gameObject);
        //    }
        //}

    }

    #region 미사용
    IEnumerator InitMonsters()
    {
        yield return new WaitForSeconds(_respawnDelay);
        GetComponent<SphereCollider>().enabled = true;
    }


    void SpawnMonster()
    {
        for (int i = 0; i < _monsters.Count; i++)
        {
            _monsters[i].GetComponent<CEnemyPara>().respawnAgain();
            _monsters[i].SetActive(true);
        }
    }
    #endregion

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            SpawnMonster();
            GetComponent<SphereCollider>().enabled = false;
        }
    }
}