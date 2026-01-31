using UnityEngine;
using UnityEngine.Serialization;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField]private float _InteractionRadius = 1f;
    public float RobDuration;
    public float RobDistance;

    public bool CastForInteractables(Vector3 point, out IInteractable interactable)
    {
        interactable = default;
        var colliders = Physics.OverlapSphere(point, _InteractionRadius);
        if (colliders.Length == 0)
            return false;

        var bestCollider = colliders[0];
        var bestAngle = Vector3.Angle(transform.forward, (colliders[0].transform.position - transform.position).normalized);
        foreach (var collider in colliders)
        {
            if (!collider.TryGetComponent<IInteractable>(out interactable))
                return false;
            
            float angle = Vector3.Angle(transform.forward,
                (collider.transform.position - transform.position).normalized);
            
            if (angle < bestAngle)
            {
                bestCollider = collider;
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
        var colliders = Physics.OverlapSphere(point, RobDistance);
        if (colliders.Length == 0)
            return false;

        var bestCollider = colliders[0];
        var bestAngle = Vector3.Angle(transform.forward, (colliders[0].transform.position - transform.position).normalized);
        foreach (var collider in colliders)
        {
            if (!collider.TryGetComponent<ICustomer>(out customer))
                return false;
            
            float angle = Vector3.Angle(transform.forward,
                (collider.transform.position - transform.position).normalized);
            
            if (angle < bestAngle)
            {
                bestCollider = collider;
                bestAngle = angle;
            }
        }

        if (customer == null)
            return false;
        
        return true;
    }
    
    public bool CastForHostages(Vector3 point, out HostageController hotsage)
    {
        hotsage = default;
        var colliders = Physics.OverlapSphere(point, RobDistance);
        if (colliders.Length == 0)
            return false;

        var bestCollider = colliders[0];
        var bestAngle = Vector3.Angle(transform.forward, (colliders[0].transform.position - transform.position).normalized);
        foreach (var collider in colliders)
        {
            if (!collider.TryGetComponent<HostageController>(out hotsage))
                return false;
            
            float angle = Vector3.Angle(transform.forward,
                (collider.transform.position - transform.position).normalized);
            
            if (angle < bestAngle)
            {
                bestCollider = collider;
                bestAngle = angle;
            }
        }

        if (hotsage == null)
            return false;
        
        return true;
    }
    
}
