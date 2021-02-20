using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/* @help
 * 이 클래스는 CRespawn에서 사용되는 맵 전체의 몬스터 배열을 관리하는 클래스이다.
 * 
 */
public class CMonsterManager : MonoBehaviour
{
    private class MonsterInfo
    {
        public int id;
        public GameObject mObject;

        public MonsterInfo(int id, GameObject monsterObject)
        {
            this.id = id;
            mObject = monsterObject;
        }
    }

    private int _monsterCount = 0;
    private List<MonsterInfo> _monsterList = new List<MonsterInfo>();
    
    public static CMonsterManager instance;

    #region 초기화
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        SetMonsterStatusViewer();
    }
    #endregion

    #region 몬스터 추가 / 제거
    private int CreateMonsterID()
    {
        return _monsterCount++;
    }

    public int AddMonsterInfo(GameObject mon)
    {
        int monsterNum = CreateMonsterID();
        _monsterList.Add(new MonsterInfo(monsterNum, mon));
        return monsterNum;
    }

    // 현재 존재하는 방안의 몬스터의 수를 알려줌
    public int GetMonsterCount()
    {
        return _monsterCount;
    }

    public GameObject GetMonsterInfo(int monsterID)
    {
        Debug.Log(_monsterList.Find(monsterInfo => monsterInfo.id == monsterID).id);
        return _monsterList.Find(monsterInfo => monsterInfo.id == monsterID).mObject;
    }

    public void RemoveMonster(int monsterID)
    {
        _monsterList.Remove(_monsterList.Find(monsterInfo => monsterInfo.id == monsterID));
    }
    #endregion

    #region 몬스터 상태창 처리
    public class MonsterHitEvent : UnityEvent<CEnemyPara> { }
    MonsterHitEvent monsterHitEvent = new MonsterHitEvent();
    [SerializeField] CStatusViewer _monsterStatusViewer;


    private void SetMonsterStatusViewer()
    {
        monsterHitEvent.AddListener(_monsterStatusViewer.Change);
        _monsterStatusViewer.SetActive(false);
    }

    public void MonsterHit(CEnemyPara cPara)
    {
        _monsterStatusViewer.SetActive(true);
        monsterHitEvent.Invoke(cPara);
        CancelInvoke("OffStatus");
        Invoke("OffStatus", 3.0f);
    }

    private void OffStatus()
    {
        _monsterStatusViewer.SetActive(false);
    }
    #endregion

    #region 몬스터에게 명령 내리기
    /// <summary>
    /// 몬스터(monsterID)에게 플레이어(targetPlayerNumber)에게 특정 행동(actionNumber)을 하도록 명령한다
    /// </summary>
    /// <param name="monsterID">대상 몬스터 번호</param>
    /// <param name="actionNumber">몬스터의 행동</param>
    /// <param name="targetPlayerNumber">행동 대상</param>
    public void OrderAction(int monsterID, int actionNumber, int targetPlayerNumber)
    {
        
    }

    /// <summary>
    /// 해당 몬스터를 죽음 처리한다
    /// </summary>
    /// <param name="monsterID">대상 몬스터의 번호</param>
    public void KillMonster(int monsterID)
    {

    }
    #endregion

    #region 디버그용
    public void Say()
    {
        foreach (var monster in _monsterList)
        {
            Debug.Log($"{monster.id} - {monster.mObject.GetComponent<CEnemyPara>()._name}");
        }
    }

    public void DestroyAllMonsters()
    {
        
    }
    #endregion
}