using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 스킬을 등록해서 실행하는 포맷
public class CSkillFormat
{
    private static CLogComponent _logger = new CLogComponent(ELogType.Skill);
    public delegate void RetentionSkill(GameObject user, Vector3 targetPos);

<<<<<<< HEAD
    public CSkillTimer _timer;

    private GameObject _userObject;
    private RetentionSkill _usingSkill;
    public bool _isCooldown;
=======
    private GameObject _userObject;
    private CSkillTimer _timer;
    private RetentionSkill _usingSkill;
    private bool _isCooldown;
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
    private int _timerRegisterNumber;
    private float _cooldown;

    public CSkillFormat(int registerNumber, float cooldown, GameObject user)
    {
        _userObject = user;
        _timer = _userObject.GetComponent<CSkillTimer>();
        _isCooldown = false;
        _timerRegisterNumber = registerNumber;
        _cooldown = cooldown;
    }

<<<<<<< HEAD
    // 인터페이스 변경 필요 - Unit이 모두 Timer를 가지므로 자기 자신의 Timer를 추적하게 만들어야 함
    public CSkillFormat(int registerNumber, float cooldown)
    {
        _timer = GameObject.Find("SkillScript").GetComponent<CSkillTimer>();
        _userObject = GameObject.Find("Player");
        _isCooldown = false;
        _timerRegisterNumber = registerNumber;
        _cooldown = cooldown;
    }

=======
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
    public void RegisterSkill(RetentionSkill register)
    {
        _usingSkill = register;
    }

<<<<<<< HEAD
    public void Use(Vector3 targetPos)
=======
    public bool Use(Vector3 targetPos)
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
    {
        if(_isCooldown)
        {
            // 실행 거부
            _logger.Log("Skill is Cooldown!");
<<<<<<< HEAD
=======
            return false;
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
        }
        else
        {
            _isCooldown = true;
            // 스킬 실행
            _usingSkill?.Invoke(_userObject, targetPos);
            _timer.Register(_timerRegisterNumber, _cooldown, EndCooldown);
<<<<<<< HEAD
        }
    }

    private void EndCooldown() =>_isCooldown = false;
=======
            return true;
        }
    }

    private void EndCooldown(int notUsed) =>_isCooldown = false;
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
}
