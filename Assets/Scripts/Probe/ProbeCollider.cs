using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProbeCollider : Probe
{
    [SerializeField] Collider2D collider;
    List<Collider2D> contactColliders;

    public override bool IsContacted
    {
        get
        {
            if (contactColliders != null)
                return contactColliders.Count > 0;
            else return false;
        }
    }

    private void Awake()
    {
        contactColliders = new List<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (layerMask.Contains(collision.gameObject.layer))
        {
            contactColliders.Add(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        contactColliders.Remove(collision);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color =  Color.green;
        if (collider is BoxCollider2D box)
        {
            Gizmos.DrawWireCube(transform.position + (Vector3)box.offset, box.size);
        }
        else if (collider is CircleCollider2D circle)
        {
            Gizmos.DrawWireSphere(transform.position + (Vector3)circle.offset, circle.radius);
        }
    }
}
