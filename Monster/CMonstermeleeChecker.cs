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

public struct CrowdControl
{
    public bool Stun;
    public bool Push;
    public bool Slow;
}

public struct CrowdControlLevel
{
    public int Stun;
    public int Push;
    public int Slow;
}

public class CMonstermeleeChecker : MonoBehaviour
{
    bool _exist;
    MeshCollider _mesh;
    CEnemyPara _myPara;

    [HideInInspector]
    public List<string> _attackedPlayer = new List<string>();
    [Tooltip("데미지 타입 결정하기")]
    public AttackPower _attackPower;
    public CrowdControl _ccType;
    public CrowdControlLevel _ccLevel;

    // Start is called before the first frame update
    void Start()
    {
        _mesh = GetComponent<MeshCollider>();
        _myPara = transform.root.GetComponent<CEnemyPara>();
        if (_myPara == null)
        {
            Debug.Log("fuck");
        }
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        _exist = false;
        if (other.tag == "Player")
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
                var para = other.GetComponent<CPlayerPara>();
                if (_attackPower == AttackPower.Attack)
                {
                    para.DamegedRegardDefence(_myPara.RandomAttackDamage());
                }
                if (_attackPower == AttackPower.Skill1)
                {
                    para.DamegedRegardDefence(_myPara.RandomAttackDamage() * _myPara._skillType[0].SkillPower);
                    SkillTypeChecker(other.gameObject, _myPara._skillType[0].CCType, _myPara._skillType[0].CCLevel);
                }
                if (_attackPower == AttackPower.Skill2)
                {
                    para.DamegedRegardDefence(_myPara.RandomAttackDamage() * _myPara._skillType[1].SkillPower);
                    SkillTypeChecker(other.gameObject, _myPara._skillType[1].CCType, _myPara._skillType[0].CCLevel);
                }
            }
        }
    }

    void SkillTypeChecker(GameObject player, CrowdControl cc, CrowdControlLevel ccL)
    {
        var cntl = player.GetComponent<CCntl>();
        // 내 환경이랑 UseEffect 좀 부딪히는 경향이 있어서 내가 따로 짜뒀음.
        // 만약에 쓸 일이 있다면, 합치는 게 어떨까?
        if (cc.Push)
        {
            Debug.Log("Add force!");
            cntl.CCController("Push", ccL.Push, -transform.root.forward);
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
