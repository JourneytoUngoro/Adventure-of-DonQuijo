using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class InteractBase : MonoBehaviour, IInteractable
{
    [SerializeField] private InteractionType interactionType;
    [SerializeField, EnumFlags] private ObjectType interactionObjectType;
    public bool canInteract { get; set; }
    private enum InteractionType { Object, NPC, Door, SceneTransition }
    private enum ObjectType { Interactable, Breakable, Moveable }

    public abstract void Interact();

    protected virtual void Update()
    {
        if (canInteract)
        {
            switch (interactionType)
            {
                case InteractionType.SceneTransition:
                    Interact();
                    break;

                case InteractionType.Door:
                case InteractionType.NPC:
                case InteractionType.Object:
                    if (Manager.Instance.inputHandler.interactInputPressed)
                    {
                        Interact();
                    }
                    break;

                default:
                    break;
            }
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerInteraction"))
        {
            Player player = collision.gameObject.GetComponentInParent<Player>();

            if (interactionType != InteractionType.SceneTransition)
            {
                player.interactableGameObjects.Add(this);
                player.interactableGameObjects.OrderBy(interactable => Vector3.SqrMagnitude(player.transform.position - interactable.transform.position));
                player.interactableGameObjects.ForEach(interactable => interactable.canInteract = false);
                player.interactableGameObjects.FirstOrDefault().canInteract = true; 
            }
        }
    }

    protected virtual void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerInteraction"))
        {
            Player player = collision.gameObject.GetComponentInParent<Player>();

            if (interactionType != InteractionType.SceneTransition)
            {
                canInteract = false;
                player.interactableGameObjects.Remove(this);
                if (!player.interactableGameObjects.Empty())
                {
                    player.interactableGameObjects.FirstOrDefault().canInteract = true;
                }
            }
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerInteraction"))
        {
            Player player = collision.gameObject.GetComponentInParent<Player>();

            if (interactionType != InteractionType.SceneTransition)
            {
                player.interactableGameObjects.Add(this);
                player.interactableGameObjects.OrderBy(interactable => Vector3.SqrMagnitude(player.transform.position - interactable.transform.position));
                player.interactableGameObjects.ForEach(interactable => interactable.canInteract = false);
                player.interactableGameObjects.FirstOrDefault().canInteract = true;
            }
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerInteraction"))
        {
            Player player = collision.gameObject.GetComponentInParent<Player>();

            if (interactionType != InteractionType.SceneTransition)
            {
                canInteract = false;
                player.interactableGameObjects.Remove(this);
                if (!player.interactableGameObjects.Empty())
                {
                    player.interactableGameObjects.FirstOrDefault().canInteract = true;
                }
            }
        }
    }
}
