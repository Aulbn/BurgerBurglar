using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance; 
    
    [SerializeField] private Camera _Cam;
    public static Camera Cam => Instance._Cam;
    
    public Transform TargetTransform;
    
    public float FollowSpeed = 10f;
    
    private void Awake()
    {
        if (Instance == null)
            Instance  = this;
        else 
            Destroy(gameObject);
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, TargetTransform.position, Time.deltaTime * FollowSpeed);
    }
}
