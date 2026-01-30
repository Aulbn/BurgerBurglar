using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance; 
    
    [SerializeField] private Camera _Cam;
    public static Camera Cam => Instance._Cam;
    
    private void Awake()
    {
        if (Instance == null)
            Instance  = this;
        else 
            Destroy(gameObject);
    }
}
