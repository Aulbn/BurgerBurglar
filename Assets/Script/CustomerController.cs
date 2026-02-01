using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class CustomerController : MonoBehaviour, ICustomer, IInteractable
{
    [Header("Interaction")]
    public Vector2 InteractionUIOffset;
    [SerializeField] private bool _IsInteractable;
    
    [Header("Other")]
    public float NormalSpeed;
    public float FleeSpeed;
    
    private NavMeshAgent _Agent;

    public bool HasGottenFood;
    public float MaxWaitTime = 30f;
    public float AlertTime = 2f;
    public float DealertTime = 2f;
    
    [Header("Sight")]
    public float FieldOfView;
    public float RangeOfView;
    
    [Header("Queueing")]
    [SerializeField] private float _LeaveQueueTime;
    
    [Header("Alert")]
    [Range(0,1)] public float AlertAmount;
    public GameObject AlertImageParent;
    public Image AlertImageFill;

    public float GiveMoneyTime;
    public float Timer;

    public Animator _Animator;

    public AIState CurrentState;

    private int _EnteredStateFrame = 0;

    private void Awake()
    {
        _Agent =  GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        ChangeState(AIState.Queuing);
        // _Agent.SetDestination(GameManager.ExitPosition);
        _Agent.speed = NormalSpeed;
    }

    private void Update()
    {
        AlertImageParent.SetActive(AlertAmount > 0);
        AlertImageParent.transform.rotation = Quaternion.LookRotation(CameraController.Cam.transform.forward);
        AlertImageFill.fillAmount = AlertAmount;
        UpdateState();
        _Animator.SetFloat("float_velocity", _Agent.velocity.magnitude);
    }

    private void ChangeState(AIState newState)
    {
        _EnteredStateFrame = Time.frameCount;
        ExitState();
        CurrentState = newState;
        switch (newState)
        {
            case AIState.Queuing:
                // Debug.Log("Queuing");
                _LeaveQueueTime = Time.time + MaxWaitTime;
                JoinQueue();
                break;
            case AIState.Alert:
                // Debug.Log("Alert");
                break;
            case AIState.Combat:
                // Debug.Log("Combat");
                break;
            case AIState.Fleeing:
                // Debug.Log("Fleeing");
                LeaveQueue();
                _Agent.speed = FleeSpeed;
                _Agent.SetDestination(GameManager.ExitPosition);
                break;
            case AIState.Leaving:
                // Debug.Log("Leaving");
                LeaveQueue();
                _Agent.speed = NormalSpeed;
                _Agent.SetDestination(GameManager.ExitPosition);
                break;            
            case AIState.Threatened:
                _Animator.SetTrigger("trigger_robbed");
                // Debug.Log("Threatened");
                _Agent.SetDestination(transform.position);
                AlertAmount = 1;
                Timer = 0;
                break;
        }
    }

    private void UpdateState()
    {
        if (_EnteredStateFrame == Time.frameCount) //Don't run update same frame as enter
            return;
        
        switch (CurrentState)
        {
            case AIState.Queuing:
                if (GameManager.RegisterQueue.TryGetQueuePosition(this, out var pos))
                {
                    _Agent.SetDestination(pos);
                }
                UpdateAlert();
                if (AgentHasArrived())
                    transform.rotation =Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(-GameManager.RegisterQueue.transform.forward), Time.deltaTime * 8f);
                if (AlertAmount > 0)
                    ChangeState(AIState.Alert);
                
                if (Time.time > _LeaveQueueTime)
                    ChangeState(AIState.Leaving);
                
                break;
            case AIState.Alert:
                UpdateAlert();
                if (AlertAmount == 0)
                    ChangeState(AIState.Queuing);        
                if (AlertAmount >= 1)
                    ChangeState(AIState.Fleeing);   
                break;
            case AIState.Combat:
                break;
            case AIState.Fleeing:
                if (AgentHasArrived())
                    Destroy(gameObject);
                break;
            case AIState.Leaving:
                if (AgentHasArrived())
                    Destroy(gameObject);
                break;
            case AIState.Threatened:
                Timer += Time.deltaTime;
                // AlertAmount = Timer /  GiveMoneyTime;
                AlertImageFill.fillAmount = Timer / GiveMoneyTime;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(PlayerController.Instance.transform.position - transform.position), Time.deltaTime * 8f);
                // Debug.Log($"{Timer} / {GiveMoneyTime}");
                if (Timer >= GiveMoneyTime)
                {
                    GameManager.AddStolenMoney();
                    ChangeState(AIState.Fleeing);
                }
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
                if (_Agent != null)
                    _Agent.speed = NormalSpeed;
                break;
            case AIState.Leaving:
                break;
            case AIState.Threatened:
                // Debug.Log("Exit Threatened");
                Timer = 0;
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

    private void UpdateAlert()
    {
        //Could multiply the speed with some value, like distance, or if player holds gun.
        // Debug.Log(PlayerInSight(), gameObject);
        if (PlayerInSight() && PlayerController.Instance.HasMaskOn)
            AlertAmount += Time.deltaTime / AlertTime;
        else
            AlertAmount -= Time.deltaTime / DealertTime;

        AlertAmount = Mathf.Clamp01(AlertAmount);
    }
    
    private bool PlayerInSight()
    {
        if (Vector3.Distance(transform.position, PlayerController.Instance.transform.position) > RangeOfView)
            return false;
        
        if (Vector3.Angle(transform.forward, (PlayerController.Instance.transform.position - transform.position).normalized) >
            FieldOfView)
            return false;
        
        return true;
    }

    private bool AgentHasArrived()
    {
        float dist = _Agent.remainingDistance;
        return !float.IsPositiveInfinity(dist) && _Agent.pathStatus == NavMeshPathStatus.PathComplete &&
            _Agent.remainingDistance == 0;
    }

    public bool IsInteractable() => false;

    public Vector2 GetOffset() => InteractionUIOffset;
    public Vector3 GetPosition() => transform.position;

    public void Interact()
    {
        Debug.Log("Interact with Customer", gameObject);
        PlayerController.ToggleMeat(false);
        PlayerController.ToggleBread(false);
    }

    public void GiveOrder()
    {
        ChangeState(AIState.Leaving);
        PlayerController.ToggleMeat(false);
        PlayerController.ToggleBread(false);
        GameManager.AddBurgerMoney();
    }

    public Transform GetTransform() => transform;
    public void OnThreaten()
    {
        if (CurrentState != AIState.Fleeing && CurrentState != AIState.Threatened)
        {
            ChangeState(AIState.Threatened);
        }
    }
    public void OnUnThreaten()
    {
        ChangeState(AIState.Fleeing);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + Vector3.up, transform.forward * RangeOfView);
        Gizmos.DrawRay(transform.position + Vector3.up, Quaternion.AngleAxis(FieldOfView * 0.5f, Vector3.up) * transform.forward * RangeOfView);
        Gizmos.DrawRay(transform.position + Vector3.up, Quaternion.AngleAxis(-FieldOfView * 0.5f, Vector3.up) * transform.forward * RangeOfView);
    }
    
    private void OnDrawGizmosSelected()
    {
        if (_Agent == null)
            return;
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position + Vector3.up, _Agent.destination);
    }
}
