using UnityEngine;
using System.Collections;
using Simulation.Enums.Battle;
using Simulation.Enums.Character;
using Simulation.Enums.Duel;

public class CharacterComponentRigidbody : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;

    /*
    public void Initialize(CharacterData characterData, Character character)
    {
        this.character = character;
    }
    */

    void OnEnable()
    {
        BattleEvents.OnBattlePhaseChanged += HandleBattlePhaseChanged;
    }

    void OnDisable()
    {
        BattleEvents.OnBattlePhaseChanged -= HandleBattlePhaseChanged;
    }

    private void HandleBattlePhaseChanged(BattlePhase newPhase, BattlePhase oldPhase) 
    {
        if (newPhase == BattlePhase.Selection && oldPhase == BattlePhase.Battle)
        {
            PausePhysics();
            return;
        }

        if (newPhase == BattlePhase.Battle && oldPhase == BattlePhase.Selection)
            ResumePhysics();
    }

    private void PausePhysics()
    {
        rb.constraints = RigidbodyConstraints.FreezePosition;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void ResumePhysics()
    {
        rb.constraints = RigidbodyConstraints.FreezeRotation;

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

        rb.Sleep();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.WakeUp();
    }

    public void SetRotation(Quaternion rotation)
    {
        rb.rotation = rotation;
    }
}
