using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    public static HUD Instance;
    
    public RectTransform InteractionPrompt;
    public TextMeshProUGUI MoneyText;

    private Coroutine MoneyBounceCoroutine;
    public AnimationCurve MoneyBounceCurve;
    public float MoneyBounceDuration = 0.2f;
    public float MoneyBounceDistance = 10f;
    
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
    
    
    private IEnumerator IEMoneyBounce()
    {
        var startPos = MoneyText.rectTransform.anchoredPosition;
        var maxPos = MoneyText.rectTransform.anchoredPosition + Vector2.down * MoneyBounceDistance;
        float timer = 0;

        while (timer < MoneyBounceDuration)
        {
            MoneyText.rectTransform.anchoredPosition = Vector2.Lerp(startPos, maxPos, MoneyBounceCurve.Evaluate(timer / MoneyBounceDuration));
            timer += Time.deltaTime;
            yield return null;
        }
    }

    public static void SetMoneyText(float money)
    {
        Instance.MoneyText.text = $"${money}";
        if (Instance.MoneyBounceCoroutine != null)
            Instance.StopCoroutine(Instance.MoneyBounceCoroutine);
        Instance.MoneyBounceCoroutine = Instance.StartCoroutine(Instance.IEMoneyBounce());
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
