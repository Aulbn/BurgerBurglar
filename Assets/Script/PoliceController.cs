using UnityEngine;

public class PoliceController : MonoBehaviour, ICustomer
{
    public void GiveOrder() { }
    public Transform GetTransform() => transform;
}
