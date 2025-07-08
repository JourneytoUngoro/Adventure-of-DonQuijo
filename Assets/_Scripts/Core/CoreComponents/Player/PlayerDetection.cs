using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDetection : Detection, IDataPersistance
{
    private Vector3 lastGroundedPosition;
    private Player player;

    protected override void Awake()
    {
        base.Awake();

        player = entity as Player;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (IsGrounded())
        {
            lastGroundedPosition = currentSpacePosition;
        }
    }

    public bool isTouchingWall()
    {
        return forwardObstacleColliders.Where(overlap => overlap != null && overlap != currentGroundCollider).Any(overlap => !(overlap.transform.position.z > currentEntityHeight + player.playerData.wallTouchHeight || overlap.transform.position.z + overlap.GetComponent<HeightData>().height < currentEntityHeight));
    }

    public void LoadData(GameData data)
    {
        currentSpacePosition = data.lastPlayerPosition;
        currentProjectedPosition = currentSpacePosition;
        currentEntityHeight = currentSpacePosition.z;
        currentGroundHeight = currentSpacePosition.z;
        player.transform.position = new Vector3(currentSpacePosition.x, currentSpacePosition.y, 0.0f);
        player.orthogonalRigidbody.transform.localPosition = new Vector3(0.0f, currentEntityHeight, currentEntityHeight);
        currentScreenPosition = player.orthogonalRigidbody.transform.position;
    }

    public void SaveData(GameData data)
    {
        data.lastPlayerPosition = lastGroundedPosition;
    }
}
