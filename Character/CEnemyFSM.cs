using System.Collections;
using System.Collections.Generic;
using UnityEngine;

<<<<<<< HEAD
public class CEnemyFSM : CharacterFSM
{
    CSkeletonAni myAni;
    Transform player;
    CEnemyPara myPara;
    CPlayerPara playerPara;

    float chaseDistance = 20f; // 플레이어를 향해 몬스터가 추적을 시작할 거리
    float attackDistance = 2.5f; // 플레이어가 안쪽으로 들오오게 되면 공격을 시작
    float reChaseDistance = 5f; // 플레이어가 도망 갈 경우 얼마나 떨어져야 다시 추적

    GameObject myRespawn;
    public int spawnID { get; set; }
    Vector3 originPos;
    int WhoFlag;

    public override void InitStat()
    {
        moveSpeed = 5f;
        attackDelay = 1.5f;
        attackTimer = 3f;
        attackDistance = 4.5f;
        
        myAni = GetComponent<CSkeletonAni>();

        myPara = GetComponent<CEnemyPara>();
        myPara.deadEvent.AddListener(CallDeadEvent);
        // 첫 상태를 Idle
        ChangeState(EState.Idle, CSkeletonAni.IDLE);
        // player 태그를 가진 사람을 변수로 받음
        player = GameObject.FindGameObjectWithTag("Player").transform;
        // player의 스텟을 받아옴
        playerPara = player.gameObject.GetComponent<CPlayerPara>();
    }
    
    public void SetRespawn(GameObject respawn, int spawnID, Vector3 originPos)
    {
        myRespawn = respawn;
        this.spawnID = spawnID;
        this.originPos = originPos;
        //Debug.Log(respawn + " " + spawnID + " " + originPos + " ");
    }

    void CallDeadEvent()
    {
        ChangeState(EState.Dead, CSkeletonAni.DEATH);
        player.gameObject.SendMessage("CurrentEnemyDead");
        StartCoroutine("RemoveMe");
    }

    IEnumerable RemoveMe()
    {
        yield return new WaitForSeconds(2f);

        ChangeState(EState.Idle, CSkeletonAni.IDLE);

        myRespawn.GetComponent<CRespawn>().RemoveMonster(spawnID);
    }

    public override void AttackCalculate()
    {
        playerPara.SetEnemyAttack(myPara.GetRandomAttack(playerPara.eType, myPara.eType));
    }
    
    public void ChangeState(EState newState, string aniName)
    {
        if (currentState == newState)
        {
            return;
        }
        myAni.ChangeAni(aniName);
        currentState = newState;
    }
    
    public override void UpdateState()
    {
        switch (currentState)
        {
            case EState.Idle:
                IdleState();
                break;
            case EState.Chase:
                ChaseState();
                break;
            case EState.Attack:
                AttackState();
                break;
            case EState.Dead:
                DeadState();
                break;
        }
    }

    public override void IdleState()
    {
        if (GetDistanceFromPlayer() < chaseDistance)
        {
            ChangeState(EState.Chase, CSkeletonAni.WALK);
        }
    }

    public void ChaseState()
    {
        myAni.ChangeAni(CSkeletonAni.WALK);
        //몬스터가 공격 가능 거리 안으로 들어가면 공격 상태
        if (GetDistanceFromPlayer() < attackDistance)
        {
            ChangeState(EState.Attack, CSkeletonAni.ATTACK);
        }
        else
        {
            TurnToDestination();
            MoveToDestination();
        }
    }

    // 거리재는 함수로 재추격 거리보다 길 경우 뛰어가고
    // 아니면 공격하는데, 이게 쿨타임이 있음 2초 설정
    // 추후 이 함수 안에서, 패턴을 나눠야 할 듯 하다.
    public override void AttackState()
    {
        if (player.GetComponent<CPlayerFSM>().currentState == CPlayerFSM.EState.Dead)
        {
            ChangeState(EState.Idle, CSkeletonAni.IDLE);
        }
        if (GetDistanceFromPlayer() > reChaseDistance)
        {
            attackTimer = 0f;
            ChangeState(EState.Chase, CSkeletonAni.RUN);
        }
        else
        {
            if (attackTimer > attackDelay)
            {
                transform.LookAt(player.position);
                myAni.ChangeAni(CSkeletonAni.ATTACK);
            }
            else
            {
                myAni.ChangeAni(CSkeletonAni.STAND);
            }
            attackTimer += Time.deltaTime;
        }
    }


