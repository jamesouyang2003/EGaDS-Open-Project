using System;
using UnityEditor;
using UnityEngine;
 
[CustomPropertyDrawer(typeof(EnumMaskAttribute))]
public class EnumMaskDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var enumMaskAttribute = attribute as EnumMaskAttribute;

        if (enumMaskAttribute.EnumType.IsEnum)
        {
            Enum enumValue = (Enum)Enum.ToObject(enumMaskAttribute.EnumType, property.enumValueFlag);
            property.enumValueFlag = Convert.ToInt32(EditorGUI.EnumFlagsField(position, label, enumValue));
        }
    }
}