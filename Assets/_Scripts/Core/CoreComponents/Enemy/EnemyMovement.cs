using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : Movement
{
    protected Enemy enemy;
    private NavMeshAgent navMeshAgent;
    private Vector3 recaliberationPosition;

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
            enemy.movement.CheckIfShouldFlip(enemy.detection.currentTarget.transform.position.x - navMeshAgent.transform.position.x);
            navMeshAgent.SetDestination(enemy.detection.currentTarget.transform.position);
            recaliberationPosition = enemy.transform.position;
            Vector3 direction = navMeshAgent.desiredVelocity == Vector3.zero ? (enemy.detection.currentTarget.transform.position - navMeshAgent.transform.position).normalized : navMeshAgent.velocity.normalized;
            enemy.navMeshAgent.speed = Mathf.Abs(direction.x) * enemy.enemyData.moveSpeed.x + Mathf.Abs(direction.y) * enemy.enemyData.moveSpeed.y;
        }
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
}
