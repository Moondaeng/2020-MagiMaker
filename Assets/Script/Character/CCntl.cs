using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class CCntl : MonoBehaviour
{
    GameObject player;
    [SerializeField]
    private GameObject[] spell;
    CPlayerPara myPara;
    CEnemyPara curEnemyPara;
    //마우스 클릭 지점, 플레이어가 이동할 목적지의 좌표를 저장할 예정
    Vector3 curTargetPos;
    GameObject curEnemy;
    float exitTime;

    public Animator anim;
    public AnimatorStateInfo currentBaseState;     // 기본 레이어에 사용되는 애니메이터의 현재 상테에 대한 참조

    string aniName;

    static int idleState = Animator.StringToHash("Base Layer.Idle");
    static int runState = Animator.StringToHash("Base Layer.Run");
    static int attackIdleState = Animator.StringToHash("Base Layer.AttackIdle");
    static int attackState = Animator.StringToHash("Base Layer.Attack");
    static int skillState = Animator.StringToHash("Base Layer.Skill");
    static int warpState = Animator.StringToHash("Base Layer.Warp");
    static int getHitState = Animator.StringToHash("Base Layer.Hit");
    public static int deadState = Animator.StringToHash("Base Layer.Death");

    public float rotAnglePerSecond = 1080f; //1초에 플레이어의 방향을 360도 회전
    public float moveSpeed; //초당 2미터의 속도로 이동
    public float attackDistance = 3f;

    void Start()
    {
        // 플레이어 태그를 가진 개체를 받아옴
        anim = GetComponent<Animator>();
        player = gameObject;
        myPara = GetComponent<CPlayerPara>();
        myPara.InitPara();
        myPara.deadEvent.AddListener(ChangeToPlayerDead);
        curTargetPos = transform.position;
    }

    private void ChangeToPlayerDead()
    {
        print("player was dead");
        Invoke("turnToEarth", 1f);
    }

    private void turnToEarth()
    {
        player.active = false;
    }

    public void SkillAction(int actionNumber)
    {
        TurnTo(curTargetPos);
        anim.SetInteger("Action", actionNumber);
        aniName = "Skill"; exitTime = 0.3f;
        StartCoroutine(CheckAnimationState());
    }

    public void WarpAction()
    {
        TurnTo(curTargetPos);
        anim.SetInteger("Action", 4);
        aniName = "Warp"; exitTime = 0.8f;
        WarpMotion();
        StartCoroutine(CheckAnimationState());
    }

    public void TargetEnemy(GameObject Enemy)
    {
        //Debug.Log(Enemy);
        if (curEnemy != null && curEnemy == Enemy)
        {
            return;
        }
        // 적(몬스터)의 파라미터를 변수에 저장
        curEnemyPara = Enemy.GetComponent<CEnemyPara>();
        anim.SetFloat("Distance", 0f);
        if (curEnemyPara != null)
        {
            if (curEnemyPara._isDead == false)
            {
                curEnemy = Enemy;
                curTargetPos = curEnemy.transform.position;
                CManager.instance.ChangeCurrentTarget(curEnemy);
            }
        }
        else
        {
            curEnemyPara = null;
        }
    }

    public void AttackCalculate(Collider obj)
    {
        curEnemy = obj.gameObject.transform.root.gameObject;
        Debug.Log(curEnemy);
        if (curEnemy == null)
        {
            return;
        }
        curEnemy.GetComponent<CEnemyPara>().ShowHitEffect();
        curEnemyPara = curEnemy.GetComponent<CEnemyPara>();
        float attackPower = myPara.GetRandomAttack(curEnemyPara._eType, myPara._eType);
        curEnemyPara.SetEnemyAttack(attackPower);
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

    public void MoveTo(Vector3 tPos)
    {
        //Vector3.MoveTowards(시작 지점, 목표지점, 최대 이동거리)
        transform.position = Vector3.MoveTowards(transform.position,
                             curTargetPos, moveSpeed * Time.deltaTime);

        if (curEnemy != null)
        {
            curEnemy.GetComponent<CEnemyPara>().HideSelection();
        }

        if (currentBaseState.nameHash == deadState)
        {
            return;
        }
        curEnemy = null;
        curTargetPos = tPos;
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

    private float DistanceMovingPosition()
    {
        return Vector3.Distance(transform.position, curTargetPos);
    }

    public void hitEnemyAttack()
    {
        anim.SetBool("Hit", true);
        aniName = "Hit"; exitTime = 0.1f;
        StartCoroutine(CheckAnimationState());
    }

    private void TurnTo(Vector3 tPos)
    {
        transform.LookAt(tPos);
    }

    private void MoveToDestination()
    {
        //Vector3.MoveTowards(시작지점, 목표지점,최대이동거리)
        transform.position = Vector3.MoveTowards(transform.position, curTargetPos, moveSpeed * Time.deltaTime);

    }

    private void TurnToDestination()
    {
        // 눌렀을 떄,  y축으로 캐릭터가 기울어짐을 방지하기 위함
        curTargetPos = curTargetPos - new Vector3(0f, curTargetPos.y, 0f);
        // Quaternion lookRotation(회전할 목표 방향) : 목표 방향은 목적지 위치에서 자신의 위치를 빼면 구함
        Quaternion lookRotation = Quaternion.LookRotation(curTargetPos - transform.position);
        //Quaternion.RotateTowards(현재의 rotation값, 최종 목표 rotation 값, 최대 회전각)
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation,
                                                        Time.deltaTime * rotAnglePerSecond);
    }

    IEnumerator CheckAnimationState()
    {
        while (!anim.GetCurrentAnimatorStateInfo(0)
        .IsName(aniName))
        {
            //전환 중일 때 실행되는 부분
            yield return null;
        }
        while (anim.GetCurrentAnimatorStateInfo(0)
        .normalizedTime < exitTime)
        {
            //애니메이션 재생 중 실행되는 부분
            yield return null;
        }
        anim.SetInteger("Action", 0);
        anim.SetBool("Hit", false);
        curTargetPos = transform.position;
        yield break;
    }

    private void Update()
    {
        currentBaseState = anim.GetCurrentAnimatorStateInfo(0); // set our currentState variable to the current state of the Base Layer (0) of animation
        anim.SetFloat("Distance", DistanceMovingPosition());
        anim.SetFloat("Hp", myPara._curHp);

        if (currentBaseState.nameHash == runState)
        {
            MoveState();
        }
    }
}
