using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(AnimationCurveSet))]
public class AnimationCurveSetEditor : PropertyDrawer
{
    private SerializedProperty incrementPerLevel;
    private SerializedProperty accumulationPerLevel;
    private AnimationCurve prevIncrementCurve;
    private AnimationCurve prevAccumulationCurve;
    private float valueThreshold = 0.01f;

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

                bool incrementCurveModified = !AnimationCurveEquals(incrementCurve, prevIncrementCurve);
                bool accumulationCurveModified = !AnimationCurveEquals(accumulationCurve, prevIncrementCurve);

                if (incrementCurveModified)
                {
                    incrementCurve = SetIndexes(incrementCurve);
                    accumulationCurve = IncrementToAccumulation(incrementCurve);
                }

                if (accumulationCurveModified)
                {
                    accumulationCurve = SetIndexes(accumulationCurve);
                    incrementCurve = AccumulationToIncrement(accumulationCurve);
                }

                incrementPerLevel.animationCurveValue = incrementCurve;
                accumulationPerLevel.animationCurveValue = accumulationCurve;
            }
        }

        prevIncrementCurve = incrementPerLevel.animationCurveValue;
        prevAccumulationCurve = accumulationPerLevel.animationCurveValue;

        EditorGUI.EndProperty();
    }

    private AnimationCurve SetIndexes(AnimationCurve curve)
    {
        AnimationCurve animationCurve = new AnimationCurve();

        int totalKeyCount = curve.keys.Length;
        int maxKey = Mathf.RoundToInt(curve.keys[totalKeyCount - 1].time);

        for (int keyValue = 0; keyValue < maxKey; keyValue++)
        {
            Keyframe keyframe = new Keyframe(keyValue + 1, curve.Evaluate(keyValue + 1));
            animationCurve.AddKey(keyframe);
        }

        return animationCurve;
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

    public bool AnimationCurveEquals(AnimationCurve curveA, AnimationCurve curveB)
    {
        Keyframe[] keysA = curveA.keys;
        Keyframe[] keysB = curveB.keys;

        if (keysA.Length != keysB.Length)
        {
            return false;
        }

        for (int index = 0; index < keysA.Length; index++)
        {
            if (keysA[index].time != keysB[index].time || keysA[index].value != keysB[index].value)
            {
                return false;
            }
        }

        return true;
    }
}
