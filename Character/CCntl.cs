using System;
using System.Collections;
using UnityEngine;
[System.Serializable]

public class CCntl : MonoBehaviour
{
    GameObject _player;
    CPlayerPara _myPara;
    CEnemyPara _curEnemyPara;
    //마우스 클릭 지점, 플레이어가 이동할 목적지의 좌표를 저장할 예정
    Vector3 _curTargetPos;
    GameObject _curEnemy;
    public GameObject _dummy;

    float _exitTime;

    public Animator _anim;
    public AnimatorStateInfo _currentBaseState;     // 기본 레이어에 사용되는 애니메이터의 현재 상테에 대한 참조

    string _aniName;

    static int _idleState = Animator.StringToHash("Base Layer.Idle");
    static int _runState = Animator.StringToHash("Base Layer.Run");
    static int _attackIdleState = Animator.StringToHash("Base Layer.AttackIdle");
    static int _attackState = Animator.StringToHash("Base Layer.Attack");
    static int _skillState = Animator.StringToHash("Base Layer.Skill");
    static int _warpState = Animator.StringToHash("Base Layer.Warp");
    static int _getHitState = Animator.StringToHash("Base Layer.Hit");
    public static int _deadState = Animator.StringToHash("Base Layer.Death");

    float _rotAnglePerSecond = 1080f;
    float _moveSpeed = 2f; //초당 2미터의 속도로 이동
    float _attackDistance = 3f;

    void Start()
    {
        // 플레이어 태그를 가진 개체를 받아옴
        _anim = GetComponent<Animator>();
        _player = gameObject;
        _myPara = transform.GetChild(0).GetComponent<CPlayerPara>();
        _myPara.InitPara();
        _myPara.deadEvent.AddListener(ChangeToPlayerDead);
        _curTargetPos = transform.position;
    }

    public void CurrentEnemyDead()
    {
        //CManager.instance.ChangeCurrentTarget(_curEnemy);
        CManager.instance.DeselectAllMonsters();
    }

    private void ChangeToPlayerDead()
    {
        print("player was dead");
        _dummy.SetActive(true);
        _player.transform.GetChild(0).gameObject.SetActive(false);
    }
    
    public void TargetEnemy(GameObject Enemy)
    {
        //Debug.Log(Enemy);
        if (_curEnemy != null && _curEnemy == Enemy)
        {
            return;
        }
        // 적(몬스터)의 파라미터를 변수에 저장
        _curEnemyPara = Enemy.GetComponent<CEnemyPara>();
        _anim.SetFloat("Distance", 0f);
        if (_curEnemyPara != null)
        {
            if (_curEnemyPara._isDead == false)
            {
                _curEnemy = Enemy;
                _curTargetPos = _curEnemy.transform.position;
                CManager.instance.ChangeCurrentTarget(_curEnemy);
            }
        }
        else
        {
            _curEnemyPara = null;
        }
    }

    public void AttackCalculate(Collider obj)
    {
        _curEnemy = obj.gameObject.transform.root.gameObject;
        Debug.Log(_curEnemy);
        if (_curEnemy == null)
        {
            return;
        }
        _curEnemy.GetComponent<CEnemyPara>().ShowHitEffect();
        _curEnemyPara = _curEnemy.GetComponent<CEnemyPara>();
        float attackPower = _myPara.GetRandomAttack(_curEnemyPara._eType, _myPara._eType);
        _curEnemyPara.SetEnemyAttack(attackPower);
    }

    public void MoveTo(Vector3 tPos)
    {
        //Vector3.MoveTowards(시작 지점, 목표지점, 최대 이동거리)
        transform.position = Vector3.MoveTowards(transform.position,
                             _curTargetPos, _moveSpeed * Time.deltaTime);

        if (_curEnemy != null)
        {
            _curEnemy.GetComponent<CEnemyPara>().HideSelection();
        }

        if (_currentBaseState.nameHash == _deadState)
        {
            return;
        }
        _curEnemy = null;
        _curTargetPos = tPos;
    }

    public void SkillAction(int actionNumber, Vector3 targetPos)
    {
        TurnTo(targetPos);
        _anim.SetInteger("Action", actionNumber);
        _aniName = "Skill"; _exitTime = 0.3f;
        StartCoroutine(CheckAnimationState());
    }

