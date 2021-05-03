using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CPlayerMeleecheker : MonoBehaviour
{
    [Tooltip("충돌 시 적용될 효과")]
    public List<CUseEffectHandle> useEffects;

    bool _exist;
    CPlayerPara _myPara;
    List<int> _attackedMonster = new List<int>();

    void Start()
    {
        _myPara = transform.parent.GetComponent<CPlayerPara>();
    }

    private void OnTriggerEnter(Collider other)
    {
        _exist = false;

        other.GetComponent<CharacterPara>()?.DamegedRegardDefence(_myPara.RandomAttackDamage());
        //if (other.tag == "Monster" || other.tag == "Boss")
        //{
        //    for (int i = 0; i < _attackedMonster.Count; i++)
        //    {
        //        //Debug.Log(_attackedPlayer[i]);
        //        if (other.GetComponent<CEnemyPara>()._spawnID == _attackedMonster[i])
        //        {
        //            _exist = true;
        //        }
        //    }
        //    if (!_exist)
        //    {
        //        _attackedMonster.Add(other.GetComponent<CEnemyPara>()._spawnID);
        //        var para = other.GetComponent<CEnemyPara>();
        //        para.DamegedRegardDefence(_myPara.RandomAttackDamage());
        //    }
        //}
    }

    public void DiscardList()
    {
        _attackedMonster.Clear();
    }
}
