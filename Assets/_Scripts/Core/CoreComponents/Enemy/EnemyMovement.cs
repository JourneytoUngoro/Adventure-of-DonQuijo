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
    private NavMeshPath tempPath;
    private Vector3 recaliberationPosition;
    private Vector3 currentDestination;
    private Vector3 baseDestinationPosition;
    private Vector2 positionOffset;

    private Coroutine waypointTraverseCoroutine;

    protected override void Awake()
    {
        base.Awake();

        enemy = entity as Enemy;
    }

    protected override void Start()
    {
        base.Start();

        navMeshAgent = enemy.navMeshAgent;
        enemy.navMeshAgent.autoTraverseOffMeshLink = false;
        StartCoroutine(TraverseNavmeshLink());
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (enemy.navMeshAgent.enabled)
        {
            Vector3 chaseDirection = enemy.navMeshAgent.desiredVelocity == Vector3.zero ? (enemy.navMeshAgent.destination - enemy.navMeshAgent.transform.position).normalized : enemy.navMeshAgent.velocity.normalized;

            if (enemy.animator.GetBool("dash"))
            {
                enemy.navMeshAgent.speed = Mathf.Abs(chaseDirection.x) * enemy.enemyData.dashSpeed.x + Mathf.Abs(chaseDirection.y) * enemy.enemyData.dashSpeed.y;
            }
            else
            {
                enemy.navMeshAgent.speed = Mathf.Abs(chaseDirection.x) * enemy.enemyData.moveSpeed.x + Mathf.Abs(chaseDirection.y) * enemy.enemyData.moveSpeed.y;
            }
        }


        switch (navMeshAgentState)
        {
            case NavMeshAgentState.Chase:
                if (enemy.detection.currentTarget.entityDetection.currentProjectedPosition.x > enemy.detection.currentProjectedPosition.x)
                {
                    currentDestination = enemy.detection.currentTarget.entityDetection.currentProjectedPosition + Vector3.right * enemy.enemyData.surroundingDistance + (Vector3)positionOffset;

                    while (!enemy.detection.GetPositionGroundCollider(currentDestination).Equals(enemy.detection.currentGroundCollider) && enemy.movement.HasPathTo(currentDestination))
                    {
                        currentDestination += Vector3.left * enemy.enemyData.stepSize;
                    }
                }
                else
                {
                    currentDestination = enemy.detection.currentTarget.entityDetection.currentProjectedPosition + Vector3.left * enemy.enemyData.surroundingDistance;

                    while (!enemy.detection.GetPositionGroundCollider(currentDestination).Equals(enemy.detection.currentGroundCollider) && enemy.movement.HasPathTo(currentDestination))
                    {
                        currentDestination += Vector3.right * enemy.enemyData.stepSize;
                    }
                }

                if (enemy.navMeshAgent.enabled)
                {
                    enemy.navMeshAgent.SetDestination(currentDestination);

                    if (!enemy.navMeshAgent.pathPending && enemy.navMeshAgent.remainingDistance <= enemy.navMeshAgent.stoppingDistance && !enemy.navMeshAgent.hasPath)
                    {
                        enemy.navMeshAgent.enabled = false;
                    }
                }
                else
                {
                    if (Vector3.Distance(enemy.detection.currentProjectedPosition, enemy.detection.currentTarget.entityDetection.currentProjectedPosition) > enemy.enemyData.maxDistance)
                    {
                        positionOffset = Random.insideUnitCircle * enemy.enemyData.repositionOffsetDistance;
                        enemy.navMeshAgent.enabled = true;
                    }
                }
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
    }

    public void ChangeNavMeshAgentState(NavMeshAgentState agentState)
    {
        switch (agentState)
        {
            case NavMeshAgentState.Chase:
                break;
            case NavMeshAgentState.TraverseAround:
                break;
        }
    }

    public void TraverseThroughWayPoints(NavMeshAgent agent, List<Vector3> waypoints)
    {
        if (agent == null || waypoints == null || waypoints.Count == 0)
            return;

        if (waypointTraverseCoroutine != null)
        {
            StopCoroutine(waypointTraverseCoroutine);
        }
        waypointTraverseCoroutine = StartCoroutine(TraverseThroughWayPointsCoroutine(agent, waypoints));
    }

    private IEnumerator TraverseThroughWayPointsCoroutine(NavMeshAgent agent, List<Vector3> waypoints)
    {
        foreach (Vector3 waypoint in waypoints)
        {
            agent.SetDestination(waypoint);

            while (agent.pathPending)
            {
                yield return null;
            }

            while (agent.remainingDistance > agent.stoppingDistance)
            {
                yield return null;
            }
        }
    }

    public bool HasPathTo(Vector3 destination)
    {
        bool pathFound = NavMesh.CalculatePath(enemy.detection.currentProjectedPosition, destination, NavMesh.AllAreas, tempPath);
        return pathFound && tempPath.status == NavMeshPathStatus.PathComplete;
    }

    public bool HasPathTo(List<Vector3> waypoints)
    {
        for(int index = 0; index < waypoints.Count; index++)
        {
            Vector3 startPoint = waypoints[index];
            Vector3 endPoint = waypoints[index + 1];

            if (!NavMesh.CalculatePath(startPoint, endPoint, NavMesh.AllAreas, tempPath))
            {
                return false;
            }
        }

        return true;
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

    public void ChangeNavMeshState(NavMeshAgentState navMeshAgentState)
    {
        // traverseAroundFlag = false;
        positionOffset = Random.insideUnitCircle * enemy.enemyData.repositionOffsetDistance;

        enemy.animator.SetBool("move", navMeshAgentState != NavMeshAgentState.Halt);
        enemy.animator.SetBool("idle", navMeshAgentState == NavMeshAgentState.Halt);

        if (navMeshAgentState == NavMeshAgentState.TraverseAround)
        {
            baseDestinationPosition = enemy.detection.currentTarget.entityDetection.currentProjectedPosition;

            if (UtilityFunctions.RandomSuccess(0.5f))
            {
                if (enemy.detection.GetPositionGroundCollider(baseDestinationPosition + Vector3.up * enemy.enemyData.repositionOffsetDistance + (Vector3)positionOffset).Equals(enemy.detection.currentTarget.entityDetection.currentGroundCollider))
                {
                    currentDestination = enemy.detection.currentTarget.transform.position + Vector3.up * enemy.enemyData.repositionOffsetDistance + (Vector3)positionOffset;
                }
                else if (enemy.detection.GetPositionGroundCollider(baseDestinationPosition + Vector3.down * enemy.enemyData.repositionOffsetDistance + (Vector3)positionOffset).Equals(enemy.detection.currentTarget.entityDetection.currentGroundCollider))
                {
                    currentDestination = enemy.detection.currentTarget.transform.position + Vector3.down * enemy.enemyData.repositionOffsetDistance + (Vector3)positionOffset;
                }
                else
                {
                    navMeshAgentState = NavMeshAgentState.InDistance;
                }
            }
            else
            {
                if (enemy.detection.GetPositionGroundCollider(baseDestinationPosition + Vector3.down * enemy.enemyData.repositionOffsetDistance + (Vector3)positionOffset).Equals(enemy.detection.currentTarget.entityDetection.currentGroundCollider))
                {
                    currentDestination = enemy.detection.currentTarget.transform.position + Vector3.down * enemy.enemyData.repositionOffsetDistance + (Vector3)positionOffset;
                }
                else if (enemy.detection.GetPositionGroundCollider(baseDestinationPosition + Vector3.up * enemy.enemyData.repositionOffsetDistance + (Vector3)positionOffset).Equals(enemy.detection.currentTarget.entityDetection.currentGroundCollider))
                {
                    currentDestination = enemy.detection.currentTarget.transform.position + Vector3.up * enemy.enemyData.repositionOffsetDistance + (Vector3)positionOffset;
                }
                else
                {
                    navMeshAgentState = NavMeshAgentState.InDistance;
                }
            }

            navMeshAgent.SetDestination(currentDestination);
        }

        this.navMeshAgentState = navMeshAgentState;
    }

    public override void Flip()
    {
        base.Flip();

        enemy.stats.statsCanvas.transform.Rotate(new Vector3(0.0f, 180.0f, 0.0f));
    }
}
