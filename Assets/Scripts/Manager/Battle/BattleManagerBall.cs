using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class BattleManagerBall
{
    #region Fields

    private GameObject ballPrefab; //inspector
    private Transform spawnPoint;
    private Vector3 defaultBallPosition;
    private Ball ball;

    public Ball Ball => ball; 

    #endregion

    #region Constructor

    public BattleManagerBall(GameObject ballPrefab)
    {
        this.ballPrefab = ballPrefab;
    }

    #endregion

    #region Logic

    public void RegisterSpawnPoint(Transform spawner)
    {
        spawnPoint = spawner.transform;
        defaultBallPosition = spawnPoint.position;
    }

    public void UnregisterSpawnPoint()
    {
        spawnPoint = null;
        ball = null;
    }

    public void Spawn()
    {
        if (spawnPoint == null)
        {
            LogManager.Warning("[BattleBallManager] No spawn point registered! Cannot spawn ball.");
            return;
        }

        GameObject go = BattleManager.Instance.InstantiateBall(ballPrefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);
        ball = go.GetComponent<Ball>();
        ball.Initialize(BallManager.Instance.GetBallData(BattleArgs.BallId));
        ball.name = ball.BallId;
        BallEvents.RaiseBallSpawned(ball);
        Debug.Log("[BattleBallManager] Ball spawned successfully.");
    }

    public void ResetBallPosition() 
    {
        if (ball == null)
        {
            LogManager.Info("No ball to reset. Spawning a new one...");
            Spawn();
            return;
        }
        
        PossessionManager.Instance.Release();
        ball.ResetPhysics();
        ball.transform.position = defaultBallPosition;
        ball.transform.rotation = Quaternion.identity;
        
        LogManager.Trace("[BattleBallManager] Ball reset to default position: " + defaultBallPosition);
    }

    #endregion

    #region Helpers

    #endregion

    #region Events

    //public void Subscribe() { }
    //public void Unsubscribe() { }

    #endregion

}
