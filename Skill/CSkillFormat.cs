using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 스킬을 등록해서 실행하는 포맷
[System.Serializable]
public class CSkillFormat
{
    public delegate void RetentionSkill(GameObject user, Vector3 targetPos);
    public delegate void SkillUseCallback(Vector3 targetPos);

    // CharacterSkill에서 자동으로 설정하는 변수들
    private GameObject _userObject;
    private CSkillTimer _timer;
    private RetentionSkill _usingSkill;
    private SkillUseCallback _skillUseCallback;

    private int _currentStack;
    public int MaxStack { private set; get; }

    private int _timerRegisterNumber;
    [SerializeField, Tooltip("스킬 쿨다운")]
    private float _cooldown;
    [SerializeField, Tooltip("스킬 사용 시 취할 모션 번호")]
    private int _actionNumber;
    [SerializeField, Tooltip("스킬 사용 시 나가는 오브젝트")]
    public GameObject skillObject;

    public CSkillFormat()
    {
        MaxStack = 1;
        _currentStack = 1;
        _timerRegisterNumber = -1;
        _actionNumber = 0;
    }
    private int _animationNumber;

    public CSkillFormat(int registerNumber, float cooldown, GameObject user, int ani)
    {
        MaxStack = 1;
        _currentStack = 1;
        _userObject = user;
        _timer = _userObject.GetComponent<CSkillTimer>();
        _timerRegisterNumber = registerNumber;
        _cooldown = cooldown;
        _actionNumber = 0;
        _animationNumber = ani;
    }

    public CSkillFormat(int maxStack, int registerNumber, float cooldown, GameObject user)
    {
        MaxStack = maxStack;
        _currentStack = MaxStack;
        _userObject = user;
        _timer = _userObject.GetComponent<CSkillTimer>();
        _timerRegisterNumber = registerNumber;
        _cooldown = cooldown;
        _actionNumber = 0;
    }

    /// <summary>
    /// 초기화되지 않은 RegisteredNumber 멤버 변수 설정
    /// </summary>
    /// <param name="initRegisteredNumber">설정값</param>
    /// <returns></returns>
    public bool InitRegisteredNumber(int initRegisteredNumber)
    {
        if(_timerRegisterNumber == -1)
        {
            _timerRegisterNumber = initRegisteredNumber;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool InitSkillUser(GameObject user)
    {
        if (_userObject == null)
        {
            _userObject = user;
            _timer = _userObject.GetComponent<CSkillTimer>();
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetSkillUseEvent(int thisSkillIndex, SkillUseEvent useEvent)
    {
        Debug.Log("Set Skill Use Event");
        _skillUseCallback = (targetPos) => useEvent.Invoke(thisSkillIndex, targetPos);
    }

    /// <summary>
    /// 캐릭터 행동 번호 설정
    /// </summary>
    /// <param name="actionNum"></param>
    public void SetActionNumber(int characterActionNumber)
    {
        _actionNumber = characterActionNumber;
    }

    public void RegisterSkill(RetentionSkill register)
    {
        _usingSkill = register;
    }

    public bool Use(Vector3 targetPos)
    {
        if (0 >= _currentStack)
        {
            // 실행 거부
            Debug.Log($"Skill {_timerRegisterNumber} is Cooldown");
            return false;
        }
        else
        {
            _currentStack--;
            // 여기에 행동 코드 추가
            //_skillUseCallback?.Invoke(targetPos);
            //_usingSkill?.Invoke(_userObject, targetPos);
            _timer.Register(_timerRegisterNumber, _cooldown, EndCooldown);
            return true;
        }
    }

    private void EndCooldown(int notUsed)
    {
        _currentStack++;
        if(_currentStack > MaxStack)
        {
            _currentStack = MaxStack;
        }
    }
}
