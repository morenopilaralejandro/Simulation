using System;
using System.Collections;
using UnityEngine;
using Simulation.Enums.World;

/// <summary>
/// Central manager that orchestrates all overworld player systems.
/// Attach to a persistent GameObject (survives scene loads).
/// </summary>
public class WorldManagerPlayer : MonoBehaviour
{
    public static WorldManagerPlayer Instance { get; private set; }

    [SerializeField] private PlayerWorldEntity playerWorldEntity;
    [SerializeField] private PlayerWorldConfig config;
    public PlayerWorldConfig PlayerWorldConfig => config;
    public PlayerWorldEntity PlayerWorldEntity => playerWorldEntity;


    // ================================================================
    //  LIFECYCLE
    // ================================================================

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

    private void Start()
    {
        playerWorldEntity.Initialize(null);
    }

}
