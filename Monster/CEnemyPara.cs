using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CEnemyPara : CharacterPara
{
    public string _name;
    public int _spawnID { get; set; }
    [SerializeField] public GameObject _myRespawn;
    Vector3 _originPos;

    public override void InitPara()
    {
        _isStunned = false;
        _isDead = false;
        _curHp = _maxHp;
    }
    
    public void SetRespawn(GameObject respawn, int spawnID, Vector3 originPos)
    {
        _myRespawn = respawn;
        this._spawnID = spawnID;
        this._originPos = originPos;
        Debug.Log(_spawnID);
    }

    public void respawnAgain()
    {
        //  리스폰 오브젝트에서 처음 생성될때의 위치와 같게 함
        transform.position = _originPos;
        this.tag = "Monster";
        InitPara();
    }
    protected override void UpdateAfterReceiveAttack()
    {
        base.UpdateAfterReceiveAttack();
        Debug.Log(_curHp); 
    }
}