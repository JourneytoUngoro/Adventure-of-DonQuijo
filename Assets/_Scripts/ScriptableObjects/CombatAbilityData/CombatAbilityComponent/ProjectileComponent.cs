using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ProjectileDirection { Manual, Targeted };

public class ProjectileComponent : CombatAbilityComponent
{
    [field: SerializeField] public GameObject projectilePrefab { get; private set; }
    [field: SerializeField] public LayerMask whatIsDamageable { get; private set; }
    [field: SerializeField] public float projectilePlaneSpeed { get; private set; }
    [field: SerializeField] public float projectileOrthogonalSpeed { get; private set; }
    [field: SerializeField] public ProjectileDirection projectileType { get; private set; }

    public override void ApplyCombatAbility(Collider2D target, OverlapCollider[] overlapColliders)
    {
        /*Transform projectileFireBaseTransform = overlapColliders.Select(overlapCollider => overlapCollider.angleCheckBaseTransform).FirstOrDefault();
        Transform[] projectileFireTransforms = projectileFireBaseTransform.GetComponentsInChildren<Transform>().Skip(1).ToArray();

        if (projectileFireTransforms.Count() == 0)
        {
            FireProjectile(projectileFireBaseTransform);
        }
        else
        {
            foreach (Transform projectileFireTransform in projectileFireTransforms)
            {
                // float deg2Rad = Mathf.Deg2Rad * projectileFireTransform.eulerAngles.z;
                // Vector2 direction = new Vector2(Mathf.Cos(deg2Rad), Mathf.Sin(deg2Rad));
                FireProjectile(projectileFireTransform);
            }
        }*/
        // GameObject projectileGameObject = Manager.Instance.objectPoolingManager.GetGameObject(projectilePrefab.name);
        GameObject projectileGameObject = Object.Instantiate(projectilePrefab);
        projectileGameObject.transform.position = target.transform.position;
        Debug.Log("TargetPosition: " + target.transform.position);
        Debug.Log("ProjectilePosition: " + projectileGameObject.transform.position);
    }

    private void FireProjectile(Transform projectileFireTransform)
    {
        GameObject projectileGameObject = Manager.Instance.objectPoolingManager.GetGameObject(projectilePrefab.name);
        Projectile projectile = projectileGameObject.GetComponent<Projectile>();
        Vector2 direction = pertainedCombatAbility.sourceEntity.orthogonalRigidbody.transform.right;
        
        switch (projectileType)
        {
            case ProjectileDirection.Manual:
                direction = projectileFireTransform.right;
                projectile.FireProjectile(pertainedCombatAbility.sourceEntity, null, projectilePlaneSpeed, direction, projectileOrthogonalSpeed);
                break;
            case ProjectileDirection.Targeted:
                if (pertainedCombatAbility.sourceEntity.entityDetection.currentTarget != null)
                {
                    direction = (pertainedCombatAbility.sourceEntity.entityDetection.currentTarget.entityDetection.currentSpacePosition - projectileFireTransform.position).normalized;
                    projectile.transform.position = projectileFireTransform.position;
                    projectile.FireProjectile(pertainedCombatAbility.sourceEntity, pertainedCombatAbility.sourceEntity.entityDetection.currentTarget, projectilePlaneSpeed, direction, projectileOrthogonalSpeed);
                }
                else
                {
                    direction = (pertainedCombatAbility.sourceEntity.entityDetection.currentTargetLastPosition - projectileFireTransform.position).normalized;
                    projectile.FireProjectile(pertainedCombatAbility.sourceEntity, null, projectilePlaneSpeed, direction, projectileOrthogonalSpeed);
                }
                break;
            default:
                Debug.LogWarning($"Unknown projectile type of {projectileType} from {pertainedCombatAbility.sourceEntity.name}.");
                break;
        }
    }
}
