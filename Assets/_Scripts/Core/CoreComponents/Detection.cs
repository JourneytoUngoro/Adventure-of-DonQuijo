using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum CheckPositionAxis { Horizontal, Vertical }
public enum CheckPositionDirection { Front, Back, Heading }

public abstract class Detection : CoreComponent
{
    // TODO: All functions should be revised when using extra script for describing height.

    [field: SerializeField] public LayerMask whatIsGround { get; private set; }

    #region Check Transform
    // [SerializeField] protected Vector2 groundCheckLength;
    // [SerializeField] protected OverlapCollider ledgeCheck;
    // [SerializeField, Min(0.01f)] private float recaliberationStepSize;
    // [SerializeField] private const int maxIteration = 1000;
    #endregion

    [SerializeField] private bool showPositionInformation;
    [SerializeField] private bool showColliderInformation;

    // Suppose that we want the character to appear in (x, y, z) position in 3D space
    public Vector3 currentScreenPosition { get; protected set; } // orthogonal rigidbody's screen position: (x, y + z, z)
    public Vector3 currentSpacePosition { get; protected set; } // orthogonal rigidbody's position in space: (x, y, z)
    public Vector3 currentProjectedPosition { get; protected set; } // orthogonal rigidbody's position when it is projected on plane: (x, y, projected ground height)
    public float currentEntityHeight { get; protected set; } // orthogonal rigidbody's local position = (0, z, z)
    public float currentGroundHeight { get; protected set; } // projected ground height
    public float currentCeilingHeight { get; protected set; } // Do we need this?
    public Vector2 horizontalGroundHeight { get; protected set; } // height of the ground in horizontal axis (forwardDirection, backwardDirection)
    public Vector2 verticalGroundHeight { get; protected set; } // height of the ground in vertical axis (upwardDirection, downwardDirection)

    protected Collider2D currentGroundCollider;
    protected Collider2D forwardGroundCollider;
    protected Collider2D backwardGroundCollider;
    protected Collider2D upwardGroundCollider;
    protected Collider2D downwardGroundCollider;

    // Collider offset's y position should be zero.
    protected virtual void FixedUpdate()
    {
        currentScreenPosition = entity.orthogonalRigidbody.transform.position;
        currentEntityHeight = entity.orthogonalRigidbody.transform.localPosition.z;

        workSpace.Set(entity.transform.position.x, entity.transform.position.y, currentEntityHeight);
        currentSpacePosition = workSpace;

        workSpace.Set(entity.collider.offset.x * entity.entityMovement.facingDirection, entity.collider.offset.y, 0.0f);
        Vector2 groundCheckPosition = entity.transform.position + workSpace;
        currentGroundHeight = GetCurrentGroundHeight(groundCheckPosition);

        workSpace.Set(entity.transform.position.x, entity.transform.position.y, currentGroundHeight);
        currentProjectedPosition = workSpace;

        workSpace.Set(GetSurroundingGroundHeight(groundCheckPosition, CheckPositionAxis.Horizontal, CheckPositionDirection.Front), GetSurroundingGroundHeight(groundCheckPosition, CheckPositionAxis.Horizontal, CheckPositionDirection.Back), 0.0f);
        horizontalGroundHeight = workSpace;
        workSpace.Set(GetSurroundingGroundHeight(groundCheckPosition, CheckPositionAxis.Vertical, CheckPositionDirection.Front), GetSurroundingGroundHeight(groundCheckPosition, CheckPositionAxis.Vertical, CheckPositionDirection.Back), 0.0f);
        verticalGroundHeight = workSpace;

        if (showPositionInformation)
        {
            Debug.Log($"ScreenPosition: {currentScreenPosition}, SpacePosition: {currentSpacePosition}, ProjectedPosition: {currentProjectedPosition}, EntityHeight: {currentEntityHeight}, GroundHeight: {currentGroundHeight}, FacingHeight: {horizontalGroundHeight}");
        }
        if (showColliderInformation)
        {
            //Debug.Log($"CurrentGround: {currentGroundCollider?.name}, HorizontalFacingObstacle: ({forwardGroundCollider?.name}: {horizontalGroundHeight.x}, {backwardGroundCollider?.name}: {horizontalGroundHeight.y}), VerticalFacingObstacle: ({upwardGroundCollider?.name}: {verticalGroundHeight.x}, {downwardGroundCollider?.name}: {verticalGroundHeight.y})");
        }
    }

    public virtual bool isGrounded()
    {
        return currentEntityHeight <= currentGroundHeight + epsilon && entity.orthogonalRigidbody.velocity < epsilon;
    }

    public float GetCurrentGroundHeight(Vector2 groundCheckPosition)
    {
        currentGroundCollider = Physics2D.OverlapBoxAll(groundCheckPosition, entity.collider.size, 0.0f, whatIsGround, -Mathf.Infinity, currentEntityHeight).OrderByDescending(groundCollider => groundCollider.transform.position.z).FirstOrDefault();
        return currentGroundCollider ? currentGroundCollider.transform.position.z : 0.0f;
    }

