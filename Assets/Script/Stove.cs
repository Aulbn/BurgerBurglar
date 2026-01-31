using System;
using UnityEngine;

public class Stove : MonoBehaviour, IInteractable
{
    [Header("Interaction")]
    public Vector2 InteractionUIOffset;
    [SerializeField] private bool _IsInteractable;
    [Header("Stove")]
    public GameObject BeefVisuals;
    public GameObject BurntBeefVisuals;
    
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
    }

    private void Update()
    {
        UpdateState();
    }

    private void ChangeState(StoveState newState)
    {
        ExitState();
        CurrentState = newState;
        switch (newState)
        {
            case StoveState.Idle:
                _IsInteractable = true;
                BeefVisuals.SetActive(false);
                BurntBeefVisuals.SetActive(false);
                break;
            case StoveState.Cooking:
                BeefVisuals.SetActive(true);
                _IsInteractable = false;
                _Timer = 0;
                break;
            case StoveState.Burning:
                _IsInteractable = false;
                _Timer = 0;
                break;
            case StoveState.Burnt:
                _IsInteractable = true;
                BeefVisuals.SetActive(false);
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
                BeefVisuals.SetActive(true);
                _Timer += Time.deltaTime;
                if (_Timer >= CookTime)
                {
                    //Change look
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
