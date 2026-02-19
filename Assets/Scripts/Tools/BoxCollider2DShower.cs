using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VisualizationTools
{
    public class BoxCollider2DShower : MonoBehaviour
    {
        BoxCollider2D box;
        BoxCollider2D Box
        {
            get
            {
                if (box == null)
                    box = GetComponent<BoxCollider2D>();
                return box;
            }
        }


        private void OnDrawGizmos()
        {
            if (!Box.enabled) return;
            Vector3 parentSize = transform.lossyScale;
            Func<Vector3, Vector3, Vector3> multiVec3 = (Vector3 v1, Vector3 v2) =>
            {
                v1.x *= v2.x;
                v1.y *= v2.y;
                v1.z *= v2.z;
                return v1;
            };

            Vector3 center = transform.position + multiVec3(parentSize, (Vector3)Box.offset);
            Vector3 size = multiVec3(parentSize, (Vector3)Box.size);

            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(center, size);
        }
    }
}