    // 죽으면 선택안되게끔 하려고함
    public override void DeadState()
    {
        //GetComponent<BoxCollider>().enabled = false;
        GetComponent<Collider>().enabled = false;
    }

    public override void TurnToDestination()
    {
        Quaternion lookRotation = Quaternion.LookRotation(player.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, Time.deltaTime * rotAnglePerSecond);
    }

    public override void MoveToDestination()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
    }

    //플레이어와 거리을 재는 함수
    float GetDistanceFromPlayer()
    {
        return Vector3.Distance(transform.position, player.position);
=======
public class CEnemyFSM : MonoBehaviour
{
    protected Animator _anim;
    protected AnimatorStateInfo _currentBaseState;     // 기본 레이어에 사용되는 애니메이터의 현재 상테에 대한 참조

    protected GameObject _player;
    protected CEnemyPara _myPara;
    protected CPlayerPara _playerPara;
    protected GameObject _myRespawn;

    public int _spawnID { get; set; }
    public float _rotAnglePerSecond = 360f; //1초에 플레이어의 방향을 360도 회전
    public float _moveSpeed { get; set; } //초당 2미터의 속도로 이동
    public float _attackDelay { get; set; } // 공격을 한번 하고 다시 공격할 때까지의 지연
    public float _attackTimer { get; set; } //공격을 하고 난 뒤에 경과되는 시간을 계산하기 위한 변수
    public float _attackDistance { get; set; } // 공격 거리 (적과의 거리)

    public int _idleState { get; set; }
    public int _idleBattleState { get; set; }
    public int _standState { get; set; }
    public int _walkState { get; set; }
    public int _runState { get; set; }
    public int _attackState { get; set; }
    public int _skillState { get; set; }
    public int _gethitState { get; set; }
    public int _deadState1 { get; set; }
    public int _deadState2 { get; set; }

    public List<GameObject> _players = new List<GameObject>();
    
    void Start()
    {
        InitStat();
    }

    protected virtual void InitStat()
    {

    }
    public virtual void MoveState()
    {
        TurnToDestination();
        MoveToDestination();
    }

    protected virtual void MoveToDestination()
    {
        transform.position = Vector3.MoveTowards(transform.position,
            _player.transform.position, _moveSpeed * Time.deltaTime);
    }

    protected virtual void TurnToDestination()
    {
        Quaternion lookRotation =
            Quaternion.LookRotation(_player.transform.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation,
            lookRotation, Time.deltaTime * _rotAnglePerSecond);
    }

    public List<GameObject> DetectPlayer(List<GameObject> players)
    {
        players.Clear();
        GameObject temp = null;
        for (int i = 1; i < 5; i++)
        {
            if ((temp = GameObject.Find("Player" + i.ToString())) != null)
            {
                players.Add(temp);
            }
        }
        return players;
    }

    public List<float> CalculateDistance(List<GameObject> players)
    {
        List<float> distances = new List<float>();
        for (int i = 0; i < players.Count; i++)
        {
            distances.Add(Vector3.Distance(
                players[i].transform.position, transform.position));
        }
        //_distances.Sort();
        return distances;
    }

    protected bool IsTargetInSight(float SightAngle, Transform Target)
    {
        //타겟의 방향 
        Vector3 targetDir = (Target.position - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, targetDir);

        //내적을 이용한 각 계산하기
        // thetha = cos^-1( a dot b / |a||b|)
        float theta = Mathf.Acos(dot) * Mathf.Rad2Deg;
        
        if (theta <= SightAngle) return true;
        else return false;
    }

    protected virtual void Update()
    {
        _players = DetectPlayer(_players);
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
    }
}