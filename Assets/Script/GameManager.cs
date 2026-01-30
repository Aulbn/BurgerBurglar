using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private Transform _CustomerSpawnTransform;
    public Vector3 _CustomerSpawnPosition =>  _CustomerSpawnTransform.position;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
}
