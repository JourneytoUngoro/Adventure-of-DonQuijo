using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyDetection : Detection
{
    [field: SerializeField] public LayerMask whatIsChaseTarget { get; private set; }

    #region Detection Variables
    [SerializeField] private Collider2D detectionRangeCollider;
    [SerializeField] private Collider2D chaseRangeCollider;
    #endregion

    #region Other Variables
    [SerializeField] private bool constantlyFollowPlayer;
    #endregion

    private Enemy enemy;
    private ContactFilter2D contactFilter;
    private Collider2D[] detectionRangeColliders = new Collider2D[maxDetectionCount];
    private Collider2D[] chaseRangeColliders = new Collider2D[maxDetectionCount];
    private Collider2D[] designatedPositionColliders = new Collider2D[maxDetectionCount];

    protected override void Awake()
    {
        base.Awake();

        enemy = entity as Enemy;
        contactFilter.SetLayerMask(whatIsChaseTarget);
        contactFilter.useLayerMask = true;
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

    public Collider2D GetPositionGroundCollider(Vector2 groundCheckPosition)
    {
        Array.Clear(designatedPositionColliders, 0, maxDetectionCount);
        Physics2D.OverlapBoxNonAlloc(groundCheckPosition, entity.entityCollider.size, 0.0f, designatedPositionColliders, whatIsGround);
        return projectedPositionColliders.Where(groundCollider => groundCollider != null && groundCollider.transform.position.z + groundCollider.GetComponent<HeightData>().height <= currentEntityHeight).OrderByDescending(groundCollider => groundCollider.transform.position.z + groundCollider.GetComponent<HeightData>().height).FirstOrDefault();
    }

    /*public bool isTargetInAggroRange(bool exclusive)
    {

    }

    public bool isTargetInVigilanceRange(bool exclusive)
    {
        
    }*/

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
                currentTarget = detectionRangeColliders.Where(collider => collider != null).Select(collider => collider.gameObject.GetComponent<Entity>()).OrderBy(collider => Vector3.SqrMagnitude(collider.transform.position - entity.transform.position)).FirstOrDefault();
                currentTarget?.entityCombat.targetedBy.Add(enemy);

                return currentTarget != null;
            }
            else
            {
                if (detectionRangeColliders.Contains(currentTarget.entityCollider))
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
}
