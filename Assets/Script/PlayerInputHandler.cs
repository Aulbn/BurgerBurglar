using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInput _Input;

    public Vector2 Movement;
    public bool IsSprinting;
    [HideInInspector] public UnityEvent OnMaskToggle;
    [HideInInspector] public UnityEvent OnInteract;
    [HideInInspector] public bool InteractWasPerformedThisFrame;
    [HideInInspector] public bool MaskWasPerformedThisFrame;

    void Awake()
    {
        _Input = GetComponent<PlayerInput>();
        
        OnMaskToggle = new UnityEvent();
        OnInteract = new UnityEvent();
    }

    private void OnEnable()
    {
        _Input.actions.FindAction("Move").performed += SetMovement;
        _Input.actions.FindAction("Move").canceled += SetMovement;
        _Input.actions.FindAction("Sprint").performed += SetIsSprinting;
        _Input.actions.FindAction("Sprint").canceled += SetIsNotSprinting;
        _Input.actions.FindAction("Mask").performed += MaskToggle;
        _Input.actions.FindAction("Interact").performed += Interact;
    }

    private void OnDisable()
    {
        _Input.actions.FindAction("Move").performed -= SetMovement;
        _Input.actions.FindAction("Move").canceled -= SetMovement;
        _Input.actions.FindAction("Sprint").performed -= SetIsSprinting;
        _Input.actions.FindAction("Sprint").performed -= SetIsNotSprinting;
        _Input.actions.FindAction("Mask").performed -= MaskToggle;
        _Input.actions.FindAction("Interact").performed -= Interact;

    }

    private void Update()
    {
        InteractWasPerformedThisFrame = _Input.actions.FindAction("Interact").WasPerformedThisFrame();
        MaskWasPerformedThisFrame = _Input.actions.FindAction("Mask").WasPerformedThisFrame();
    }

    private void SetMovement(InputAction.CallbackContext context)
    {
        Movement = context.ReadValue<Vector2>();
    }
    private void SetIsSprinting(InputAction.CallbackContext context)
    {
        IsSprinting = true;
    }
    
    private void SetIsNotSprinting(InputAction.CallbackContext context)
    {
        IsSprinting = false;
    }

    private void MaskToggle(InputAction.CallbackContext context)
    {
        OnMaskToggle.Invoke();
    }
    
    private void Interact(InputAction.CallbackContext context)
    {
        OnInteract.Invoke();
    }
    
}