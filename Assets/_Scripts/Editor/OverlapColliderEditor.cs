using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// [CustomPropertyDrawer(typeof(OverlapCollider))]
public class OverlapColliderEditor : PropertyDrawer
{
    private SerializedProperty centerTransform;
    private SerializedProperty overlapBox;
    private SerializedProperty overlapCircle;
    private SerializedProperty boxSize;
    private SerializedProperty boxRotation;
    private SerializedProperty circleRadius;
    private SerializedProperty limitAngle;
    private SerializedProperty centerRotation;
    private SerializedProperty clockwiseAngle;
    private SerializedProperty counterClockwiseAngle;
    private float vector2BoudaryWidth = 345.0f;

    private bool toggle = true;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        float singlelineHeight = EditorGUIUtility.singleLineHeight;
        float newLineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        centerTransform = property.FindPropertyRelative("<centerTransform>k__BackingField");
        overlapBox = property.FindPropertyRelative("<overlapBox>k__BackingField");
        overlapCircle = property.FindPropertyRelative("<overlapCircle>k__BackingField");
        boxSize = property.FindPropertyRelative("<boxSize>k__BackingField");
        boxRotation = property.FindPropertyRelative("<boxRotation>k__BackingField");
        circleRadius = property.FindPropertyRelative("<circleRadius>k__BackingField");
        limitAngle = property.FindPropertyRelative("<limitAngle>k__BackingField");
        centerRotation = property.FindPropertyRelative("<centerRotation>k__BackingField");
        clockwiseAngle = property.FindPropertyRelative("<clockwiseAngle>k__BackingField");
        counterClockwiseAngle = property.FindPropertyRelative("<counterClockwiseAngle>k__BackingField");

        EditorGUI.BeginProperty(position, label, property);

        property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.size.x, singlelineHeight), property.isExpanded, label);

        if (property.isExpanded)
        {
            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, singlelineHeight), centerTransform, new GUIContent("Center Transform"));
            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, singlelineHeight), overlapBox, new GUIContent("OverlapBox"));
            EditorGUI.PropertyField(new Rect(position.x + position.size.x / 2.0f, position.y, position.size.x, singlelineHeight), overlapCircle, new GUIContent("OverlapCircle"));

            if (overlapBox.boolValue)
            {
                if (toggle)
                {
                    overlapCircle.boolValue = false;
                    toggle = false;
                }
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, singlelineHeight), boxSize, new GUIContent("Box Size"));

                if (EditorGUIUtility.currentViewWidth > vector2BoudaryWidth)
                {
                    position.y += newLineHeight;
                }
                else
                {
                    position.y += newLineHeight * 2.0f;
                }
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, singlelineHeight), boxRotation, new GUIContent("Box Rotation"));
                // (centerTransform.objectReferenceValue as Transform).rotation = Quaternion.Euler(0.0f, 0.0f, boxRotation.floatValue);
            }

            if (overlapCircle.boolValue)
            {
                toggle = true;
                overlapBox.boolValue = false;
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, singlelineHeight), circleRadius, new GUIContent("Circle Radius"));
            }

            if (overlapCircle.boolValue != false || overlapBox.boolValue != false)
            {
                if (overlapBox.boolValue)
                {
                    position.y += newLineHeight;
                    EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, singlelineHeight), limitAngle, new GUIContent("Limit Angle"));

                    if (limitAngle.boolValue)
                    {
                        position.y += newLineHeight;
                        EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, singlelineHeight), centerRotation, new GUIContent("Center Rotation"));
                        position.y += newLineHeight;
                        EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, singlelineHeight), clockwiseAngle, new GUIContent("Clockwise Angle"));
                        position.y += newLineHeight;
                        EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, singlelineHeight), counterClockwiseAngle, new GUIContent("Counter Clockwise Angle"));
                    }
                }
                else if (overlapCircle.boolValue)
                {
                    position.y += newLineHeight;
                    EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, singlelineHeight), limitAngle, new GUIContent("Limit Angle"));

                    if (limitAngle.boolValue)
                    {
                        position.y += newLineHeight;
                        EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, singlelineHeight), centerRotation, new GUIContent("Center Rotation"));
                        position.y += newLineHeight;
                        EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, singlelineHeight), clockwiseAngle, new GUIContent("Clockwise Angle"));
                        position.y += newLineHeight;
                        EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, singlelineHeight), counterClockwiseAngle, new GUIContent("Counter Clockwise Angle"));
                    }
                }
            }
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float newLineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        overlapBox = property.FindPropertyRelative("<overlapBox>k__BackingField");
        overlapCircle = property.FindPropertyRelative("<overlapCircle>k__BackingField");
        limitAngle = property.FindPropertyRelative("<limitAngle>k__BackingField");

        if (!property.isExpanded)
        {
            return newLineHeight;
        }
        else
        {
            if (overlapBox.boolValue)
            {
                if (EditorGUIUtility.currentViewWidth > vector2BoudaryWidth)
                {
                    if (limitAngle.boolValue)
                    {
                        return newLineHeight * 9.0f;
                    }
                    else
                    {
                        return newLineHeight * 6.0f;
                    }
                }
                else
                {
                    if (limitAngle.boolValue)
                    {
                        return newLineHeight * 10.0f;
                    }
                    else
                    {
                        return newLineHeight * 7.0f;
                    }
                }
            }

            if (overlapCircle.boolValue)
            {
                if (limitAngle.boolValue)
                {
                    return newLineHeight * 8.0f;
                }
                else
                {
                    return newLineHeight * 5.0f;
                }
            }

            return newLineHeight * 3.0f;
        }
    }
}

