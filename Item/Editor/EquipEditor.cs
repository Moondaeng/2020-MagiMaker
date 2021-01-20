using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(CEquipComponent))]
public class EquipEditor : Editor
{
    private SerializedProperty equip;

    // 아이템 기본 정보 관련 GUI 필드
    SerializedProperty itemName;
    SerializedProperty itemImage;
    SerializedProperty itemCode;

    ReorderableList AbilityList;

    // 패시브 관련 GUI 필드
    SerializedProperty passiveCondition;
    SerializedProperty passiveConditionOption;
    SerializedProperty passiveUseCount;
    SerializedProperty passiveEffect;

    // 성장 능력치 관련 GUI 필드
    SerializedProperty upgradeCondition;
    SerializedProperty upgradeCount;
    ReorderableList upgradeAbilityList;


    private void OnEnable()
    {
        equip = serializedObject.FindProperty("equipStat");
        SetItemInformation();
        SetAbilityList();
        SetPassive();
        Information();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawItemInfor();
        DrawAbility();
        DrawPassive();
        DrawUpgrade();
        serializedObject.ApplyModifiedProperties();
    }

    private void SetItemInformation()
    {
        itemName = equip.FindPropertyRelative("_itemName");
        itemCode = equip.FindPropertyRelative("_itemCode");
        itemImage = equip.FindPropertyRelative("_itemImage");
    }

    private void DrawItemInfor()
    {
        EditorGUILayout.PropertyField(itemName);
        EditorGUILayout.PropertyField(itemCode);
        EditorGUILayout.PropertyField(itemImage);
    }

    private void SetAbilityList()
    {
        AbilityList = new ReorderableList(serializedObject,
            equip.FindPropertyRelative("equipAbilities"),
            true, true, true, true);
        AbilityList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
            var element = AbilityList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, 200, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("equipEffect"), GUIContent.none);
            EditorGUI.PropertyField(
                new Rect(rect.x + 200, rect.y, rect.width - 200, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("value"), GUIContent.none);
        };
        AbilityList.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Equip Ability");
        };
    }

    private void DrawAbility()
    {
        EditorGUILayout.LabelField("Upgrade", EditorStyles.boldLabel);
        AbilityList.DoLayoutList();
    }

    private void SetPassive()
    {
        passiveCondition = equip.FindPropertyRelative("PassiveCondition");
        passiveConditionOption = equip.FindPropertyRelative("PassiveConditionOption");
        passiveUseCount = equip.FindPropertyRelative("PassiveUseCount");
        passiveEffect = equip.FindPropertyRelative("passiveEffect");

        //EquipEffectList = new ReorderableList(serializedObject,
        //    equip.FindPropertyRelative("EquipEffectList"),
        //    true, true, true, true);
        //EquipEffectList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
        //    var element = EquipEffectList.serializedProperty.GetArrayElementAtIndex(index);
        //    rect.y += 2;
        //    EditorGUI.PropertyField(
        //        new Rect(rect.x, rect.y, 200, EditorGUIUtility.singleLineHeight),
        //        element.FindPropertyRelative("useEffect"), GUIContent.none);
        //    EditorGUI.PropertyField(
        //        new Rect(rect.x + 200, rect.y, rect.width - 200, EditorGUIUtility.singleLineHeight),
        //        element.FindPropertyRelative("Chance"), GUIContent.none);
        //};
        //EquipEffectList.drawHeaderCallback = (Rect rect) => {
        //    EditorGUI.LabelField(rect, "Passive Effect");
        //};
    }

    private void DrawPassive()
    {
        EditorGUILayout.LabelField("Passive", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(passiveCondition, GUIContent.none);
            EditorGUILayout.PropertyField(passiveConditionOption, GUIContent.none);
            EditorGUILayout.PropertyField(passiveUseCount, GUIContent.none);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.PropertyField(passiveEffect, true);

        //EquipEffectList.DoLayoutList();
    }

    private void Information()
    {
        upgradeCondition = equip.FindPropertyRelative("UpgradeCondition");
        upgradeCount = equip.FindPropertyRelative("UpgradeCount");

        upgradeAbilityList = new ReorderableList(serializedObject,
            equip.FindPropertyRelative("upgradeAbilities"),
            true, true, true, true);
        upgradeAbilityList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
            var element = upgradeAbilityList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, 200, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("equipEffect"), GUIContent.none);
            EditorGUI.PropertyField(
                new Rect(rect.x + 200, rect.y, rect.width - 200, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("value"), GUIContent.none);
        };
        upgradeAbilityList.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Upgrade Ability");
        };
    }

    private void DrawUpgrade()
    {
        EditorGUILayout.LabelField("Upgrade", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(upgradeCondition, GUIContent.none);
            EditorGUILayout.PropertyField(upgradeCount, GUIContent.none);
        EditorGUILayout.EndHorizontal();

        upgradeAbilityList.DoLayoutList();
    }
}
