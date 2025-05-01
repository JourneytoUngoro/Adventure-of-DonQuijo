using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(KnockbackComponent))]
public class KnockbackComponentEditor : PropertyDrawer
{
    private CombatAbility pertainedCombatAbility;

    private SerializedProperty airborne;
    private SerializedProperty directionBase;
    private SerializedProperty knockbackDirection;
    private SerializedProperty knockbackSpeed;
    private SerializedProperty knockbackTime;
    private SerializedProperty easeFunction;
    private SerializedProperty orthogonalVelocity;

    private SerializedProperty knockbackDirectionWhenBlocked;
    private SerializedProperty knockbackSpeedWhenBlocked;
    private SerializedProperty knockbackTimeWhenBlocked;
    private SerializedProperty easeFunctionWhenBlocked;

    private SerializedProperty knockbackDirectionWhenParried;
    private SerializedProperty knockbackSpeedWhenParried;
    private SerializedProperty knockbackTimeWhenParried;
    private SerializedProperty easeFunctionWhenParried;
    private SerializedProperty counterKnockbackDirectionWhenParried;
    private SerializedProperty counterKnockbackSpeedWhenParried;
    private SerializedProperty counterKnockbackTimeWhenParried;
    private SerializedProperty counterEaseFunctionWhenParried;
    private SerializedProperty counterOrthogonalVelocityWhenParried;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        float lineHeight = EditorGUIUtility.singleLineHeight;
        float newLineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        pertainedCombatAbility = property.FindPropertyRelative("<pertainedCombatAbility>k__BackingField").objectReferenceValue as CombatAbility;

        directionBase = property.FindPropertyRelative("<directionBase>k__BackingField");
        airborne = property.FindPropertyRelative("<airborne>k__BackingField");
        knockbackDirection = property.FindPropertyRelative("<knockbackDirection>k__BackingField");
        knockbackSpeed = property.FindPropertyRelative("<knockbackSpeed>k__BackingField");
        knockbackTime = property.FindPropertyRelative("<knockbackTime>k__BackingField");
        easeFunction = property.FindPropertyRelative("<easeFunction>k__BackingField");
        orthogonalVelocity = property.FindPropertyRelative("<orthogonalVelocity>k__BackingField");

        knockbackDirectionWhenBlocked = property.FindPropertyRelative("<knockbackDirectionWhenBlocked>k__BackingField");
        knockbackSpeedWhenBlocked = property.FindPropertyRelative("<knockbackSpeedWhenBlocked>k__BackingField");
        knockbackTimeWhenBlocked = property.FindPropertyRelative("<knockbackTimeWhenBlocked>k__BackingField");
        easeFunctionWhenBlocked = property.FindPropertyRelative("<easeFunctionWhenBlocked>k__BackingField");

        knockbackDirectionWhenParried = property.FindPropertyRelative("<knockbackDirectionWhenParried>k__BackingField");
        knockbackSpeedWhenParried = property.FindPropertyRelative("<knockbackSpeedWhenParried>k__BackingField");
        knockbackTimeWhenParried = property.FindPropertyRelative("<knockbackTimeWhenParried>k__BackingField");
        easeFunctionWhenParried = property.FindPropertyRelative("<easeFunctionWhenParried>k__BackingField");
        counterKnockbackDirectionWhenParried = property.FindPropertyRelative("<counterKnockbackDirectionWhenParried>k__BackingField");
        counterKnockbackSpeedWhenParried = property.FindPropertyRelative("<counterKnockbackSpeedWhenParried>k__BackingField");
        counterKnockbackTimeWhenParried = property.FindPropertyRelative("<counterKnockbackTimeWhenParried>k__BackingField");
        counterEaseFunctionWhenParried = property.FindPropertyRelative("<counterEaseFunctionWhenParried>k__BackingField");
        counterOrthogonalVelocityWhenParried = property.FindPropertyRelative("<counterOrthogonalVelocityWhenParried>k__BackingField");

        EditorGUI.BeginProperty(position, label, property);

