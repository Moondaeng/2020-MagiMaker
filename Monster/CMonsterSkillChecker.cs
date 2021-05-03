using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class CMonsterSkillChecker : MonoBehaviour
{
    [Tooltip("원 거리")]
    [Range(0, 30f)] public float _distance = 5f;
    [Tooltip("타이머")]
    [Range(0, 5f)] public float _time = 1f;
    [SerializeField] ParticleSystem _playForEffect;
    [SerializeField] ParticleSystem[] _manualParticleSystems;
    SphereCollider _col;

    private void Start()
    {
        SetParticleSystems();
    }

    private void SetParticleSystems()
    {
        foreach (ParticleSystem p in _manualParticleSystems)
        {
            var main = p.main;
            main.startSize = _distance * 0.4f;
        }
    }

    void Starting()
    {
        transform.localScale = new Vector3(1f,1f,1f) * _distance;
        _col = GetComponent<SphereCollider>();
        StartCoroutine(IncreaseSize());
    }
    
    // 프레임 당, 사이즈 크기를 늘려가는 코루틴
    // deltaTime 자체가 1초를 프레임 다위로 나눈 것이라 적당하게 들어맞음.
    // 물론 원하던 1초 2초 정확한 시간 체크는 안됨.
    IEnumerator IncreaseSize()
    {
        int i = 0;
        float value = Time.deltaTime / _time;
        while (transform.GetChild(0).transform.localScale.x < 1f)
        {
            yield return new WaitForEndOfFrame();
            i++;
            if (1f < transform.GetChild(0).transform.localScale.x + value)
            {
                transform.GetChild(0).transform.localScale = new Vector3(1f, 1f, 1f);
                _col.enabled = true;
                if (_playForEffect != null)
                {
                    _playForEffect.gameObject.SetActive(true);
                    _playForEffect.Play();
                }
            }
            else
            {
                transform.GetChild(0).transform.localScale += new Vector3(1f, 1f, 1f) * value;
            }
        }
        StartCoroutine(RemoveMe(2f));
        yield return null;
    }

    IEnumerator RemoveMe(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }
}
