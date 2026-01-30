using System;
using UnityEngine;

public class HUD : MonoBehaviour
{
    public static HUD Instance;
    
    public RectTransform InteractionPrompt;

    private void Awake()
    {
        if (Instance == null)
            Instance  = this;
        else 
            Destroy(gameObject);
    }

    public void SetInteractionPoint(Vector3 worldPos, Vector2 offset)
    {
        InteractionPrompt.position = CameraController.Cam.WorldToScreenPoint(worldPos) + new Vector3(offset.x / Screen.width, offset.y / Screen.height, 0);
    }
}
