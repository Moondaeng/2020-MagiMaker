using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CEnemyPara : CharacterPara
{
    public string _name;
    public Image _hpBar;
    public GameObject _selection;
    public ParticleSystem _hitEffect;
    public int _spawnID { get; set; }
    [System.NonSerialized]
    public GameObject _myRespawn;
    GameObject _effect;
    Vector3 _originPos;

    public override void InitPara()
    {
        JsonConvert.instance.loadToMonster(name, this);
        _isStunned = false;
        _isDead = false;
        _curHp = _maxHp;
        HideSelection();
        HideHitEffect();
        InitHpBarSize();
    }
    
    public void SetRespawn(GameObject respawn, int spawnID, Vector3 originPos)
    {
        _myRespawn = respawn;
        this._spawnID = spawnID;
        this._originPos = originPos;
    }

    public void respawnAgain()
    {
        //  리스폰 오브젝트에서 처음 생성될때의 위치와 같게 함
        transform.position = _originPos;
        InitPara();
        GetComponent<Collider>().enabled = true;
        GetComponent<Animator>().enabled = true;
    }

    void InitHpBarSize()
    {
        _hpBar.rectTransform.localScale = new Vector3(1f, 1f, 1f);
    }

    protected override void UpdateAfterReceiveAttack()
    {
        base.UpdateAfterReceiveAttack();
        if(_curHp == 0)
        {
            HideSelection();
        }
        Debug.Log(_curHp);
        _hpBar.rectTransform.localScale = new Vector3((float)_curHp / _maxHp, 1f, 1f);
    }

    public void ShowSelection()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public void HideSelection()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }
    
    // 타격 이펙트 함수
    public void ShowHitEffect()
    {
        transform.GetChild(1).gameObject.SetActive(true);
        _hitEffect.Play();
    }

    public void HideHitEffect()
    {
        transform.GetChild(1).gameObject.SetActive(false);
    }
}