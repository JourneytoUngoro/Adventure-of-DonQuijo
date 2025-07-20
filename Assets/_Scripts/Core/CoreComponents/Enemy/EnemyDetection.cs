using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyDetection : Detection
{
    [field: SerializeField] public LayerMask whatIsChaseTarget { get; private set; }

    #region Detection Variables
    [SerializeField] private Vector2 detectionRangeEllipseRadius;
    [SerializeField] private PolygonCollider2D detectionRangeCollider;
    [SerializeField] private Transform detectionAngleCheckTransform;
    [SerializeField, Range(0.0f, 180.0f)] private float detectionClockwiseAngle;
    [SerializeField, Range(0.0f, 180.0f)] private float detectionCounterclockwiseAngle;
    [SerializeField] private Vector2 chaseRangeEllipseRadius;
    [SerializeField] private PolygonCollider2D chaseRangeCollider;
    #endregion

    #region Other Variables
    [SerializeField] private bool constantlyFollowPlayer;
    #endregion

    private Enemy enemy;
    private ContactFilter2D contactFilter;
    private Collider2D[] detectionRangeColliders = new Collider2D[maxDetectionCount];
    private Collider2D[] chaseRangeColliders = new Collider2D[maxDetectionCount];
    private Collider2D[] designatedPositionColliders = new Collider2D[maxDetectionCount];

    private const int ellipseSegments = 64;

    protected override void Awake()
    {
        base.Awake();

        enemy = entity as Enemy;
        contactFilter.SetLayerMask(whatIsChaseTarget);
        contactFilter.useLayerMask = true;

        List<Vector2> path = new List<Vector2>();
        float angleStep = 360.0f / ellipseSegments;

        for (int index = 0; index <= ellipseSegments; index++)
        {
            float angle = index * angleStep;
            workSpace.Set(
                detectionRangeEllipseRadius.x * Mathf.Cos(Mathf.Deg2Rad * angle),
                detectionRangeEllipseRadius.y * Mathf.Sin(Mathf.Deg2Rad * angle),
                0.0f);
            path.Add(workSpace);
        }
        detectionRangeCollider.SetPath(0, path);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (constantlyFollowPlayer)
        {
            currentTarget = Manager.Instance.gameManager.player;
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        
    }

    /// <summary>
    /// Returns highest ground collider of the given position regardless of z position.
    /// </summary>
    /// <param name="groundCheckPosition"></param>
    /// <returns></returns>
    public Collider2D GetPositionGroundCollider(Vector2 groundCheckPosition)
    {
        Array.Clear(designatedPositionColliders, 0, maxDetectionCount);
        Physics2D.OverlapBoxNonAlloc(groundCheckPosition, entity.entityCollider.size, 0.0f, designatedPositionColliders, whatIsGround);
        return projectedPositionColliders.Where(groundCollider => groundCollider != null).OrderByDescending(groundCollider => groundCollider.transform.position.z + groundCollider.GetComponent<HeightData>().height).FirstOrDefault();
    }

    public bool isTargetInDetectionRange()
    {
        if (constantlyFollowPlayer)
        {
            return true;
        }
        else
        {
            Array.Clear(detectionRangeColliders, 0, maxDetectionCount);
            detectionRangeCollider.OverlapCollider(contactFilter, detectionRangeColliders);

            if (currentTarget == null)
            {
                currentTarget = detectionRangeColliders
                    .Where(collider => collider != null &&
                    enemy.combat.CheckWithinAngle(detectionAngleCheckTransform.right, collider.gameObject.GetComponent<Entity>().entityDetection.currentProjectedPosition - detectionAngleCheckTransform.position, detectionClockwiseAngle, detectionCounterclockwiseAngle))
                    .Select(collider => collider.gameObject.GetComponent<Entity>())
                    .OrderBy(collider => Vector3.SqrMagnitude(collider.transform.position - entity.transform.position)).FirstOrDefault();
                currentTarget?.entityCombat.targetedBy.Add(enemy);

                return currentTarget != null && !currentTarget.isDead;
            }
            else
            {
                if (detectionRangeColliders.Contains(currentTarget.entityCollider) && enemy.combat.CheckWithinAngle(detectionAngleCheckTransform.right, currentTarget.entityDetection.currentProjectedPosition - detectionAngleCheckTransform.position, detectionClockwiseAngle, detectionCounterclockwiseAngle))
                {
                    return !currentTarget.isDead;
                }
                else
                {
                    currentTarget = null;
                    return false;
                }
            }
        }
    }

    public bool isTargetInDetectionRange(bool exclusive = false)
    {
        Array.Clear(chaseRangeColliders, 0, maxDetectionCount);
        chaseRangeCollider.OverlapCollider(contactFilter, chaseRangeColliders);

        if (currentTarget == null)
        {
            currentTarget = chaseRangeColliders.Where(collider => collider != null).Select(collider => collider.gameObject.GetComponent<Entity>()).OrderBy(collider => Vector3.SqrMagnitude(collider.transform.position - entity.transform.position)).FirstOrDefault();
            currentTarget?.entityCombat.targetedBy.Add(enemy);

            return currentTarget != null;
        }
        else
        {
            if (chaseRangeColliders.Contains(currentTarget.entityCollider))
            {
                return true;
            }
            else
            {
                currentTarget = null;
                return false;
            }
        }
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.green;

        float angleStep = 360.0f / ellipseSegments;
        Vector3 lastPoint = transform.position + Vector3.right * detectionRangeEllipseRadius.x;

        for (int index = 1; index <= ellipseSegments; index++)
        {
            float angle = index * angleStep;
            Vector3 nextPoint = transform.position + new Vector3(
                detectionRangeEllipseRadius.x * Mathf.Cos(Mathf.Deg2Rad * angle),
                detectionRangeEllipseRadius.y * Mathf.Sin(Mathf.Deg2Rad * angle),
                0.0f);
            Gizmos.DrawLine(lastPoint, nextPoint);
            lastPoint = nextPoint;
        }

        Vector2 clockwiseDirection = RotateVector(detectionAngleCheckTransform.right, detectionClockwiseAngle);
        Vector2 counterClockwiseDirection = RotateVector(detectionAngleCheckTransform.right, -detectionCounterclockwiseAngle);
        Gizmos.DrawRay(detectionAngleCheckTransform.position, clockwiseDirection * 200.0f);
        Gizmos.DrawRay(detectionAngleCheckTransform.position, counterClockwiseDirection * 200.0f);

        /*float angleRad = detectionClockwiseAngle * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        float A = (direction.x * direction.x) / (detectionRangeEllipseRadius.x * detectionRangeEllipseRadius.x) + (direction.y * direction.y) / (detectionRangeEllipseRadius.y * detectionRangeEllipseRadius.y);
        float B = 2 * detectionAngleCheckTransform.position.x * direction.x / (detectionRangeEllipseRadius.x * detectionRangeEllipseRadius.x) + detectionAngleCheckTransform.position.y * direction.y / (detectionRangeEllipseRadius.y * detectionRangeEllipseRadius.y);
        float C = (detectionAngleCheckTransform.position.x * detectionAngleCheckTransform.position.x) / (detectionRangeEllipseRadius.x * detectionRangeEllipseRadius.x) + (detectionAngleCheckTransform.position.y * detectionAngleCheckTransform.position.y) / (detectionRangeEllipseRadius.y * detectionRangeEllipseRadius.y) - 1;

        float discriminant = B * B - 4 * A * C;

        if (discriminant > 0.0f)
        {
            float sqrtDisc = Mathf.Sqrt(discriminant);
            float t1 = (-B - sqrtDisc) / (2 * A);
            float t2 = (-B + sqrtDisc) / (2 * A);

            float t = (t1 >= 0) ? t1 : (t2 >= 0 ? t2 : -1);

            if (t > 0.0f)
            {
                Gizmos.DrawLine(detectionAngleCheckTransform.position, detectionAngleCheckTransform.position + (Vector3)direction * t);
            }
        }*/
    }

    private Vector2 RotateVector(Vector2 vector, float angleDegrees)
    {
        float angleRad = angleDegrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(angleRad);
        float sin = Mathf.Sin(angleRad);
        return new Vector2(vector.x * cos - vector.y * sin, vector.x * sin + vector.y * cos);
    }
}
