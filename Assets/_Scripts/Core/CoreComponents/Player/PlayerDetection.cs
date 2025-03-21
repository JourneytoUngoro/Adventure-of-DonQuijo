using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
                Physics2D.IgnoreCollision(groundCollider, player.collider, player.transform.position.z >= groundCollider.transform.position.z);
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
            return currentEntityHeight <= currentGroundHeight + epsilon;
        }
        else
        {
            return currentEntityHeight <= currentGroundHeight + epsilon && player.rigidBody.velocity.y < epsilon;
        }
    }
}
