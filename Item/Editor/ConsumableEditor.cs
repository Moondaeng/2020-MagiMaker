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

    SerializedProperty UseEffectList;


    private void OnEnable()
    {
        consumable = serializedObject.FindProperty("ConsumableStat");
        SetItemInformation();
        UseEffectList = consumable.FindPropertyRelative("UseEffectList");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawItemInfor();
        EditorGUILayout.LabelField("Use Effect", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(UseEffectList, true);
        serializedObject.ApplyModifiedProperties();
    }

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
