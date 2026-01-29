using System.Collections.Generic;
using UnityEngine;

public class OffsideSnapshot
{
    public Team attackingTeam;
    public bool attacksPositiveZ;
    public float ballZ;
    public float offsideLineZ;
    public bool isActive;

    public readonly HashSet<Character> offsideCandidates = new(11);
}
