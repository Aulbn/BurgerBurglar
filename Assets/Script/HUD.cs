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

    private void Start()
    {
        HideInteractionPrompt();
    }

    public static void SetInteractionPrompt(Vector3 worldPos, Vector2 offset)
    {
        Instance.InteractionPrompt.gameObject.SetActive(true);
        Instance.InteractionPrompt.anchoredPosition = CameraController.Cam.WorldToScreenPoint(worldPos) + new Vector3(offset.x, offset.y, 0);
    }

    public static void HideInteractionPrompt()
    {
        Instance.InteractionPrompt.gameObject.SetActive(false);
    }
}
