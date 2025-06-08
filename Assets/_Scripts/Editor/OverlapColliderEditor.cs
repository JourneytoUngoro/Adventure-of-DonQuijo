using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// [CustomPropertyDrawer(typeof(OverlapCollider))]
public class OverlapColliderEditor : PropertyDrawer
{
    private SerializedProperty overlapCollider;
    private SerializedProperty height;
    private SerializedProperty limitAngle;
    private SerializedProperty angleCheckBaseTransform;
    private SerializedProperty clockwiseAngle;
    private SerializedProperty counterClockwiseAngle;
    private float vector2BoudaryWidth = 345.0f;

    private bool toggle = true;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        float singlelineHeight = EditorGUIUtility.singleLineHeight;
        float newLineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        overlapCollider = property.FindPropertyRelative("<overlapCollider>k__BackingField");
        height = property.FindPropertyRelative("<height>k__BackingField");
        limitAngle = property.FindPropertyRelative("<limitAngle>k__BackingField");
        angleCheckBaseTransform = property.FindPropertyRelative("<angleCheckBaseTransform>k__BackingField");
        clockwiseAngle = property.FindPropertyRelative("<clockwiseAngle>k__BackingField");
        counterClockwiseAngle = property.FindPropertyRelative("<counterClockwiseAngle>k__BackingField");

        EditorGUI.BeginProperty(position, label, property);

        property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.size.x, singlelineHeight), property.isExpanded, label);

        if (property.isExpanded)
        {
            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, singlelineHeight), overlapCollider, new GUIContent("Overlap Collider"));
            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, singlelineHeight), height, new GUIContent("Height"));
            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, singlelineHeight), limitAngle, new GUIContent("Limit Angle"));

            if (limitAngle.boolValue)
            {
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, singlelineHeight), angleCheckBaseTransform, new GUIContent("Angle Check Base Transform"));
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, singlelineHeight), clockwiseAngle, new GUIContent("Clockwise Angle"));
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, singlelineHeight), counterClockwiseAngle, new GUIContent("Counter Clockwise Angle"));
            }
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float newLineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        int multiplier = 1;

        limitAngle = property.FindPropertyRelative("<limitAngle>k__BackingField");

        if (property.isExpanded)
        {
            multiplier += 3;

            if (limitAngle.boolValue)
            {
                multiplier += 3;
            }
        }

        return multiplier * newLineHeight;
    }
}

