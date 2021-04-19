using UnityEngine;

public class EnumNamedNestedArrayAttribute : PropertyAttribute
{
    public string[] names;
    public int Dimension;
    public EnumNamedNestedArrayAttribute(System.Type names_enum_type, int dimension)
    {
        this.names = System.Enum.GetNames(names_enum_type);
        Dimension = dimension;
    }
}
