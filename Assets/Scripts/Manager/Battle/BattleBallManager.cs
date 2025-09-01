using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class BattleBallManager : MonoBehaviour
{

    public static BattleBallManager Instance { get; private set; }

    private Transform spawnPoint; 
    private string ballKey = "BallPrefab";
    private Vector3 defaultBallPosition;
    private GameObject ball; 

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void RegisterSpawnPoint(Transform spawner)
    {
        spawnPoint = spawner.transform;
        defaultBallPosition = spawnPoint.position;
    }

    public void UnregisterSpawnPoint()
    {
        spawnPoint = null;
    }

    public void Spawn()
    {
        if (spawnPoint == null)
        {
            LogManager.Warning("[BattleBallManager] No spawn point registered! Cannot spawn ball.");
            return;
        }

        Addressables.InstantiateAsync(ballKey, spawnPoint.position, spawnPoint.rotation, spawnPoint)
            .Completed += (AsyncOperationHandle<GameObject> handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                ball = handle.Result;
                LogManager.Info("[BattleBallManager] Ball spawned successfully: " + ball.name);
            }
            else
            {
                LogManager.Error("[BattleBallManager] Failed to spawn ball with key: " + ballKey);
            }
        };
    }

    public void ResetBallPosition() 
    {
        if (ball == null)
        {
            LogManager.Info("No ball to reset. Spawning a new one...");
            Spawn();
            return;
        }

        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        ball.transform.position = defaultBallPosition;
        ball.transform.rotation = Quaternion.identity;
        
        LogManager.Trace("[BattleBallManager] Ball reset to default position: " + defaultBallPosition);
    }

}
