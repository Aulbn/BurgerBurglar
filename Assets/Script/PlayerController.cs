using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]private float _JogSpeed;
    [SerializeField]private float _RunSpeed;
    [SerializeField]private float _CurrentSpeed;
    [SerializeField]private float _RotationSpeed = 5f;

    private Quaternion _TargetRotation;

    public bool HasMaskOn;

    public float AlertSpeedMultiplier; //1 if jogging, 2 if running

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
        _Cc = GetComponent<CharacterController>();
        _Input = GetComponent<PlayerInputHandler>();
        _Interact = GetComponent<PlayerInteraction>();
    }

    private void Start()
    {
        _CurrentSpeed = _JogSpeed;
        ChangeState(PlayerState.Idle);
    }

    private void Update()
    {
        //Interaction cast
        if (_Interact.CastForInteractables(transform.position, out var interactable))
        {
            //Show prompt
            //Check for input
        }
        
        //Robbing
        if (HasMaskOn)
        {
            if (_Interact.CastForVictims(transform.position, out var victim))
            {
                //Show prompt
                //Check for input
            }
        }
        
        UpdateState();
    }
    
    private void ChangeState(PlayerState newState)
    {
        ExitState();
        _CurrentState = newState;
        switch (newState)
        {
            case PlayerState.Idle:
                break;
            case PlayerState.Cooking:
                break;
            case PlayerState.Robbing:
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
                UpdateMovement(_Input.IsSprinting ? _RunSpeed : _JogSpeed);
                break;
            case PlayerState.Cooking:
                break;
            case PlayerState.Robbing:
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
