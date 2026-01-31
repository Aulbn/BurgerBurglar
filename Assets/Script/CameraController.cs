using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance; 
    
    [SerializeField] private Camera _Cam;
    public static Camera Cam => Instance._Cam;
    
    public Transform TargetTransform;
    public Transform SecondaryTargetTransform;
    
    public float FollowSpeed = 10f;
    
    private void Awake()
    {
        if (Instance == null)
            Instance  = this;
        else 
            Destroy(gameObject);
    }

    private void LateUpdate()
    {
        Vector3 targetPos = TargetTransform.position;
        if (SecondaryTargetTransform != null)
            targetPos = (Vector3.Lerp(targetPos, SecondaryTargetTransform.position, 0.5f));
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * FollowSpeed);
    }
}