        property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.size.x, lineHeight), property.isExpanded, label);

        if (property.isExpanded)
        {
            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), directionBase, new GUIContent("Direction Base"));
            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), airborne, new GUIContent("Airborne"));
            position.y += newLineHeight;

            if (directionBase.intValue != (int)DirectionBase.positionRelativeDirection)
            {
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), knockbackDirection, new GUIContent("Knockback Direction"));
            }

            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), knockbackSpeed, new GUIContent("Knockback Speed"));
            if (!airborne.boolValue)
            {
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), knockbackTime, new GUIContent("Knockback Time"));
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), easeFunction, new GUIContent("Ease Function"));
            }
            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), orthogonalVelocity, new GUIContent("Orthogonal Velocity"));

            if (pertainedCombatAbility.canBeBlocked)
            {
                position.y += 2.0f * newLineHeight;
                EditorGUI.LabelField(new Rect(position.x, position.y, position.size.x, lineHeight), new GUIContent("When Blocked"), EditorStyles.boldLabel);

                if (directionBase.intValue != (int)DirectionBase.positionRelativeDirection)
                {
                    position.y += newLineHeight;
                    EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), knockbackDirectionWhenBlocked, new GUIContent("Knockback Direction When Blocked"));
                }
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), knockbackSpeedWhenBlocked, new GUIContent("Knockback Speed When Blocked"));
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), knockbackTimeWhenBlocked, new GUIContent("Knockback Time When Blocked"));
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), easeFunctionWhenBlocked, new GUIContent("Ease Function When Blocked"));
                // position.y += newLineHeight;
                // EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), orthogonalVelocityWhenBlocked, new GUIContent("Orthogonal Velocity When Blocked"));
            }

            if (pertainedCombatAbility.canBeParried)
            {
                position.y += 2.0f * newLineHeight;
                EditorGUI.LabelField(new Rect(position.x, position.y, position.size.x, lineHeight), new GUIContent("When Parried"), EditorStyles.boldLabel);

                if (directionBase.intValue != (int)DirectionBase.positionRelativeDirection)
                {
                    position.y += newLineHeight;
                    EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), knockbackDirectionWhenParried, new GUIContent("Knockback Direction When Parried"));
                }
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), knockbackSpeedWhenParried, new GUIContent("Knockback Speed When Parried"));
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), knockbackTimeWhenParried, new GUIContent("Knockback Time When Parried"));
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), easeFunctionWhenParried, new GUIContent("Ease Function When Parried"));
                // position.y += newLineHeight;
                // EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), orthogonalVelocityWhenParried, new GUIContent("Orthogonal Velocity When Parried"));
                if (directionBase.intValue != (int)DirectionBase.positionRelativeDirection)
                {
                    position.y += newLineHeight;
                    EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), counterKnockbackDirectionWhenParried, new GUIContent("Counter Knockback Direction When Parried"));
                }
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), counterKnockbackSpeedWhenParried, new GUIContent("Counter Knockback Speed When Parried"));
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), counterKnockbackTimeWhenParried, new GUIContent("Counter Knockback Time When Parried"));
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), counterEaseFunctionWhenParried, new GUIContent("Counter Ease Function When Parried"));

                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), counterOrthogonalVelocityWhenParried, new GUIContent("Counter Orthogonal Velocity When Parried Parried"));
            }
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int multiplier = 1;
        float newLineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        pertainedCombatAbility = property.FindPropertyRelative("<pertainedCombatAbility>k__BackingField").objectReferenceValue as CombatAbility;
        directionBase = property.FindPropertyRelative("<directionBase>k__BackingField");

        if (property.isExpanded)
        {
            multiplier += 8;

            if (directionBase.intValue == (int)DirectionBase.positionRelativeDirection)
            {
                multiplier -= 1;
            }

            if (pertainedCombatAbility.canBeBlocked)
            {
                multiplier += 7;

                if (directionBase.intValue == (int)DirectionBase.positionRelativeDirection)
                {
                    multiplier -= 1;
                }
            }

            if (pertainedCombatAbility.canBeParried)
            {
                multiplier += 12;

                if (directionBase.intValue == (int)DirectionBase.positionRelativeDirection)
                {
                    multiplier -= 2;
                }
            }
        }

        return multiplier * newLineHeight;
    }
}
