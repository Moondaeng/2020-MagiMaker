using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

/*
[CustomPropertyDrawer(typeof(CUseEffect.PersistEffect))]
public class PersistEffectDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

    }
}
*/

/*
[CustomPropertyDrawer(typeof(CUseEffect.PersistCustomEffect))]
public class PersistCustomEffectDrawer : PropertyDrawer
{
    ReorderableList changeAbilityList;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        using (new EditorGUI.PropertyScope(position, label, property))
        {
            position.height = EditorGUIUtility.singleLineHeight;
            int indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var idPropertyRect = new Rect(position);
            var DotAmountPropertyRect = new Rect(idPropertyRect)
            {
                y = idPropertyRect.y + EditorGUIUtility.singleLineHeight + 2
            };
            var DotPeriodPropertyRect = new Rect(DotAmountPropertyRect)
            {
                y = DotAmountPropertyRect.y + EditorGUIUtility.singleLineHeight + 2
            };
            var changeAbilityRect = new Rect(DotPeriodPropertyRect)
            {
                y = DotPeriodPropertyRect.y + EditorGUIUtility.singleLineHeight + 2
            };
            var maxStackPropertyRect = new Rect(changeAbilityRect)
            {
                y = changeAbilityRect.y + EditorGUIUtility.singleLineHeight + 2
            };
            var stackAccumulateEffectPropertyRect = new Rect(maxStackPropertyRect)
            {
                y = maxStackPropertyRect.y + EditorGUIUtility.singleLineHeight + 2
            };

            //각 프로퍼티의 SerializedProperty를 구합니다
            var idProperty = property.FindPropertyRelative("id");
            var DotAmountProperty = property.FindPropertyRelative("DotAmount");
            var DotPeriodProperty = property.FindPropertyRelative("DotPeriod");
            var listProperty = property.FindPropertyRelative("changeAbilities");
            var maxStackProperty = property.FindPropertyRelative("maxStack");
            var stackAccumulateEffectProperty = property.FindPropertyRelative("stackAccumulateEffect");

            
            //SerializedProperty listProperty = property.FindPropertyRelative("changeAbilities");
            //changeAbilityList = new ReorderableList(property.serializedObject, property.FindPropertyRelative("changeAbilities"), true, true, true, true);
            //changeAbilityList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
            //    var element = changeAbilityList.serializedProperty.GetArrayElementAtIndex(index);
            //    rect.y += 2;
            //    EditorGUI.PropertyField(rect, element);
            //};
            //changeAbilityList.drawHeaderCallback = (Rect rect) =>
            //{
            //    EditorGUI.LabelField(rect, "Buff Abilities");
            //};

            //float height = 0f;
            //for (var i = 0; i < listProperty.arraySize; i++)
            //{
            //    height = Mathf.Max(height, EditorGUI.GetPropertyHeight(listProperty.GetArrayElementAtIndex(i)));
            //}
            //changeAbilityList.elementHeight = height;
            

            //각 프로퍼티의 GUI을 표시
            EditorGUI.PropertyField(idPropertyRect, idProperty);
            EditorGUI.PropertyField(DotAmountPropertyRect, DotAmountProperty);
            EditorGUI.PropertyField(DotPeriodPropertyRect, DotPeriodProperty);
            EditorGUI.PropertyField(changeAbilityRect, listProperty);
            //changeAbilityList.DoList(position);
            EditorGUI.PropertyField(maxStackPropertyRect, maxStackProperty);
            EditorGUI.PropertyField(stackAccumulateEffectPropertyRect, stackAccumulateEffectProperty);

            EditorGUI.indentLevel = indentLevel;
        }
    }
}
*/

