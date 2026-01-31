using UnityEngine;

public class DressingTable : MonoBehaviour, IInteractable
{
    [Header("Interaction")]
    public Vector2 InteractionUIOffset;
    [SerializeField] private bool _IsInteractable;
    
    
    
    
    public bool IsInteractable() => !PlayerController.Instance.IsCarryingBread;

    public Vector2 GetOffset() => InteractionUIOffset;
    public Vector3 GetPosition() => transform.position;

    public void Interact()
    {
        Debug.Log("Picked up bread", gameObject);
        PlayerController.Instance.IsCarryingBread = true;
    }
}
