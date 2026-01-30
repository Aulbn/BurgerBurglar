using System;
using UnityEngine;
using UnityEngine.AI;

public class CustomerController : MonoBehaviour
{
    private NavMeshAgent _Agent;

    private void Awake()
    {
        _Agent =  GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        _Agent.SetDestination(GameManager.CustomerSpawnPosition);
    }
}