[CustomPropertyDrawer(typeof(CUseEffect.ChangeAbilityInfo))]
public class ChangeAbilityInfoDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //원래는 1개의 프로퍼티인 것을 나타내기 위해서 PropertyScope로 둘러쌉니다
        using (new EditorGUI.PropertyScope(position, label, property))
        {
            //썸네일의 영역을 확보하기 위해서 라벨 영역의 폭을 줄입니다
            EditorGUIUtility.labelWidth = 48;

            position.height = EditorGUIUtility.singleLineHeight;

            const int space = 3;

            // indent 레벨에 따라서 들여쓰기 간격이 달라짐
            // indent 레벨이 0이 아니면 Rect Width만큼 정해지지 않음
            int indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // indentLevel에 맞춰 들여쓰기 - 기능 개발 중
            //var indentedRect = EditorGUI.IndentedRect(position);
            //float indentPosX = indentedRect.x;

            float size = position.width / 4;

            //각 프로퍼티의 Rect을 구합니다
            var abilitySelectRect = new Rect(position)
            {
                width = size
            };

            var isBuffRect = new Rect(abilitySelectRect)
            {
                //width = size, 
                x = abilitySelectRect.x + abilitySelectRect.width + space
            };

            var increaseBaseRect = new Rect(isBuffRect)
            {
                //width = size, 
                x = isBuffRect.x + isBuffRect.width + space
            };

            var increasePerStackRect = new Rect(increaseBaseRect)
            {
                x = increaseBaseRect.x + increaseBaseRect.width + space
            };

            EditorGUI.PropertyField(abilitySelectRect, property.FindPropertyRelative("ability"), GUIContent.none);
            EditorGUI.PropertyField(isBuffRect, property.FindPropertyRelative("isBuff"));
            EditorGUI.PropertyField(increaseBaseRect, property.FindPropertyRelative("increaseBase"), GUIContent.none);
            EditorGUI.PropertyField(increasePerStackRect, property.FindPropertyRelative("increasePerStack"), GUIContent.none);

            EditorGUI.indentLevel = indentLevel;
        }
    }
}

/*
[CustomPropertyDrawer(typeof(UseEffectWithChance))]
public class UseEffectWithChanceDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var useEffectProperty = property.FindPropertyRelative("useEffect");
        var useEffectObjectProperty = property.FindPropertyRelative("useEffectObject");
        var chanceProperty = property.FindPropertyRelative("Chance");

        EditorGUILayout.PropertyField(useEffectProperty);
        EditorGUILayout.PropertyField(useEffectObjectProperty);
        EditorGUILayout.Slider(chanceProperty, 0, 1);
        
        using (new EditorGUI.PropertyScope(position, label, property))
        {
            //썸네일의 영역을 확보하기 위해서 라벨 영역의 폭을 줄입니다

            position.height = EditorGUIUtility.singleLineHeight;

            var useEffectRect = new Rect(position)
            {
            };

            var useEffectObjectRect = new Rect(useEffectRect)
            {
                y = useEffectRect.y + EditorGUIUtility.singleLineHeight + 2
            };

            var chanceRect = new Rect(useEffectObjectRect)
            {
                y = useEffectObjectRect.y + EditorGUIUtility.singleLineHeight + 2
            };

            //각 프로퍼티의 SerializedProperty를 구합니다
            var useEffectProperty = property.FindPropertyRelative("useEffect");
            var useEffectObjectProperty = property.FindPropertyRelative("useEffectObject");
            var chanceProperty = property.FindPropertyRelative("Chance");

            //각 프로퍼티의 GUI을 표시

            EditorGUI.PropertyField(useEffectRect, useEffectProperty);
            EditorGUI.PropertyField(useEffectObjectRect, useEffectObjectProperty);
            EditorGUI.Slider(chanceRect, chanceProperty, 0, 1);
        }
    }
}
*/

/*
[CustomPropertyDrawer(typeof(UseEffectList))]
public class UseEffectListDrawer : PropertyDrawer
{
    ReorderableList list;

    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (list == null)
        {
            list = BuildUseEffectsReorderableList(property.FindPropertyRelative("UseEffects"));
        }
        list.DoLayoutList();
    }

    private ReorderableList BuildUseEffectsReorderableList(SerializedProperty property)
    {
        ReorderableList list = new ReorderableList(property.serializedObject, property, true, true, true, true);
        list.elementHeight = 60;
        list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
            rect.height -= 4;
            rect.y += 2;
            EditorGUI.PropertyField(rect, property.GetArrayElementAtIndex(index));
        };
        list.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Use Effect List");
        };

        return list;
    }
}
*/