using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProbeLine : Probe
{
    public Vector2 direction;
    public float length;

    public override bool IsContacted
    {
        get
        {
            RaycastHit2D hit;
            hit = Physics2D.Raycast(transform.position, direction, length, layerMask);
            return hit;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + ((Vector3)direction).normalized * length);
    }
}
