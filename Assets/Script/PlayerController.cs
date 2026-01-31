using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    
    [Header("Movement")]
    [SerializeField]private float _JogSpeed;
    [SerializeField]private float _RunSpeed;
    [SerializeField]private float _CurrentSpeed;
    [SerializeField]private float _RotationSpeed = 5f;

    private Quaternion _TargetRotation;

    public GameObject GunMesh;
    public GameObject MaskMesh;
    public bool HasMaskOn;
    public bool IsCarryingMeat;
    public bool IsCarryingBread;

    public float AlertSpeedMultiplier;

    private CharacterController _Cc;
    private PlayerInputHandler _Input;
    private PlayerInteraction _Interact;

    public enum PlayerState
    {
        None,
        Idle,
        Cooking,
        Robbing,
        Dead
    }

    [SerializeField] private PlayerState _CurrentState;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else 
            Destroy(gameObject);
        
        _Cc = GetComponent<CharacterController>();
        _Input = GetComponent<PlayerInputHandler>();
        _Interact = GetComponent<PlayerInteraction>();
    }

    private void Start()
    {
        _TargetRotation = transform.rotation;
        _CurrentSpeed = _JogSpeed;
        ToggleMask(false);
        ChangeState(PlayerState.Idle);
    }

    private void Update()
    {
        
        //Robbing
        // if (HasMaskOn)
        // {
        //     if (_Interact.CastForVictims(transform.position, out var victim))
        //     {
        //         //Show prompt
        //         //Check for input
        //     }
        // }
        
        UpdateState();
    }

    private void ToggleMask() => ToggleMask(!HasMaskOn);
    private void ToggleMask(bool maskIsOn)
    {
        HasMaskOn = maskIsOn;
        
        MaskMesh.SetActive(HasMaskOn);
        DropBurger();
    }
    
    private void DropBurger()
    {
        IsCarryingMeat = false;
        IsCarryingBread  = false;
        
        //Spawn some visuals (if you are carrying a burger)
    }
    
    private void ChangeState(PlayerState newState)
    {
        ExitState();
        _CurrentState = newState;
        switch (newState)
        {
            case PlayerState.Idle:
                GunMesh.SetActive(false);
                break;
            case PlayerState.Cooking:
                break;
            case PlayerState.Robbing:
                GunMesh.SetActive(true);
                break;
            case PlayerState.Dead:
                break;
        }
    }

    private void UpdateState()
    {
        switch (_CurrentState)
        {
            case PlayerState.Idle:
                //Interaction cast
                if (_Interact.CastForInteractables(transform.position, out var interactable))
                {
                    //Show prompt
                    HUD.SetInteractionPrompt(interactable.GetPosition(), interactable.GetOffset());
                    
                    //Check for input
                    if (_Input.InteractWasPerformedThisFrame)
                    {
                        interactable.Interact();
                    }
                }
                else
                {
                    HUD.HideInteractionPrompt();
                }
                
                AlertSpeedMultiplier = _Input.IsSprinting ? 2 : 1;
                
                if (HasMaskOn && _Input.IsHoldingAim)
                    ChangeState(PlayerState.Robbing);

                if (_Input.MaskWasPerformedThisFrame)
                    ToggleMask();
                
                UpdateMovement(_Input.IsSprinting ? _RunSpeed : _JogSpeed);
                break;
            case PlayerState.Cooking:
                break;
            case PlayerState.Robbing:
                AlertSpeedMultiplier = 3;
                if (_Interact.CastForVictims(transform.position, out var customer))
                {
                    _TargetRotation = Quaternion.LookRotation(customer.GetTransform().position - transform.position);
                    transform.rotation = Quaternion.Lerp(transform.rotation,_TargetRotation,Time.deltaTime * _RotationSpeed);
                    CameraController.Instance.SecondaryTargetTransform = customer.GetTransform();
                }
                else
                {
                    CameraController.Instance.SecondaryTargetTransform = null;
                }
                
                if (!_Input.IsHoldingAim)
                    ChangeState(PlayerState.Idle);
                break;
            case PlayerState.Dead:
                break;
        }
    }
    
    private void ExitState()
    {
        switch (_CurrentState)
        {
            case PlayerState.Idle:
                break;
            case PlayerState.Cooking:
                break;
            case PlayerState.Robbing:
                CameraController.Instance.SecondaryTargetTransform = null;
                GunMesh.SetActive(false);
                break;
            case PlayerState.Dead:
                break;
        }
    }

    private void UpdateMovement(float speed)
    {
        //Movement
        _CurrentSpeed = speed;
        Vector3 moveDir = new Vector3(_Input.Movement.x, 0, _Input.Movement.y);
        if (_Input.Movement != Vector2.zero)
            _TargetRotation = Quaternion.LookRotation(moveDir);
        _Cc.Move(moveDir * (_CurrentSpeed * Time.deltaTime));
        transform.rotation = Quaternion.Lerp(transform.rotation,_TargetRotation,Time.deltaTime * _RotationSpeed);
    }
}
