using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTestContorller : MonoBehaviour
{
    delegate void Action();

    private Dictionary<KeyCode, Action> keyDictionary;

    public CPlayerCommand commander;
    public CUIManager ui;

    private void Awake()
    {
        // 조작 관리
        keyDictionary = new Dictionary<KeyCode, Action>
        {
            {KeyCode.Alpha1, () => commander.SetMyCharacter(0) },
            {KeyCode.Alpha2, () => commander.SetMyCharacter(1) },
            {KeyCode.Alpha3, () => commander.SetMyCharacter(2) },
            {KeyCode.Alpha4, () => commander.SetMyCharacter(3) },
            {KeyCode.Alpha8, () => commander.Add(GetHitPoint()) },
            {KeyCode.Alpha9, () => commander.Move(1, GetHitPoint()) },
            {KeyCode.Alpha0, () => commander.Move(2, GetHitPoint()) }
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
