using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneConnectorInteraction : InteractBase
{
    public enum Direction { None, Up, Down, Left, Right, All }

    [SerializeField] private Direction direction;
    [SerializeField] private SceneField targetScene;
    [SerializeField] private Transform destinationPosition;

    private Direction initialDirection;

    private void Awake()
    {
        initialDirection = direction;
    }

    public override void Interact()
    {
        canInteract = false;

        switch (direction)
        {
            case Direction.None:
                return;
            case Direction.Up:
                if (Player.Instance.movement.currentVelocity.y < 0) return; break;
            case Direction.Down:
                if (Player.Instance.movement.currentVelocity.y > 0) return; break;
            case Direction.Left:
                if (Player.Instance.movement.currentVelocity.x > 0) return; break;
            case Direction.Right:
                if (Player.Instance.movement.currentVelocity.x < 0) return; break;
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

    protected override void Update()
    {
        base.Update();

        bool activate = true;

        Enemy[] currentSceneEnemies = FindObjectsByType<Enemy>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        foreach (Enemy enemy in currentSceneEnemies)
        {
            if (enemy.gameObject.scene.Equals(gameObject.scene))
            {
                if (!enemy.isDead)
                {
                    activate = false;
                    break;
                }
            }
        }

        direction = activate ? initialDirection : Direction.None;
    }
}
