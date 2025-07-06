using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(UnityEventInvoker))]
public class UnityEventInvokerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        UnityEvent onClick = ((UnityEventInvoker)target).onClick;

        if (Application.isPlaying)
        {
            if (GUILayout.Button("Excute!"))
            {
                onClick?.Invoke();
            }
        }

    }
}
#endif