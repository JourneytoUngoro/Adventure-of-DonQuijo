using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : PooledObject
{
    public Entity sourceEntity { get; protected set; }
    public Entity targetEntity { get; protected set; }

    [field: SerializeField] public CombatAbility combatAbility { get; private set; }
    [SerializeField] private LayerMask whatIsDamageable;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private bool piercingProjectile;
    [SerializeField] private float autoDestructionTime = 10.0f;

    private List<Collider2D> damagedTargets = new List<Collider2D>();
    private Rigidbody2D projectileRigidbody;
    private Collider2D projectileCollider;
    private OrthogonalRigidbody orthogonalRigidbody;

    private void Awake()
    {
        projectileRigidbody = GetComponent<Rigidbody2D>();
        orthogonalRigidbody = GetComponentInChildren<OrthogonalRigidbody>();
        projectileCollider = GetComponent<Collider2D>();

        if (combatAbility != null)
        {
            foreach (CombatAbilityComponent combatAbilityComponent in combatAbility.combatAbilityComponents)
            {
                combatAbilityComponent.pertainedCombatAbility = combatAbility;

                if (combatAbilityComponent.GetType().Equals(typeof(KnockbackComponent)))
                {
                    KnockbackComponent knockbackComponent = combatAbilityComponent as KnockbackComponent;
                    // knockbackComponent.knockbackSourceTransform = knockbackSourceTransform ? knockbackSourceTransform : transform;
                }
            }
        }
    }

    protected virtual void OnEnable()
    {
        CancelInvoke("ReleaseObject");
        Invoke("ReleaseObject", autoDestructionTime);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        bool isTouchedTarget = UtilityFunctions.IsInLayerMask(collider.gameObject.layer, whatIsDamageable);

        if (isTouchedTarget)
        {

        }
    }

    private void SetProjectile(float velocity, Vector3 direction)
    {

    }

    public void FireProjectile(Entity sourceEntity, Entity targetEntity, float speed, Vector3 direction)
    {

    }
}
