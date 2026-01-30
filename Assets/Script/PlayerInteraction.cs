using UnityEngine;
using UnityEngine.Serialization;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField]private float _InteractionRadius = 1f;

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
}
