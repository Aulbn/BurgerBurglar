using UnityEngine;

public class FoodDropOffPoint : MonoBehaviour, IInteractable
{
    public Vector2 InteractionUIOffset;

    public bool IsInteractable() => PlayerController.Instance.IsCarryingMeat && PlayerController.Instance.IsCarryingBread;

    public Vector2 GetOffset() => InteractionUIOffset;
    public Vector3 GetPosition() => transform.position;

    public void Interact()
    {
        Debug.Log("Gave burger to customer", gameObject);
        PlayerController.Instance.IsCarryingMeat = false;
        PlayerController.Instance.IsCarryingBread = false;
        GameManager.RegisterQueue.QueueList[0].GiveOrder();
    }
}
