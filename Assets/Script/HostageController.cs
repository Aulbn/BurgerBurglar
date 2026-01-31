using UnityEngine;

public class HostageController : MonoBehaviour
{
    [Range(0,1)]public float AlertAmount;
    [Range(0,1)]public float ScaredAmount;

    public float RangeOfView;
    public float AlertTime;
    public float DealertTime;
    public float ScaredIncreaseTime;
    public float ScaredDecreaseTime;

    public enum HostageState
    {
        Idle,
        Escaping,
        Returning
    }

    public HostageState CurrentState;
    private int _EnteredStateFrame;
    
    private void ChangeState(HostageState newState)
    {
        _EnteredStateFrame = Time.frameCount;
        ExitState();
        CurrentState = newState;
        
        switch (newState)
        {
            case HostageState.Idle:
                break;
            case HostageState.Escaping:
                break;
            case HostageState.Returning:
                break;
        }
    }

    private void UpdateState()
    {
        if (_EnteredStateFrame == Time.frameCount) //Don't run update same frame as enter
            return;
        
        switch (CurrentState)
        {
            case HostageState.Idle:
                break;
            case HostageState.Escaping:
                break;
            case HostageState.Returning:
                break;
        }
    }
    
    private void ExitState()
    {
        switch (CurrentState)
        {
            case HostageState.Idle:
                break;
            case HostageState.Escaping:
                break;
            case HostageState.Returning:
                break;
        }
    }
    
    private void UpdateAlert()
    {
        //Could multiply the speed with some value, like distance, or if player holds gun.
        // Debug.Log(PlayerInSight(), gameObject);
        if (PlayerInSight() && PlayerController.Instance.HasMaskOn)
            AlertAmount += Time.deltaTime / AlertTime * PlayerController.Instance.AlertSpeedMultiplier;
        else
            AlertAmount -= Time.deltaTime / DealertTime * PlayerController.Instance.AlertSpeedMultiplier;

        AlertAmount = Mathf.Clamp01(AlertAmount);
    }
    
    private void UpdateScared()
    {
        if (PlayerInSight() && PlayerController.Instance.HasMaskOn)
            ScaredAmount += Time.deltaTime / ScaredIncreaseTime * PlayerController.Instance.AlertSpeedMultiplier;
        else
            ScaredAmount -= Time.deltaTime / ScaredDecreaseTime * PlayerController.Instance.AlertSpeedMultiplier;

        ScaredAmount = Mathf.Clamp01(ScaredAmount);
    }
    
    private bool PlayerInSight()
    {
        if (Vector3.Distance(transform.position, PlayerController.Instance.transform.position) > RangeOfView)
            return false;
        
        return true;
    }
}
