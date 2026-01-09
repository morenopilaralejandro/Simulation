using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Simulation.Enums.Character;

public class CharacterComponentSpeechBubble : MonoBehaviour
{
    private Character character;

    [SerializeField] private SpeechBubble speechBubble;

    public void Initialize(CharacterData characterData, Character character) 
    {
        this.character = character;
    }

    public void ShowSpeechBubble(SpeechMessage speechMessage)
    {
        speechBubble.ShowMessage(speechMessage, character);
    }

    public void HideSpeechBubble() 
    {
        speechBubble.HideImmediate(character);
    }

    private void OnEnable()
    {
        CharacterEvents.OnSpeechBubbleShown += HandleSpeechBubbleShown;
        CharacterEvents.OnSpeechBubbleHidden += HandleSpeechBubbleHidden;

        BattleEvents.OnShootPerformed += HandleShootPerformed;
    }

    private void OnDisable()
    {
        CharacterEvents.OnSpeechBubbleShown -= HandleSpeechBubbleShown;
        CharacterEvents.OnSpeechBubbleHidden -= HandleSpeechBubbleHidden;

        BattleEvents.OnShootPerformed -= HandleShootPerformed;
    }

    private void HandleSpeechBubbleShown(Character character)
    {
        if (this.character == character)
        {
            character.SetElementIndicatorEnabled(false);
        }
    }

    private void HandleSpeechBubbleHidden(Character character)
    {
        if (this.character == character)
        {
            character.SetElementIndicatorEnabled(true);
        }
    }

    private void HandleShootPerformed(Character character, bool isDirect)
    {
        if (this.character == character && isDirect)
            ShowSpeechBubble(SpeechMessage.Direct);
    }
}
