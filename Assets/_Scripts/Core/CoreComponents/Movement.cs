using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Movement : CoreComponent
{
    [field: SerializeField] public float horizontalSpeed { get; private set; }
    [field: SerializeField] public float verticalSpeed { get; private set; }
    
    public event Action synchronizeValues;

    public int facingDirection { get; private set; }

    protected virtual void Start()
    {
        facingDirection = entity.transform.rotation.y == 0 ? 1 : -1;
    }

    public void SetVelocityX(float velocity)
    {
        if (facingDirection * velocity < 0)
        {
            if (entity.entityDetection.horizontalObstacleHeight.y > entity.entityDetection.currentEntityHeight)
            {
                v2WorkSpace.Set(0.0f, entity.rigidBody.velocity.y);
            }
            else
            {
                v2WorkSpace.Set(velocity, entity.rigidBody.velocity.y);
            }
        }
        else
        {
            if (entity.entityDetection.horizontalObstacleHeight.x > entity.entityDetection.currentEntityHeight)
            {
                v2WorkSpace.Set(0.0f, entity.rigidBody.velocity.y);
            }
            else
            {
                v2WorkSpace.Set(velocity, entity.rigidBody.velocity.y);
            }
        }

        entity.rigidBody.velocity = v2WorkSpace;
        synchronizeValues?.Invoke();
    }

    public void SetVelocityY(float velocity)
    {
        if (velocity < 0)
        {
            if (entity.entityDetection.verticalObstacleHeight.y > entity.entityDetection.currentEntityHeight)
            {
                v2WorkSpace.Set(entity.rigidBody.velocity.x, 0.0f);
            }
            else
            {
                v2WorkSpace.Set(entity.rigidBody.velocity.x, velocity);
            }
        }
        else
        {
            if (entity.entityDetection.verticalObstacleHeight.x > entity.entityDetection.currentEntityHeight)
            {
                v2WorkSpace.Set(entity.rigidBody.velocity.x, 0.0f);
            }
            else
            {
                v2WorkSpace.Set(entity.rigidBody.velocity.x, velocity);
            }
        }
        
        entity.rigidBody.velocity = v2WorkSpace;
        synchronizeValues?.Invoke();
    }

    public void SetVelocityZ(float velocity)
    {
        v2WorkSpace.Set(entity.rigidBody.velocity.x, velocity);
        entity.rigidBody.velocity = v2WorkSpace;
        synchronizeValues?.Invoke();
    }

    public void SetVelocityZero()
    {
        entity.rigidBody.velocity = Vector2.zero;
        synchronizeValues?.Invoke();
    }

    public void SetVelocity(float velocityX, float velocityY)
    {
        SetVelocityX(velocityX);
        SetVelocityY(velocityY);
    }

    public void SetVelocity(Vector2 velocity)
    {
        SetVelocityX(velocity.x);
        SetVelocityY(velocity.y);
    }

    public void CheckIfShouldFlip(float velocityX)
    {
        if (Mathf.Abs(velocityX) > epsilon && facingDirection * velocityX < 0)
        {
            Flip();
        }
    }

    public void Flip()
    {
        facingDirection *= -1;
        entity.transform.Rotate(new Vector3(0.0f, 180.0f, 0.0f));
        entity.shadow.transform.Rotate(new Vector3(0.0f, 180.0f, 0.0f));
        synchronizeValues?.Invoke();
    }

    /*public void SetVelocityChangeOverTime(float speed, Vector2 direction, float moveTime, Ease easeFunction, bool slowDown, bool considerGroundCondition, bool isDetectingLedge)
    {

    }

    public void SetVelocityXChangeOverTime(float velocity, float moveTime, Ease easeFunction, bool slowDown, bool isDetectingLedge = false, bool considerGroundCondition = true)
    {
        if (moveTime == 0.0f)
        {
            SetVelocityX(velocity, considerGroundCondition);
        }
        else
        {
            if (velocityChangeOverTimeCoroutine != null)
            {
                StopCoroutine(velocityChangeOverTimeCoroutine);
            }
            velocityChangeOverTimeCoroutine = StartCoroutine(VelocityChangeOverTime(velocity, moveTime, easeFunction, slowDown, isDetectingLedge));
        }
    }

    private IEnumerator VelocityChangeOverTime(float velocity, float moveTime, Ease easeFunction, bool slowDown, bool isDetectingLedge)
    {
        coroutineEnabled = true;
        float coroutineElapsedTime = 0.0f;
        WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

        while (true)
        {
            float velocityMultiplierOverTime = slowDown ? Mathf.Clamp(DOVirtual.EasedValue(1.0f, 0.0f, coroutineElapsedTime / moveTime, easeFunction), 0.0f, 1.0f) : Mathf.Clamp(DOVirtual.EasedValue(0.0f, 1.0f, coroutineElapsedTime / moveTime, easeFunction), 0.0f, 1.0f);

            if (isGrounded && isDetectingLedge)
            {
                SetVelocityX(0.0f);
            }
            else
            {
                SetVelocityX(velocityMultiplierOverTime * velocity, true);
            }

            if (coroutineElapsedTime > moveTime)
            {
                coroutineEnabled = false;
                yield break;
            }
            else
            {
                yield return waitForFixedUpdate;
            }

            coroutineElapsedTime += Time.deltaTime;
        }
    }

    public void StopVelocityXChangeOverTime()
    {
        if (velocityChangeOverTimeCoroutine != null)
        {
            coroutineEnabled = false;
            StopCoroutine(velocityChangeOverTimeCoroutine);
        }
    }

    public void SetVelocityY(float velocity)
    {
        // workSpace.Set(rigidBody.velocity.x, velocity);
        SetWorkSpace(rigidBody.velocity.x, velocity);
        rigidBody.velocity = workSpace;
        synchronizeValues?.Invoke();
        // player.playerStateMachine.currentState.SetMovementVariables();
    }

    public void SetVelocityLimitY(float velocity)
    {
        if (rigidBody.velocity.y > velocity)
        {
            SetWorkSpace(rigidBody.velocity.x, velocity);
            rigidBody.velocity = workSpace;
            synchronizeValues?.Invoke();
        }
    }

    public void SetVelocity(Vector2 velocity)
    {
        SetWorkSpace(velocity.x, velocity.y);
        rigidBody.velocity = workSpace;
        synchronizeValues?.Invoke();
    }

    public void AddVelocityX(float velocity)
    {
        rigidBody.velocity = new Vector2(rigidBody.velocity.x + velocity, rigidBody.velocity.y);
        synchronizeValues?.Invoke();
    }

    public void MultiplyVelocity(float multiplier)
    {
        rigidBody.velocity = multiplier * rigidBody.velocity;
        synchronizeValues?.Invoke();
    }

    public void SetVelocityWithDirection(Vector2 angleVector, int direction, float speed)
    {
        workSpace.Set(angleVector.x * direction, angleVector.y);
        rigidBody.velocity = workSpace.normalized * speed;
        synchronizeValues?.Invoke();
    }

    public void SetVelocityZero(bool considerGroundCondition = true)
    {
        if (considerGroundCondition)
        {
            if (isGrounded)
            {
                SetVelocityX(0.0f);

                if (isOnSlope)
                {
                    SetVelocityY(0.0f);
                }
            }
        }
        else
        {
            workSpace = Vector2.zero;
            rigidBody.velocity = workSpace;
            synchronizeValues?.Invoke();
        }
    }

    public void SetPositionX(float xPos)
    {
        workSpace.Set(xPos, transform.position.y);
        entity.transform.position = workSpace;
    }

    public void SetPositionY(float yPos)
    {
        workSpace.Set(transform.position.x, yPos);
        entity.transform.position = workSpace;
    }

    public void SetPosition(Vector2 position)
    {
        entity.transform.position = position;
    }

    public void MovePosition(Vector2 angleVector, float direction, float distance)
    {
        workSpace.Set(angleVector.x * direction, angleVector.y);
        transform.position += (Vector3)workSpace.normalized * distance;
    }

    public void ChangeBaseVelocity(Vector2 baseVelocity)
    {
        this.baseVelocity += baseVelocity;
    }

    public void SetBaseVelocity(Vector2 baseVelocity)
    {
        this.baseVelocity = baseVelocity;
    }

    public void MultiplyVelocityMultiplier(Vector2 velocityMultiplier)
    {
        workSpace.Set(this.velocityMultiplier.x * velocityMultiplier.x, this.velocityMultiplier.y * velocityMultiplier.y);
        this.velocityMultiplier = workSpace;
    }

    public void SetVelocityMultiplier(Vector2 velocityMultiplier)
    {
        this.velocityMultiplier = velocityMultiplier;
    }

    public void RigidBodyController(bool isMovingForward = true, bool limitYVelocity = true)
    {
        if (isGrounded)
        {
            if (isOnSlope)
            {
                rigidBody.gravityScale = rigidBody.velocity.magnitude < epsilon ? 0.0f : 9.5f;

                if (isMovingForward)
                {
                    if (entity.entityDetection.slopePerpNormal.y * facingDirection > 0)
                    {
                        SetVelocityMultiplier(Vector2.one * 0.8f);
                    }
                    else
                    {
                        SetVelocityMultiplier(Vector2.one * 1.4f);
                    }
                }
                else
                {
                    if (entity.entityDetection.slopePerpNormal.y * -facingDirection > 0)
                    {
                        SetVelocityMultiplier(Vector2.one * 0.8f);
                    }
                    else
                    {
                        SetVelocityMultiplier(Vector2.one * 1.4f);
                    }
                }
            }
            else
            {
                SetVelocityMultiplier(Vector2.one);
                entity.rigidBody.gravityScale = 9.5f;

                if (limitYVelocity)
                {
                    SetVelocityLimitY(0.0f);
                }
            }
        }
        else
        {
            if (entity.entityDetection.isOnSlopeVertical(CheckPositionHorizontal.Back))
            {
                if (limitYVelocity)
                {
                    SetVelocityLimitY(0.0f); // 튀어오르는 현상 방지
                }
            }

            SetVelocityMultiplier(Vector2.one);
            entity.rigidBody.gravityScale = 9.5f;
        }
    }

    private void SetWorkSpace(float x, float y)
    {
        workSpace.Set(x, y);
        workSpace += baseVelocity;
        workSpace *= velocityMultiplier;
    }*/
}
