using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApproachOutline : MonoBehaviour, IInteractable
{
    public GameObject outlineObject;
    public LayerMask layer;

    public bool canInteract { get; set; }

    public void Interact()
    {
        // TODO : 플레이어와 상호작용해야 한다 
    }

    private void Awake()
    {
        outlineObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & layer) != 0)
        {
            outlineObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & layer) != 0)
        {
            outlineObject.SetActive(false);
        }
    }

}
