using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HostageGroupController : MonoBehaviour
{
    public static HostageGroupController Instance;

    [Range(0,1)]public float ScaredAmount = 1;
    public float ScaredIncreaseTime;
    public float ScaredDecreaseTime;
    public float RangeOfView;
    
    public Image FillImage;

    [SerializeField] private List<HostageController> HostageList;
    [SerializeField] private Transform[] HostageSitPositions;
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < HostageList.Count; i++)
        {
            var hostage = HostageList[i];
            hostage.transform.position = HostageSitPositions[i].position;
        }
    }

    private void Update()
    {
        UpdateAlert();

        if (ScaredAmount == 0)
        {
            ReleaseHostage(); //Release a Hostage
            ScaredAmount = 1; //Reset scared amount
        }

        FillImage.fillAmount = ScaredAmount;
    }

    private void ReleaseHostage()
    {
        if (GameManager.CurrentState == GameManager.GameState.GameOver)
            return;
        
        foreach (var hostage in HostageList)
        {
            if (hostage.CurrentState == HostageController.HostageState.Idle)
            {
                hostage.Release();
                break;
            }
        }
    }

    public static Vector3 GetSitPosition(HostageController hostage)
    {
        return Instance.HostageSitPositions[Instance.HostageList.IndexOf(hostage)].position;
    }

    private void UpdateAlert()
    {
        //Could multiply the speed with some value, like distance, or if player holds gun.
        if (PlayerInView() && PlayerController.Instance.HasMaskOn)
        {
            float gunMultiplier = PlayerController.Instance.GunMesh.activeSelf ? 2 : 1;
            ScaredAmount += Time.deltaTime / ScaredIncreaseTime * gunMultiplier;
        }
        else
        {
            ScaredAmount -= Time.deltaTime / ScaredDecreaseTime;
        }

        ScaredAmount = Mathf.Clamp01(ScaredAmount);
    }

    public static void RemoveHostage(HostageController hostage)
    {
        Instance.HostageList.Remove(hostage);
        if (Instance.HostageList.Count == 0)
            GameManager.GameOver_HostagesEscaped();
    }
    
    private bool PlayerInView()
    {
        if (Vector3.Distance(transform.position, PlayerController.Instance.transform.position) > RangeOfView)
            return false;
        
        var origin = transform.position + Vector3.up;
        Ray ray = new Ray(origin, (PlayerController.Instance.transform.position + Vector3.up - origin).normalized);
        if (Physics.Raycast(ray, out var hit, RangeOfView))
        {
            // Debug.Log("PlayerInView HIT: " + hit.collider.gameObject.name);
            if (hit.collider.gameObject == PlayerController.Instance.gameObject)
            {        
                Debug.DrawRay(ray.origin, ray.direction * RangeOfView, Color.green);
                return true;
            }
            else
            {
                Debug.DrawRay(ray.origin, ray.direction * RangeOfView, Color.red);
                return false;
            }
        }
        
        return true;
    }
}
