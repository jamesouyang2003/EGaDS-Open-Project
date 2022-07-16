using System;
using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(EnumMaskConditionalAttribute))]
public class EnumMaskConditionalDrawer : PropertyDrawer
{

    private float _width; // width of the property in the inspector, used to calculate height

    private EnumMaskConditionalAttribute Attribute => attribute as EnumMaskConditionalAttribute;

    private string GetError(SerializedProperty property)
    {
        var prop = property.serializedObject.FindProperty(Attribute.Field);
        if (prop == null) 
            return $"Property \"{Attribute.Field}\" not found";
        if (prop.propertyType != SerializedPropertyType.Enum) 
            return $"Property \"{Attribute.Field}\" is not an Enum";
        return null;
    }

    // Check if a property matches the conditions
    // assumes GetError is already checked
    private bool ShouldShow(SerializedProperty property) 
    {
        var otherProp = property.serializedObject.FindProperty(Attribute.Field);
        return (otherProp.enumValueFlag & Attribute.Matcher) != 0;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var error = GetError(property);
        if (error != null)
        {
            var style = new GUIStyle();
            style.wordWrap = true;
            float width = _width-EditorGUIUtility.labelWidth;
            float height = style.CalcHeight(new GUIContent("Invalid condition: " + error), width);
            return height;
        }

        if (ShouldShow(property)) return EditorGUI.GetPropertyHeight(property);

        return 0;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        _width = position.width;

        // show error label if the attribute was set incorrectly
        var error = GetError(property);
        if (error != null)
        {
            var style = new GUIStyle();
            style.normal.textColor = Color.red;
            style.wordWrap = true;
            EditorGUI.LabelField(position, property.displayName, "Invalid condition: " + error, style);
            return;
        }

        if (ShouldShow(property)) EditorGUI.PropertyField(position, property, label, true);
    }
}
