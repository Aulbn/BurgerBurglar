using System;
using UnityEngine;

public class ExitCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter");
        if (other.TryGetComponent<PlayerController>(out var player))
        {
            GameManager.GameOver_ExitedDoor();
        }
        else if (other.TryGetComponent<HostageController>(out var hostage))
        {
            HostageGroupController.RemoveHostage(hostage);
            Destroy(hostage.gameObject);
        }
        else if (other.TryGetComponent<CustomerController>(out var customer))
        {
            if (customer.CurrentState != AIState.Queuing)
            {
                Destroy(customer.gameObject);
            }
        }
        else if (other.TryGetComponent<PoliceController>(out var police))
        {
            if (police.CurrentState != AIState.Queuing)
            {
                if (customer.gameObject != null)
                    Destroy(customer.gameObject);
            }
        }

    }
}
