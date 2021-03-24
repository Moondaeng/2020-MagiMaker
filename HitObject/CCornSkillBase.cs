using UnityEngine;

public class CCornSkillBase : CHitObjectBase
{
    [Tooltip("시작 시간 애니메이션이랑 사운드에 사용")]
    [SerializeField] protected float _startTime = 1f;

    [Tooltip("끝 시간. 애니메이션이랑 사운드에 사용")]
    [SerializeField] protected float _stopTime = 3f;

    [Tooltip("사정 거리")]
    [SerializeField] public float _distance = 5f;

    [Tooltip("공격 간격")]
    [SerializeField] public float _attackTickTime = 1f;

    [Tooltip("폭발 파티클")]
    [SerializeField] private ParticleSystem _explosion = null;

    [Tooltip("유저가 사용 시 스태프에서 나갈거냐?")]
    [SerializeField] private bool _staffAnchor;

    private CCntl _myControl;
    private float _startTimeMultiplier;
    private float _startTimeIncrement;

    private float _stopTimeMultiplier;
    private float _stopTimeIncrement;

    Vector3 forward;
    Vector3 up;

    [SerializeField] private float _actionPeriod;
    private static readonly int EXPECT_FRAMERATE = 60;
    private int operateStack = 0;

    protected void Awake()
    {
        // 0으로 초기화되서 무한 호출되는 경우 방지
        if (_actionPeriod == 0)
        {
            _actionPeriod = 1f;
        }
    }

    private void FixedUpdate()
    {
        operateStack = (operateStack + 1) % GetFramePerActionPeriod();
    }

    private int GetFramePerActionPeriod()
    {
        return (int)(EXPECT_FRAMERATE * _actionPeriod);
    }

    private void OnTriggerStay(Collider other)
    {
        if (operateStack == 0 && !IsTriggeredRecently(other))
        {
            GetUseEffect(other);
        }
    }

    // 데미지 2번 줄 필요 없음
    protected override void OnTriggerEnter(Collider other)
    {
        return;
    }

    protected override void Start()
    {
        _stopTimeMultiplier = 1.0f / _stopTime;
        _startTimeMultiplier = 1.0f / _startTime;
        base.Start();
    }

    //public Vector3 PointOnCircle(float x, float z, Vector3 angle)
    //{
    //    float xPos = x + Mathf.Sin(angle.y * Mathf.PI / 180) * ColliderSet.arg2;
    //    float zPos = z + Mathf.Cos(angle.y * Mathf.PI / 180) * ColliderSet.arg2;
    //    Vector3 pos = new Vector3(xPos, 0, zPos);
    //    Debug.Log(gameObject.name);

    //    return pos;
    //}


    protected override void Update()
    {
        //if (Stopping)
        //{
        //    // increase the stop time
        //    _stopTimeIncrement += Time.deltaTime;
        //    if (_stopTimeIncrement < _stopTime)
        //    {
        //        StopPercent = _stopTimeIncrement * _stopTimeMultiplier;
        //    }
        //}
        //else if (Starting)
        //{
        //    // increase the start time
        //    _startTimeIncrement += Time.deltaTime;
        //    if (_startTimeIncrement < _startTime)
        //    {
        //        StartPercent = _startTimeIncrement * _startTimeMultiplier;
        //    }
        //    else
        //    {
        //        Starting = false;
        //    }
        //}
        base.Update();

        //float speed = _flameThrowerGroupController.degreePerSecond * Time.deltaTime;
        //transform.Rotate(_flameThrowerGroupController._flamethrowerDir * speed);
    }
}
