using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Simulation.Enums.Character;

public class CharacterComponentSpeechBubble : MonoBehaviour
{
    private Character character;

    [SerializeField] private SpeechBubble speechBubble;

    public void Initialize(Character character) 
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
        BattleEvents.OnShootPerformed += HandleShootPerformed;
    }

    private void OnDisable()
    {
        BattleEvents.OnShootPerformed -= HandleShootPerformed;
    }

    private void HandleShootPerformed(CharacterEntityBattle characterEntityBattle, bool isDirect)
    {
        if (this.character == characterEntityBattle.Character && isDirect)
            ShowSpeechBubble(SpeechMessage.Direct);
    }
}
