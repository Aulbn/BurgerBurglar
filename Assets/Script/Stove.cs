using System;
using UnityEngine;

public class Stove : MonoBehaviour, IInteractable
{
    [Header("Interaction")]
    public Vector2 InteractionUIOffset;
    [SerializeField] private bool _IsInteractable;
    [Header("Stove")]
    public GameObject RawBeefVisuals;
    public GameObject CookedBeefVisuals;
    public GameObject BurntBeefVisuals;
    
    [Header("Particle System")]
    public ParticleSystem FrySmokeParicles;
    public Color GoodSmokeColor;
    public Color BurntSmokeColor;
    
    public enum StoveState
    {
        Idle,
        Cooking,
        Burning,
        Burnt,
    }
    public StoveState CurrentState;

    public float CookTime = 5f;
    public float BeforeBurningTime = 5f;
    public float BurnTime = 3f;

    [SerializeField] private float _Timer;

    private void Start()
    {
        ChangeState(StoveState.Idle);
        FrySmokeParicles.Stop();
    }

    private void Update()
    {
        UpdateState();
    }

    private void SetSmokeColor(Color color)
    {
        var main = FrySmokeParicles.main;
        main.startColor = new ParticleSystem.MinMaxGradient(color);
    }
    
    private void ChangeState(StoveState newState)
    {
        ExitState();
        CurrentState = newState;
        switch (newState)
        {
            case StoveState.Idle:
                FrySmokeParicles.Stop();
                _IsInteractable = true;
                RawBeefVisuals.SetActive(false);
                CookedBeefVisuals.SetActive(false);
                BurntBeefVisuals.SetActive(false);
                break;
            case StoveState.Cooking:
                FrySmokeParicles.Play();
                SetSmokeColor(GoodSmokeColor);
                RawBeefVisuals.SetActive(true);
                _IsInteractable = false;
                _Timer = 0;
                break;
            case StoveState.Burning:
                SetSmokeColor(BurntSmokeColor);
                _IsInteractable = false;
                _Timer = 0;
                break;
            case StoveState.Burnt:
                FrySmokeParicles.Stop();
                _IsInteractable = true;
                RawBeefVisuals.SetActive(false);
                CookedBeefVisuals.SetActive(false);
                BurntBeefVisuals.SetActive(true);
                _Timer = 0;
                break;
        }
    }

    private void UpdateState()
    {
        switch (CurrentState)
        {
            case StoveState.Idle:
                break;
            case StoveState.Cooking:
                _Timer += Time.deltaTime;
                if (_Timer >= CookTime)
                {
                    //Change look
                    RawBeefVisuals.SetActive(false);
                    CookedBeefVisuals.SetActive(true);
                    _IsInteractable = true;
                }
                if (_Timer >= CookTime + BeforeBurningTime)
                {
                    ChangeState(StoveState.Burning);
                }
                break;
            case StoveState.Burning:
                _Timer += Time.deltaTime;
                if (_Timer >= BurnTime)
                {
                    ChangeState(StoveState.Burnt);
                }
                break;
            case StoveState.Burnt:
                break;
        }
    }
    
    private void ExitState()
    {
        switch (CurrentState)
        {
            case StoveState.Idle:
                break;
            case StoveState.Cooking:
                break;
            case StoveState.Burning:
                break;
            case StoveState.Burnt:
                break;
        }
    }

    public bool IsInteractable() => _IsInteractable && !PlayerController.Instance.IsCarryingMeat;

    public Vector2 GetOffset() => InteractionUIOffset;
    public Vector3 GetPosition() => transform.position;

    public void Interact()
    {
        Debug.Log("Interact with STOVE", gameObject);
        switch (CurrentState)
        {
            case StoveState.Idle:
                ChangeState(StoveState.Cooking);
                break;
            case StoveState.Cooking:
                if (_Timer >= CookTime)
                {
                    PlayerController.Instance.IsCarryingMeat = true;
                    ChangeState(StoveState.Idle);
                }
                break;
            case StoveState.Burning:
                break;
            case StoveState.Burnt:
                ChangeState(StoveState.Idle);
                break;
        }
    }
}
