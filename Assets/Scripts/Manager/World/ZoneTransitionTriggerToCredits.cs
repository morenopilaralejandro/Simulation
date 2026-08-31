using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Aremoreno.Enums.World;

/// <summary>
/// Place on a trigger collider in the scene. When the player enters,
/// it transitions to the connected zone/spawn.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class ZoneTransitionTriggerToCredits : MonoBehaviour
{
    [Header("Destination")]
    [SerializeField] private SceneGroup sceneCredits;

    [Header("Transition Settings")]
    private float transitionDelayInternal = 0.4f;
    public float transitionDelay => transitionDelayInternal;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        InitiateTransition();
    }

    private async void InitiateTransition()
    {
        WorldManager.Instance.PlayerWorldEntity.StopMovement();
        StorySystemManager.Instance.SetFlag("allow_quick_travel", true);
        PersistenceManager.Instance.SaveGame();



        var worldManager = WorldManager.Instance;
        var player = WorldManager.Instance.PlayerWorldEntity;

        worldManager.SetIsTransitioning(true);
        player.SetControlEnabled(false);

        await worldManager.FadeOut();

        if (worldManager.CurrentZone != null && worldManager.CurrentZone.zoneType == ZoneType.Overworld)
            await ChunkStreamingManager.Instance.StopStreaming();

        bool unloadSuccess = await worldManager.UnloadCurrentZone();
        worldManager.SetState(WorldState.InEncounter);




        SceneLoader.Instance.LoadGroup(sceneCredits);

        /*
        bool unloadSuccess = await WorldManager.Instance.UnloadCurrentZone();
        SceneLoader.Instance.LoadGroup(sceneMainMenu);
        */
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
        var col = GetComponent<Collider2D>();
        if (col is BoxCollider2D box)
        {
            Gizmos.DrawCube(
                transform.position + (Vector3)box.offset,
                box.size
            );
        }
    }
}
