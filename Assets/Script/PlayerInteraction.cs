using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField]private float _InteractionRadius = 1f;
    public float RobDuration;
    public float RobDistance;

    public LayerMask InteractableLayerMask;

    public bool CastForInteractables(Vector3 point, out IInteractable interactable)
    {
        interactable = default;
        var colliders = Physics.OverlapSphere(point, _InteractionRadius, InteractableLayerMask);
        if (colliders.Length == 0)
            return false;
        
        float bestAngle = 360f;
        foreach (var col in colliders)
        {
            if (!col.TryGetComponent<IInteractable>(out var currentInteractable))
                continue;
            if (currentInteractable.IsInteractable() == false)
                continue;
            
            float angle = Vector3.Angle(transform.forward,
                (col.transform.position - transform.position).normalized);

            if (angle < bestAngle)
            {
                interactable = currentInteractable;
                bestAngle = angle;
            }
        }

        if (interactable == null)
            return false;
        
        return true;
    }
    public bool CastForVictims(Vector3 point, out ICustomer customer) //Can TRY to rob both customers and police
    {
        customer = default;
        var colliders = Physics.OverlapSphere(point, RobDistance, InteractableLayerMask);
        if (colliders.Length == 0)
            return false;
        
        float bestAngle = 360f;
        foreach (var col in colliders)
        {
            if (!col.TryGetComponent<ICustomer>(out var currentCustomer))
                continue;

            float angle = Vector3.Angle(transform.forward,
                (col.transform.position - transform.position).normalized);

            if (angle < bestAngle)
            {
                customer = currentCustomer;
                bestAngle = angle;
            }
        }

        if (customer == null)
            return false;
        
        return true;
    }
    
    public bool CastForHostages(Vector3 point, out HostageController hostage)
    {
        hostage = default;
        var colliders = Physics.OverlapSphere(point, RobDistance);
        if (colliders.Length == 0)
            return false;

        var bestAngle = 360f;
        foreach (var collider in colliders)
        {
            if (!collider.TryGetComponent<HostageController>(out var currentHostage))
                continue;
            if (currentHostage.CurrentState != HostageController.HostageState.Escaping)
                continue;
            
            float angle = Vector3.Angle(transform.forward,
                (collider.transform.position - transform.position).normalized);
            
            if (angle < bestAngle)
            {
                hostage = currentHostage;
                bestAngle = angle;
            }
        }

        if (hostage == null)
            return false;
        
        return true;
    }

    public bool CastForEveryone(Vector3 point, out MonoBehaviour controller)
    {
        controller = default;
        var colliders = Physics.OverlapSphere(point, RobDistance);
        if (colliders.Length == 0)
            return false;

        var bestAngle = 500f;
        foreach (var col in colliders)
        {
            if (col.TryGetComponent<HostageController>(out var currentHostage))
            {
                if (currentHostage.CurrentState != HostageController.HostageState.Escaping)
                    currentHostage = null;
            }
            col.TryGetComponent<CustomerController>(out var currentCustomer);
            col.TryGetComponent<PoliceController>(out var currentPolice);
            
            if (currentHostage == null && currentCustomer == null &&  currentPolice == null)
                continue;
            
            Vector3 playerDir = new Vector3(PlayerController.Instance.Input.Movement.x, 0, PlayerController.Instance.Input.Movement.y);
            if (playerDir == Vector3.zero)
                playerDir = transform.forward;
            float angle = Vector3.Angle(playerDir, (col.transform.position - transform.position).normalized);
            
            if (angle < bestAngle)
            {
                if (currentHostage != null)
                {
                    controller = currentHostage;
                }
                else if  (currentCustomer != null)
                    controller = currentCustomer;
                else if   (currentPolice != null)
                    controller = currentPolice;
                bestAngle = angle;
            }
        }
        
        if (controller == null)
            return false;
        
        return true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _InteractionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, RobDistance);
    }
}
