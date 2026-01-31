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
    public GameObject CapMesh;
    public GameObject MeatMesh;
    public GameObject BreadMesh;
    public bool HasMaskOn;
    public bool IsCarryingMeat;
    public bool IsCarryingBread;

    public float AlertSpeedMultiplier;

    private ICustomer _ThreatenedCustomer;

    private CharacterController _Cc;
    private PlayerInputHandler _Input;
    public PlayerInputHandler Input => _Input;
    private PlayerInteraction _Interact;
    private PlayerAnimationController _Animation;

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
        _Animation = GetComponent<PlayerAnimationController>();
    }

    private void Start()
    {
        _TargetRotation = transform.rotation;
        _CurrentSpeed = _JogSpeed;
        ToggleMask(false);
        ChangeState(PlayerState.Idle);
        ToggleBread(false);
        ToggleMeat(false);
    }

    private void Update()
    {
        UpdateState();
        _Animation.SetVelocity(_Cc.velocity.magnitude);

        if (_CurrentState != PlayerState.Dead)
        {
            if (_Input.PauseWasPerformedThisFrame)
                GameMenu.TogglePauseMenu();
        }
    }

    public static void ToggleBread(bool value)
    {
        Instance.IsCarryingBread = value;
        Instance.BreadMesh.SetActive(value);
    }

    public static void ToggleMeat(bool value)
    {
        Instance.IsCarryingMeat = value;
        Instance.MeatMesh.SetActive(value);
    }

    private void ToggleMask() => ToggleMask(!HasMaskOn);
    private void ToggleMask(bool maskIsOn)
    {
        HasMaskOn = maskIsOn;
        
        if (HasMaskOn)
            _Animation.Trigger_Mask();
        else
            _Animation.Trigger_Cap();
        MaskMesh.SetActive(HasMaskOn);
        CapMesh.SetActive(!HasMaskOn);
        DropBurger();
    }
    
    private void DropBurger()
    {
        ToggleMeat(false);
        ToggleBread(false);
        
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
                _Animation.ToggleAim(true);
                DropBurger();
                break;
            case PlayerState.Dead:
                DropBurger();
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
                if (!_Input.IsHoldingAim)
                {
                    ChangeState(PlayerState.Idle);
                    break;
                }
                _Cc.Move(Vector3.zero);
                AlertSpeedMultiplier = 3;
                if (_Interact.CastForHostages(transform.position, out var hostage))
                {
                    _TargetRotation = Quaternion.LookRotation(hostage.transform.position - transform.position);
                    transform.rotation = Quaternion.Lerp(transform.rotation,_TargetRotation,Time.deltaTime * _RotationSpeed);
                    CameraController.Instance.SecondaryTargetTransform = hostage.transform;
                    hostage.IncreaseScared();
                    if (_ThreatenedCustomer != null)
                    {
                        _ThreatenedCustomer.OnUnThreaten();
                        _ThreatenedCustomer = null;
                    }
                }
                else if (_Interact.CastForVictims(transform.position, out var customer))
                {
                    _TargetRotation = Quaternion.LookRotation(customer.GetTransform().position - transform.position);
                    transform.rotation = Quaternion.Lerp(transform.rotation,_TargetRotation,Time.deltaTime * _RotationSpeed);
                    CameraController.Instance.SecondaryTargetTransform = customer.GetTransform();
                    if (_ThreatenedCustomer != null && _ThreatenedCustomer != customer)
                        _ThreatenedCustomer.OnUnThreaten();
                    _ThreatenedCustomer = customer;
                    _ThreatenedCustomer.OnThreaten();
                }
                else
                {
                    if (_ThreatenedCustomer != null)
                    {
                        // Debug.Log("Unthreaten customer " + _ThreatenedCustomer, _ThreatenedCustomer.GetTransform().gameObject);
                        _ThreatenedCustomer.OnUnThreaten();
                        _ThreatenedCustomer = null;
                    }
                    CameraController.Instance.SecondaryTargetTransform = null;
                }
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
                _Cc.Move(Vector3.zero);
                HUD.HideInteractionPrompt();
                break;
            case PlayerState.Cooking:
                break;
            case PlayerState.Robbing:
                if (_ThreatenedCustomer != null)
                {
                    _ThreatenedCustomer.OnUnThreaten();
                    _ThreatenedCustomer = null;
                }               
                
                CameraController.Instance.SecondaryTargetTransform = null;
                GunMesh.SetActive(false);
                _Animation.ToggleAim(false);
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
