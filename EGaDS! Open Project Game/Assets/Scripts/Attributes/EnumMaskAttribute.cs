using System;
using UnityEngine;

/// <summary>
/// Attribute for creating a dropdown menu for enums, similar to that of LayerMask.
/// Derived from https://forum.unity.com/threads/editor-bit-mask.16841/#post-7484294
/// </summary>
public class EnumMaskAttribute : PropertyAttribute
{
    public readonly Type EnumType;

    public EnumMaskAttribute(Type enumType) => EnumType = enumType;
}