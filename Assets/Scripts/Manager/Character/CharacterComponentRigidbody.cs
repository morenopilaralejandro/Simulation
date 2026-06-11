using UnityEngine;
using System.Collections;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Duel;

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
        WingEvents.OnWingCutsceneStart += HandleWingCutsceneStart;
        WingEvents.OnWingCutsceneEnd += HandleWingCutsceneEnd;
    }

    void OnDisable()
    {
        BattleEvents.OnBattlePhaseChanged -= HandleBattlePhaseChanged;
        MoveEvents.OnMoveCutsceneStart -= HandleMoveCutsceneStart;
        MoveEvents.OnMoveCutsceneEnd -= HandleMoveCutsceneEnd;
        WingEvents.OnWingCutsceneStart -= HandleWingCutsceneStart;
        WingEvents.OnWingCutsceneEnd -= HandleWingCutsceneEnd;
    }

    private void HandleBattlePhaseChanged(BattlePhase newPhase, BattlePhase oldPhase) 
    {
        if (newPhase == BattlePhase.Selection && oldPhase == BattlePhase.Battle)
        {
            PausePhysics();
            return;
        }

        if (newPhase == BattlePhase.Battle && oldPhase == BattlePhase.Selection && !BattleManager.Instance.IsPlayingMove && !BattleManager.Instance.IsPlayingWing)
            ResumePhysics();
    }

    private void HandleMoveCutsceneStart(Move move) => PausePhysics();
    private void HandleMoveCutsceneEnd() => ResumePhysics();
    private void HandleWingCutsceneStart(Wing wing) => PausePhysics();
    private void HandleWingCutsceneEnd() => ResumePhysics();

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
