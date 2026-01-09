using UnityEngine;
using Simulation.Enums.Character;

public class CharacterComponentModel : MonoBehaviour
{
    private Character character;

    [SerializeField] private Transform model;
    public Transform Model => model;

    public void Initialize(CharacterData characterData, Character character)
    {
        this.character = character;
    }
}
