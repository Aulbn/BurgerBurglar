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

    public float AlertSpeedMultiplier; //1 if jogging, 2 if running

    private CharacterController _Cc;
    private PlayerInputHandler _Input;
    private PlayerInteraction _Interact;


    private void Awake()
    {
        _Cc = GetComponent<CharacterController>();
        _Input = GetComponent<PlayerInputHandler>();
        _Interact = GetComponent<PlayerInteraction>();
    }

    private void Start()
    {
        _CurrentSpeed = _JogSpeed;
    }

    private void Update()
    {
        //Interaction
        if (_Interact.CastForInteractables(transform.position, out var interactable))
        {
            
        }
        
        //Movement
        _CurrentSpeed = _Input.IsSprinting ? _RunSpeed : _JogSpeed;
        Vector3 moveDir = new Vector3(_Input.Movement.x, 0, _Input.Movement.y);
        if (_Input.Movement != Vector2.zero)
            _TargetRotation = Quaternion.LookRotation(moveDir);
        _Cc.Move(moveDir * (_CurrentSpeed * Time.deltaTime));
        transform.rotation = Quaternion.Lerp(transform.rotation,_TargetRotation,Time.deltaTime * _RotationSpeed);
    }
}
