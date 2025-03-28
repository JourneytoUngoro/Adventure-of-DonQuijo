using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDetection : Detection, IDataPersistance
{
    public List<Collider2D> groundColliders;
    private Vector3 lastGroundedPosition;
    private Player player;

    protected override void Awake()
    {
        base.Awake();

        player = entity as Player;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += GetLoadedGrounds;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= GetLoadedGrounds;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (isGrounded())
        {
            lastGroundedPosition = currentSpacePosition;
        }

        foreach (Collider2D groundCollider in groundColliders)
        {
            if (groundCollider != null)
            {
                Physics2D.IgnoreCollision(groundCollider, player.collider, player.orthogonalRigidbody.transform.localPosition.z >= groundCollider.transform.position.z);
            }
        }
    }

    private void GetLoadedGrounds(Scene loadedScene, LoadSceneMode loadSceneMode)
    {
        groundColliders = UtilityFunctions.FindGameObjectsByLayer(whatIsGround, FindObjectsSortMode.None).Select(groundObject => groundObject.GetComponent<Collider2D>()).ToList();
    }

    public bool isTouchingWall()
    {
        return currentGroundHeight + epsilon < currentEntityHeight && currentEntityHeight + player.playerData.wallTouchHeight < horizontalGroundHeight.x;
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