    public float GetSurroundingGroundHeight(Vector2 groundCheckPosition, CheckPositionAxis checkPositionAxis, CheckPositionDirection checkPositionDirection)
    {
        if (checkPositionAxis == CheckPositionAxis.Horizontal)
        {
            if (checkPositionDirection == CheckPositionDirection.Front)
            {
                forwardGroundCollider = Physics2D.OverlapBoxAll(groundCheckPosition + (Vector2)entity.orthogonalRigidbody.transform.right * (entity.collider.bounds.extents.x + entity.collider.bounds.extents.y), Vector2.one * entity.collider.bounds.size.y, 0.0f, whatIsGround).Where(overlap => overlap != currentGroundCollider).OrderByDescending(overlap => overlap.transform.position.z).FirstOrDefault();
                return forwardGroundCollider ? forwardGroundCollider.transform.position.z : currentGroundHeight;
            }
            else if (checkPositionDirection == CheckPositionDirection.Back)
            {
                backwardGroundCollider = Physics2D.OverlapBoxAll(groundCheckPosition - (Vector2)entity.orthogonalRigidbody.transform.right * (entity.collider.bounds.extents.x + entity.collider.bounds.extents.y), Vector2.one * entity.collider.bounds.size.y, 0.0f, whatIsGround).Where(overlap => overlap != currentGroundCollider).OrderByDescending(overlap => overlap.transform.position.z).FirstOrDefault();
                return backwardGroundCollider ? backwardGroundCollider.transform.position.z : currentGroundHeight;
            }
            else if (checkPositionDirection == CheckPositionDirection.Heading)
            {
                if (entity.rigidbody.velocity.x * entity.entityMovement.facingDirection < 0)
                {
                    return GetSurroundingGroundHeight(groundCheckPosition, checkPositionAxis, CheckPositionDirection.Back);
                }
                else
                {
                    return GetSurroundingGroundHeight(groundCheckPosition, checkPositionAxis, CheckPositionDirection.Front);
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(checkPositionDirection), $"Unknown type of CheckPositionDirection \"{checkPositionDirection}\" in {entity.name}.");
            }
        }
        else if (checkPositionAxis == CheckPositionAxis.Vertical)
        {
            if (checkPositionDirection == CheckPositionDirection.Front)
            {
                upwardGroundCollider = Physics2D.OverlapBoxAll(groundCheckPosition + (Vector2)entity.orthogonalRigidbody.transform.up * entity.collider.bounds.size.y, entity.collider.bounds.size, 0.0f, whatIsGround).Where(overlap => overlap != currentGroundCollider).OrderByDescending(overlap => overlap.transform.position.z).FirstOrDefault();
                return upwardGroundCollider ? upwardGroundCollider.transform.position.z : currentGroundHeight;
            }
            else if (checkPositionDirection == CheckPositionDirection.Back)
            {
                downwardGroundCollider = Physics2D.OverlapBoxAll(groundCheckPosition - (Vector2)entity.orthogonalRigidbody.transform.up * entity.collider.bounds.size.y, entity.collider.bounds.size, 0.0f, whatIsGround).Where(overlap => overlap != currentGroundCollider).OrderByDescending(overlap => overlap.transform.position.z).FirstOrDefault();
                return downwardGroundCollider ? downwardGroundCollider.transform.position.z : currentGroundHeight;
            }
            else if (checkPositionDirection == CheckPositionDirection.Heading)
            {
                if (entity.rigidbody.velocity.x * entity.entityMovement.facingDirection < 0)
                {
                    return GetSurroundingGroundHeight(groundCheckPosition, checkPositionAxis, CheckPositionDirection.Back);
                }
                else
                {
                    return GetSurroundingGroundHeight(groundCheckPosition, checkPositionAxis, CheckPositionDirection.Front);
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(checkPositionDirection), $"Unknown type of CheckPositionDirection \"{checkPositionDirection}\" in {entity.name}.");
            }
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(checkPositionAxis), $"Unknown type of CheckPositionAxis \"{checkPositionAxis}\" in {entity.name}.");
        }
    }

    public bool isDetectingCeiling()
    {
        // TODO: Implement ceiling detection. To implement this, we should use extra script to describe informations for each ground objects.
        return false;
    }

    public bool isDetectingLedge(CheckPositionAxis checkPositionAxis, CheckPositionDirection checkPositionDirection)
    {
        if (checkPositionAxis == CheckPositionAxis.Horizontal)
        {
            if (checkPositionDirection == CheckPositionDirection.Front)
            {
                return currentEntityHeight > horizontalGroundHeight.x;
            }
            else if (checkPositionDirection == CheckPositionDirection.Back)
            {
                return currentEntityHeight > horizontalGroundHeight.y;
            }
            else if (checkPositionDirection == CheckPositionDirection.Heading)
            {
                if (entity.rigidbody.velocity.x * entity.entityMovement.facingDirection < 0)
                {
                    return isDetectingLedge(checkPositionAxis, CheckPositionDirection.Front);
                }
                else
                {
                    return isDetectingLedge(checkPositionAxis, CheckPositionDirection.Front);
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(checkPositionDirection), $"Unknown type of CheckPositionDirection \"{checkPositionDirection}\" in {entity.name}.");
            }
        }
        else if (checkPositionAxis == CheckPositionAxis.Vertical)
        {
            if (checkPositionDirection == CheckPositionDirection.Front)
            {
                return currentEntityHeight > verticalGroundHeight.x;
            }
            else if (checkPositionDirection == CheckPositionDirection.Back)
            {
                return currentEntityHeight > verticalGroundHeight.y;
            }
            else if (checkPositionDirection == CheckPositionDirection.Heading)
            {
                if (entity.rigidbody.velocity.x * entity.entityMovement.facingDirection < 0)
                {
                    return isDetectingLedge(checkPositionAxis, CheckPositionDirection.Back);
                }
                else
                {
                    return isDetectingLedge(checkPositionAxis, CheckPositionDirection.Front);
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(checkPositionDirection), $"Unknown type of CheckPositionDirection \"{checkPositionDirection}\" in {entity.name}.");
            }
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(checkPositionAxis), $"Unknown type of CheckPositionAxis \"{checkPositionAxis}\" in {entity.name}.");
        }
    }

    /*private void RecaliberatePosition(Vector2 groundCheckPosition)
    {
        if (currentEntityHeight >= currentGroundHeight) return;

        if (entity.rigidbody.velocity.sqrMagnitude > epsilon)
        {
            entity.transform.position = RecaliberatePosition(groundCheckPosition, -entity.rigidbody.velocity, recaliberationStepSize);
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

        workSpace.Set(entity.transform.position.x, entity.transform.position.y, currentEntityHeight);
        currentProjectedPosition = workSpace;

        if (isGrounded())
        {
            if (Mathf.Abs(entity.rigidbody.velocity.x) > epsilon)
            {
                entity.entityMovement.SetVelocityX(0.0f);
            }

            if (Mathf.Abs(entity.rigidbody.velocity.y) > epsilon)
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
            if (GetCurrentGroundHeight(currentPosition) <= currentEntityHeight)
            {
                break;
            }
            currentIteration += 1;
            currentPosition += recaliberateStepSize * recaliberateDirection.normalized;
        }

        workSpace.Set(currentPosition.x, currentPosition.y, currentEntityHeight);
        return workSpace;
    }*/

    protected virtual void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            // TODO: Detection range a bit wrong. Should flip collider offset when flips the character.
            workSpace.Set(entity.collider.offset.x * entity.entityMovement.facingDirection, entity.collider.offset.y, 0.0f);
            Vector2 groundCheckPosition = currentProjectedPosition + workSpace;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(groundCheckPosition + (Vector2)entity.orthogonalRigidbody.transform.right * (entity.collider.bounds.extents.x + entity.collider.bounds.extents.y), Vector2.one * entity.collider.bounds.size.y);
            Gizmos.DrawWireCube(groundCheckPosition - (Vector2)entity.orthogonalRigidbody.transform.right * (entity.collider.bounds.extents.x + entity.collider.bounds.extents.y), Vector2.one * entity.collider.bounds.size.y);
            Gizmos.DrawWireCube(groundCheckPosition + (Vector2)entity.orthogonalRigidbody.transform.up * entity.collider.bounds.size.y, entity.collider.bounds.size);
            Gizmos.DrawWireCube(groundCheckPosition - (Vector2)entity.orthogonalRigidbody.transform.up * entity.collider.bounds.size.y, entity.collider.bounds.size);

            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(groundCheckPosition, entity.collider.size);
        }
        else
        {
            /*Gizmos.color = Color.green;
            Gizmos.DrawWireCube(entity.collider.bounds.center, entity.collider.size);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(entity.collider.bounds.center + entity.orthogonalRigidbody.transform.right * (entity.collider.bounds.extents.x + entity.collider.bounds.extents.y), Vector2.one * entity.collider.bounds.size.y);
            Gizmos.DrawWireCube(entity.collider.bounds.center - entity.orthogonalRigidbody.transform.right * (entity.collider.bounds.extents.x + entity.collider.bounds.extents.y), Vector2.one * entity.collider.bounds.size.y);
            Gizmos.DrawWireCube(entity.collider.bounds.center + entity.orthogonalRigidbody.transform.up * entity.collider.bounds.size.y, entity.collider.bounds.size);
            Gizmos.DrawWireCube(entity.collider.bounds.center - entity.orthogonalRigidbody.transform.up * entity.collider.bounds.size.y, entity.collider.bounds.size);*/
        }
    }
}
