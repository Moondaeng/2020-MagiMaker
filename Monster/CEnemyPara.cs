using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CEnemyPara : CharacterPara
{
    public string _name;
    string _originTag = "Monster";
    [HideInInspector] public GameObject _myRespawn;
    Vector3 _originPos;

    public override void InitPara()
    {
        deadEvent.AddListener(SetOffMonster);
        base.InitPara();
        _isStunned = false;
        _isDead = false;
        _curHp = _maxHp;
    }
    
    public void SetRespawn(GameObject respawn, int spawnID, Vector3 originPos)
    {
        _myRespawn = respawn;
        _spawnID = spawnID;
        _originPos = originPos;
        //Debug.Log("My Respawn is : " + _myRespawn + "  My SpawnID is : " + _spawnID
        //    + "  My originPos is : " + _originPos);
    }

    public void SetOffMonster()
    {
        Invoke("SetActiveFalse", 2f);
    }

    public void SetActiveFalse()
    {
        CMonsterManager.instance.RemoveMonster(_spawnID);
    }

    public void respawnAgain()
    {
        //  리스폰 오브젝트에서 처음 생성될때의 위치와 같게 함
        transform.position = _originPos;
        tag = _originTag;
        gameObject.layer = LayerMask.NameToLayer("Monster");
        InitPara();
    }

    protected override void UpdateAfterReceiveAttack()
    {
        base.UpdateAfterReceiveAttack();
    }
}