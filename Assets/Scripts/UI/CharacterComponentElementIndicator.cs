using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Simulation.Enums.Character;

public class CharacterComponentElementIndicator : MonoBehaviour
{
    private Character character;

    [SerializeField] private SpriteRenderer elementRenderer;

    public void Initialize(CharacterData characterData, Character character) 
    {
        this.character = character;
        elementRenderer.sprite = IconManager.Instance.Element.GetIcon(characterData.Element);
    }

    public void SetEnabled(bool enabled)
    {
        elementRenderer.enabled = enabled;
    }

    public void SetActive(bool active) 
    {
        elementRenderer.gameObject.SetActive(active);
    }

}
