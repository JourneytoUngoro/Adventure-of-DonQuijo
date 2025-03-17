using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public bool canInteract { get; set; }
    public abstract void Interact();
}
