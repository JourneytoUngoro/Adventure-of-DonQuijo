using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class ClickableObject : MonoBehaviour, IClickable
{
    public bool canClick { get; set; }

    [field: SerializeField] public UnityAction onClick {  get; set; }

    private void OnMouseDown()
    {
        Debug.Log("clicked!");
        onClick?.Invoke();
    }
}
