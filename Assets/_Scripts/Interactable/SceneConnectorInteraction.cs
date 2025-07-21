using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneConnectorInteraction : InteractBase
{
    public enum Direction { None, Up, Down, Left, Right, All }

    [SerializeField] private Direction direction;
    [SerializeField] private SceneField targetScene;
    [SerializeField] private Transform destinationPosition;

    public override void Interact()
    {
        canInteract = false;

        switch (direction)
        {
            case Direction.None:
                return;
            case Direction.Up:
                if (Manager.Instance.player.movement.currentVelocity.y < 0) return; break;
            case Direction.Down:
                if (Manager.Instance.player.movement.currentVelocity.y > 0) return; break;
            case Direction.Left:
                if (Manager.Instance.player.movement.currentVelocity.x > 0) return; break;
            case Direction.Right:
                if (Manager.Instance.player.movement.currentVelocity.x < 0) return; break;
            case Direction.All:
                break;
            default: return;
        }

        if (destinationPosition == null)
        {
            Manager.Instance.sceneTransitionManager.SceneTransition(targetScene, direction);
        }
        else
        {
            Manager.Instance.sceneTransitionManager.SceneTransition(targetScene, destinationPosition);
        }
    }
}
