using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/* @help
 * 이 클래스는 CRespawn에서 사용되는 맵 전체의 몬스터 배열을 관리하는 클래스이다.
 * 
 */
public class CMonsterManager : MonoBehaviour
{
    public static int _idleState = Animator.StringToHash("Base Layer.Idle");
    public static int _standState = Animator.StringToHash("Base Layer.MovingSub.Stand");
    public static int _traceState = Animator.StringToHash("Base Layer.MovingSub.trace");
    public static int _attackState1 = Animator.StringToHash("Base Layer.AttackSub.Attack1");
    public static int _attackState2 = Animator.StringToHash("Base Layer.AttackSub.Attack2");
    public static int _waitState = Animator.StringToHash("Base Layer.AttackSub.AttackWait");
    public static int _skillState1 = Animator.StringToHash("Base Layer.AnySub.Skill1");
    public static int _skillState2 = Animator.StringToHash("Base Layer.AnySub.Skill2");
    public static int _skillState3 = Animator.StringToHash("Base Layer.AnySub.Skill3");
    public static int _skillWaitState1 = Animator.StringToHash("Base Layer.AnySub.SkillWait1");
    public static int _skillWaitState2 = Animator.StringToHash("Base Layer.AnySub.SkillWait2");
    public static int _skillWaitState3 = Animator.StringToHash("Base Layer.AnySub.SkillWait3");
    public static int _gethitState = Animator.StringToHash("Base Layer.AnySub.GetHit");
    public static int _deadState = Animator.StringToHash("Base Layer.AnySub.Dead");
    [HideInInspector]
    public bool _IsOrder = false;

    private class MonsterInfo
    {
        public int id;
        public GameObject mObject;
        public Vector3 mPosition;

        public MonsterInfo(int id, GameObject monsterObject, Vector3 monsterPosition)
        {
            this.id = id;
            mObject = monsterObject;
            mPosition = monsterPosition;
        }
    }

    private class MonsterActionInfo
    {
        public int mActionHash;
        public string mActionName;

        public MonsterActionInfo(int ActionHash, string ActionName)
        {

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
        //SetMonsterStatusViewer();
    }
    #endregion

    #region 몬스터 추가 / 제거 / 현재 개체수 출력
    private int CreateMonsterID()
    {
        return _monsterCount++;
    }

    // 현재 존재하는 방안의 몬스터의 수를 알려줌
    public int GetMonsterCount()
    {
        return _monsterCount;
    }

    public int AddMonsterInfo(GameObject mon)
    {
        int monsterNum = CreateMonsterID();
        _monsterList.Add(new MonsterInfo(monsterNum, mon, mon.transform.position));
        return monsterNum;
    }

    //public List<MonsterInfo> GetMonsterInfo()
    //{
    //    return _monsterList;
    //}

    public GameObject GetMonsterInfo(int monsterID)
    {
        return _monsterList.Find(monsterInfo => monsterInfo.id == monsterID).mObject;
    }

    public void RemoveMonster()
    {
        _monsterList.Clear();
    }

    public void RemoveMonster(int monsterID)
    {
        Debug.Log(monsterID);
        _monsterList.Remove(_monsterList.Find(monsterInfo => monsterInfo.id == monsterID));
    }


    #endregion

    #region 몬스터 상태창 처리
    public class MonsterHitEvent : UnityEvent<CEnemyPara> { }
    MonsterHitEvent monsterHitEvent = new MonsterHitEvent();
    //[SerializeField] CStatusViewer _monsterStatusViewer;


    //private void SetMonsterStatusViewer()
    //{
    //    monsterHitEvent.AddListener(_monsterStatusViewer.Change);
    //    _monsterStatusViewer.SetActive(false);
    //}

    //public void MonsterHit(CEnemyPara cPara)
    //{
    //    _monsterStatusViewer.SetActive(true);
    //    monsterHitEvent.Invoke(cPara);
    //    CancelInvoke("OffStatus");
    //    Invoke("OffStatus", 3.0f);
    //}

