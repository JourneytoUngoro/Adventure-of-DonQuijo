using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Detection : CoreComponent
{
    [SerializeField] protected LayerMask whatIsGround;

    #region Check Transform
    [SerializeField] protected OverlapCollider groundCheck;
    #endregion

    [SerializeField] private bool showPositionInformation;
    [SerializeField] private bool showColliderInformation;

    // Suppose that we want the character to appear in (x, y, z) position in 3D space
    public Vector3 currentScreenPosition { get; protected set; } // entity's screen position: (x, y + z, z)
    public Vector3 currentSpacePosition { get; protected set; } // entity's position in space: (x, y, z)
    public Vector3 currentProjectedPosition { get; protected set; } // entity's position when it is projected on plane: (x, y, projected ground height)
    public float currentEntityHeight { get; protected set; } // current entity's height: z
    public float currentGroundHeight { get; protected set; } // height of the ground at entity's projected position: projected ground height
    public float currentCeilingHeight { get; protected set; } // Do we need this?
    public Vector2 facingObstacleHeight { get; protected set; } // height of the ground where entity's heading

    protected Collider2D currentGroundCollider;
    protected Collider2D horizontalFacingObstacleCollider;
    protected Collider2D verticalFacingObstacleCollider;

    protected Vector3 groundCheckOffset;

    protected override void Awake()
    {
        base.Awake();

        groundCheckOffset = groundCheck.centerTransform.localPosition;
    }

    protected virtual void Update()
    {
        currentEntityHeight = entity.transform.position.z;
    }

    protected virtual void FixedUpdate()
    {
        currentScreenPosition = entity.transform.position;
        currentEntityHeight = entity.transform.position.z;
        currentSpacePosition = currentScreenPosition + Vector3.down * currentEntityHeight;
        currentGroundHeight = GetCurrentGroundHeight();
        currentProjectedPosition = new Vector3(currentSpacePosition.x, currentSpacePosition.y, currentGroundHeight);
        facingObstacleHeight = GetFacingObstacleHeight();

        if (showPositionInformation)
        {
            Debug.Log($"ScreenPosition: {currentScreenPosition}, SpacePosition: {currentSpacePosition}, ProjectedPosition: {currentProjectedPosition}, EntityHeight: {currentEntityHeight}, GroundHeight: {currentGroundHeight}, FacingHeight: {facingObstacleHeight}");
        }
        if (showColliderInformation)
        {
            Debug.Log($"CurrentGround: {currentGroundCollider?.name}, HorizontalFacingObstacle: {horizontalFacingObstacleCollider?.name}, VerticalFacingObstacle: {verticalFacingObstacleCollider?.name}");
        }
    }

    public virtual bool isGrounded()
    {
        return currentEntityHeight <= currentGroundHeight + epsilon;
    }

    public float GetCurrentGroundHeight()
    {
        currentGroundCollider = Physics2D.OverlapBoxAll(currentProjectedPosition + groundCheckOffset * entity.entityMovement.facingDirection, groundCheck.boxSize, 0.0f, whatIsGround).OrderByDescending(groundCollider => groundCollider.transform.position.z).FirstOrDefault();
        return currentGroundCollider ? currentGroundCollider.transform.position.z : 0.0f;
    }

    // Below function works supposing that obstacle will be on ground.
    public virtual Vector2 GetFacingObstacleHeight()
    {
        RaycastHit2D horizontalFacingObstacle = Physics2D.BoxCastAll(currentProjectedPosition + groundCheckOffset * entity.entityMovement.facingDirection, groundCheck.boxSize, 0.0f, entity.transform.right, 0.1f, whatIsGround, currentEntityHeight).Where(facingObstacle => facingObstacle.collider != currentGroundCollider).OrderBy(facingObstacle => facingObstacle.distance).FirstOrDefault();
        horizontalFacingObstacleCollider = horizontalFacingObstacle ? horizontalFacingObstacle.collider : null;

        return Vector2.zero;
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        if (groundCheck.overlapCircle)
        {
            Gizmos.DrawWireSphere(groundCheck.centerTransform.position, groundCheck.circleRadius);
        }
        else if (groundCheck.overlapBox)
        {
            Gizmos.DrawWireCube(groundCheck.centerTransform.position, groundCheck.boxSize);
        }
    }
}
