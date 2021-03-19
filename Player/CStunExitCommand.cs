using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CStunExitCommand : MonoBehaviour
{
    #region Properties
    // direction이 true면 right, 아니면 left
    bool _start, _direction, _end;
    int _count, _keyDownCount;
    float _barGauge = 100f;
    public GameObject _left, _right, _barFront, _barBack;
    private Image _leftImage, _rightImage;
    private RawImage _barFrontImage, _barBackImage;
    Color _originLeft, _originRight;
    #endregion
    
    private void Start()
    {
        _keyDownCount = 0;
        _leftImage = _left.GetComponent<Image>();
        _rightImage = _right.GetComponent<Image>();
        _barFrontImage = _barFront.GetComponent<RawImage>();
        _barBackImage = _barBack.GetComponent<RawImage>();
        _originLeft = _leftImage.color;
        _originRight = _rightImage.color;
    }
    
    #region properties 관리하는 것들
    public void Start(int count)
    {
        Debug.Log(_leftImage.type);
        _keyDownCount = 0;
        _start = true;
        _left.SetActive(true);
        _right.SetActive(true);
        _barFront.SetActive(true);
        _barBack.SetActive(true);
        _count = count;
    }

    public void EndTime()
    {
        _start = false;
        ResetAllParameters();
    }

    public void ResetAllParameters()
    {
        _barFrontImage.rectTransform.localScale = new Vector3(2f, 1f, 1f);
        _start = false;
        _left.SetActive(false);
        _right.SetActive(false);
        _barFront.SetActive(false);
        _barBack.SetActive(false);
        SendMessage("ExitStun");
    }
    #endregion

    #region 키 입력 처리
    void LeftKeyDown()
    {
        _keyDownCount++;
        _leftImage.color = _originLeft;
        _rightImage.color = Color.red;
        _direction = true;
    }
    
    void RightKeyDown()
    {
        _keyDownCount++;
        _leftImage.color = Color.red;
        _rightImage.color = _originRight;
        _direction = false;
    }
    #endregion

    #region __Update__
    void Update()
    {
        if (_start)
        {
            _barFrontImage.rectTransform.localScale =
                new Vector3(2f * _barGauge * (_count - _keyDownCount) / (_count * 100f), 1f, 1f);
            if (_keyDownCount == 0)
            {
                if (Input.GetKeyDown(KeyCode.A)) LeftKeyDown();
                else if (Input.GetKeyDown(KeyCode.D)) RightKeyDown();
            }
            else if (_keyDownCount > 0)
            {
                if (Input.GetKeyDown(KeyCode.A) && !_direction) LeftKeyDown();
                else if (Input.GetKeyDown(KeyCode.D) && _direction) RightKeyDown();
            }
            if (_keyDownCount == _count) ResetAllParameters();
        }
    }
    #endregion
}
