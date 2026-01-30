using System;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    public float MaxLength = 10f;
    public float Distance = 1;
    
    
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position + Vector3.up * 0.1f, 0.2f);
        Gizmos.DrawRay(transform.position + Vector3.up * 0.1f, transform.forward * (MaxLength-1) * Distance);
        for (int i = 1; i < MaxLength; i++)
        {
            Gizmos.DrawSphere(transform.position + transform.forward * Distance * i, 0.1f);
        }
    }
}