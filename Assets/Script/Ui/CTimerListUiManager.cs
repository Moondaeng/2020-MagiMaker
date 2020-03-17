using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Timer를 UI로 나타내는 클래스
public class CTimerListUiManager : MonoBehaviour
{
    public class TimerUi
    {
        public int timerRegisteredNumber;
        public GameObject timerDrawer;
    }

    // 현재 Canvas 왼쪽 아래 기준으로 위치가 설정됨
    // 그러므로 피벗 설정 옵션을 넣어주든 하는게 필요함
    [SerializeField]
    protected int TimerUiPosX = 1084;
    [SerializeField]
    protected int TimerUiPosY = 284;
    [SerializeField]
    protected int TimerUiSquareSize = 50;
    [SerializeField]
    protected int TimerUiSpaceSize = 5;

    // 갱신 시간 조절
    protected const float _updateTime = 0.1f;
    protected int _updateThreshold;
    protected int _updateCount;

    protected Transform _uiCanvas;

    protected CTimer _timer;
    protected LinkedList<TimerUi> _timerUiList = new LinkedList<TimerUi>();

    // 임시 : 그릴 타이머 모양 지정
    public GameObject timerDrawer;

    protected void Awake()
    {
        _updateThreshold = (int)(_updateTime / Time.fixedDeltaTime);
        _updateCount = 0;
    }

    protected void Start()
    {
        _uiCanvas = GameObject.Find("Canvas").transform;
    }

    // ui 갱신
    protected void FixedUpdate()
    {
        _updateCount++;

        // 업데이트 작동 횟수 조절
        if (_updateCount % _updateThreshold != 1)
        {
            return;
        }

        Draw();
    }

    // 추적해서 그릴 타이머 등록
    // 이후 등록한 대상의 타이머를 따라 그림
    public void RegisterTimer(string parentName)
    {
        _timer = GameObject.Find(parentName).GetComponent<CTimer>();
        _timer.TimerStart += Register;
        _timer.TimerEnd += Deregister;
    }

    // 버프 이미지 등록
    // LinkedList의 AddLast()에 해당
    protected void Register(int registeredNumber)
    {
        var drawer = Instantiate(timerDrawer);
        // initialize drawer
        drawer.transform.SetParent(_uiCanvas, false);
        drawer.SetActive(true);
        drawer.GetComponent<Image>().rectTransform.sizeDelta
            = new Vector2(TimerUiSquareSize, TimerUiSquareSize);
        drawer.GetComponent<CTimerDrawer>().CooldownEnable();

        // 링크드리스트에 추가하기
        var timerUi = new TimerUi()
        {
            timerRegisteredNumber = registeredNumber,
            timerDrawer = drawer
        };
        _timerUiList.AddLast(timerUi);

        Relocate();
    }

    // 버프 이미지 등록 해제
    // LinkedList의 Remove()에 해당
    protected void Deregister(int registeredNumber)
    {
        var deregistered = FindByNumber(registeredNumber);
        if (deregistered == null)
            return;

        Destroy(deregistered.Value.timerDrawer);
        _timerUiList.Remove(deregistered);

        Relocate();
    }

    // 모든 타이머 UI에 시간을 그리는 명령을 내림
    // 단, 활성화되지 않은 경우 그리지 않음
    protected void Draw()
    {
        var timer = _timerUiList.First;
        while(timer != null)
        {
            // 그리기 명령
            var drawer = timer.Value.timerDrawer.GetComponent<CTimerDrawer>();
            drawer.Draw(_timer.GetCurrentCooldown(timer.Value.timerRegisteredNumber), 
                _timer.GetMaxCooldown(timer.Value.timerRegisteredNumber));
            timer = timer.Next;
        }
    }

    // 타이머 UI를 재배치
    // Register, Deregister에 사용
    protected void Relocate()
    {
        var drawer = _timerUiList.First;
        float posX = TimerUiPosX;
        float posY = TimerUiPosY;
        while (drawer != null)
        {
            var drawerPos = drawer.Value.timerDrawer.transform.position;
            drawerPos.x = posX;
            drawerPos.y = posY;
            drawer.Value.timerDrawer.transform.position = drawerPos;

            posX += TimerUiSquareSize + TimerUiSpaceSize;
            drawer = drawer.Next;
        }
    }

    protected LinkedListNode<TimerUi> FindByNumber(int registeredNumber)
    {
        var find = _timerUiList.First;
        while (find != null)
        {
            // 탐색
            if (find.Value.timerRegisteredNumber == registeredNumber)
            {
                return find;
            }
            find = find.Next;
        }
        return find;
    }
}
