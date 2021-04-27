using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class CPlayerMeleecheker : MonoBehaviour
{
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
        if (other.tag == "Monster" || other.tag == "Boss")
        {
            for (int i = 0; i < _attackedMonster.Count; i++)
            {
                //Debug.Log(_attackedPlayer[i]);
                if (other.GetComponent<CEnemyPara>()._spawnID == _attackedMonster[i])
                {
                    _exist = true;
                }
            }
            if (!_exist)
            {
                _attackedMonster.Add(other.GetComponent<CEnemyPara>()._spawnID);
                var para = other.GetComponent<CEnemyPara>();
                string tempstr = Regex.Replace(transform.root.gameObject.name, @"\D", "");
                int rstInt = int.Parse(tempstr);
                para.DamegedRegardDefence(_myPara.RandomAttackDamage(), rstInt);
            }
        }
    }

    public void DiscardList()
    {
        _attackedMonster.Clear();
    }
}
