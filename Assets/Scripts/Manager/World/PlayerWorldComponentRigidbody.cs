using UnityEngine;
using System.Collections;
using Simulation.Enums.Battle;
using Simulation.Enums.Character;
using Simulation.Enums.Duel;

public class PlayerWorldComponentRigidbody : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    public Rigidbody2D Rb => rb;
    private bool isPaused;
    private PlayerWorldEntity playerWorldEntity;

    public void Initialize(PlayerWorldEntity playerWorldEntity)
    {
        this.playerWorldEntity = playerWorldEntity;
    }

    void OnEnable()
    {

    }

    void OnDisable()
    {

    }

    public void Teleport(Vector2 position)
    {
        rb.interpolation = RigidbodyInterpolation2D.None;

        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;

        rb.position = position;
        transform.position = position;

        rb.Sleep();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.WakeUp();
        WorldEvents.RaisePlayerTeleported(new Vector3(position.x, position.y, 0f));
    }

    public Vector2 GetPosition2d() 
    {
        return rb.position;
    }

    public Vector3 GetPosition3d() 
    {
        return new Vector3(rb.position.x, rb.position.y, transform.position.z);
    }

}
