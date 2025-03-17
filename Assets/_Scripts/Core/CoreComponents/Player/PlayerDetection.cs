using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDetection : Detection
{
    public List<Collider2D> groundColliders;

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

        foreach (Collider2D groundCollider in groundColliders)
        {
            if (groundCollider != null)
            {
                Physics2D.IgnoreCollision(groundCollider, player.entityCollider, player.transform.position.z >= groundCollider.transform.position.z);
            }
        }
    }

    private void GetLoadedGrounds(Scene loadedScene, LoadSceneMode loadSceneMode)
    {
        groundColliders = UtilityFunctions.FindGameObjectsByLayer(whatIsGround, FindObjectsSortMode.None).Select(groundObject => groundObject.GetComponent<Collider2D>()).ToList();
    }

    public override bool isGrounded()
    {
        if (player.playerStateMachine.currentState.GetType().IsSubclassOf(typeof(PlayerGroundedState)))
        {
            return currentEntityHeight <= currentGroundHeight + 0.005f;
        }
        else
        {
            return currentEntityHeight <= currentGroundHeight + 0.005f && player.rigidBody.velocity.y < epsilon;
        }
    }

    public override Vector2 GetFacingObstacleHeight()
    {
        base.GetFacingObstacleHeight();
        
        v2WorkSpace.Set(0.0f, Manager.Instance.inputHandler.normInputY);
        RaycastHit2D verticalFacingObstacle = Physics2D.BoxCastAll(currentProjectedPosition + groundCheckOffset * player.movement.facingDirection, groundCheck.boxSize, 0.0f, v2WorkSpace, 0.1f, whatIsGround, currentEntityHeight).Where(facingObstacle => facingObstacle.collider != currentGroundCollider).OrderBy(facingObstacle => facingObstacle.distance).FirstOrDefault();
        verticalFacingObstacleCollider = verticalFacingObstacle ? verticalFacingObstacle.collider : null;

        float horizontalHeight = horizontalFacingObstacleCollider ? horizontalFacingObstacleCollider.transform.position.z : currentGroundHeight;
        float verticalHeight = verticalFacingObstacleCollider ? verticalFacingObstacleCollider.transform.position.z : currentGroundHeight;
        v2WorkSpace.Set(horizontalHeight, verticalHeight);

        return v2WorkSpace;
    }
}
