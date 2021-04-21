using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(EnumNamedNestedArrayAttribute))]
public class DrawerEnumNamedNestedArray : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EnumNamedNestedArrayAttribute enumNames = attribute as EnumNamedNestedArrayAttribute;
        //propertyPath returns something like DropItemChanceInStages.Array.data[1].chances.Array.data[0]
        //so get the index from there
        int index = GetIndex(property.propertyPath, enumNames.Dimension);
        //change the label
        label.text = enumNames.names[index];
        //draw field
        EditorGUI.PropertyField(position, property, label, true);
    }

    static int GetIndex(string properthPath, int demension)
    {
        int currentNestedCount = 0;
        int index;

        do
        {
            var openArrayOperatorPos = properthPath.IndexOf("[");
            var closeArrayOperatorPos = properthPath.IndexOf("]");
            index = System.Convert.ToInt32(properthPath.Substring(openArrayOperatorPos + 1, closeArrayOperatorPos - openArrayOperatorPos - 1));
            properthPath = properthPath.Substring(closeArrayOperatorPos + 1);
        } while (currentNestedCount++ != demension);

        return index;
    }
}
