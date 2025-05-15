using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Movement : CoreComponent
{
    public event Action synchronizeValues;
    public int facingDirection { get; private set; }
    public Vector3 currentVelocity { get; private set; }
    public bool coroutineEnabled { get; private set; }
    public bool onContact { get; private set; }

    private Coroutine velocityChangeCoroutine;

    protected virtual void Start()
    {
        facingDirection = entity.orthogonalRigidbody.transform.rotation.y == 0 ? 1 : -1;
    }

    protected virtual void FixedUpdate()
    {
        workSpace.Set(entity.entityRigidbody.velocity.x, entity.entityRigidbody.velocity.y, entity.orthogonalRigidbody.velocity);
        currentVelocity = workSpace;
    }

    public virtual void SetVelocityX(float velocity)
    {
        if (facingDirection * velocity < 0)
        {
            if (onContact && entity.entityDetection.detectingHorizontalObstacle.second)
            {
                workSpace.Set(0.0f, entity.entityRigidbody.velocity.y, 0.0f);
            }
            else
            {
                workSpace.Set(velocity, entity.entityRigidbody.velocity.y, 0.0f);
            }
        }
        else
        {
            if (onContact && entity.entityDetection.detectingHorizontalObstacle.first)
            {
                workSpace.Set(0.0f, entity.entityRigidbody.velocity.y, 0.0f);
            }
            else
            {
                workSpace.Set(velocity, entity.entityRigidbody.velocity.y, 0.0f);
            }
        }
        
        entity.entityRigidbody.velocity = workSpace;
        synchronizeValues?.Invoke();
    }

    public virtual void SetVelocityY(float velocity)
    {
        workSpace.Set(entity.entityRigidbody.velocity.x, velocity, 0.0f);
        entity.entityRigidbody.velocity = workSpace;
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
        entity.entityRigidbody.velocity = Vector2.zero;
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

    /*public void SetVelocityChangeOverTime(Vector2 velocity, float moveTime, Ease easeFunction, bool slowDown, bool stopBeforeLedge, Vector2? horizontalVerticalRatio = null)
    {
        horizontalVerticalRatio = horizontalVerticalRatio ?? Vector2.one;
        horizontalVerticalRatio = new Vector2(horizontalVerticalRatio.Value.x / Mathf.Max(horizontalVerticalRatio.Value.x, horizontalVerticalRatio.Value.y), horizontalVerticalRatio.Value.y / Mathf.Max(horizontalVerticalRatio.Value.x, horizontalVerticalRatio.Value.y));

        StopVelocityChangeOverTime();
        velocityChangeCoroutine = StartCoroutine(VelocityChangeOverTime(velocity, moveTime, easeFunction, slowDown, stopBeforeLedge, horizontalVerticalRatio.Value));
    }*/

    public void SetVelocityChangeOverTime(Vector2 direction, float speed, float moveTime, Ease easeFunction, bool reverseTime, bool stopBeforeLedge, Vector2? horizontalVerticalRatio = null)
    {
        horizontalVerticalRatio = horizontalVerticalRatio ?? Vector2.one;
        horizontalVerticalRatio = new Vector2(horizontalVerticalRatio.Value.x / Mathf.Max(horizontalVerticalRatio.Value.x, horizontalVerticalRatio.Value.y), horizontalVerticalRatio.Value.y / Mathf.Max(horizontalVerticalRatio.Value.x, horizontalVerticalRatio.Value.y));

        StopVelocityChangeOverTime();
        velocityChangeCoroutine = StartCoroutine(VelocityChangeOverTime(speed * direction.normalized, moveTime, easeFunction, reverseTime, stopBeforeLedge, horizontalVerticalRatio.Value));
    }

    public void SetVelocityChangeOverTime(Vector2 direction, float speed, float moveTime, AnimationCurve animationCurve, bool reverseTime, bool stopBeforeLedge, Vector2? horizontalVerticalRatio = null)
    {
        horizontalVerticalRatio = horizontalVerticalRatio ?? Vector2.one;
        horizontalVerticalRatio = new Vector2(horizontalVerticalRatio.Value.x / Mathf.Max(horizontalVerticalRatio.Value.x, horizontalVerticalRatio.Value.y), horizontalVerticalRatio.Value.y / Mathf.Max(horizontalVerticalRatio.Value.x, horizontalVerticalRatio.Value.y));

        StopVelocityChangeOverTime();
        velocityChangeCoroutine = StartCoroutine(VelocityChangeOverTime(speed * direction.normalized, moveTime, animationCurve, reverseTime, stopBeforeLedge, horizontalVerticalRatio.Value));
    }

    private IEnumerator VelocityChangeOverTime(Vector2 velocity, float moveTime, Ease easeFunction, bool reverseTime, bool stopBeforeLedge, Vector2 horizontalVerticalRatio)
    {
        if (moveTime <= 0.0f)
        {
            coroutineEnabled = false;
            velocityChangeCoroutine = null;
            yield break;
        }

        // Debug.Log($"Start Coroutine | velocity: {velocity}, moveTime: {moveTime}, easeFunction: {easeFunction}");
        coroutineEnabled = true;
        float coroutineElapsedTime = 0.0f;
        WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

        while (true)
        {
            float velocityMultiplierOverTime = reverseTime ? Mathf.Clamp(DOVirtual.EasedValue(1.0f, 0.0f, coroutineElapsedTime / moveTime, easeFunction), 0.0f, 1.0f) : Mathf.Clamp(DOVirtual.EasedValue(0.0f, 1.0f, coroutineElapsedTime / moveTime, easeFunction), 0.0f, 1.0f);

            if (entity.entityDetection.isGrounded && stopBeforeLedge)
            {
                if (entity.entityDetection.IsDetectingLedge(CheckPositionAxis.Horizontal, CheckPositionDirection.Heading))
                {
                    SetVelocity(0.0f, velocityMultiplierOverTime * velocity.y * horizontalVerticalRatio.y);
                }
                
                if (entity.entityDetection.IsDetectingLedge(CheckPositionAxis.Vertical, CheckPositionDirection.Heading))
                {
                    SetVelocity(velocityMultiplierOverTime * velocity.x * horizontalVerticalRatio.x, 0.0f);
                }
            }
            else
            {
                // Debug.Log(velocityMultiplierOverTime * velocity.x * horizontalVerticalRatio.x + ", " + velocityMultiplierOverTime * velocity.y * horizontalVerticalRatio.y);
                SetVelocity(velocityMultiplierOverTime * velocity.x * horizontalVerticalRatio.x, velocityMultiplierOverTime * velocity.y * horizontalVerticalRatio.y);
            }

            if (coroutineElapsedTime > moveTime)
            {
                coroutineEnabled = false;
                velocityChangeCoroutine = null;
                yield break;
            }
            else
            {
                yield return waitForFixedUpdate;
            }

            coroutineElapsedTime += Time.fixedDeltaTime;
        }
    }

    private IEnumerator VelocityChangeOverTime(Vector2 velocity, float moveTime, AnimationCurve animationCurve, bool slowDown, bool stopBeforeLedge, Vector2 horizontalVerticalRatio)
    {
        if (moveTime <= 0.0f)
        {
            coroutineEnabled = false;
            velocityChangeCoroutine = null;
            yield break;
        }

        coroutineEnabled = true;
        float coroutineElapsedTime = 0.0f;
        WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

        while (true)
        {
            float velocityMultiplierOverTime = slowDown ? Mathf.Clamp(DOVirtual.EasedValue(1.0f, 0.0f, coroutineElapsedTime / moveTime, animationCurve), 0.0f, 1.0f) : Mathf.Clamp(DOVirtual.EasedValue(0.0f, 1.0f, coroutineElapsedTime / moveTime, animationCurve), 0.0f, 1.0f);

            if (entity.entityDetection.isGrounded && stopBeforeLedge)
            {
                if (entity.entityDetection.IsDetectingLedge(CheckPositionAxis.Horizontal, CheckPositionDirection.Heading))
                {
                    SetVelocity(0.0f, velocityMultiplierOverTime * velocity.y * horizontalVerticalRatio.y);
                }

                if (entity.entityDetection.IsDetectingLedge(CheckPositionAxis.Vertical, CheckPositionDirection.Heading))
                {
                    SetVelocity(velocityMultiplierOverTime * velocity.x * horizontalVerticalRatio.x, 0.0f);
                }
            }
            else
            {
                // Debug.Log(velocityMultiplierOverTime * velocity.x * horizontalVerticalRatio.x + ", " + velocityMultiplierOverTime * velocity.y * horizontalVerticalRatio.y);
                SetVelocity(velocityMultiplierOverTime * velocity.x * horizontalVerticalRatio.x, velocityMultiplierOverTime * velocity.y * horizontalVerticalRatio.y);
            }

            if (coroutineElapsedTime > moveTime)
            {
                coroutineEnabled = false;
                velocityChangeCoroutine = null;
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
            velocityChangeCoroutine = null;
        }

        if (entity.entityCombat.dashAttackCoroutine != null)
        {
            StopCoroutine(entity.entityCombat.dashAttackCoroutine);
        }
    }

    public void OnContact(bool onContact) => this.onContact = onContact;
}
