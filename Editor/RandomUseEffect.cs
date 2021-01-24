using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CanEditMultipleObjects]
[CustomEditor(typeof(CRandomUseEffect))]
public class RandomUseEffectEditor : Editor
{
    ReorderableList effectWithChanceList;

    private void OnEnable()
    {
        effectWithChanceList = new ReorderableList(serializedObject, serializedObject.FindProperty("effects"),
            true, true, true, true);
        effectWithChanceList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = effectWithChanceList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, 200, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("effect"), GUIContent.none);
            EditorGUI.PropertyField(
                new Rect(rect.x + 200, rect.y, rect.width - 200, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("chance"), GUIContent.none);
        };
        effectWithChanceList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Random Effect List");
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        effectWithChanceList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}
