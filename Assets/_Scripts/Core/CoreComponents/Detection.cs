using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum CheckPositionAxis { Horizontal, Vertical }
public enum CheckPositionDirection { Front, Back, Heading }

public abstract class Detection : CoreComponent
{
    [field: SerializeField] public LayerMask whatIsGround { get; private set; }
    public List<Collider2D> groundColliders { get; private set; }

    [SerializeField] private bool showPositionInformation;
    [SerializeField] private bool showColliderInformation;
    [SerializeField] private bool showFacingObstacleInformation;

    // Suppose that we want the character to appear in (x, y, z) position in 3D space
    public Vector3 currentScreenPosition { get; protected set; } // orthogonal rigidbody's screen position: (x, y + z, z)
    public Vector3 currentSpacePosition { get; protected set; } // orthogonal rigidbody's position in space: (x, y, z)
    public Vector3 currentProjectedPosition { get; protected set; } // orthogonal rigidbody's position when it is projected on plane: (x, y, projected ground height)
    public float currentEntityHeight { get; protected set; } // orthogonal rigidbody's local position = (0, z, z)
    public float currentGroundHeight { get; protected set; } // projected ground height
    public float? currentCeilingHeight { get; protected set; } // Do we need this?
    public bool isGrounded { get; protected set; }
    public pair<bool, bool> detectingHorizontalObstacle { get; protected set; } = new pair<bool, bool>(false, false);
    public pair<bool, bool> detectingVerticalObstacle { get; protected set; } = new pair<bool, bool>(false, false);

    protected Collider2D currentGroundCollider;
    protected Collider2D currentCeilingCollider;
    protected Collider2D[] projectedPositionColliders = new Collider2D[maxDetectionCount];
    protected Collider2D[] forwardObstacleColliders = new Collider2D[maxDetectionCount];
    protected Collider2D[] backwardObstacleColliders = new Collider2D[maxDetectionCount];
    protected Collider2D[] upwardObstacleColliders = new Collider2D[maxDetectionCount];
    protected Collider2D[] downwardObstacleColliders = new Collider2D[maxDetectionCount];

    protected const int maxDetectionCount = 10;

    protected virtual void OnEnable()
    {
        SceneManager.sceneLoaded += GetLoadedGrounds;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= GetLoadedGrounds;
    }

