using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTestContorller : MonoBehaviour
{
    delegate void Action();

    private Dictionary<KeyCode, Action> keyDictionary;

    public CPlayerCommand commander;
    public CUIManager ui;
    public CWaitingForAccept waiting;

    private void Awake()
    {
        // 조작 관리
        keyDictionary = new Dictionary<KeyCode, Action>
        {
            {KeyCode.Alpha1, () => commander.SetMyCharacter(0) },
            //{KeyCode.Alpha2, () => commander.SetMyCharacter(1) },
            //{KeyCode.Alpha3, () => commander.SetMyCharacter(2) },
            //{KeyCode.Alpha4, () => commander.SetActivePlayers(4) },
            //{KeyCode.Alpha5, () => commander.UseSkill(1, 0,  GetHitPoint()) },
            {KeyCode.Insert, () => commander.SetActivePlayers(4) },
            {KeyCode.Alpha9, () => commander.Move(1, GetHitPoint()) },
            {KeyCode.Alpha0, () => commander.Move(2, GetHitPoint()) },
            {KeyCode.Delete, () => Destroy(GameObject.FindGameObjectWithTag("ITEM")) },
            {KeyCode.M, () => commander.Teleport(0, new Vector3(0.09f, 0.61f, 80.0f)) },
            {KeyCode.U, () => waiting.SetPlayerAccept(1, CWaitingForAccept.EAccept._accept) },
            {KeyCode.I, () => waiting.SetPlayerAccept(2, CWaitingForAccept.EAccept._accept) },
            {KeyCode.O, () => waiting.SetPlayerAccept(3, CWaitingForAccept.EAccept._accept) },
            {KeyCode.J, () => waiting.SetPlayerAccept(1, CWaitingForAccept.EAccept._cancel) },
            {KeyCode.K, () => waiting.SetPlayerAccept(2, CWaitingForAccept.EAccept._cancel) },
            {KeyCode.L, () => waiting.SetPlayerAccept(3, CWaitingForAccept.EAccept._cancel) }

        };
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            foreach (var dic in keyDictionary)
            {
                if (Input.GetKeyDown(dic.Key))
                {
                    dic.Value();
                }
            }
        }
    }

    private Vector3 GetHitPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }

        return Vector3.one;
    }
}
