using System;
using UnityEngine;

public class EnumMaskConditionalAttribute : PropertyAttribute
{

    public readonly string Field; // the field used to check whether to show the property
    public readonly int Matcher; // if one of the bits in Matcher is in Field, then show the property

    public EnumMaskConditionalAttribute(string enumField, int matcher)
    {
        this.Field = enumField;
        this.Matcher = matcher;
    }
}