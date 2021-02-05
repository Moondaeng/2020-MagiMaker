using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTestController : MonoBehaviour
{
    delegate void Action();

    private Dictionary<KeyCode, Action> keyDictionary;

    public CPlayerCommand commander;
    public CUIManager ui;
    public CWaitingForAccept waiting;
    public CMonsterManager monsterManager;

    private void Awake()
    {
        // 조작 관리
        keyDictionary = new Dictionary<KeyCode, Action>
        {
            {KeyCode.KeypadPlus, () => {
<<<<<<< HEAD:Test/CTestContorller.cs
                    var item = CItemDropTable.instance.PopRandomItemByGrade(CItemDropTable.ItemGrade.Normal, CConstants.EQUIP_ITEM_TYPE);
=======
                    var item = CItemDropTable.instance.PopRandomItemByGrade(CItemDropTable.ItemGrade.Normal);
                    if(item == null) return;
>>>>>>> origin/ZeroFe:Test/CTestController.cs
                    item.SetActive(true);
                    item.transform.position = GetHitPoint();                    
                } },
            {KeyCode.KeypadMinus, () => {
                    var item = CItemDropTable.instance.DropConsumable();
                    item.SetActive(true);
                    item.transform.position = GetHitPoint();
                }},
            {KeyCode.U, () => commander.SetMyCharacter(0) },
            {KeyCode.I, () => commander.SetMyCharacter(1) },
            {KeyCode.O, () => commander.SetMyCharacter(2) },
            {KeyCode.P, () => commander.SetMyCharacter(3) },
            //{KeyCode.Alpha4, () => commander.SetActivePlayers(4) },
            //{KeyCode.Alpha5, () => commander.UseSkill(1, 0,  GetHitPoint()) },
            {KeyCode.Insert, () => commander.SetActivePlayers(4) },
            {KeyCode.Alpha5, () => commander.DamageToCharacter(0, 300) },
            {KeyCode.Alpha6, () => commander.DamageToCharacter(1, 300) },
            {KeyCode.Alpha7, () => commander.DamageToCharacter(2, 300) },
            {KeyCode.Alpha8, () => commander.DamageToCharacter(3, 300) },
            {KeyCode.Alpha9, () => commander.Follow(1) },
            {KeyCode.Alpha0, () => commander.Follow(2) },
            {KeyCode.X, () => commander.Call(1) },
            {KeyCode.C, () => commander.SkillTo(1) },
            {KeyCode.J, () => CWaitingForAccept.instance.SetPortalUseSelect(1, CWaitingForAccept.EAccept._accept) },
            {KeyCode.K, () => CWaitingForAccept.instance.SetPortalUseSelect(2, CWaitingForAccept.EAccept._accept) },
            {KeyCode.L, () => CWaitingForAccept.instance.SetPortalUseSelect(3, CWaitingForAccept.EAccept._accept) },
            {KeyCode.B, () => CWaitingForAccept.instance.SetPortalUseSelect(1, CWaitingForAccept.EAccept._cancle) },
            {KeyCode.N, () => CWaitingForAccept.instance.SetPortalUseSelect(2, CWaitingForAccept.EAccept._cancle) },
            {KeyCode.M, () => CWaitingForAccept.instance.SetPortalUseSelect(3, CWaitingForAccept.EAccept._cancle) }
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

        if (monsterManager == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            monsterManager.DestroyAllMonsters();
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
