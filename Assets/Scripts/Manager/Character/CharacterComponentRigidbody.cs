using UnityEngine;
using System.Collections;
using Simulation.Enums.Battle;
using Simulation.Enums.Character;
using Simulation.Enums.Duel;

public class CharacterComponentRigidbody : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;

    private bool isPaused;

    /*
    public void Initialize(CharacterData characterData, Character character)
    {
        this.character = character;
    }
    */

    void OnEnable()
    {
        BattleEvents.OnBattlePhaseChanged += HandleBattlePhaseChanged;
        MoveEvents.OnMoveCutsceneStart += HandleMoveCutsceneStart;
        MoveEvents.OnMoveCutsceneEnd += HandleMoveCutsceneEnd;
    }

    void OnDisable()
    {
        BattleEvents.OnBattlePhaseChanged -= HandleBattlePhaseChanged;
        MoveEvents.OnMoveCutsceneStart -= HandleMoveCutsceneStart;
        MoveEvents.OnMoveCutsceneEnd -= HandleMoveCutsceneEnd;
    }

    private void HandleBattlePhaseChanged(BattlePhase newPhase, BattlePhase oldPhase) 
    {
        if (newPhase == BattlePhase.Selection && oldPhase == BattlePhase.Battle)
        {
            PausePhysics();
            return;
        }

        if (newPhase == BattlePhase.Battle && oldPhase == BattlePhase.Selection && !BattleEffectManager.Instance.IsPlayingMove)
            ResumePhysics();
    }

    private void HandleMoveCutsceneStart() => PausePhysics();
    private void HandleMoveCutsceneEnd() => ResumePhysics();

    private void PausePhysics()
    {
        if(isPaused) return;

        rb.constraints = RigidbodyConstraints.FreezePosition;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        isPaused = true;
    }

    private void ResumePhysics()
    {
        if(!isPaused) return;

        rb.constraints = RigidbodyConstraints.FreezeRotation;

        isPaused = false;

        //rb.velocity = cachedVelocity;
        //rb.angularVelocity = cachedAngularVelocity;
    }

    public void ResetPhysics() 
    {
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.Sleep();
    }

    public void StopVelocity() 
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public void Teleport(Vector3 position)
    {
        rb.interpolation = RigidbodyInterpolation.None;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.position = position;
        transform.position = position;

        rb.Sleep();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.WakeUp();
    }

    public void SetRotation(Quaternion rotation)
    {
        rb.rotation = rotation;
    }
}
