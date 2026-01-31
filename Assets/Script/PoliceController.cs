using UnityEngine;

public class PoliceController : MonoBehaviour, ICustomer
{
    public void GiveOrder() { }
    public Transform GetTransform() => transform;
    public void OnThreaten()
    {
        //Shoot player
    }

    public void OnUnThreaten() { }
}
