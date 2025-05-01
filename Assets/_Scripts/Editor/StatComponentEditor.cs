using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(StatComponent))]
public class StatComponentEditor : PropertyDrawer
{
    private SerializedProperty maxValue;
    private SerializedProperty minValue;
    private SerializedProperty currentValue;
    private SerializedProperty enableRecovery;
    private SerializedProperty recoveryStartTime;
    private SerializedProperty recoveryDuration;
    private SerializedProperty recoveryValue;
    private SerializedProperty graph;

    private Vector2 scroll;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        float singleLineHeight = EditorGUIUtility.singleLineHeight;
        float newLineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        maxValue = property.FindPropertyRelative("<maxValue>k__BackingField");
        minValue = property.FindPropertyRelative("<minValue>k__BackingField");
        currentValue = property.FindPropertyRelative("<currentValue>k__BackingField");
        enableRecovery = property.FindPropertyRelative("<enableRecovery>k__BackingField");
        recoveryStartTime = property.FindPropertyRelative("<recoveryStartTime>k__BackingField");
        recoveryDuration = property.FindPropertyRelative("<recoveryDuration>k__BackingField");
        recoveryValue = property.FindPropertyRelative("<recoveryValue>k__BackingField");
        graph = property.FindPropertyRelative("<graph>k__BackingField");

        EditorGUI.BeginProperty(position, label, property);

        property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.size.x, singleLineHeight), property.isExpanded, label);

        if (property.isExpanded)
        {
            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, singleLineHeight), maxValue, new GUIContent("Max Value"));
            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, singleLineHeight), minValue, new GUIContent("Min Value"));
            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, singleLineHeight), currentValue, new GUIContent("Current Value"));
            if (property.displayName != "Level")
            {
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, singleLineHeight), enableRecovery, new GUIContent("Enable Recovery"));
            }

            if (enableRecovery.boolValue)
            {
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, singleLineHeight), recoveryStartTime, new GUIContent("Recovery Start Time"));
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, singleLineHeight), recoveryDuration, new GUIContent("Recovery Duration"));
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, singleLineHeight), recoveryValue, new GUIContent("Recovery Value"));
            }

            if (property.displayName != "Level")
            {
                position.y += newLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, singleLineHeight), graph, new GUIContent("Increment/Accumulation Graph"));
            }

            /*EditorGUILayout.Space();

            scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.MaxHeight(300.0f));

            for (int level = 1; level < 100; level++)
            {
                EditorGUILayout.BeginHorizontal("box");
                EditorGUILayout.LabelField("Level " + level);
                EditorGUILayout.LabelField(((int)accumulationPerLevel.animationCurveValue.Evaluate(level)).ToString());
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();*/
        }
        
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float newLineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        float multiplier = 1.0f;

        enableRecovery = property.FindPropertyRelative("<enableRecovery>k__BackingField");
        graph = property.FindPropertyRelative("<graph>k__BackingField");

        if (property.isExpanded)
        {
            if (property.displayName != "Level")
            {
                multiplier += 6.0f;
            }
            else
            {
                multiplier += 4.0f;
            }

            if (enableRecovery.boolValue)
            {
                multiplier += 3.0f;
            }

            if (graph.isExpanded)
            {
                multiplier += 2.0f;
            }
        }

        return multiplier * newLineHeight;
    }
}
