using System;
using System.Collections.Generic;
using UnityEngine;

public class RegisterQueue : MonoBehaviour
{
    public float MaxLength = 10f;
    public float Distance = 1;

    [SerializeField] private List<ICustomer> _QueueList = new List<ICustomer>(); 

    public void AddCustomer(ICustomer customer)
    {
        if (_QueueList.Contains(customer))
            return;
        _QueueList.Add(customer);
    }
    
    public void RemoveCustomer(ICustomer customer)
    {
        _QueueList.Remove(customer);
    }

    public bool TryGetQueuePosition(ICustomer customer, out Vector3 position)
    {
        position = Vector3.zero;
        if (!_QueueList.Contains(customer))
            return false;

        position = transform.position + transform.forward * Distance * _QueueList.IndexOf(customer);
        return true;
    }
    
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