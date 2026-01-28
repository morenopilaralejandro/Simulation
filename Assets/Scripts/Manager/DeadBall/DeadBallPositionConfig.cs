using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Battle;
using Simulation.Enums.Character;
using Simulation.Enums.DeadBall;
using Simulation.Enums.Input;

public class DeadBallPositionConfig
{
    private Vector3[] throwInCornerOffense;
    private Vector3[] throwInCornerDefense;
    private Vector3[] throwInDefaultOffense;
    private Vector3[] throwInDefaultDefense;
    private Vector3[] cornerOffense;
    private Vector3[] cornerDefense;
    private Vector3 throwIncornerKicker;
    private Vector3 cornerKicker;

    public Vector3[] ThrowInCornerOffense => throwInCornerOffense;
    public Vector3[] ThrowInCornerDefense => throwInCornerDefense;
    public Vector3[] ThrowInDefaultOffense => throwInDefaultOffense;
    public Vector3[] ThrowInDefaultDefense => throwInDefaultDefense;
    public Vector3[] CornerOffense => cornerOffense;
    public Vector3[] CornerDefense => cornerDefense;
    public Vector3 ThrowIncornerKicker => throwIncornerKicker;
    public Vector3 CornerKicker => cornerKicker;

    public void Initialize()
    {
        //based on top left
        throwInCornerOffense = new[]
        {
            new Vector3(-3.85f, 0.35f, 6.70f),
            new Vector3(-4.10f, 0.35f, 4.50f),
            new Vector3(-5.05f, 0.35f, 3f)
        };

        throwInCornerDefense = new[]
        {
            new Vector3(-3.35f, 0.35f, 6.85f),
            new Vector3(-3.30f, 0.35f, 4.05f),
            new Vector3(-3.5f, 0.35f, 2.3f)
        };

        //based on 0 0 0 left
        throwInDefaultOffense = new[]
        {
            new Vector3(-4.05f, 0.35f, 1.50f),
            new Vector3(-4.10f, 0.35f, -1f),
            new Vector3(-5f, 0.35f, -3f)
        };

        throwInDefaultDefense = new[]
        {
            new Vector3(-5.10f, 0.35f, 2.15f),
            new Vector3(-3.70f, 0.35f, -0.50f),
            new Vector3(-3f, 0.35f, -2f)
        };

        //based on top left
        cornerOffense = new[]
        {
            new Vector3(0.85f, 0.35f, 6.3f),
            new Vector3(-2f, 0.35f, 5.00f),
            new Vector3(-5.50f, 0.35f, 4.00f)
        };

        cornerDefense = new[]
        {
            new Vector3(-3f, 0.35f, 6.70f),
            new Vector3(-0.80f, 0.35f, 5.70f),
            new Vector3(-3.40f, 0.35f, 4.70f)
        };

        throwIncornerKicker = new Vector3(-6.5f, 0.35f, 0f);        
        cornerKicker = new Vector3(-6.55f, 0.35f, 8.05f);

    }
}
