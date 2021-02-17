﻿using UnityEngine;

public class CControlRenderer : MonoBehaviour
{
    public Renderer _rend;

    [SerializeField]
    public Material[] _materials;

    private CCntl _myStatus;

    // Start is called before the first frame update
    private void Start()
    {
        _rend = GetComponent<Renderer>();
        ChangeTest();
    }

    private void ChangeTest()
    {
        _rend.sharedMaterial = _materials[0];
    }
}