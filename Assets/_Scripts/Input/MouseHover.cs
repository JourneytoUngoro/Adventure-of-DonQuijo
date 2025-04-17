using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHover : MonoBehaviour, IHoverable
{
    private GameObject hoverObject;

    private void Awake()
    {
        hoverObject = transform.Find("Outline Object")?.gameObject;

        if (hoverObject != null )
        {
            hoverObject.SetActive(false);
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
        Debug.Assert(hoverObject != null, "Outline Object is null");
        hoverObject?.gameObject.SetActive(true);
    }

    public void OnHorverExit()
    {
        hoverObject?.gameObject.SetActive(false);
    }

}
