using UnityEngine;

public class FoodDropOffPoint : MonoBehaviour, IInteractable
{
    public Vector2 InteractionUIOffset;

    public bool IsInteractable()
    {
        bool hasIngredients = PlayerController.Instance.IsCarryingMeat && PlayerController.Instance.IsCarryingBread;
        bool thereIsQueue = GameManager.RegisterQueue.QueueList.Count > 0;
        bool customerIsClose = thereIsQueue && Vector3.Distance(transform.position, GameManager.RegisterQueue.QueueList[0].GetTransform().position) < 4f;
        return hasIngredients && customerIsClose;
    }

    public Vector2 GetOffset() => InteractionUIOffset;
    public Vector3 GetPosition() => transform.position;

    public void Interact()
    {
        Debug.Log("Gave burger to customer", gameObject);
        PlayerController.ToggleMeat(false);
        PlayerController.ToggleBread(false);
        GameManager.RegisterQueue.QueueList[0].GiveOrder();
    }
}
