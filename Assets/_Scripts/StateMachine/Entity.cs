using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

// TODO: Conditions
public enum CurrentStatus { gotHit, HealthDamage, PostureDamage, Knockback, Parried, Blocked, wasParried }

public class Entity : MonoBehaviour
{
    #region Base Components
    public Animator animator { get; protected set; }
    public SpriteRenderer spriteRenderer { get; protected set; }
    public BoxCollider2D entityCollider { get; protected set; }
    public Rigidbody2D entityRigidbody { get; protected set; }
    public OrthogonalRigidbody orthogonalRigidbody { get; protected set; }
    public EntityStateMachine entityStateMachine { get; protected set; }
    public Core core { get; protected set; }
    public StateMachineToAnimator stateMachineToAnimator { get; protected set; }
    #endregion

    #region Entity Components
    public Movement entityMovement { get; protected set; }
    public Detection entityDetection { get; protected set; }
    public Combat entityCombat { get; protected set; }
    public Stats entityStats { get; protected set; }
    [field: SerializeField] public Transform shadow { get; private set; }
    [field: SerializeField] public EntityData entityData { get; private set; }
    #endregion

    #region Other Variables
    [field: SerializeField] public bool printStateChange { get; private set; }
    [field: SerializeField] public int entityLevel { get; protected set; }
    public bool isDead { get; protected set; }
    public bool[] status { get; protected set; }
    public float currentEntityStature { get; protected set; }

    protected Vector3 workSpace;
    #endregion

    protected virtual void Awake()
    {
        entityRigidbody = GetComponent<Rigidbody2D>();
        entityCollider = GetComponent<BoxCollider2D>();
        animator = GetComponentInChildren<Animator>();
        stateMachineToAnimator = GetComponentInChildren<StateMachineToAnimator>();
        orthogonalRigidbody = GetComponentInChildren<OrthogonalRigidbody>();
        spriteRenderer = GetComponentsInChildren<SpriteRenderer>().Where(component => component.gameObject != gameObject).FirstOrDefault();

        core = GetComponentInChildren<Core>();
        status = new bool[Enum.GetValues(typeof(CurrentStatus)).Length];
    }

    protected virtual void Start()
    {
        entityDetection = core.GetCoreComponent<Detection>();
        entityMovement = core.GetCoreComponent<Movement>();
        entityCombat = core.GetCoreComponent<Combat>();
        entityStats = core.GetCoreComponent<Stats>();
    }

    protected virtual void Update()
    {
        entityStateMachine.entityCurrentState.LogicUpdate();
    }

    protected virtual void FixedUpdate()
    {
        // prevPosition = currentPosition;
        // currentPosition = planeMovementRigidbody.position;
        // verticalMovementRigidbody.position += currentPosition - prevPosition;
        entityStateMachine.entityCurrentState.PhysicsUpdate();
    }

    protected virtual void LateUpdate()
    {
        entityStateMachine.entityCurrentState.LateLogicUpdate();
        Array.Fill(status, false);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("collision.gameObject.name: " + collision.gameObject.name);
        Debug.Log("collision.gameObject.layer: " + collision.gameObject.layer);
        if (collision.gameObject.IsInLayerMask(entityDetection.whatIsGround))
        {
            entityMovement.OnContact(true);
        }
    }

    protected virtual void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.IsInLayerMask(entityDetection.whatIsGround))
        {
            entityMovement.OnContact(false);
        }
    }

    /*public void UseAfterImage(Color color)
    {
        GameObject afterImage = Manager.Instance.objectPoolingManager.GetGameObject("AfterImage");
        afterImage.GetComponent<AfterImage>().spriteRenderer.sprite = spriteRenderer.sprite;
        afterImage.GetComponent<AfterImage>().spriteRenderer.color = color;
        afterImage.transform.position = transform.position;
        afterImage.transform.rotation = transform.rotation;
    }*/

    public void SetStatusValues(CurrentStatus currentStatus) => status[(int)currentStatus] = true;

    public void SetCurrentEntityStature(float currentStature) => currentEntityStature = currentStature;
}
