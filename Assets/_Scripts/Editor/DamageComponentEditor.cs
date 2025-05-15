using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DamageComponent))]
public class DamageComponentEditor : PropertyDrawer
{
    private CombatAbility pertainedCombatAbility;

    private SerializedProperty healthDamage;
    private SerializedProperty postureDamage;
    private SerializedProperty pauseTimeWhenHit;

    private SerializedProperty healthDamageShieldRate;
    private SerializedProperty postureDamageShieldRate;
    private SerializedProperty pauseTimeWhenShielded;

    private SerializedProperty healthDamageParryRate;
    private SerializedProperty postureDamageParryRate;
    private SerializedProperty healthCounterDamageRate;
    private SerializedProperty postureCounterDamageRate;
    private SerializedProperty pauseTimeWhenParried;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        float lineHeight = EditorGUIUtility.singleLineHeight;
        float newLineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        pertainedCombatAbility = property.FindPropertyRelative("<pertainedCombatAbility>k__BackingField").objectReferenceValue as CombatAbility;

        healthDamage = property.FindPropertyRelative("<healthDamage>k__BackingField");
        postureDamage = property.FindPropertyRelative("<postureDamage>k__BackingField");
        pauseTimeWhenHit = property.FindPropertyRelative("<pauseTimeWhenHit>k__BackingField");

        healthDamageShieldRate = property.FindPropertyRelative("<healthDamageShieldRate>k__BackingField");
        postureDamageShieldRate = property.FindPropertyRelative("<postureDamageShieldRate>k__BackingField");
        pauseTimeWhenShielded = property.FindPropertyRelative("<pauseTimeWhenShielded>k__BackingField");

        healthDamageParryRate = property.FindPropertyRelative("<healthDamageParryRate>k__BackingField");
        postureDamageParryRate = property.FindPropertyRelative("<postureDamageParryRate>k__BackingField");
        healthCounterDamageRate = property.FindPropertyRelative("<healthCounterDamageRate>k__BackingField");
        postureCounterDamageRate = property.FindPropertyRelative("<postureCounterDamageRate>k__BackingField");
        pauseTimeWhenParried = property.FindPropertyRelative("<pauseTimeWhenParried>k__BackingField");

        EditorGUI.BeginProperty(position, label, property);

        property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.size.x, lineHeight), property.isExpanded, label);

        if (property.isExpanded)
        {
            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), healthDamage, new GUIContent("Health Damage"));
            if (healthDamage.isExpanded)
            {
                position.y += newLineHeight * 2.0f;
            }
            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), postureDamage, new GUIContent("Posture Damage"));
            if (postureDamage.isExpanded)
            {
                position.y += newLineHeight * 2.0f;
            }
            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), pauseTimeWhenHit, new GUIContent("Pause Time When Hit"));

            if (pertainedCombatAbility.canBeBlocked)
            {
                position.y += newLineHeight * 2.0f;
                EditorGUI.LabelField(new Rect(position.x, position.y, position.size.x, lineHeight), new GUIContent("When Blocked"), EditorStyles.boldLabel);
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), healthDamageShieldRate, new GUIContent("Health Damage Block Rate"));
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), postureDamageShieldRate, new GUIContent("Posture Damage Block Rate"));
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), pauseTimeWhenShielded, new GUIContent("Pause Time When Blocked"));
            }

            if (pertainedCombatAbility.canBeParried)
            {
                position.y += 2.0f * newLineHeight;
                EditorGUI.LabelField(new Rect(position.x, position.y, position.size.x, lineHeight), new GUIContent("When Parried"), EditorStyles.boldLabel);
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), healthDamageParryRate, new GUIContent("Health Damage Parry Rate"));
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), postureDamageParryRate, new GUIContent("Posture Damage Parry Rate"));
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), healthCounterDamageRate, new GUIContent("Health Counter Damage Rate"));
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), postureCounterDamageRate, new GUIContent("Posture Counter Damage Rate"));
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, lineHeight), pauseTimeWhenParried, new GUIContent("Pause Time When Parried"));
            }
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float newLineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        int multiplier = 1;

        pertainedCombatAbility = property.FindPropertyRelative("<pertainedCombatAbility>k__BackingField").objectReferenceValue as CombatAbility;
        healthDamage = property.FindPropertyRelative("<healthDamage>k__BackingField");
        postureDamage = property.FindPropertyRelative("<postureDamage>k__BackingField");

        if (property.isExpanded)
        {
            multiplier += 3;

            if (healthDamage.isExpanded)
            {
                multiplier += 2;
            }

            if (postureDamage.isExpanded)
            {
                multiplier += 2;
            }

            if (pertainedCombatAbility.canBeBlocked)
            {
                multiplier += 5;
            }

            if (pertainedCombatAbility.canBeParried)
            {
                multiplier += 7;
            }
        }

        return multiplier * newLineHeight;
    }
}
