using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class Cntl : MonoBehaviour
{
    GameObject player;

    void Start()
    {
        // 플레이어 태그를 가진 개체를 받아옴
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void CheckClick()
    {
        if (Input.GetMouseButton(0))
        {
            // Ray API 참고 직선 방향으로 나아가는 무한대 길이의 선
            // 카메라 클래스의 메인 카메라로 부터의 스크린의 점을 통해 레이 반환
            // 여기서 스크린의 점은, Input.mousePosition으로 받음
            // 포지션.z를 무시한 값임.
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                string s = hit.collider.gameObject.name;
                string s2 = hit.collider.gameObject.tag;
                Debug.Log(s);
                Debug.Log(s2);
                bool stringExists = s.Contains("Terrain");
                // 누른게 지형이면, 이동함.
                if (stringExists)
                {
                    // PlayerFSM에 정의한 
                    player.GetComponent<PlayerFSM>().MoveTo(hit.point);
                    // 강제이동 코드
                    // player.transform.position = hit.point;
                }
                else if (s2 == "Monster")//마우스 클릭한 대상이 적 캐릭터인 경우
                {
                    player.GetComponent<PlayerFSM>().AttackEnemy(hit.collider.gameObject);
                }
            }
        }
    }
    void Update()
    {
        CheckClick();
    }
}
