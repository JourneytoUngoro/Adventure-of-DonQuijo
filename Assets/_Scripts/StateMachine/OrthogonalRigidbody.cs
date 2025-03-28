using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrthogonalRigidbody : MonoBehaviour
{
    [HideInInspector] public float velocity;
    public float gravityScale;

    private Entity entity;
    private Vector3 workSpace;

    private void Awake()
    {
        entity = GetComponentInParent<Entity>();
    }

    private void FixedUpdate()
    {
        float currentGroundHeight = entity.entityDetection.currentGroundHeight;
        float currentEntityHeight = entity.entityDetection.currentEntityHeight;

        if (velocity > 0.0f || currentEntityHeight > currentGroundHeight)
        {
            float delta = velocity * Time.fixedDeltaTime + 0.5f * Physics2D.gravity.y * gravityScale * Mathf.Pow(Time.fixedDeltaTime, 2.0f);

            delta = delta < 0 ? Mathf.Max(currentGroundHeight - currentEntityHeight, delta) : delta;

            workSpace.Set(0.0f, delta, delta);
            transform.localPosition += workSpace;

            velocity += Physics2D.gravity.y * gravityScale * Time.fixedDeltaTime;
        }
        else
        {
            velocity = 0.0f;
        }
    }
}
