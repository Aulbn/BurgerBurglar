using System;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private Animator _Animator;

    private void Update()
    {
        _Animator.SetLayerWeight(1, _Animator.GetFloat("float_velocity") > 0 ? 0 : 1);
    }

    public void SetVelocity(float velocity)
    {
        _Animator.SetFloat("float_velocity", velocity);
    }

    public void Trigger_Mask()
    {
        _Animator.SetTrigger("trigger_mask");
    }

    public void Trigger_Cap()
    {
        _Animator.SetTrigger("trigger_cap");
    }
    
    public void ToggleAim(bool aim)
    {
        _Animator.SetBool("bool_aim", aim);
    }
    
    public void TriggerDeath()
    {
        _Animator.SetTrigger("trigger_death");
    }
}
