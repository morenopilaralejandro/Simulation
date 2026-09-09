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

    private bool isTransitioning = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(isTransitioning) return;
        if (!other.CompareTag("Player")) return;
        isTransitioning = true;
        InitiateTransition();
    }

    private async void InitiateTransition()
    {
        WorldManager.Instance.PlayerWorldEntity.StopMovement();
        StorySystemManager.Instance.SetFlag("allow_quick_travel", true);
        PersistenceManager.Instance.SaveGame();

        bool unloadSuccess = await WorldManager.Instance.UnloadCurrentZone();
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
