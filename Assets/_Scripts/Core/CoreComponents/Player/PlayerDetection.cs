using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

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
        workSpace.Set(data.lastPlayerPosition.x, data.lastPlayerPosition.y + data.lastPlayerPosition.z, data.lastPlayerPosition.z);
        currentScreenPosition = workSpace;
        currentSpacePosition = data.lastPlayerPosition;
        entity.entityRigidbody.position = data.lastPlayerPosition;
        workSpace.Set(0, data.lastPlayerPosition.z, data.lastPlayerPosition.z);
        entity.orthogonalRigidbody.transform.localPosition = workSpace;
    }

    public void SaveData(GameData data)
    {
        data.lastPlayerPosition = lastGroundedPosition;
    }
}
