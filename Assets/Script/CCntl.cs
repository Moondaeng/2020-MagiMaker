using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class CCntl : MonoBehaviour
{
    GameObject player;
    public bool telepotationCheck = true;
    [SerializeField]
    private GameObject[] spell;
    private GameObject temp, temp2;
    CPlayerPara myPara;

    void Start()
    {
        // 플레이어 태그를 가진 개체를 받아옴
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void CheckClick()
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
            //Debug.Log(s);
            //Debug.Log(s2);
            bool stringExists = s.Contains("Terrain");
            // 누른게 지형이면, 이동함.
            if (stringExists)
            {
                 player.GetComponent<CPlayerFSM>().MoveTo(hit.point);
            }
            else if (s2 == "Monster")//마우스 클릭한 대상이 적 캐릭터인 경우
            {
                 //player.GetComponent<CPlayerFSM>().
                 player.GetComponent<CPlayerFSM>().AttackEnemy(hit.collider.gameObject);
            }
        }
    }

    void RotateMotion()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            player.GetComponent<CPlayerFSM>().TurnTo(hit.point);
//            player.GetComponent<CPlayerFSM>().SkillState();
        }
    }

    void WarpMotion()
    {
        Destroy(temp2);
        player.GetComponent<CPlayerAni>().ChangeAni(CPlayerAni.ANI_WARP);
        temp = Instantiate(spell[0], player.transform.position + new Vector3(0, 0.2f, 0), Quaternion.identity);
        Invoke("WarpToDestination",1f);
    }

    void WarpToDestination()
    {
        Destroy(temp);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            string s = hit.collider.gameObject.name;
            bool stringExists = s.Contains("Terrain");
            // 누른게 지형이면, 이동함.
            if (stringExists)
            {
                temp2 = Instantiate(spell[0], hit.point + new Vector3(0, 0.2f, 0), Quaternion.identity);
                player.transform.position = hit.point;
            }
        }
        player.GetComponent<CPlayerAni>().ChangeAni(CPlayerAni.ANI_IDLE);
    }

    private IEnumerator WaitForIt()
    {
        yield return new WaitForSeconds(1.0f);
        telepotationCheck = true;
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            CheckClick();
        }

        if (Input.GetMouseButton(1))
        {
            RotateMotion();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("input key Q");
            Debug.Log(player.GetComponent<CPlayerFSM>().currentState);
            player.GetComponent<CPlayerFSM>().SkillState();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("input key W");
            Debug.Log(player.GetComponent<CPlayerFSM>().currentState);
            player.GetComponent<CPlayerFSM>().SkillState();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("input key E");
            Debug.Log(player.GetComponent<CPlayerFSM>().currentState);
            player.GetComponent<CPlayerFSM>().SkillState();
        }

        if (Input.GetKeyDown("space"))
        {
            telepotationCheck = false;
            WarpMotion();
        }
    }
}
