using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum NavMeshAgentState { Halt, Chase, InDistance, TraverseAround }

public class EnemyMovement : Movement
{
    [SerializeField] private float stepSize = 5.0f;

    public NavMeshAgentState navMeshAgentState { get; private set; }

    protected Enemy enemy;

    private NavMeshAgent navMeshAgent;
    private NavMeshPath calculatedPath;
    private Vector3 recaliberationPosition;

    private Vector3 targetProjectedPosition;
    private Vector3 baseDestinationPosition; // 
    private Vector3 currentDestinationPosition; // currentDestination = baseDestination + positionOffset
    private Vector3 positionOffset;
    private Vector3 traverseDirection;

    protected override void Awake()
    {
        base.Awake();

        enemy = entity as Enemy;
    }

    protected override void Start()
    {
        base.Start();

        navMeshAgent = enemy.navMeshAgent;
        navMeshAgent.autoTraverseOffMeshLink = false;
        StartCoroutine(TraverseNavmeshLink());
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (navMeshAgent.enabled)
        {
            float targetDirection = enemy.detection.currentTarget.entityDetection.currentProjectedPosition.x - enemy.detection.currentProjectedPosition.x > 0 ? 1 : -1;
            enemy.movement.CheckIfShouldFlip(targetDirection);

            switch (navMeshAgentState)
            {
                case NavMeshAgentState.Chase:
                    Vector3? destinationPosition = GetDestinationPosition(enemy.detection.currentProjectedPosition, enemy.detection.currentTarget.entityDetection.currentProjectedPosition, false);

                    if (destinationPosition.HasValue)
                    {
                        navMeshAgent.SetDestination(destinationPosition.Value);

                        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && !enemy.navMeshAgent.hasPath)
                        {
                            navMeshAgent.enabled = false;
                        }
                    }
                    else
                    {
                        navMeshAgent.enabled = false;
                    }
                    break;

                case NavMeshAgentState.TraverseAround:

                    break;
                    /*case NavMeshAgentState.TraverseAround:
                       if (!traverseAroundFlag && !enemy.navMeshAgent.pathPending && enemy.navMeshAgent.remainingDistance <= enemy.navMeshAgent.stoppingDistance && !enemy.navMeshAgent.hasPath)
                       {
                           currentDestination = traverseDirection == 1 ? baseDestinationPosition + Vector3.right * enemy.enemyData.surroundingDistance + (Vector3)positionOffset : baseDestinationPosition - Vector3.right * enemy.enemyData.surroundingDistance;
                           enemy.navMeshAgent.SetDestination(currentDestination);
                           traverseAroundFlag = true;
                       }
                       else if (traverseAroundFlag && !enemy.navMeshAgent.pathPending && enemy.navMeshAgent.remainingDistance <= enemy.navMeshAgent.stoppingDistance && !enemy.navMeshAgent.hasPath)
                       {
                           navMeshAgentState = NavMeshAgentState.Chase;
                       }
                       break;
                       currentDestination = enemy.detection.currentTarget.entityDetection.currentProjectedPosition - enemy.orthogonalRigidbody.transform.right * enemy.enemyData.surroundingDistance + (Vector3)positionOffset;

                       while (!enemy.detection.GetPositionGroundCollider(currentDestination).Equals(enemy.detection.currentGroundCollider) && enemy.movement.HasPathTo(currentDestination))
                       {
                           currentDestination += enemy.orthogonalRigidbody.transform.right * enemy.enemyData.stepSize;
                       }*/
            }

            Vector3 chaseDirection = navMeshAgent.desiredVelocity == Vector3.zero ? (navMeshAgent.destination - navMeshAgent.transform.position).normalized : navMeshAgent.velocity.normalized;

            navMeshAgent.speed = enemy.animator.GetBool("dash") ? Mathf.Abs(chaseDirection.x) * enemy.enemyData.dashSpeed.x + Mathf.Abs(chaseDirection.y) * enemy.enemyData.dashSpeed.y : Mathf.Abs(chaseDirection.x) * enemy.enemyData.moveSpeed.x + Mathf.Abs(chaseDirection.y) * enemy.enemyData.moveSpeed.y;
        }
        else if (Vector3.Distance(enemy.detection.currentProjectedPosition, enemy.detection.currentTarget.entityDetection.currentProjectedPosition) > enemy.enemyData.maxChaseDistance)
        {
            positionOffset = Random.insideUnitCircle * enemy.enemyData.repositionOffsetDistance;
            enemy.navMeshAgent.enabled = true;
        }
    }

    private Vector3? GetDestinationPosition(Vector3 currentProjectedPosition, Vector3 targetProjectedPosition, bool traverse)
    {
        Vector3 baseDestinationPosition;
        Vector3 currentDestinationPosition;

        bool direction = !traverse ? targetProjectedPosition.x > currentProjectedPosition.x : targetProjectedPosition.x < currentProjectedPosition.x;

        if (direction)
        {
            baseDestinationPosition = currentProjectedPosition + Vector3.right * enemy.enemyData.chaseDistance;
            currentDestinationPosition = baseDestinationPosition;

            while (!HasPathTo(currentDestinationPosition, positionOffset, enemy.enemyData.repositionOffsetDistance) && Vector3.Distance(targetProjectedPosition, currentDestinationPosition + positionOffset) > enemy.enemyData.minChaseDistance)
            {
                currentDestinationPosition += Vector3.left * enemy.enemyData.stepSize;
            }
        }
        else
        {
            baseDestinationPosition = currentProjectedPosition + Vector3.left * enemy.enemyData.chaseDistance;
            currentDestinationPosition = baseDestinationPosition;

            while (!HasPathTo(currentDestinationPosition, positionOffset, enemy.enemyData.repositionOffsetDistance) && Vector3.Distance(targetProjectedPosition, currentDestinationPosition + positionOffset) > enemy.enemyData.minChaseDistance)
            {
                currentDestinationPosition += Vector3.right * enemy.enemyData.stepSize;
            }
        }

        if (HasPathTo(currentDestinationPosition, positionOffset, enemy.enemyData.repositionOffsetDistance) && Vector3.Distance(targetProjectedPosition, currentDestinationPosition + positionOffset) > enemy.enemyData.minChaseDistance)
        {
            return currentDestinationPosition;
        }
        else
        {
            return null;
        }
    }

    public bool HasPathTo(Vector3 destination)
    {
        bool pathFound = NavMesh.CalculatePath(enemy.detection.currentProjectedPosition, destination, NavMesh.AllAreas, calculatedPath);
        return pathFound && calculatedPath.status == NavMeshPathStatus.PathComplete;
    }

    // TODO: Z position of calculatedPath.corners is always 0 regardless of ground height.
    public bool HasPathTo(Vector3 destination, Vector3 offset, float distance)
    {
        bool pathFound = NavMesh.CalculatePath(enemy.detection.currentProjectedPosition, destination + offset, NavMesh.AllAreas, calculatedPath);

        if (!pathFound || calculatedPath.corners.Length == 0)
        {
            return false;
        }

        Collider2D groundCollider = enemy.detection.GetPositionGroundCollider(calculatedPath.corners[^1]);
        workSpace.Set(calculatedPath.corners[^1].x, calculatedPath.corners[^1].y, groundCollider.transform.position.z + groundCollider.GetComponent<HeightData>().height);

        return Vector3.Distance(calculatedPath.corners[^1], destination) < distance && enemy.detection.GetPositionGroundCollider(calculatedPath.corners[^1]).Equals(enemy.detection.currentGroundCollider);
    }

    private IEnumerator TraverseNavmeshLink()
    {
        while (true)
        {
            if (navMeshAgent.isOnOffMeshLink)
            {
                yield return StartCoroutine(NormalSpeed());

                navMeshAgent.CompleteOffMeshLink();
            }

            yield return null;
        }
    }

    private IEnumerator NormalSpeed()
    {
        WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

        OffMeshLinkData offMeshLinkData = navMeshAgent.currentOffMeshLinkData;
        Vector3 startPos = offMeshLinkData.startPos;
        Vector3 endPos = offMeshLinkData.endPos;

        navMeshAgent.transform.position = recaliberationPosition;

        while (Vector3.SqrMagnitude(navMeshAgent.transform.position - startPos) > epsilon)
        {
            if (!navMeshAgent.enabled)
            {
                yield break;
            }

            navMeshAgent.transform.position = Vector3.MoveTowards(navMeshAgent.transform.position, startPos, navMeshAgent.speed * Time.deltaTime);
            yield return waitForFixedUpdate;
        }

        if (enemy.detection.detectingHorizontalObstacle.first || enemy.detection.detectingVerticalObstacle.first)
        {
            SetVelocityZ(enemy.enemyData.jumpSpeed);
        }

        while (Vector3.SqrMagnitude(navMeshAgent.transform.position - endPos) > epsilon)
        {
            if (!navMeshAgent.enabled)
            {
                yield break;
            }

            navMeshAgent.transform.position = Vector3.MoveTowards(navMeshAgent.transform.position, endPos, navMeshAgent.speed * Time.deltaTime);
            yield return waitForFixedUpdate;
        }
    }

    public void ChangeNavMeshAgentState(NavMeshAgentState navMeshAgentState)
    {
        // traverseAroundFlag = false;
        positionOffset = Random.insideUnitCircle * enemy.enemyData.repositionOffsetDistance;

        enemy.animator.SetBool("move", navMeshAgentState != NavMeshAgentState.Halt);
        enemy.animator.SetBool("idle", navMeshAgentState == NavMeshAgentState.Halt);

        if (navMeshAgentState == NavMeshAgentState.TraverseAround)
        {
            // If traverse arounding point is invalid
            if (GetDestinationPosition(enemy.detection.currentProjectedPosition, enemy.detection.currentTarget.entityDetection.currentProjectedPosition, true) == null)
            {
                ChangeNavMeshAgentState(NavMeshAgentState.Chase);
                return;
            }

            // Set destination to up or down of the target's position
            bool traverseDirection = UtilityFunctions.RandomSuccess(0.5f);
            baseDestinationPosition = traverseDirection ? targetProjectedPosition + Vector3.up * enemy.enemyData.traverseAroundDistance : targetProjectedPosition + Vector3.down * enemy.enemyData.traverseAroundDistance;
            currentDestinationPosition = baseDestinationPosition + positionOffset;
            NavMesh.CalculatePath(enemy.detection.currentProjectedPosition, currentDestinationPosition, NavMesh.AllAreas, calculatedPath);

            // If endPoint's groundcollider is different or too far away from destination
            if (!enemy.detection.GetPositionGroundCollider(calculatedPath.corners[^1]).Equals(enemy.detection.currentTarget.entityDetection.currentGroundCollider) || Vector3.Distance(calculatedPath.corners[^1], baseDestinationPosition) > enemy.enemyData.repositionOffsetDistance)
            {
                // Change Path
                baseDestinationPosition = traverseDirection ? targetProjectedPosition + Vector3.down * enemy.enemyData.traverseAroundDistance : targetProjectedPosition + Vector3.up * enemy.enemyData.traverseAroundDistance;
                currentDestinationPosition = baseDestinationPosition + positionOffset;
                NavMesh.CalculatePath(enemy.detection.currentProjectedPosition, currentDestinationPosition, NavMesh.AllAreas, calculatedPath);

                if (!enemy.detection.GetPositionGroundCollider(calculatedPath.corners[^1]).Equals(enemy.detection.currentTarget.entityDetection.currentGroundCollider) || Vector3.Distance(calculatedPath.corners[^1], baseDestinationPosition) > enemy.enemyData.repositionOffsetDistance)
                {
                    ChangeNavMeshAgentState(NavMeshAgentState.Chase);
                    return;
                }
            }

            navMeshAgent.SetDestination(currentDestinationPosition);
        }

        this.navMeshAgentState = navMeshAgentState;
    }

    public override void Flip()
    {
        base.Flip();

        enemy.stats.statsCanvas.transform.Rotate(new Vector3(0.0f, 180.0f, 0.0f));
    }
}
