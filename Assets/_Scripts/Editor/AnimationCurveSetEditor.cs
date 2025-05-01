using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Codice.Client.BaseCommands.BranchExplorer.Layout.BrExLayout;

[CustomPropertyDrawer(typeof(AnimationCurveSet))]
public class AnimationCurveSetEditor : PropertyDrawer
{
    private SerializedProperty incrementPerLevel;
    private SerializedProperty accumulationPerLevel;
    private bool modificationCheck = false;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        float singleLineHeight = EditorGUIUtility.singleLineHeight;
        float newLineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        incrementPerLevel = property.FindPropertyRelative("<incrementPerLevel>k__BackingField");
        accumulationPerLevel = property.FindPropertyRelative("<accumulationPerLevel>k__BackingField");

        EditorGUI.BeginProperty(position, label, property);

        property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.size.x, singleLineHeight), property.isExpanded, label);

        if (property.isExpanded)
        {
            EditorGUI.BeginChangeCheck();

            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, singleLineHeight), incrementPerLevel, new GUIContent("Increment Per Level"));
            position.y += newLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, singleLineHeight), accumulationPerLevel, new GUIContent("Accumulation Per Level"));

            if (EditorGUI.EndChangeCheck())
            {
                AnimationCurve incrementCurve = incrementPerLevel.animationCurveValue;
                AnimationCurve accumulationCurve = accumulationPerLevel.animationCurveValue;

                if (incrementPerLevel.serializedObject.hasModifiedProperties)
                {
                    AnimationCurve modifiedAccumulationCurve = IncrementToAccumulation(incrementCurve);
                    accumulationPerLevel.animationCurveValue = modifiedAccumulationCurve;
                    modificationCheck = true;
                }

                if (accumulationPerLevel.serializedObject.hasModifiedProperties && !modificationCheck)
                {
                    AnimationCurve modifiedIncrementCurve = AccumulationToIncrement(accumulationCurve);
                    incrementPerLevel.animationCurveValue = modifiedIncrementCurve;
                }
            }
        }

        EditorGUI.EndProperty();
    }

    private AnimationCurve IncrementToAccumulation(AnimationCurve incrementCurve)
    {
        AnimationCurve accumulationCurve = new AnimationCurve();
        float accumulatedValue = 0.0f;

        for (int keyIndex = 0; keyIndex < incrementCurve.keys.Length; keyIndex++)
        {
            Keyframe currentKey = incrementCurve.keys[keyIndex];
            accumulatedValue += currentKey.value;

            Keyframe accumulationKey = new Keyframe(currentKey.time, accumulatedValue);
            accumulationCurve.AddKey(accumulationKey);
        }

        return accumulationCurve;
    }

    private AnimationCurve AccumulationToIncrement(AnimationCurve accumulationCurve)
    {
        AnimationCurve incrementCurve = new AnimationCurve();

        for (int keyIndex = 0; keyIndex < accumulationCurve.keys.Length; keyIndex++)
        {
            Keyframe currentKey = accumulationCurve.keys[keyIndex];
            float incrementValue = keyIndex == 0 ? currentKey.value : currentKey.value - accumulationCurve.keys[keyIndex - 1].value;

            Keyframe incrementKey = new Keyframe(currentKey.time, incrementValue);
            incrementCurve.AddKey(incrementKey);
        }

        return incrementCurve;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float newLineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        float multiplier = 1;

        incrementPerLevel = property.FindPropertyRelative("<incrementPerLevel>k__BackingField");
        accumulationPerLevel = property.FindPropertyRelative("<accumulationPerLevel>k__BackingField");

        if (incrementPerLevel.isExpanded)
        {
            multiplier += 2;
        }
        if (accumulationPerLevel.isExpanded)
        {
            multiplier += 2;
        }

        return multiplier * newLineHeight;
    }
}
