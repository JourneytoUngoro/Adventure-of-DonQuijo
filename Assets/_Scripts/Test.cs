using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private Vector2 boxSize;
    [SerializeField] private LayerMask whatIsGround;

    // Update is called once per frame
    void FixedUpdate()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, boxSize, 0.0f, whatIsGround);
        Debug.Log($"Colliders: {string.Join(", ", colliders.Select(collider => collider.name))}");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireCube(transform.position, boxSize);
    }
}
