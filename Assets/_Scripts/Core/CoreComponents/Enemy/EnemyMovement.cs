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
    private Vector3 currentProjectedPosition;
    private Vector3 positionOffset;
    private Vector3? destinationPosition;
    private List<Vector3> stopOvers = new List<Vector3>();

    private bool rightSide;
    private bool onTraverseFlag;

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
        calculatedPath = new NavMeshPath();
        StartCoroutine(TraverseNavmeshLink());
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (!enemy.enemyStateMachine.currentState.GetType().IsSubclassOf(typeof(EnemyTargetInDetectionRangeState))) return;

        enemy.animator.SetBool("move", navMeshAgent.enabled);
        enemy.animator.SetBool("idle", !navMeshAgent.enabled);

        if (enemy.detection.currentTarget != null)
        {
            currentProjectedPosition = enemy.detection.currentProjectedPosition;
            targetProjectedPosition = enemy.detection.currentTarget.entityDetection.currentProjectedPosition;

            float targetDirection = targetProjectedPosition.x - currentProjectedPosition.x > 0 ? 1 : -1;
            enemy.movement.CheckIfShouldFlip(targetDirection);

            if (navMeshAgent.enabled)
            {
                switch (navMeshAgentState)
                {
                    case NavMeshAgentState.Chase:
                        destinationPosition = GetDestinationPosition(currentProjectedPosition, targetProjectedPosition, false);

                        if (destinationPosition.HasValue)
                        {
                            navMeshAgent.SetDestination(destinationPosition.Value);

                            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && !enemy.navMeshAgent.hasPath)
                            {
                                Debug.Log("Destination Arrived. NavMeshAgent Disabled.");
                                navMeshAgent.enabled = false;
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"Invalid Position for {enemy.name}. Disable NavMeshAgent.");
                            navMeshAgent.enabled = false;
                        }
                        break;

                    case NavMeshAgentState.TraverseAround:
                        if (onTraverseFlag)
                        {
                            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && !navMeshAgent.hasPath)
                            {
                                onTraverseFlag = false;
                            }
                        }
                        else
                        {
                            destinationPosition = GetDestinationPosition(currentProjectedPosition, targetProjectedPosition, true);

                            if (destinationPosition.HasValue)
                            {
                                navMeshAgent.SetDestination(destinationPosition.Value);

                                if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && !navMeshAgent.hasPath)
                                {
                                    ChangeNavMeshAgentState(NavMeshAgentState.Chase);
                                }
                            }
                            else
                            {
                                ChangeNavMeshAgentState(NavMeshAgentState.Chase);
                            }
                        }
                        break;
                }

                if (navMeshAgent.enabled)
                {
                    Vector3 chaseDirection = navMeshAgent.desiredVelocity == Vector3.zero ? (navMeshAgent.destination - navMeshAgent.transform.position).normalized : navMeshAgent.velocity.normalized;

                    navMeshAgent.speed = enemy.animator.GetBool("dash") ? Mathf.Abs(chaseDirection.x) * enemy.enemyData.dashSpeed.x + Mathf.Abs(chaseDirection.y) * enemy.enemyData.dashSpeed.y : Mathf.Abs(chaseDirection.x) * enemy.enemyData.moveSpeed.x + Mathf.Abs(chaseDirection.y) * enemy.enemyData.moveSpeed.y;
                }
            }
            else if (Vector3.Distance(currentProjectedPosition, targetProjectedPosition) > enemy.enemyData.maxChaseDistance)
            {
                positionOffset = Random.insideUnitCircle * enemy.enemyData.repositionOffsetDistance;
                ChangeNavMeshAgentState(NavMeshAgentState.Chase);
            }
        }
    }

    private Vector3? GetDestinationPosition(Vector3 currentProjectedPosition, Vector3 targetProjectedPosition, bool traverse, bool allowOtherDirections = false)
    {
        bool gotoRightSide = !traverse ? rightSide : !rightSide;

        Vector3 baseDestinationPosition;
        Vector3 currentDestinationPosition;

        if (gotoRightSide)
        {
            baseDestinationPosition = targetProjectedPosition + Vector3.right * enemy.enemyData.chaseDistance;
            currentDestinationPosition = baseDestinationPosition;

            while (!HasPathTo(currentDestinationPosition, positionOffset, enemy.enemyData.repositionOffsetDistance) && Vector3.Distance(targetProjectedPosition, currentDestinationPosition + positionOffset) > enemy.enemyData.minChaseDistance)
            {
                currentDestinationPosition += Vector3.left * enemy.enemyData.stepSize;
            }
        }
        else
        {
            baseDestinationPosition = targetProjectedPosition + Vector3.left * enemy.enemyData.chaseDistance;
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
            if (allowOtherDirections)
            {
                return FindPointByRemainingDistance(100);
            }
            else
            {
                return null;
            }
        }
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

    private Vector3? FindPointByRemainingDistance(float remainingDistance)
    {
        if (navMeshAgent.pathPending || navMeshAgent.path.status == NavMeshPathStatus.PathInvalid || navMeshAgent.path.corners.Length == 0)
        {
            return null;
        }

        // 1. 전체 경로 길이 계산
        float totalPathLength = 0f;
        for (int i = 0; i < navMeshAgent.path.corners.Length - 1; i++)
        {
            totalPathLength += Vector3.Distance(navMeshAgent.path.corners[i], navMeshAgent.path.corners[i + 1]);
        }

        // 2. 목표 지점까지의 거리 계산
        float distanceToTargetPoint = totalPathLength - remainingDistance;

        // 남은 거리가 전체 경로 길이보다 길거나 0보다 작으면 경로의 시작 또는 끝 지점을 반환
        if (distanceToTargetPoint <= 0)
        {
            return navMeshAgent.path.corners[0];
        }
        if (distanceToTargetPoint >= totalPathLength)
        {
            return navMeshAgent.path.corners[^1];
        }

        // 3. 경로 순회 및 지점 찾기
        float cumulativeDistance = 0f;
        for (int i = 0; i < navMeshAgent.path.corners.Length - 1; i++)
        {
            float segmentLength = Vector3.Distance(navMeshAgent.path.corners[i], navMeshAgent.path.corners[i + 1]);

            // 목표 지점이 현재 세그먼트 내에 있는지 확인
            if (cumulativeDistance + segmentLength >= distanceToTargetPoint)
            {
                // 4. 정확한 위치 보간
                float distanceIntoSegment = distanceToTargetPoint - cumulativeDistance;
                float t = distanceIntoSegment / segmentLength;
                return Vector3.Lerp(navMeshAgent.path.corners[i], navMeshAgent.path.corners[i + 1], t);
            }

            cumulativeDistance += segmentLength;
        }

        // 예외적인 경우, 경로의 마지막 지점을 반환
        return navMeshAgent.path.corners[^1];
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
        Vector3 baseDestinationPosition;
        Vector3 currentDestinationPosition;

        navMeshAgent.enabled = true;
        targetProjectedPosition = enemy.detection.currentTarget.entityDetection.currentProjectedPosition;
        currentProjectedPosition = enemy.detection.currentProjectedPosition;
        positionOffset = Random.insideUnitCircle * enemy.enemyData.repositionOffsetDistance;

        rightSide = targetProjectedPosition.x < currentProjectedPosition.x;

        if (navMeshAgentState == NavMeshAgentState.TraverseAround)
        {
            // If traverse arounding point is invalid
            if (GetDestinationPosition(currentProjectedPosition, targetProjectedPosition, true) == null)
            {
                ChangeNavMeshAgentState(NavMeshAgentState.Chase);
                return;
            }

            // Set destination to up or down of the target's position
            bool traverseDirection = UtilityFunctions.RandomSuccess(0.5f);
            baseDestinationPosition = traverseDirection ? targetProjectedPosition + Vector3.up * enemy.enemyData.traverseAroundDistance : targetProjectedPosition + Vector3.down * enemy.enemyData.traverseAroundDistance;
            baseDestinationPosition = targetProjectedPosition + Vector3.down * enemy.enemyData.traverseAroundDistance;
            currentDestinationPosition = baseDestinationPosition + positionOffset;
            NavMesh.CalculatePath(currentProjectedPosition, currentDestinationPosition, NavMesh.AllAreas, calculatedPath);

            // If endPoint's groundcollider is different or too far away from destination
            if (calculatedPath.corners.Length > 0 && (!enemy.detection.GetPositionGroundCollider(calculatedPath.corners[^1]).Equals(enemy.detection.currentTarget.entityDetection.currentGroundCollider) || Vector3.Distance(calculatedPath.corners[^1], baseDestinationPosition) > enemy.enemyData.repositionOffsetDistance))
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
            onTraverseFlag = true;
        }

        this.navMeshAgentState = navMeshAgentState;
    }

    public override void Flip()
    {
        base.Flip();

        enemy.stats.statsCanvas.transform.Rotate(new Vector3(0.0f, 180.0f, 0.0f));
    }
}
