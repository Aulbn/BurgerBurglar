using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class HostageController : MonoBehaviour
{
    [Range(0,1)]public float ScaredAmount;
    public float RunSpeed = 3;
    public float ScaredIncreaseTime;

    private NavMeshAgent _Agent;
    
    public enum HostageState
    {
        Idle,
        Escaping,
        Returning
    }

    public HostageState CurrentState;
    private int _EnteredStateFrame;

    private void Awake()
    {
        _Agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        _Agent.speed = RunSpeed;
    }

    private void Update()
    {
        UpdateState();
    }

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
                _Agent.SetDestination(GameManager.ExitPosition);
                break;
            case HostageState.Returning:
                _Agent.SetDestination(HostageGroupController.GetSitPosition(this));
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
                if (AgentHasArrived())
                {
                    HostageGroupController.RemoveHostage(this);
                    Destroy(gameObject);
                }
                
                if (ScaredAmount >= 1)
                {
                    ChangeState(HostageState.Returning);
                }
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

    public void Release()
    {
        ChangeState(HostageState.Escaping);
    }
    
    public void IncreaseScared()
    {
        //Controlled from player
        ScaredAmount += Time.deltaTime / ScaredIncreaseTime;
        ScaredAmount = Mathf.Clamp01(ScaredAmount);
    }
    
    private bool AgentHasArrived()
    {
        float dist = _Agent.remainingDistance;
        return !float.IsPositiveInfinity(dist) && _Agent.pathStatus == NavMeshPathStatus.PathComplete &&
               _Agent.remainingDistance == 0;
    }
}
