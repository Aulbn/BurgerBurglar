using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PoliceController : MonoBehaviour, ICustomer
{
    [Header("Interaction")]
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
    public float GunReactionSpeedMultiplier = 3f;
    [Range(0,1)] public float AlertAmount; 
    public GameObject AlertImageParent;
    public Image AlertImageFill;

    public AIState CurrentState;

    public Animator _Animator;
    
    [Header("Audio")] 
    public AudioSource GunShotSound;
    
    private int _EnteredStateFrame = 0;
    
    private void Awake()
    {
        _Agent =  GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        ChangeState(AIState.Queuing);
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
                _LeaveQueueTime = Time.time + MaxWaitTime;
                JoinQueue();
                break;
            case AIState.Alert:
                break;
            case AIState.Combat:
                //TODO: ADD VFX
                StartCoroutine(IEShootPlayer());
                // _Animator.SetTrigger("trigger_shoot");
                // _Animator.SetBool("bool_aim", true);
                // GameManager.GameOver_Death();
                break;
            case AIState.Fleeing:
                LeaveQueue();
                _Agent.speed = FleeSpeed;
                _Agent.SetDestination(GameManager.ExitPosition);
                break;
            case AIState.Leaving:
                LeaveQueue();
                _Agent.speed = NormalSpeed;
                _Agent.SetDestination(GameManager.ExitPosition);
                break;            
            case AIState.Threatened: 
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
                    ChangeState(AIState.Combat);   
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

    private IEnumerator IEShootPlayer()
    {
        _Agent.isStopped = true;
        _Animator.SetBool("bool_aim", true);
        yield return new WaitForSeconds(1f);
        StartCoroutine(IEEmptyClip());
        PlayerController.Instance.Kill();
        yield return new WaitForSeconds(3f);
        GameManager.GameOver_Death();
    }
    
    private IEnumerator IEEmptyClip()
    {
        for (int i = 0; i < 6; i++)
        {
            _Animator.SetTrigger("trigger_shoot");
            GunShotSound.Play();
            yield return new WaitForSeconds(0.2f);
        }
    }
    
    private void UpdateAlert()
    {
        //Could multiply the speed with some value, like distance, or if player holds gun.
        // Debug.Log(PlayerInSight(), gameObject);
        if (PlayerInSight() && PlayerController.Instance.HasMaskOn)
        {
            float multiplier = 1;
            if (PlayerController.Instance.GunMesh.activeSelf)
                multiplier = GunReactionSpeedMultiplier;
            AlertAmount += Time.deltaTime / AlertTime * multiplier;
        }
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
    
    private void JoinQueue()
    {
        GameManager.RegisterQueue.AddCustomer(this);
    }
    
    private void LeaveQueue()
    {
        GameManager.RegisterQueue.RemoveCustomer(this);
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
        //Shoot player
    }

    public void OnUnThreaten() { }
}
