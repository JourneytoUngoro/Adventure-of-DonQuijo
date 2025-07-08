using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class UnityEventInvoker : MonoBehaviour
{
    [PropertyOrder(-10)]
    [InfoBox("@customEditorDescription")]

    [SerializeField, TextArea(2, 3)]
    private string customEditorDescription = "";

    public UnityEvent onClick;
}