    protected virtual void FixedUpdate()
    {
        currentScreenPosition = entity.orthogonalRigidbody.transform.position;
        currentEntityHeight = entity.orthogonalRigidbody.transform.localPosition.z;

        workSpace.Set(entity.transform.position.x, entity.transform.position.y, currentEntityHeight);
        currentSpacePosition = workSpace;

        workSpace.Set(entity.collider.offset.x * entity.entityMovement.facingDirection, entity.collider.offset.y, 0.0f);
        Vector3 groundCheckPosition = entity.transform.position + workSpace;

        Array.Clear(projectedPositionColliders, 0, maxDetectionCount);
        Physics2D.OverlapBoxNonAlloc(groundCheckPosition, entity.collider.size, 0.0f, projectedPositionColliders, whatIsGround);
        currentGroundHeight = GetCurrentGroundHeight();
        currentCeilingHeight = GetCurrentCeilingHeight();
        isGrounded = IsGrounded();

        workSpace.Set(entity.transform.position.x, entity.transform.position.y, currentGroundHeight);
        currentProjectedPosition = workSpace;

        Array.Clear(forwardObstacleColliders, 0, maxDetectionCount);
        Physics2D.OverlapBoxNonAlloc(groundCheckPosition + entity.orthogonalRigidbody.transform.right * (entity.collider.bounds.extents.x + entity.collider.bounds.extents.y), Vector2.one * entity.collider.bounds.size.y, 0.0f, forwardObstacleColliders, whatIsGround);
        Array.Clear(backwardObstacleColliders, 0, maxDetectionCount);
        Physics2D.OverlapBoxNonAlloc(groundCheckPosition - entity.orthogonalRigidbody.transform.right * (entity.collider.bounds.extents.x + entity.collider.bounds.extents.y), Vector2.one * entity.collider.bounds.size.y, 0.0f, backwardObstacleColliders, whatIsGround);
        Array.Clear(upwardObstacleColliders, 0, maxDetectionCount);
        Physics2D.OverlapBoxNonAlloc(groundCheckPosition + entity.orthogonalRigidbody.transform.up * entity.collider.bounds.size.y, entity.collider.bounds.size, 0.0f, upwardObstacleColliders, whatIsGround);
        Array.Clear(downwardObstacleColliders, 0, maxDetectionCount);
        Physics2D.OverlapBoxNonAlloc(groundCheckPosition - entity.orthogonalRigidbody.transform.up * entity.collider.bounds.size.y, entity.collider.bounds.size, 0.0f, downwardObstacleColliders, whatIsGround);
        detectingHorizontalObstacle.first = IsDetectingObstacle(CheckPositionAxis.Horizontal, CheckPositionDirection.Front);
        detectingHorizontalObstacle.second = IsDetectingObstacle(CheckPositionAxis.Horizontal, CheckPositionDirection.Back);
        detectingVerticalObstacle.first = IsDetectingObstacle(CheckPositionAxis.Vertical, CheckPositionDirection.Front);
        detectingVerticalObstacle.second = IsDetectingObstacle(CheckPositionAxis.Vertical, CheckPositionDirection.Back);

        foreach (Collider2D groundCollider in groundColliders)
        {
            if (groundCollider != null)
            {
                Physics2D.IgnoreCollision(groundCollider, entity.collider, currentEntityHeight >= groundCollider.transform.position.z + groundCollider.GetComponent<HeightData>().height);
            }
        }

        if (showPositionInformation)
        {
            Debug.Log($"Screen position: {currentScreenPosition} | Space position: {currentSpacePosition} | Projected position: {currentProjectedPosition} | Entity Height: {currentEntityHeight} | Ground Height: {currentGroundHeight} | Ceiling Height: {currentCeilingHeight}");
        }
        if (showFacingObstacleInformation)
        {
            Debug.Log($"Horizontal Obstacle: ({detectingHorizontalObstacle.first}, {detectingHorizontalObstacle.second}) | Vertical Obstacle: ({detectingVerticalObstacle.first}, {detectingVerticalObstacle.second})");
        }
        if (showColliderInformation)
        {
            Debug.Log($"CurrentGround: {currentGroundCollider?.name} | Current Ceiling: {currentCeilingCollider?.name} | Forward Obstacle: {string.Join(", ", forwardObstacleColliders.Where(component => component != null).Select(component => $"[{component.name} - ({component.transform.position.z}, {component.transform.position.z + component.GetComponent<HeightData>().height})]"))} | Backward Obstacle: {string.Join(", ", backwardObstacleColliders.Where(component => component != null).Select(component => $"[{component.name} - ({component.transform.position.z}, {component.transform.position.z + component.GetComponent<HeightData>().height})]"))} | Upward Obstacle: {string.Join(", ", upwardObstacleColliders.Where(component => component != null).Select(component => $"[{component.name} - ({component.transform.position.z}, {component.transform.position.z + component.GetComponent<HeightData>().height})]"))} | Downward Obstacle: {string.Join(", ", downwardObstacleColliders.Where(component => component != null).Select(component => $"[{component.name} - ({component.transform.position.z}, {component.transform.position.z + component.GetComponent<HeightData>().height})]"))}");
        }
    }

    public virtual bool IsGrounded()
    {
        return currentEntityHeight <= currentGroundHeight + epsilon && entity.orthogonalRigidbody.velocity < epsilon;
    }

    public float GetCurrentGroundHeight()
    {
        currentGroundCollider = projectedPositionColliders.Where(groundCollider => groundCollider != null && groundCollider.transform.position.z + groundCollider.GetComponent<HeightData>().height <= currentEntityHeight).OrderByDescending(groundCollider => groundCollider.transform.position.z + groundCollider.GetComponent<HeightData>().height).FirstOrDefault();
        return currentGroundCollider ? currentGroundCollider.transform.position.z + currentGroundCollider.GetComponent<HeightData>().height : 0.0f;
    }

    public float? GetCurrentCeilingHeight()
    {
        currentCeilingCollider = projectedPositionColliders.Where(groundCollider => groundCollider != null && groundCollider.transform.position.z >= currentEntityHeight).OrderBy(groundCollider => groundCollider.transform.position.z + groundCollider.GetComponent<HeightData>().height).FirstOrDefault();
        return currentCeilingCollider ? currentCeilingCollider.transform.position.z : null;
    }

