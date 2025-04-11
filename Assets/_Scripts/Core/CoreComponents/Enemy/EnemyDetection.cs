using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyDetection : Detection
{
    [field: SerializeField] public LayerMask whatIsChaseTarget { get; private set; }
    [field: SerializeField] public Entity currentTarget { get; protected set; }
    [field: SerializeField] public Vector3? currentTargetLastPosition { get; protected set; }

    #region Detection Variables
    [SerializeField] private Collider2D detectionRangeCollider;
    [SerializeField] private Collider2D vigilanceRangeCollider;
    [SerializeField] private Collider2D aggroRangeCollider;
    #endregion

    #region Other Variables
    [SerializeField] private bool targetPlayerFromTheStart;
    #endregion

    private Enemy enemy;
    private ContactFilter2D contactFilter;
    private Collider2D[] detectionRangeColliders = new Collider2D[maxDetectionCount];
    private Collider2D[] vigilanceRangeColliders = new Collider2D[maxDetectionCount];
    private Collider2D[] aggroRangeColliders = new Collider2D[maxDetectionCount];

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

        if (targetPlayerFromTheStart)
        {
            currentTarget = Manager.Instance.gameManager.player;
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        currentTargetLastPosition = currentTarget ? currentTarget.transform.position : null;
    }

    /*public bool isTargetInAggroRange(bool exclusive)
    {

    }

    public bool isTargetInVigilanceRange(bool exclusive)
    {
        
    }*/

    public bool isTargetInDetectionRange()
    {
        Array.Clear(detectionRangeColliders, 0, maxDetectionCount);
        detectionRangeCollider.OverlapCollider(contactFilter, detectionRangeColliders);

        if (currentTarget == null)
        {
            currentTarget = detectionRangeColliders.Where(collider => collider != null).Select(collider => collider.gameObject.GetComponent<Entity>()).OrderBy(collider => Vector3.SqrMagnitude(collider.transform.position - entity.transform.position)).FirstOrDefault();

            return currentTarget != null;
        }
        else
        {
            return detectionRangeColliders.Contains(currentTarget.collider);
        }
    }
}
