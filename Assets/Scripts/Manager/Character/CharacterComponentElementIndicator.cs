using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Aremoreno.Enums.Character;

public class CharacterComponentElementIndicator : MonoBehaviour
{
    private Character character;
    private CharacterEntityBattle characterEntityBattle;

    [SerializeField] private SpriteRenderer elementRenderer;

    public void Initialize(CharacterData characterData, CharacterEntityBattle characterEntityBattle) 
    {
        this.characterEntityBattle = characterEntityBattle;
        this.character = characterEntityBattle.Character;
        elementRenderer.sprite = IconManager.Instance.Element.GetIcon(character.Element);
    }

    public void SetEnabled(bool enabled)
    {
        elementRenderer.enabled = enabled;
    }

    public void SetActive(bool active) 
    {
        elementRenderer.gameObject.SetActive(active);
    }

    private void OnEnable()
    {
        CharacterEvents.OnSpeechBubbleShown += HandleSpeechBubbleShown;
        CharacterEvents.OnSpeechBubbleHidden += HandleSpeechBubbleHidden;
    }

    private void OnDisable()
    {
        CharacterEvents.OnSpeechBubbleShown -= HandleSpeechBubbleShown;
        CharacterEvents.OnSpeechBubbleHidden -= HandleSpeechBubbleHidden;
    }

    private void HandleSpeechBubbleShown(Character character)
    {
        if (this.character == character)
        {
            characterEntityBattle.SetElementIndicatorEnabled(false);
        }
    }

    private void HandleSpeechBubbleHidden(Character character)
    {
        if (this.character == character)
        {
            characterEntityBattle.SetElementIndicatorEnabled(true);
        }
    }


}