    public bool IsDetectingObstacle(CheckPositionAxis checkPositionAxis, CheckPositionDirection checkPositionDirection)
    {
        if (checkPositionAxis == CheckPositionAxis.Horizontal)
        {
            if (checkPositionDirection == CheckPositionDirection.Front)
            {
                return !forwardObstacleColliders.Where(groundCollider => groundCollider != null && groundCollider != currentGroundCollider && !(groundCollider.transform.position.z > currentEntityHeight + entity.currentEntityStature || groundCollider.transform.position.z + groundCollider.GetComponent<HeightData>().height < currentEntityHeight)).Empty();
            }
            else if (checkPositionDirection == CheckPositionDirection.Back)
            {
                return !backwardObstacleColliders.Where(groundCollider => groundCollider != null && groundCollider != currentGroundCollider && !(groundCollider.transform.position.z > currentEntityHeight + entity.currentEntityStature || groundCollider.transform.position.z + groundCollider.GetComponent<HeightData>().height < currentEntityHeight)).Empty();
            }
            else if (checkPositionDirection == CheckPositionDirection.Heading)
            {
                if (entity.rigidbody.velocity.x * entity.entityMovement.facingDirection < 0)
                {
                    return IsDetectingObstacle(checkPositionAxis, CheckPositionDirection.Back);
                }
                else
                {
                    return IsDetectingObstacle(checkPositionAxis, CheckPositionDirection.Front);
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
                return !upwardObstacleColliders.Where(groundCollider => groundCollider != null && groundCollider != currentGroundCollider && !(groundCollider.transform.position.z > currentEntityHeight + entity.currentEntityStature || groundCollider.transform.position.z + groundCollider.GetComponent<HeightData>().height < currentEntityHeight)).Empty();
            }
            else if (checkPositionDirection == CheckPositionDirection.Back)
            {
                return !downwardObstacleColliders.Where(groundCollider => groundCollider != null && groundCollider != currentGroundCollider && !(groundCollider.transform.position.z > currentEntityHeight + entity.currentEntityStature || groundCollider.transform.position.z + groundCollider.GetComponent<HeightData>().height < currentEntityHeight)).Empty();
            }
            else if (checkPositionDirection == CheckPositionDirection.Heading)
            {
                if (entity.rigidbody.velocity.x * entity.entityMovement.facingDirection < 0)
                {
                    return IsDetectingObstacle(checkPositionAxis, CheckPositionDirection.Back);
                }
                else
                {
                    return IsDetectingObstacle(checkPositionAxis, CheckPositionDirection.Front);
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

    public bool IsDetectingLedge(CheckPositionAxis checkPositionAxis, CheckPositionDirection checkPositionDirection)
    {
        if (checkPositionAxis == CheckPositionAxis.Horizontal)
        {
            if (checkPositionDirection == CheckPositionDirection.Front)
            {
                return !detectingHorizontalObstacle.first && forwardObstacleColliders.Where(overlap => overlap != null && overlap != currentGroundCollider).All(overlap => currentEntityHeight > overlap.transform.position.z + overlap.GetComponent<HeightData>().height);
            }
            else if (checkPositionDirection == CheckPositionDirection.Back)
            {
                return !detectingHorizontalObstacle.second && backwardObstacleColliders.Where(overlap => overlap != null && overlap != currentGroundCollider).All(overlap => currentEntityHeight > overlap.transform.position.z + overlap.GetComponent<HeightData>().height);
            }
            else if (checkPositionDirection == CheckPositionDirection.Heading)
            {
                if (entity.rigidbody.velocity.x * entity.entityMovement.facingDirection < 0)
                {
                    return IsDetectingLedge(checkPositionAxis, CheckPositionDirection.Front);
                }
                else
                {
                    return IsDetectingLedge(checkPositionAxis, CheckPositionDirection.Front);
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
                return !detectingVerticalObstacle.first && upwardObstacleColliders.Where(overlap => overlap != null && overlap != currentGroundCollider).All(overlap => currentEntityHeight > overlap.transform.position.z + overlap.GetComponent<HeightData>().height);
            }
            else if (checkPositionDirection == CheckPositionDirection.Back)
            {
                return !detectingVerticalObstacle.second && downwardObstacleColliders.Where(overlap => overlap != null && overlap != currentGroundCollider).All(overlap => currentEntityHeight > overlap.transform.position.z + overlap.GetComponent<HeightData>().height);
            }
            else if (checkPositionDirection == CheckPositionDirection.Heading)
            {
                if (entity.rigidbody.velocity.x * entity.entityMovement.facingDirection < 0)
                {
                    return IsDetectingLedge(checkPositionAxis, CheckPositionDirection.Back);
                }
                else
                {
                    return IsDetectingLedge(checkPositionAxis, CheckPositionDirection.Front);
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

    private void GetLoadedGrounds(Scene loadedScene, LoadSceneMode loadSceneMode)
    {
        groundColliders = UtilityFunctions.FindGameObjectsByLayer(whatIsGround, FindObjectsSortMode.None).Select(groundObject => groundObject.GetComponent<Collider2D>()).ToList();
    }

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
