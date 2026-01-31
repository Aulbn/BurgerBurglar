using UnityEngine;

public interface ICustomer
{
    public void GiveOrder();
    public Transform GetTransform();
    public void OnThreaten();
    public void OnUnThreaten();
}
