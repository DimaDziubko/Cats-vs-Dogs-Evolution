using _Game.Utils;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(IntRangeSliderAttribute))]
public class IntRangeSliderDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        int originalIndentLevel = EditorGUI.indentLevel;
        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        EditorGUI.indentLevel = 0;
        SerializedProperty minProperty = property.FindPropertyRelative("_min");
        SerializedProperty maxProperty = property.FindPropertyRelative("_max");
        int minValue = minProperty.intValue;
        int maxValue = maxProperty.intValue;
        float fieldWidth = position.width / 4f - 4f;
        float sliderWidth = position.width / 2f;
        position.width = fieldWidth;
        minValue = EditorGUI.IntField(position, minValue);
        position.x += fieldWidth + 4f;
        position.width = sliderWidth;

        float fMinValue = minValue;
        float fMaxValue = maxValue;
        IntRangeSliderAttribute limit = attribute as IntRangeSliderAttribute;
        
        EditorGUI.MinMaxSlider(position, ref fMinValue, ref fMaxValue, limit.Min, limit.Max);
        
        minValue = Mathf.RoundToInt(fMinValue);
        maxValue = Mathf.RoundToInt(fMaxValue);

        position.x += sliderWidth + 4f;
        position.width = fieldWidth;
        maxValue = EditorGUI.IntField(position, maxValue);
        
        minValue = Mathf.Max(minValue, limit.Min);
        maxValue = Mathf.Min(maxValue, limit.Max);
        if (maxValue < minValue)
        {
            maxValue = minValue;
        }

        minProperty.intValue = minValue;
        maxProperty.intValue = maxValue;

        EditorGUI.EndProperty();
        EditorGUI.indentLevel = originalIndentLevel;
    }
}
