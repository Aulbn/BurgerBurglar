using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private Transform _CustomerSpawnTransform;
    public static Vector3 CustomerSpawnPosition =>  Instance._CustomerSpawnTransform.position;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
}
