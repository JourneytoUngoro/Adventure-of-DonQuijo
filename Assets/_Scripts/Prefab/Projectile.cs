using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Projectile : PooledObject
{
    [field: SerializeField] public CombatAbilityWithColliders combatAbilityWithColliders { get; private set; }
    [SerializeField] private LayerMask whatIsDamageable;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private bool piercingProjectile;
    [SerializeField] private float autoDestructionTime = 10.0f;
    [SerializeField] private Transform knockbackSourceTransform;

    public Entity sourceEntity { get; protected set; }
    public Entity targetEntity { get; protected set; }
    public Vector3 currentScreenPosition { get; protected set; } // orthogonal rigidbody's screen position: (x, y + z, z)
    public Vector3 currentSpacePosition { get; protected set; } // orthogonal rigidbody's position in space: (x, y, z)
    public Vector3 currentProjectedPosition { get; protected set; } // orthogonal rigidbody's position when it is projected on plane: (x, y, projected ground height)
    public float currentProjectileHeight { get; protected set; } // orthogonal rigidbody's local position = (0, z, z)
    public float currentGroundHeight { get; protected set; } // projected ground height
    public bool isGrounded { get; protected set; }
    public pair<bool, bool> detectingHorizontalObstacle { get; protected set; } = new pair<bool, bool>(false, false);
    public pair<bool, bool> detectingVerticalObstacle { get; protected set; } = new pair<bool, bool>(false, false);
    public Collider2D currentGroundCollider { get; protected set; }

    private List<Collider2D> damagedTargets = new List<Collider2D>();
    private Rigidbody2D projectileRigidbody;
    private BoxCollider2D projectileCollider;
    private OrthogonalRigidbody orthogonalRigidbody;

    protected Collider2D[] projectedPositionColliders = new Collider2D[maxDetectionCount];

    private Vector3 workSpace;

    protected const int maxDetectionCount = 10;

    private void Awake()
    {
        projectileCollider = GetComponent<BoxCollider2D>();
        projectileRigidbody = GetComponent<Rigidbody2D>();
        orthogonalRigidbody = GetComponentInChildren<OrthogonalRigidbody>();

        if (combatAbilityWithColliders.combatAbilityData != null)
        {
            foreach (CombatAbilityComponent combatAbilityComponent in combatAbilityWithColliders.combatAbilityData.combatAbilityComponents)
            {
                combatAbilityComponent.pertainedCombatAbility = combatAbilityWithColliders.combatAbilityData;

                if (combatAbilityComponent.GetType().Equals(typeof(KnockbackComponent)))
                {
                    KnockbackComponent knockbackComponent = combatAbilityComponent as KnockbackComponent;
                    knockbackComponent.knockbackSourceTransform = knockbackSourceTransform ? knockbackSourceTransform : transform;
                }
            }
        }
    }

    protected virtual void OnEnable()
    {
        CancelInvoke("ReleaseObject");
        Invoke("ReleaseObject", autoDestructionTime);
    }

    protected virtual void OnDisable()
    {
        damagedTargets.Clear();
    }

    protected virtual void FixedUpdate()
    {
        currentProjectedPosition = orthogonalRigidbody.transform.position;
        currentProjectileHeight = orthogonalRigidbody.transform.localPosition.z;

        workSpace.Set(transform.position.x, transform.position.y, currentProjectileHeight);
        currentSpacePosition = workSpace;

        Array.Clear(projectedPositionColliders, 0, maxDetectionCount);
        Physics2D.OverlapBoxNonAlloc(currentProjectedPosition, projectileCollider.size, 0.0f, projectedPositionColliders, whatIsGround);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        bool isTouchedTarget = UtilityFunctions.IsInLayerMask(collider.gameObject.layer, whatIsDamageable);
        bool isTouchedGround = UtilityFunctions.IsInLayerMask(collider.gameObject.layer, whatIsGround);

        float projectileBottomHeight = currentProjectileHeight;
        float projectileTopHeight = projectileBottomHeight + combatAbilityWithColliders.overlapColliders[0].height;

        if (isTouchedTarget)
        {
            Entity targetEntity = collider.GetComponent<Entity>();
            float targetEntityFeetHeight = targetEntity.entityDetection.currentEntityHeight;
            float targetEntityHeadHeight = targetEntityFeetHeight + targetEntity.currentEntityStature;
            
            if (!(targetEntityHeadHeight <= projectileBottomHeight || projectileTopHeight <= targetEntityFeetHeight))
            {
                OnCollision(collider);

                if (!piercingProjectile)
                {
                    ReleaseObject();
                }
            }
        }

        if (isTouchedGround)
        {
            float groundBottomHeight = collider.transform.position.z;
            float groundTopHeight = collider.GetComponent<HeightData>().height + groundBottomHeight;
            
            if (!(groundTopHeight <= projectileBottomHeight || projectileTopHeight <= groundBottomHeight))
            {
                OnCollision(collider);

                if (combatAbilityWithColliders.combatAbilityData == null)
                {
                    Explode();
                }
                else
                {
                    ReleaseObject();
                }
            }
        }
    }

    protected virtual void OnCollision(Collider2D collider)
    {
        if (combatAbilityWithColliders.combatAbilityData == null)
        {
            Explode();
        }
        else
        {
            if (damagedTargets.Contains(collider)) return;
            if (collider.CompareTag("Invinsible")) return;
            if (combatAbilityWithColliders.combatAbilityData.canBeDodged && collider.CompareTag("Dodge")) return;

            foreach (CombatAbilityComponent combatAbilityComponent in combatAbilityWithColliders.combatAbilityData.combatAbilityComponents)
            {
                switch (combatAbilityComponent)
                {
                    case DamageComponent damageComponent:
                        damageComponent.ApplyCombatAbility(collider, combatAbilityWithColliders.overlapColliders); break;
                    case KnockbackComponent knockbackComponent:
                        knockbackComponent.ApplyCombatAbility(collider, combatAbilityWithColliders.overlapColliders); break;
                    default:
                        break;
                }
            }

            damagedTargets.Add(collider);
        }
    }

    private void Explode()
    {
        GameObject projectileExplosion = Manager.Instance.objectPoolingManager.GetGameObject(gameObject.name.Replace("(Clone)", "") + "Explosion");
        Explosion explosion = projectileExplosion.GetComponent<Explosion>();
        projectileExplosion.transform.position = transform.position;
    }

    public void FireProjectile(Entity sourceEntity, Entity targetEntity, float planeSpeed, Vector2 planeDirection, float orthogonalSpeed)
    {
        this.sourceEntity = sourceEntity;
        this.targetEntity = targetEntity;
        projectileRigidbody.velocity = planeSpeed * planeDirection;
        orthogonalRigidbody.velocity = orthogonalSpeed;
    }

    public override void ReleaseObject()
    {
        base.ReleaseObject();

        sourceEntity = null;
        damagedTargets.Clear();
        CancelInvoke("ReleaseObject");
    }
}
