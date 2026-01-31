using UnityEngine;

public interface IInteractable
{
    // public Sprite GetSprite();
    public bool IsInteractable();
    public Vector2 GetOffset();
    public Vector3 GetPosition();
    public void Interact();
}
