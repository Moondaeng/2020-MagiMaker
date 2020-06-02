using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseMerchant : MonoBehaviour
{
    private Camera mainCamera;  //메인 카메라
    private GameObject player;
    private GameObject shop;    //상점 캔버스

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        shop = GameObject.FindGameObjectWithTag("SHOP"); //변수명 추후 수정
        //shop.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        ClickMerchant();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            offShop();  //esc키 누를경우 상점 끄기
        }
    }

    void ClickMerchant()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        Physics.Raycast(ray, out hit);

        if (hit.collider.gameObject.tag == "MERCHANT")//마우스 가져간 대상이 상인 캐릭터인 경우
        {
            UseShop();
        }
    }

    void UseShop()
    {
        //마우스 좌클릭
        if (Input.GetMouseButtonDown(0))
            for(int i = 0; i < shop.transform.childCount; i++)
                shop.transform.GetChild(i).gameObject.SetActive(true);
    }

    void offShop()
    {
        if (shop.activeSelf == true)
            for (int i = 0; i < shop.transform.childCount; i++)
                shop.transform.GetChild(i).gameObject.SetActive(false);
    }
}
