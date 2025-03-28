using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

// TODO: Conditions
public enum CurrentStatus { GotHit, HealthDamage, PostureDamage, Knockback, Parried, Shielded, wasParried }

public class Entity : MonoBehaviour
{
    #region Base Components
    public Animator animator { get; protected set; }
    public SpriteRenderer spriteRenderer { get; protected set; }
    new public BoxCollider2D collider { get; protected set; }
    new public Rigidbody2D rigidbody { get; protected set; }
    public OrthogonalRigidbody orthogonalRigidbody { get; protected set; }
    public EntityStateMachine entityStateMachine { get; protected set; }
    public Core core { get; protected set; }
    public StateMachineToAnimator stateMachineToAnimator { get; protected set; }
    #endregion

    #region Entity Components
    public Movement entityMovement { get; protected set; }
    public Detection entityDetection { get; protected set; }
    public Stats entityStats { get; protected set; }
    [field: SerializeField] public Transform shadow { get; private set; }
    [field: SerializeField] public EntityData entityData { get; private set; }
    #endregion

    #region Other Variables
    [field: SerializeField] public bool printStateChange { get; private set; }
    public int entityLevel { get; protected set; }
    public bool isDead { get; protected set; }
    public bool[] status { get; protected set; }

    protected Vector3 workSpace;
    #endregion

    protected virtual void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
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
        // entityCombat = core.GetCoreComponent<Combat>();
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

    /*public void UseAfterImage(Color color)
    {
        GameObject afterImage = Manager.Instance.objectPoolingManager.GetGameObject("AfterImage");
        afterImage.GetComponent<AfterImage>().spriteRenderer.sprite = spriteRenderer.sprite;
        afterImage.GetComponent<AfterImage>().spriteRenderer.color = color;
        afterImage.transform.position = transform.position;
        afterImage.transform.rotation = transform.rotation;
    }*/

    public void SetStatusValues(CurrentStatus currentStatus) => status[(int)currentStatus] = true;
}
