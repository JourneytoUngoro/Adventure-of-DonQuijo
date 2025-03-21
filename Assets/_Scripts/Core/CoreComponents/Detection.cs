using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Detection : CoreComponent
{
    [SerializeField] protected LayerMask whatIsGround;

    #region Check Transform
    [SerializeField] protected Transform groundCheckTransform;
    [SerializeField] protected Vector2 groundCheckSize;
    [SerializeField] protected Vector2 groundCheckLength;
    [SerializeField, Min(0.01f)] private float recaliberationStepSize;
    [SerializeField] private const int maxIteration = 1000;
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
    public Vector2 horizontalObstacleHeight { get; protected set; } // height of the ground in horizontal axis (facingDirection, oppositeDirection)
    public Vector2 verticalObstacleHeight { get; protected set; } // height of the ground in vertical axis (upDirection, downDirection)

    protected Collider2D currentGroundCollider;
    protected Collider2D facingObstacleCollider;
    protected Collider2D oppositeObstacleCollider;
    protected Collider2D upwardObstacleCollider;
    protected Collider2D downwardObstacleCollider;

    protected virtual void Update()
    {
        currentEntityHeight = entity.transform.position.z;
        // entity.collider.offset = new Vector2(groundCheckOffset.x, -currentEntityHeight);
    }

    protected virtual void FixedUpdate()
    {
        RecaliberatePosition(currentProjectedPosition + groundCheckTransform.localPosition * entity.entityMovement.facingDirection);
        currentScreenPosition = entity.transform.position;
        currentEntityHeight = entity.transform.position.z;
        currentSpacePosition = currentScreenPosition + Vector3.down * currentEntityHeight;
        currentGroundHeight = GetCurrentGroundHeight(currentProjectedPosition + groundCheckTransform.localPosition * entity.entityMovement.facingDirection);
        horizontalObstacleHeight = GetHorizontalObstacleHeight(currentProjectedPosition + groundCheckTransform.localPosition * entity.entityMovement.facingDirection);
        verticalObstacleHeight = GetVerticalObstacleHeight(currentProjectedPosition + groundCheckTransform.localPosition * entity.entityMovement.facingDirection);
        currentProjectedPosition = new Vector3(currentSpacePosition.x, currentSpacePosition.y, currentGroundHeight);

        if (showPositionInformation)
        {
            Debug.Log($"ScreenPosition: {currentScreenPosition}, SpacePosition: {currentSpacePosition}, ProjectedPosition: {currentProjectedPosition}, EntityHeight: {currentEntityHeight}, GroundHeight: {currentGroundHeight}, FacingHeight: {horizontalObstacleHeight}");
        }
        if (showColliderInformation)
        {
            Debug.Log($"CurrentGround: {currentGroundCollider?.name}, HorizontalFacingObstacle: ({facingObstacleCollider?.name}: {horizontalObstacleHeight.x}, {oppositeObstacleCollider?.name}: {horizontalObstacleHeight.y}), VerticalFacingObstacle: ({upwardObstacleCollider?.name}: {verticalObstacleHeight.x}, {downwardObstacleCollider?.name}: {verticalObstacleHeight.y})");
        }
    }

    public virtual bool isGrounded()
    {
        return currentEntityHeight <= currentGroundHeight + epsilon;
    }

    public float GetCurrentGroundHeight(Vector2 groundCheckPosition)
    {
        currentGroundCollider = Physics2D.OverlapBoxAll(groundCheckPosition, groundCheckSize, 0.0f, whatIsGround).OrderByDescending(groundCollider => groundCollider.transform.position.z).FirstOrDefault();
        return currentGroundCollider ? currentGroundCollider.transform.position.z : 0.0f;
    }

    // Below function works supposing that obstacle will be on ground.
    public Vector2 GetHorizontalObstacleHeight(Vector2 groundCheckPosition)
    {
        RaycastHit2D facingObstacle = Physics2D.BoxCastAll(groundCheckPosition, groundCheckSize, 0.0f, entity.transform.right, groundCheckLength.x, whatIsGround, currentEntityHeight).Where(facingObstacle => facingObstacle.collider != currentGroundCollider).OrderBy(facingObstacle => facingObstacle.distance).FirstOrDefault();
        facingObstacleCollider = facingObstacle ? facingObstacle.collider : null;

        RaycastHit2D oppositeObstacle = Physics2D.BoxCastAll(groundCheckPosition, groundCheckSize, 0.0f, -entity.transform.right, groundCheckLength.x, whatIsGround, currentEntityHeight).Where(facingObstacle => facingObstacle.collider != currentGroundCollider).OrderBy(facingObstacle => facingObstacle.distance).FirstOrDefault();
        oppositeObstacleCollider = oppositeObstacle ? oppositeObstacle.collider : null;

        float facingHeight = facingObstacleCollider ? facingObstacleCollider.transform.position.z : currentGroundHeight;
        float oppositeHeight = oppositeObstacleCollider ? oppositeObstacleCollider.transform.position.z : currentGroundHeight;
        v2WorkSpace.Set(facingHeight, oppositeHeight);

        return v2WorkSpace;
    }

    public Vector2 GetVerticalObstacleHeight(Vector2 groundCheckPosition)
    {
        RaycastHit2D upwardObstacle = Physics2D.BoxCastAll(groundCheckPosition, groundCheckSize, 0.0f, transform.up, groundCheckLength.y, whatIsGround, currentEntityHeight).Where(facingObstacle => facingObstacle.collider != currentGroundCollider).OrderBy(facingObstacle => facingObstacle.distance).FirstOrDefault();
        upwardObstacleCollider = upwardObstacle ? upwardObstacle.collider : null;

        RaycastHit2D downwardObstacle = Physics2D.BoxCastAll(groundCheckPosition, groundCheckSize, 0.0f, -transform.up, groundCheckLength.y, whatIsGround, currentEntityHeight).Where(facingObstacle => facingObstacle.collider != currentGroundCollider).OrderBy(facingObstacle => facingObstacle.distance).FirstOrDefault();
        downwardObstacleCollider = downwardObstacle ? downwardObstacle.collider : null;

        float upwardHeight = upwardObstacleCollider ? upwardObstacleCollider.transform.position.z : currentGroundHeight;
        float downwardHeight = downwardObstacleCollider ? downwardObstacleCollider.transform.position.z : currentGroundHeight;
        v2WorkSpace.Set(upwardHeight, downwardHeight);

        return v2WorkSpace;
    }

    private void RecaliberatePosition(Vector2 groundCheckPosition)
    {
        if (currentEntityHeight >= currentGroundHeight) return;

        if (entity.rigidBody.velocity.sqrMagnitude > epsilon)
        {
            entity.transform.position = RecaliberatePosition(groundCheckPosition, -entity.rigidBody.velocity, recaliberationStepSize);
        }
        else
        {
            if (currentGroundCollider.GetType().Equals(typeof(PolygonCollider2D)))
            {
                PolygonCollider2D groundPolygon = currentGroundCollider as PolygonCollider2D;

                float minDistance = float.MaxValue;
                Vector3 minClosestPoint = transform.position;

                for (int i = 0; i < groundPolygon.points.Count(); i++)
                {
                    Vector2 pointA = (Vector2)groundPolygon.transform.position + groundPolygon.points[i];
                    Vector2 pointB = (Vector2)groundPolygon.transform.position + groundPolygon.points[(i + 1) % groundPolygon.points.Count()];

                    Vector3 currentClosestPoint = groundCheckPosition.ClosestPointOnLine(pointA, pointB, false);
                    float currentDistance = Vector3.Distance(currentClosestPoint, transform.position);

                    if (currentDistance < minDistance)
                    {
                        minDistance = currentDistance;
                        minClosestPoint = currentClosestPoint;
                    }
                }

                Vector2 recaliberateDirection = groundPolygon.OverlapPoint(groundCheckPosition) ? (minClosestPoint - transform.position).normalized : (transform.position - minClosestPoint).normalized;
                entity.transform.position = recaliberateDirection != Vector2.zero ? RecaliberatePosition(minClosestPoint, recaliberateDirection, recaliberationStepSize) : transform.position;
            }
            else
            {
                // TODO: Use ColliderDistance2D for non-polygon type colliders
            }
        }

        v3WorkSpace.Set(entity.transform.position.x, entity.transform.position.y, currentEntityHeight);
        currentProjectedPosition = v3WorkSpace;

        if (isGrounded())
        {
            if (Mathf.Abs(entity.rigidBody.velocity.x) > epsilon)
            {
                entity.entityMovement.SetVelocityX(0.0f);
            }

            if (Mathf.Abs(entity.rigidBody.velocity.y) > epsilon)
            {
                entity.entityMovement.SetVelocityY(0.0f);
            }
        }
        else
        {
            entity.entityMovement.SetVelocityX(0.0f);
        }
    }

    private Vector3 RecaliberatePosition(Vector2 startPosition, Vector2 recaliberateDirection, float recaliberateStepSize)
    {
        int currentIteration = 0;
        Vector2 currentPosition = startPosition + recaliberateDirection.normalized * recaliberateStepSize;

        while (currentIteration < maxIteration)
        {
            /*if (Physics2D.BoxCastAll(currentPosition, groundCheckSize, 0.0f, entity.transform.right, groundCheckLength.x).Where(boxHit => boxHit.transform.position.z > currentEntityHeight).Empty() && Physics2D.BoxCastAll(currentPosition, groundCheckSize, 0.0f, -entity.transform.right, groundCheckLength.x).Where(boxHit => boxHit.transform.position.z > currentEntityHeight).Empty() && Physics2D.BoxCastAll(currentPosition, groundCheckSize,  0.0f, entity.transform.up , groundCheckLength.y).Where(boxHit => boxHit.transform.position.z > currentEntityHeight).Empty() && Physics2D.BoxCastAll(currentPosition, groundCheckSize, 0.0f, -entity.transform.up, groundCheckLength.y).Where(boxHit => boxHit.transform.position.z > currentEntityHeight).Empty())
            {
                break;
            }*/
            if (GetCurrentGroundHeight(currentPosition) <= currentEntityHeight)
            {
                break;
            }
            currentIteration += 1;
            currentPosition += recaliberateStepSize * recaliberateDirection.normalized;
        }

        return currentPosition;
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundCheckTransform.position, groundCheckSize);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(groundCheckTransform.position + transform.right * groundCheckLength.x, groundCheckSize);
        Gizmos.DrawWireCube(groundCheckTransform.position - transform.right * groundCheckLength.x, groundCheckSize);
        Gizmos.DrawWireCube(groundCheckTransform.position + transform.up * groundCheckLength.y, groundCheckSize);
        Gizmos.DrawWireCube(groundCheckTransform.position - transform.up * groundCheckLength.y, groundCheckSize);
    }
}
