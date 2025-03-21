using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

// TODO: Conditions
public enum CurrentStatus { GotHit, HealthDamage, PostureDamage, Knockback, Parried, Shielded, wasParried }

public class Entity : MonoBehaviour
{
    #region Base Components
    public Rigidbody2D rigidBody { get; protected set; }
    public EntityStateMachine entityStateMachine { get; protected set; }
    public Animator animator { get; protected set; }
    public SpriteRenderer spriteRenderer { get; protected set; }
    public BoxCollider2D collider { get; protected set; }
    public Core core { get; protected set; }
    public StateMachineToAnimator stateMachineToAnimator { get; protected set; }
    #endregion

    #region Entity Components
    [field: SerializeField] public Collider2D shadow { get; private set; }
    /* 'Shadow' is a gameobject with a collider to land on. If there is no collider, because of the time interval between each fixed update frame, the gameobject doesn't will stop slightly down from the correct position.
     * TODO: The current problem is that the shadow gameobject will slide down on landing with unknown reason even its position is continuously set in FixedUpdate. Also, setting shadow as child object will cause shadow gameobject's position change in y direction for unknown reason. Could there be a way to set the shadow gameobject as child but don't make it slide down?
     * Reason: Currently, we are setting the position of the shadow gameobject in FixedUpdate, but the problem is that the shadow gameobject's position is slightly late, which leads to unnatural game scene. */
    public Movement entityMovement { get; protected set; }
    public Detection entityDetection { get; protected set; }
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
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponentInChildren<BoxCollider2D>();
        stateMachineToAnimator = GetComponent<StateMachineToAnimator>();

        core = GetComponentInChildren<Core>();
        status = new bool[Enum.GetValues(typeof(CurrentStatus)).Length];
    }

    protected virtual void Start()
    {
        // verticalMovementRigidbody.transform.localPosition = new Vector3(0.0f, verticalMovementRigidbody.transform.localPosition.y);

        entityDetection = core.GetCoreComponent<Detection>();
        entityMovement = core.GetCoreComponent<Movement>();
        // entityCombat = core.GetCoreComponent<Combat>();
        // entityStats = core.GetCoreComponent<Stats>();
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
