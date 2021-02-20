using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRespawn : MonoBehaviour
{
    #region Properties
    [Tooltip("넣을 놈")]
    [SerializeField] GameObject[] _monPrefab;
    [Tooltip("리스폰인지 아닌지 체크")]
    [SerializeField] bool _isRespawn;
    [Tooltip("몇 마리?")]
    [SerializeField] int[] _spawnNumber;
    Transform[,] _monsterPosition;
    int _sumOfSpawnNumber;
    int _maxOfSpawnNumber;
    float _respawnDelay = 3f;
    int _deadMonsters = 0;
    Coroutine Co;
    #endregion

    void Start()
    {
        SumSpawnNumber();
        ScaleMonsterTransform();
        SpawnMonster();
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
            mon.SetActive(false);
            CMonsterManager.instance.AddMonsterInfo(mon);
            mon.GetComponent<CEnemyPara>().SetRespawn(this.gameObject, CMonsterManager.instance.GetMonsterCount() - 1, _monsterPosition[index, i].position);
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

    // CEnemyFSM에서 DeadEvent 발생 시 처리
    // 리스폰 형식이면 몬스터 매니저에서 인포를 삭제하지 않음.
    public void RemoveMonster(int monsterID)
    {
        _deadMonsters++;
        CMonsterManager.instance.GetMonsterInfo(monsterID).SetActive(false);
        if (_isRespawn)
        {
            if (_deadMonsters == CMonsterManager.instance.GetMonsterCount())
            {
                SpawnMonster();
            }
        }
        else
        {
            if (_deadMonsters == CMonsterManager.instance.GetMonsterCount())
            {
                Co = StartCoroutine(Stop());
            }
        }
        //print(spawnID + " monster was killed");
        // 리스폰 트리거
    }

    IEnumerator Stop()
    {
        Debug.Log("Cour");
        yield return new WaitForSeconds(10f);
        Debug.Log("outine");
        Destroy(this.gameObject);

    }

    // 몬스터 스폰 함수
    // SetRespawn에서 지정한 기본 정보를 토대로 소환.
    void SpawnMonster()
    {
        for (int i = 0; i < CMonsterManager.instance.GetMonsterCount(); i++)
        {
            CMonsterManager.instance.GetMonsterInfo(i).GetComponent<CEnemyPara>().respawnAgain();
            CMonsterManager.instance.GetMonsterInfo(i).SetActive(true);
        }
    }
}