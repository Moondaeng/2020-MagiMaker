using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CBuffTimerListUI : MonoBehaviour
{
    public class TimerUi
    {
        public int timerRegisteredNumber;
        public GameObject timerDrawer;
    }

    public RectTransform BuffUiPosition;
    public int squareSize;
    public int spaceSize;

    // 갱신 시간 조절
    protected const float _updateTime = 0.1f;
    protected int _updateThreshold;
    protected int _updateCount;

    protected Transform _uiCanvas;
    protected CBuffTimer _timer;
    protected LinkedList<TimerUi> _timerUiList = new LinkedList<TimerUi>();

    // 임시 : 그릴 타이머 모양 지정
    public GameObject timerDrawer;

    protected void Awake()
    {
        _updateThreshold = (int)(_updateTime / Time.fixedDeltaTime);
        _updateCount = 0;
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

    public void SetCanvas(Transform canvasTransform)
    {
        _uiCanvas = canvasTransform;
    }

    // 추적해서 그릴 타이머 등록
    // 이후 등록한 대상의 타이머를 따라 그림
    public void RegisterTimer(GameObject timerOwner)
    {
        _timer = timerOwner.GetComponent<CBuffTimer>();
        _timer.TimerStart += Register;
        _timer.TimerEnd += Deregister;
    }

    public virtual void DeregisterTimer(GameObject timerOwner)
    {
        _timer = timerOwner.GetComponent<CBuffTimer>();
        _timer.TimerStart -= Register;
        _timer.TimerEnd -= Deregister;

        // 기존에 있던 Ui 전부 삭제
        //while(_timerUiList.First != null)
        //{
        //    var first = _timerUiList.First;
        //    Destroy(first.Value.timerDrawer);
        //    _timerUiList.RemoveFirst();
        //}
    }

    // 버프 이미지 등록
    protected virtual void Register(int registeredNumber)
    {
        var drawer = Instantiate(timerDrawer);
        // initialize drawer
        drawer.transform.SetParent(_uiCanvas, false);
        drawer.SetActive(true);
        drawer.GetComponent<Image>().sprite = GetImageByRegisterNumber(registeredNumber);
        drawer.GetComponent<CTimerDrawer>().CooldownEnable();
        if(_timer.IsStackableBuff(registeredNumber) == true)
        {
            drawer.GetComponent<CTimerDrawer>().stackText.enabled = true;
        }
        var newPivot = drawer.GetComponent<RectTransform>().pivot;
        newPivot.x = 0;
        newPivot.y = 0;
        drawer.GetComponent<RectTransform>().pivot = newPivot;
        drawer.GetComponent<CTimerDrawer>().SetSize((int)BuffUiPosition.rect.height);
        

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
        var timerUi = _timerUiList.First;
        while (timerUi != null)
        {
            // 그리기 명령
            var drawer = timerUi.Value.timerDrawer.GetComponent<CTimerDrawer>();
            drawer.Draw(_timer.GetCurrentCooldown(timerUi.Value.timerRegisteredNumber),
                _timer.GetMaxCooldown(timerUi.Value.timerRegisteredNumber),
                _timer.GetBuffStack(timerUi.Value.timerRegisteredNumber));
            timerUi = timerUi.Next;
        }
    }

    // 타이머 UI를 재배치
    // Register, Deregister에 사용
    protected void Relocate()
    {
        var drawer = _timerUiList.First;
        float posX = BuffUiPosition.position.x;
        float posY = BuffUiPosition.position.y;
        while (drawer != null)
        {
            var drawerPos = drawer.Value.timerDrawer.transform.position;
            drawerPos.x = posX;
            drawerPos.y = posY;
            drawer.Value.timerDrawer.transform.position = drawerPos;

            posX += squareSize + spaceSize;
            drawer = drawer.Next;
        }
    }

    protected LinkedListNode<TimerUi> FindByNumber(int registeredNumber)
    {
        for (var find = _timerUiList.First; find != null; find = find.Next)
            if (find.Value.timerRegisteredNumber == registeredNumber) return find;
        return null;
    }

    protected Sprite GetImageByRegisterNumber(int registeredNumber)
    {
        if (!CBuffList.BuffUIList.TryGetValue(registeredNumber, out string spritePath))
        {
            return Resources.Load<Sprite>("Clean Vector Icons/T_0_empty_");
        }
        else
            return Resources.Load<Sprite>(spritePath);
    }
}
