using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(MovementComponent))]
public class MovementComponentEditor : PropertyDrawer
{
    private SerializedProperty isDashAttack;
    private SerializedProperty dashDirection;
    private SerializedProperty planeDirection;
    private SerializedProperty planeVelocity;
    private SerializedProperty moveTime;
    private SerializedProperty attackStartTime;
    private SerializedProperty attackFinishTime;
    private SerializedProperty easeFunction;
    private SerializedProperty easeCurve;
    private SerializedProperty reverseTime;
    private SerializedProperty orthogonalVelocity;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        float lineHeight = EditorGUIUtility.singleLineHeight;
        float newLineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        isDashAttack = property.FindPropertyRelative("<isDashAttack>k__BackingField");

        dashDirection = property.FindPropertyRelative("<dashDirection>k__BackingField");
        planeDirection = property.FindPropertyRelative("<planeDirection>k__BackingField");
        planeVelocity = property.FindPropertyRelative("<planeVelocity>k__BackingField");

        moveTime = property.FindPropertyRelative("<moveTime>k__BackingField");
        attackStartTime = property.FindPropertyRelative("<attackStartTime>k__BackingField");
        attackFinishTime = property.FindPropertyRelative("<attackFinishTime>k__BackingField");
        easeFunction = property.FindPropertyRelative("<easeFunction>k__BackingField");
        easeCurve = property.FindPropertyRelative("<easeCurve>k__BackingField");
        reverseTime = property.FindPropertyRelative("<reverseTime>k__BackingField");
        orthogonalVelocity = property.FindPropertyRelative("<orthogonalVelocity>k__BackingField");

        EditorGUI.BeginProperty(position, label, property);

        property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.size.x, lineHeight), property.isExpanded, label);

        if (property.isExpanded)
        {
            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), isDashAttack, new GUIContent("Is Dash Attack"));

            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), dashDirection, new GUIContent("Dash Direction"));
            if (dashDirection.intValue != (int)DashDirection.TowardTarget)
            {
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), planeDirection, new GUIContent("Plane Direction"));
            }
            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), planeVelocity, new GUIContent("Plane Velocity"));

            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), moveTime, new GUIContent("Move Time"));
            if (isDashAttack.boolValue)
            {
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x / 2, lineHeight), attackStartTime, new GUIContent("Attack Start Time"));

                if (attackStartTime.floatValue > moveTime.floatValue)
                {
                    attackStartTime.floatValue = moveTime.floatValue;
                }

                EditorGUI.PropertyField(new Rect(position.x + position.size.x / 2, position.y, position.size.x / 2, lineHeight), attackFinishTime, new GUIContent("Attack Finish Time"));

                if (attackFinishTime.floatValue < attackStartTime.floatValue)
                {
                    attackFinishTime.floatValue = attackStartTime.floatValue;
                }
            }

            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), easeFunction, new GUIContent("Ease Function"));
            if (easeFunction.intValue == (int)Ease.INTERNAL_Custom)
            {
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), easeCurve, new GUIContent("Ease Curve"));
            }
            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), reverseTime, new GUIContent("Reverse Time"));
            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), orthogonalVelocity, new GUIContent("Orthogonal Velocity"));
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float newLineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        int multiplier = 1;

        isDashAttack = property.FindPropertyRelative("<isDashAttack>k__BackingField");
        dashDirection = property.FindPropertyRelative("<dashDirection>k__BackingField");
        easeFunction = property.FindPropertyRelative("<easeFunction>k__BackingField");

        if (property.isExpanded)
        {
            multiplier += 7;

            if (dashDirection.intValue != (int)DashDirection.TowardTarget)
            {
                multiplier += 1;
            }

            if (isDashAttack.boolValue)
            {
                multiplier += 1;
            }
            
            if (easeFunction.intValue == (int)Ease.INTERNAL_Custom)
            {
                multiplier += 1;
            }
        }

        return multiplier * newLineHeight;
    }
}
