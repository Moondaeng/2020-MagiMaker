using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CMonstermeleeChecker : MonoBehaviour
{
    bool _exist;
    MeshCollider _mesh;
    CEnemyPara _myPara;
    List<string> _attackedPlayer = new List<string>();
    // Start is called before the first frame update
    void Start()
    {
        _mesh = GetComponent<MeshCollider>();
        _myPara = transform.parent.GetComponent<CEnemyPara>();
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
                para.DamegedRegardDefence(_myPara.RandomAttackDamage());
            }
        }
    }

    public void DiscardList()
    {
        _attackedPlayer.Clear();
    }
}
