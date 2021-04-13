using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton 등 초기 비활성화 상태에서도 
/// </summary>
public class CInactivedObjectAwakeCaller : MonoBehaviour
{
    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}
