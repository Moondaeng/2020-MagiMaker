using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 모든 캐릭터를 관리하는 클래스
 * 
 */
public class CManager : MonoBehaviour
{
    public static CManager instance;

    List<GameObject> players = new List<GameObject>();
    List<GameObject> monsters = new List<GameObject>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

    }

    // 캐릭터 추가
    public void AddCharacter(int charId, Vector3 createPos)
    {
        
    }

    // 캐릭터 삭제
    public void DeleteCharacter(int charId)
    {

    }

    //외부에서 전달된 몬스터가 기존에 리스트에 보관하고 있는 몬스터와 일치하는지 여부를 체크
    public void AddNewMonsters(GameObject mon)
    {

        //인자로 넘어온 몬스터가 기존의 리스트에 존재하면 sameExist = true 아니면 false
        bool sameExist = false;
        for (int i = 0; i < monsters.Count; i++)
        {
            if (monsters[i] == mon)
            {
                sameExist = true;

                break;
            }
        }

        if (sameExist == false)
        {
            monsters.Add(mon);
        }

    }

    public void RemoveMonster(GameObject mon)
    {
        foreach (GameObject monster in monsters)
        {
            if (monster == mon)
            {
                monsters.Remove(monster);
                break;
            }

        }
    }

    //현재 플레이어가 클릭한 몬스터만 선택마크가 표시
    public void ChangeCurrentTarget(GameObject mon)
    {
        DeselectAllMonsters();
        mon.GetComponent<CEnemyPara>().ShowSelection();
    }

    public void DeselectAllMonsters()
    {
        for (int i = 0; i < monsters.Count; i++)
        {
            monsters[i].GetComponent<CEnemyPara>().HideSelection();
        }
    }

    // 이동 명령
    public void CommandMoveCharacter(int charNumber, Vector3 targetPos)
    {
        var charFSM = monsters[charNumber].GetComponent<CharacterFSM>();
        charFSM.MoveTo(targetPos);
    }

    // 위치 강제 설정
    public void CommandSetPosition(int charNumber, Vector3 targetPos)
    {
        var charTransform = monsters[charNumber].transform;
        charTransform.position = targetPos;
    }

    // 스킬 사용
    public void CommandUseSkill(int charNumber, int skillNumber, Vector3 targetPos)
    {
        var charSkill = monsters[charNumber].GetComponent<CCharacterSkill>();
        charSkill.UseSkillToPosition(skillNumber, targetPos);
    }

    // 해당 캐릭터에게 데미지 주기
    public void CommandDamageCharacter(int charNumber)
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}