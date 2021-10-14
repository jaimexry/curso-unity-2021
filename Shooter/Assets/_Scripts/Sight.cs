using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sight : MonoBehaviour
{
    public float distance;
    public float angle;

    public LayerMask obstacleLayers;
    public LayerMask targetLayers;

    public Collider detectedTarget;

    private Collider[] colliders;
    private void Update()
    {
        if (Physics.OverlapSphereNonAlloc(transform.position, distance, colliders, targetLayers) == 0)
        {
            return;
        }
        
        colliders = Physics.OverlapSphere(transform.position, distance, targetLayers);
        detectedTarget = null;
        
        foreach (Collider collider in colliders)
        {
            Vector3 directionToCollider = Vector3.Normalize(collider.bounds.center - transform.position);

            float angleToCollider = Vector3.Angle(transform.forward, directionToCollider);

            if (angleToCollider < angle)
            {
                if (!Physics.Linecast(transform.position, collider.bounds.center,out RaycastHit hit, obstacleLayers))
                {
                    Debug.DrawLine(transform.position, collider.bounds.center, Color.green);
                    detectedTarget = collider;
                    break;
                }
                else
                {
                    Debug.DrawLine(transform.position, hit.point, Color.red);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distance);
        
        Vector3 rightDir = Quaternion.Euler(0, angle, 0) * transform.forward;
        Vector3 leftDir = Quaternion.Euler(0, -angle, 0) * transform.forward;
        
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(transform.position, rightDir * distance);
        Gizmos.DrawRay(transform.position, leftDir * distance);
        
    }
}