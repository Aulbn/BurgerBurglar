using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private Transform _ExitPositionTransform;
    public static Vector3 ExitPosition =>  Instance._ExitPositionTransform.position;
    
    [SerializeField] private RegisterQueue _RegisterQueue;
    public static RegisterQueue RegisterQueue => Instance._RegisterQueue;

    public static float CurrentGameplayTime = 0f;

    [Header("Customer Spawning")]
    public float TimeToMaxSpawn = 90f;
    public CustomerController CustomerPrefab;
    public Vector2 CustomerMinSpawnInterval;
    public Vector2 CustomerMaxSpawnInterval;
    private static float NextCustomerSpawnTime;
    
    public PoliceController PolicePrefab;
    public Vector2 PoliceMinSpawnInterval;
    public Vector2 PoliceMaxSpawnInterval;
    private static float NextPoliceSpawnTime;

    [Header("Money")] 
    [SerializeField] private float _CurrentMoney;
    public float CurrentMoney => _CurrentMoney;
    public float BurgerPrice = 12.9f;
    public Vector2 MoneyStealRange = new Vector2(15f, 55f);

    public enum GameState
    {
        None,
        Paused,
        MainMenu,
        Gameplay,
        GameOver,
    }
    public static GameState CurrentState { get; private set; }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        ChangeState(GameState.Gameplay);
        HUD.SetMoneyText(0);
    }

    private void Update()
    {
        UpdateState();
    }

    public static void ChangeState(GameState newState)
    {
        Instance.ExitState();
        CurrentState = newState;
        switch (newState)
        {
            case GameState.Gameplay:
                CurrentGameplayTime = 0;
                Debug.Log("Gameplay", Instance.gameObject);
                NextCustomerSpawnTime = GetNextCustomerSpawnTime();
                break;
            case GameState.Paused:
                Debug.Log("Paused", Instance.gameObject);
                break;
            case GameState.MainMenu:
                Debug.Log("Main Menu", Instance.gameObject);
                break;
            case GameState.GameOver:
                Debug.Log("GAME OVER", Instance.gameObject);
                break;
        }
    }

    private void UpdateState()
    {
        switch (CurrentState)
        {
            case GameState.Gameplay:
                CurrentGameplayTime += Time.deltaTime;
                if (Time.time >= NextCustomerSpawnTime)
                    SpawnCustomer();
                break;
            case GameState.Paused:
                break;
            case GameState.MainMenu:
                break;
            case GameState.GameOver:
                break;
        }
    }
    
    private void ExitState()
    {
        switch (CurrentState)
        {
            case GameState.Gameplay:
                break;
            case GameState.Paused:
                break;
            case GameState.MainMenu:
                break;
            case GameState.GameOver:
                break;
        }
    }

    public static void AddStolenMoney()
    {
        Instance.SetMoney(Instance._CurrentMoney + Random.Range(Instance.MoneyStealRange.x, Instance.MoneyStealRange.y));
    }

    public static void GameOver_HostagesEscaped()
    {
        
    }
    
    public static void AddBurgerMoney()
    {
        Instance.SetMoney(Instance._CurrentMoney + Instance.BurgerPrice);
    }

    private void SetMoney(float money)
    {
        _CurrentMoney = money;
        HUD.SetMoneyText(_CurrentMoney);
    }

    private static float GetNextCustomerSpawnTime()
    {
        float minTime = Random.Range(Instance.CustomerMinSpawnInterval.x, Instance.CustomerMinSpawnInterval.y);
        float maxTime = Random.Range(Instance.CustomerMaxSpawnInterval.x, Instance.CustomerMaxSpawnInterval.y);
        float value = Mathf.Lerp(minTime, maxTime, CurrentGameplayTime / Instance.TimeToMaxSpawn);
        Debug.Log("Next Customer Spawn Time Duration " + value);
        return Time.time + value;
    }

    private static void SpawnCustomer()
    {
        if (RegisterQueue.QueueList.Count < RegisterQueue.MaxLength)
            Instantiate(Instance.CustomerPrefab, ExitPosition, Quaternion.LookRotation(Instance._ExitPositionTransform.forward));
        NextCustomerSpawnTime = GetNextCustomerSpawnTime();
    }
    
    private static float GetNextPoliceSpawnTime()
    {
        float minTime = Random.Range(Instance.PoliceMinSpawnInterval.x, Instance.PoliceMinSpawnInterval.y);
        float maxTime = Random.Range(Instance.PoliceMaxSpawnInterval.x, Instance.PoliceMaxSpawnInterval.y);
        return Time.time + Mathf.Lerp(minTime, maxTime, CurrentGameplayTime / Instance.TimeToMaxSpawn);
    }
    
    private static void SpawnPolice()
    {
        
    }
    
    
}