    private void MoveState()
    {
        TurnToDestination();
        MoveToDestination();
    }

    private void CheckClick()
    {
        // Ray API 참고 직선 방향으로 나아가는 무한대 길이의 선
        // 카메라 클래스의 메인 카메라로 부터의 스크린의 점을 통해 레이 반환
        // 여기서 스크린의 점은, Input.mousePosition으로 받음
        // 포지션.z를 무시한 값임.
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            string s = hit.collider.gameObject.name;
            string s2 = hit.collider.gameObject.tag;
            bool stringExists = s2.Contains("Terrain");
            // 누른게 지형이면, 이동함.
            if (stringExists)
            {
                MoveTo(hit.point);
            }
            else if (s2 == "Monster")//마우스 클릭한 대상이 적 캐릭터인 경우
            {
                TargetEnemy(hit.collider.gameObject);
            }
        }
    }

    private void RotateMotion()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            TurnTo(hit.point);
            //            player.GetComponent<CPlayerFSM>().SkillState();
        }
    }

    /*
    Ray ray;

    private void WarpMotion()
    {
 //       Instantiate(spell[0], player.transform.position + new Vector3(0, 0.2f, 0), Quaternion.identity);
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Invoke("WarpToDestination", 1f);
    }

    private void WarpToDestination()
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            string s = hit.collider.gameObject.name;
            bool stringExists = s.Contains("Terrain");
            // 누른게 지형이면, 이동함.
            if (stringExists)
            {
//                Instantiate(spell[0], hit.point + new Vector3(0, 0.2f, 0), Quaternion.identity);
                player.transform.position = hit.point;
                curTargetPos = player.transform.position;
            }
        }
    }
    */
    private float DistanceMovingPosition()
    {
        return Vector3.Distance(transform.position, _curTargetPos);
    }
    
    public void hitEnemyAttack()
    {
        _anim.SetBool("Hit", true);
        _aniName = "Hit"; _exitTime = 0.1f;
        StartCoroutine(CheckAnimationState());
    }

    private void TurnTo(Vector3 tPos)
    {
        transform.LookAt(tPos);
    }

    private void MoveToDestination()
    {
        //Vector3.MoveTowards(시작지점, 목표지점,최대이동거리)
        transform.position = Vector3.MoveTowards(transform.position, _curTargetPos, _moveSpeed * Time.deltaTime);

    }

    private void TurnToDestination()
    {
        // 눌렀을 떄,  y축으로 캐릭터가 기울어짐을 방지하기 위함
        _curTargetPos = _curTargetPos - new Vector3(0f, _curTargetPos.y, 0f);
        // Quaternion lookRotation(회전할 목표 방향) : 목표 방향은 목적지 위치에서 자신의 위치를 빼면 구함
        Quaternion lookRotation = Quaternion.LookRotation(_curTargetPos - transform.position);
        //Quaternion.RotateTowards(현재의 rotation값, 최종 목표 rotation 값, 최대 회전각)
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation,
                                                        Time.deltaTime * _rotAnglePerSecond);
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Wall")
        {
            Debug.Log("벽 충돌");
            TurnTo(new Vector3(0f, curTargetPos.y ,0f)-curTargetPos);
            curTargetPos = transform.position;
        }
    }*/

    IEnumerator CheckAnimationState()
    {
        while (!_anim.GetCurrentAnimatorStateInfo(0)
        .IsName(_aniName))
        {
            //전환 중일 때 실행되는 부분
            yield return null;
        }
        while (_anim.GetCurrentAnimatorStateInfo(0)
        .normalizedTime < _exitTime)
        {
            //애니메이션 재생 중 실행되는 부분
            yield return null;
        }
        while (_anim.GetCurrentAnimatorStateInfo(0)
        .normalizedTime < _exitTime + 0.1f)
        {
            _anim.SetInteger("Action", 0);
            _anim.SetBool("Hit", false);
            yield return null;
        }
        _curTargetPos = transform.position;
        yield break;
    }

    private void Update()
    {
        _currentBaseState = _anim.GetCurrentAnimatorStateInfo(0); // set our currentState variable to the current state of the Base Layer (0) of animation
        _anim.SetFloat("Distance", DistanceMovingPosition());
        _anim.SetFloat("Hp", _myPara._curHp);
        
        if (_currentBaseState.nameHash == _runState)
        {
            MoveState();
        }
    }
}