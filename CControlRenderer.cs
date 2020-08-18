using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CControlRenderer : MonoBehaviour
{
    public Renderer _rend;
    [SerializeField]
    public Material[] _materials;
    CCntl _myStatus;
    // Start is called before the first frame update
    void Start()
    {
        _rend = GetComponent<Renderer>();
        ChangeTest();
    }
    
    void ChangeTest()
    {
        _rend.sharedMaterial = _materials[0];
    }
}
