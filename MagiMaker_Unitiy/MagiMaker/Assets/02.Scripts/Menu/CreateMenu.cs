using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreateMenu : MonoBehaviour
{
    private void Awake()
    {
        SceneManager.LoadScene("Menu");
        Debug.Log("LoadMenu");
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
