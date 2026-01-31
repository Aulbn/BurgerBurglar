using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CustomerController : MonoBehaviour, ICustomer
{
    private NavMeshAgent _Agent;
    public float NormalSpeed;
    public float FleeSpeed;

    public bool HasGottenFood;
    public float MaxWaitTime = 30f;
    public float AlertTime = 2f;
    
    [SerializeField] private float _LeaveQueueTime;
    [SerializeField] private float _AlertAmount;

    public AIState CurrentState;

    private void Awake()
    {
        _Agent =  GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        JoinQueue();
        ChangeState(AIState.Queuing);
        // _Agent.SetDestination(GameManager.ExitPosition);
        _Agent.speed = NormalSpeed;
    }

    private void Update()
    {
        UpdateState();
    }

    private void ChangeState(AIState newState)
    {
        ExitState();
        CurrentState = newState;
        switch (newState)
        {
            case AIState.Queuing:
                _LeaveQueueTime = Time.time + MaxWaitTime;
                JoinQueue();
                break;
            case AIState.Alert:
                _AlertAmount = 0;
                break;
            case AIState.Combat:
                break;
            case AIState.Fleeing:
                _Agent.speed = FleeSpeed;
                break;
            case AIState.Leaving:
                ExitRestaurant();
                break;            
            case AIState.Threatened: 
                break;
        }
    }

    private void UpdateState()
    {
        switch (CurrentState)
        {
            case AIState.Queuing:
                if (GameManager.RegisterQueue.TryGetQueuePosition(this, out var pos))
                    _Agent.SetDestination(pos);
                break;
            case AIState.Alert:
                //Could multiply this with some value, like distance.
                _AlertAmount += Time.deltaTime; 
                break;
            case AIState.Combat:
                break;
            case AIState.Fleeing:
                break;
            case AIState.Leaving:
                break;
            case AIState.Threatened: 
                break;
        }
    }
    
    private void ExitState()
    {
        switch (CurrentState)
        {
            case AIState.Queuing:
                break;
            case AIState.Alert:
                break;
            case AIState.Combat:
                break;
            case AIState.Fleeing:
                _Agent.speed = NormalSpeed;
                break;
            case AIState.Leaving:
                break;
            case AIState.Threatened: 
                break;
        }
    }

    private void JoinQueue()
    {
        GameManager.RegisterQueue.AddCustomer(this);
    }
    
    private void LeaveQueue()
    {
        GameManager.RegisterQueue.RemoveCustomer(this);
    }

    private void ExitRestaurant()
    {
        _Agent.SetDestination(GameManager.ExitPosition);
    }
    
}
