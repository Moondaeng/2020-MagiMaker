using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CEnemyPara : CharacterPara
{
    public string _name;
    public ParticleSystem _hitEffect;
    public int _spawnID { get; set; }
    [SerializeField] public GameObject _myRespawn;
    Vector3 _originPos;

    public override void InitPara()
    {
        JsonConvert.instance.loadToMonster(_name, this);
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

    // 타격 이펙트 함수
    public void ShowHitEffect()
    {
        transform.GetChild(1).gameObject.SetActive(true);
        _hitEffect.Play();
    }

    public void ShowSelection()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public void HideSelection()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void HideHitEffect()
    {
        transform.GetChild(1).gameObject.SetActive(false);
    }

    public void ShowHpBar()
    {
        transform.GetChild(2).gameObject.SetActive(true);
    }

    public void HideHpBar()
    {
        transform.GetChild(2).gameObject.SetActive(false);
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