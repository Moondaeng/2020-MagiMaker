using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CEffectShatter : MonoBehaviour
{
    [SerializeField] GameObject _originPrefab = null;
    [SerializeField] float _force = 0f;
    [SerializeField] Vector3 _offset = Vector3.zero;
    GameObject clone;
    Rigidbody[] rigids;
    public void Explosion()
    {
        clone = Instantiate(_originPrefab, transform.position, Quaternion.identity);
        rigids = clone.GetComponentsInChildren<Rigidbody>();
        for (int i = 0; i < rigids.Length; i++)
        {
            rigids[i].AddExplosionForce(_force, transform.position + _offset, 10f, 30f);
        }
        gameObject.SetActive(false);
        Invoke("RemoveMe", 1f);
    }

    void RemoveMe()
    {
        Destroy(clone);
    }
}
