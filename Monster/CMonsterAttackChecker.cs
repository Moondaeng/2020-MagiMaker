using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum AttackPower
{
    Attack,
    Skill1,
    Skill2,
}

[System.Serializable]
public struct CrowdControl
{
    public bool Stun;
    public bool Push;
    public bool Slow;
}
[System.Serializable]
public struct CrowdControlLevel
{
    public int Stun;
    public int Push;
    public int Slow;
}

public class CMonsterAttackChecker : MonoBehaviour
{
    bool _exist;
    public GameObject _creator;
    CEnemyPara _myPara;
    public bool _isPenetrated = true;

    [HideInInspector]
    public List<string> _attackedPlayer = new List<string>();
    [Tooltip("데미지 타입 결정하기")]
    public AttackPower _attackPower;

    void Start()
    {
        _myPara = _creator.GetComponent<CEnemyPara>();
    }

    private void OnTriggerEnter(Collider other)
    {
        _exist = false;
        if (other.tag == "Player" || other.tag == "Wall")
        {
            if (other.tag == "Wall")
            {
                SendMessage("Explosion");
                return;
            }
            if (_isPenetrated)
            {
                for (int i = 0; i < _attackedPlayer.Count; i++)
                {
                    //Debug.Log(_attackedPlayer[i]);
                    if (other.name == _attackedPlayer[i])
                    {
                        _exist = true;
                    }
                }
                if (!_exist)
                {
                    _attackedPlayer.Add(other.name);
                    SkillDamageChecker(other.gameObject);
                }
            }
            else
            {
                SendMessage("Explosion");
                SkillDamageChecker(other.gameObject);
            }
        }
    }

    void SkillDamageChecker(GameObject target)
    {
        var para = target.GetComponent<CPlayerPara>();
        if (_attackPower == AttackPower.Attack)
        {
            para.DamegedRegardDefence(_myPara.RandomAttackDamage());
        }
        if (_attackPower == AttackPower.Skill1)
        {
            para.DamegedRegardDefence(_myPara.RandomAttackDamage() * _myPara._skillType[0].SkillPower);
            SkillTypeChecker(target, _myPara._skillType[0].CCType, _myPara._skillType[0].CCLevel);
        }
        if (_attackPower == AttackPower.Skill2)
        {
            para.DamegedRegardDefence(_myPara.RandomAttackDamage() * _myPara._skillType[1].SkillPower);
            SkillTypeChecker(target, _myPara._skillType[1].CCType, _myPara._skillType[1].CCLevel);
        }
    }

    void SkillTypeChecker(GameObject player, CrowdControl cc, CrowdControlLevel ccL)
    {
        var cntl = player.GetComponent<CCntl>();
        // 내 환경이랑 UseEffect 좀 부딪히는 경향이 있어서 내가 따로 짜뒀음.
        // 만약에 쓸 일이 있다면, 합치는 게 어떨까?
        if (cc.Push)
        {
            Debug.Log("Add force to " + player.name + "!");
            cntl.CCController("Push", ccL.Push, Vector3.Normalize(player.transform.position - transform.root.transform.position));
        }
        if (cc.Slow)
        {
            Debug.Log("Add Slow!");
            cntl.CCController("Slow", ccL.Slow);
        }
        if (cc.Stun)
        {
            Debug.Log("Add Stun!");
            cntl.CCController("Stun", ccL.Stun);
        }
    }

    public void DiscardList()
    {
        _attackedPlayer.Clear();
    }
}