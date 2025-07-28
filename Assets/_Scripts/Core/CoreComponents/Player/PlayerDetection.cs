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
        Debug.Log("OnDetectionAwake: " + transform.position);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        Debug.Log("OnDetectionOnEnable: " + transform.position);
    }

    protected void Start()
    {
        Debug.Log("OnDetectionStart: " + transform.position);
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
        Debug.Log("Position Data Loaded: " + data.lastPlayerPosition);
        SetPosition(data.lastPlayerPosition);
        lastGroundedPosition = data.lastPlayerPosition;
        Debug.Log("Set Position To: " + entity.transform.position);
    }

    public void SaveData(GameData data)
    {
        data.lastPlayerPosition = lastGroundedPosition;
        Debug.Log("Position Data Saved: " + data.lastPlayerPosition);
    }
}
