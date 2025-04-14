using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHoverOutline : MonoBehaviour, IHoverable
{
    private GameObject outlineObject;

    private void Awake()
    {
        outlineObject = transform.Find("Outline Object")?.gameObject;

        if (outlineObject != null )
        {
            outlineObject.SetActive(false);
        }
    }

    private void OnMouseEnter()
    {
        OnHorverEnter();  
    }

    private void OnMouseExit()
    {
        OnHorverExit();
    }

    public void OnHorverEnter()
    {
        Debug.Assert(outlineObject != null, "Outline Object is null");
        outlineObject?.gameObject.SetActive(true);
    }

    public void OnHorverExit()
    {
        outlineObject?.gameObject.SetActive(false);
    }

}
