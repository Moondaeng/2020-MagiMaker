using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(CConsumableComponent))]
public class ConsumableEditor : Editor
{
    private SerializedProperty consumable;

    // 아이템 기본 정보 관련 GUI 필드
    SerializedProperty itemName;
    SerializedProperty itemImage;
    SerializedProperty itemCode;

    ReorderableList UseEffectList;


    private void OnEnable()
    {
        consumable = serializedObject.FindProperty("ConsumableStat");
        SetItemInformation();
        UseEffectList = new ReorderableList(serializedObject,
            consumable.FindPropertyRelative("UseEffectList"),
            true, true, true, true);
        UseEffectList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = UseEffectList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            EditorGUI.PropertyField(rect, element);
        };
        UseEffectList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Use Effect List");
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawItemInfor();
        EditorGUILayout.LabelField("Use Effect", EditorStyles.boldLabel);
        UseEffectList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }

        //EditorGUILayout.PropertyField(passiveEffect, true);
    private void SetItemInformation()
    {
        itemName = consumable.FindPropertyRelative("_itemName");
        itemCode = consumable.FindPropertyRelative("_itemCode");
        itemImage = consumable.FindPropertyRelative("_itemImage");
    }

    private void DrawItemInfor()
    {
        EditorGUILayout.PropertyField(itemName);
        EditorGUILayout.PropertyField(itemCode);
        EditorGUILayout.PropertyField(itemImage);
    }
}