    //private void OffStatus()
    //{
    //    _monsterStatusViewer.SetActive(false);
    //}
    #endregion

    #region 몬스터에게 명령 내리기
    /// <summary>
    /// 몬스터(monsterID)에게 플레이어(playerID)에게 
    /// 특정 행동(patternNumber)을 하도록 명령한다
    /// </summary>
    /// <param name="monsterID">대상 몬스터 번호</param>
    /// <param name="playerID">몬스터의 행동</param>
    /// <param name="patternNumber">행동 대상</param>

    [HideInInspector]
    public UnityEvent AttackEvent = new UnityEvent();
    [HideInInspector]
    public UnityEvent SkillEvent1 = new UnityEvent();
    [HideInInspector]
    public UnityEvent SkillEvent2 = new UnityEvent();
    [HideInInspector]
    public UnityEvent HitEvent = new UnityEvent();
    
    public void OrderAction(int monsterID, int actionNumber)
    {


    }
    public void OrderAction(int monsterID, int actionNumber, GameObject targetPlayerNumber)
    {

    }

    public void DecideMonsterPattern(int monsterID, int patternNumber)
    {
        
    }
    public void DecideMonsterPattern(int monsterID, int patternNumber, int playerID)
    {

    }

    /// <summary>
    /// 몬스터가 플레이어에게 패턴을 수행시키는 코드
    /// </summary>
    /// <param name="monsterID"></param>
    /// <param name="playerID"></param>
    /// <param name="patternNumber"></param>
    public void CommandPattern(int monsterID, int playerID, int patternNumber)
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
            Debug.Log($"{monster.id} - {monster.mObject.GetComponent<CEnemyPara>().name}");
        }
    }

    public void DestroyAllMonsters()
    {

    }

    #endregion

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            for (int i = 0; i < _monsterList.Count; i++)
            {
                var _ = _monsterList[i].mObject.GetComponent<CEnemyNavFSM>();
                // 몬스터 리스포너에서 몬스터의 리스트를 삭제하는 기능이 없으므로
                // 죽은 몬스터에게 명령을 하지 않게함.
                if (_._isDead == false)
                {
                    _.ReleaseAllAnimatorBools();
                    _IsOrder = true;
                    _.OffCoroutine();
                }
            }
        }

        if (Input.GetKeyDown("2"))
        {
            for (int i = 0; i < _monsterList.Count; i++)
            {
                var _ = _monsterList[i].mObject.GetComponent<CEnemyNavFSM>();
                if (_._isDead == false)
                {
                    _IsOrder = false;
                    _.OnCoroutine();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            for (int i = 0; i < _monsterList.Count; i++)
            {
                var _ = _monsterList[i].mObject.GetComponent<CEnemyNavFSM>();
                if (_._isDead == false)
                {
                    _.ReleaseAllAnimatorBools();
                    HitEvent.Invoke();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            for (int i = 0; i < _monsterList.Count; i++)
            {
                var _ = _monsterList[i].mObject.GetComponent<CEnemyNavFSM>();
                if (_._isDead == false)
                {
                    _.ReleaseAllAnimatorBools();
                    AttackEvent.Invoke();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            for (int i = 0; i < _monsterList.Count; i++)
            {
                var _ = _monsterList[i].mObject.GetComponent<CEnemyNavFSM>();
                if (_._isDead == false)
                {
                    _.ReleaseAllAnimatorBools();
                    SkillEvent1.Invoke();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            for (int i = 0; i < _monsterList.Count; i++)
            {
                var _ = _monsterList[i].mObject.GetComponent<CEnemyNavFSM>();
                if (_._isDead == false)
                {
                    _.ReleaseAllAnimatorBools();
                    SkillEvent2.Invoke();
                }
            }
        }
    }
}