using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Movement : CoreComponent
{
    public event Action synchronizeValues;
    public int facingDirection { get; private set; }

    protected bool isGrounded { get; private set; }

    private Coroutine velocityChangeCoroutine;

    protected virtual void Start()
    {
        facingDirection = entity.orthogonalRigidbody.transform.rotation.y == 0 ? 1 : -1;
    }

    protected virtual void FixedUpdate()
    {
        isGrounded = entity.entityDetection.isGrounded();
    }

    public void SetVelocityX(float velocity)
    {
        if (facingDirection * velocity < 0)
        {
            if (entity.entityDetection.horizontalGroundHeight.y > entity.entityDetection.currentEntityHeight)
            {
                workSpace.Set(0.0f, entity.rigidbody.velocity.y, 0.0f);
            }
            else
            {
                workSpace.Set(velocity, entity.rigidbody.velocity.y, 0.0f);
            }
        }
        else
        {
            if (entity.entityDetection.horizontalGroundHeight.x > entity.entityDetection.currentEntityHeight)
            {
                workSpace.Set(0.0f, entity.rigidbody.velocity.y, 0.0f);
            }
            else
            {
                workSpace.Set(velocity, entity.rigidbody.velocity.y, 0.0f);
            }
        }
        // workSpace.Set(velocity, entity.rigidbody.velocity.y, 0.0f);
        entity.rigidbody.velocity = workSpace;
        synchronizeValues?.Invoke();
    }

    public void SetVelocityY(float velocity)
    {
        /*if (velocity < 0)
        {
            if (entity.entityDetection.verticalGroundHeight.y > entity.entityDetection.currentEntityHeight)
            {
                workSpace.Set(entity.rigidbody.velocity.x, 0.0f, 0.0f);
            }
            else
            {
                workSpace.Set(entity.rigidbody.velocity.x, velocity, 0.0f);
            }
        }
        else
        {
            if (entity.entityDetection.verticalGroundHeight.x > entity.entityDetection.currentEntityHeight)
            {
                workSpace.Set(entity.rigidbody.velocity.x, 0.0f, 0.0f);
            }
            else
            {
                workSpace.Set(entity.rigidbody.velocity.x, velocity, 0.0f);
            }
        }*/
        workSpace.Set(entity.rigidbody.velocity.x, velocity, 0.0f);
        entity.rigidbody.velocity = workSpace;
        synchronizeValues?.Invoke();
    }

    public void SetVelocityZ(float velocity)
    {
        // v2WorkSpace.Set(entity.rigidBody.velocity.x, velocity);
        // entity.rigidBody.velocity = v2WorkSpace;
        entity.orthogonalRigidbody.velocity = velocity;
        synchronizeValues?.Invoke();
    }

    public void SetVelocityZero()
    {
        entity.rigidbody.velocity = Vector2.zero;
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

    public virtual void Flip()
    {
        facingDirection *= -1;
        entity.orthogonalRigidbody.transform.Rotate(new Vector3(0.0f, 180.0f, 0.0f));
        entity.shadow.Rotate(new Vector3(0.0f, 180.0f, 0.0f));
        synchronizeValues?.Invoke();
    }

    public void SetVelocityXChangeOverTime(float velocity, float moveTime, Ease easeFunction, bool slowDown, bool stopBeforeLedge)
    {
        StopVelocityChangeOverTime();
        velocityChangeCoroutine = StartCoroutine(VelocityChangeOverTime(Vector2.right * velocity, moveTime, easeFunction, slowDown, stopBeforeLedge, Vector2.one));
    }

    public void SetVelocityYChangeOverTime(float velocity, float moveTime, Ease easeFunction, bool slowDown, bool stopBeforeLedge)
    {
        StopVelocityChangeOverTime();
        velocityChangeCoroutine = StartCoroutine(VelocityChangeOverTime(Vector2.up * velocity, moveTime, easeFunction, slowDown, stopBeforeLedge, Vector2.one));
    }

    public void SetVelocityChangeOverTime(Vector2 velocity, float moveTime, Ease easeFunction, bool slowDown, bool stopBeforeLedge, Vector2? horizontalVerticalRatio = null)
    {
        horizontalVerticalRatio = horizontalVerticalRatio ?? Vector2.one;
        horizontalVerticalRatio = new Vector2(horizontalVerticalRatio.Value.x / Mathf.Max(horizontalVerticalRatio.Value.x, horizontalVerticalRatio.Value.y), horizontalVerticalRatio.Value.y / Mathf.Max(horizontalVerticalRatio.Value.x, horizontalVerticalRatio.Value.y));

        StopVelocityChangeOverTime();
        velocityChangeCoroutine = StartCoroutine(VelocityChangeOverTime(velocity, moveTime, easeFunction, slowDown, stopBeforeLedge, horizontalVerticalRatio.Value));
    }

    public void SetVelocityChangeOverTime(float speed, Vector2 direction, float moveTime, Ease easeFunction, bool slowDown, bool stopBeforeLedge, Vector2? horizontalVerticalRatio = null)
    {
        horizontalVerticalRatio = horizontalVerticalRatio ?? Vector2.one;
        horizontalVerticalRatio = new Vector2(horizontalVerticalRatio.Value.x / Mathf.Max(horizontalVerticalRatio.Value.x, horizontalVerticalRatio.Value.y), horizontalVerticalRatio.Value.y / Mathf.Max(horizontalVerticalRatio.Value.x, horizontalVerticalRatio.Value.y));

        StopVelocityChangeOverTime();
        velocityChangeCoroutine = StartCoroutine(VelocityChangeOverTime(speed * direction.normalized, moveTime, easeFunction, slowDown, stopBeforeLedge, horizontalVerticalRatio.Value));
    }

    private IEnumerator VelocityChangeOverTime(Vector2 velocity, float moveTime, Ease easeFunction, bool slowDown, bool stopBeforeLedge, Vector2 horizontalVerticalRatio)
    {
        float coroutineElapsedTime = 0.0f;
        WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

        while (true)
        {
            float velocityMultiplierOverTime = slowDown ? Mathf.Clamp(DOVirtual.EasedValue(1.0f, 0.0f, coroutineElapsedTime / moveTime, easeFunction), 0.0f, 1.0f) : Mathf.Clamp(DOVirtual.EasedValue(0.0f, 1.0f, coroutineElapsedTime / moveTime, easeFunction), 0.0f, 1.0f);

            if (isGrounded && stopBeforeLedge)
            {
                if (entity.entityDetection.isDetectingLedge(CheckPositionAxis.Horizontal, CheckPositionDirection.Heading))
                {
                    SetVelocity(0.0f, velocityMultiplierOverTime * velocity.y * horizontalVerticalRatio.y);
                }
                
                if (entity.entityDetection.isDetectingLedge(CheckPositionAxis.Vertical, CheckPositionDirection.Heading))
                {
                    SetVelocity(velocityMultiplierOverTime * velocity.x * horizontalVerticalRatio.x, 0.0f);
                }
            }
            else
            {
                Debug.Log(velocityMultiplierOverTime * velocity.x * horizontalVerticalRatio.x + ", " + velocityMultiplierOverTime * velocity.y * horizontalVerticalRatio.y);
                SetVelocity(velocityMultiplierOverTime * velocity.x * horizontalVerticalRatio.x, velocityMultiplierOverTime * velocity.y * horizontalVerticalRatio.y);
            }

            if (coroutineElapsedTime > moveTime)
            {
                yield break;
            }
            else
            {
                yield return waitForFixedUpdate;
            }

            coroutineElapsedTime += Time.fixedDeltaTime;
        }
    }

    public void StopVelocityChangeOverTime()
    {
        if (velocityChangeCoroutine != null)
        {
            StopCoroutine(velocityChangeCoroutine);
        }
    }
}
