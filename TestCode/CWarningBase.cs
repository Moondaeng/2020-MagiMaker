using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWarningBase : MonoBehaviour
{
    public float _x, _y, size;
    public Vector3 _targetPos;
    Vector3 localScale;
    
    public CWarningBase(float x, float y, Vector3 TargetPos)
    {
        this._x = x;
        this._y = y;
        this._targetPos = TargetPos;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        localScale = new Vector3(0f, 0f, 0f);
        size = transform.root.localScale.x;
    }
    
    private void Update()
    {
        localScale.x += 0.0001f;
        localScale.y += 0.0001f;
        transform.localScale += localScale;
        if (transform.localScale.x > 1f)
        {
            Death();
        }
    }
    // 발사 위치 지정

    // 자동 소멸
    private void Death()
    {
        Destroy(transform.root.gameObject);
    }
}
