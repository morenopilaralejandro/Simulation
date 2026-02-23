using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Simulation.Enums.Character;
using Simulation.Enums.SpriteLayer;
using Simulation.Enums.Kit;
using Simulation.Enums.Move;
using Simulation.Enums.Duel;
using Simulation.Enums.Battle;
using Simulation.Enums.Localization;
using Simulation.Enums.World;

public class PlayerWorldEntity : MonoBehaviour
{
    #region Components
    [SerializeField] private Character character;
    //[SerializeField] private CharacterComponentAppearanceWorld appearanceWorldComponent;
    #endregion

    #region Initialize
    public void Initialize(CharacterData characterData, Character character = null)
    {
        if (character != null) 
        {
            this.character = character;
        } else 
        {
            //this.character = new Character(characterData); from data
        }

        //appearanceBattleComponent.Initialize(this);

    }

    #endregion

    #region API Character
    
    #endregion

}
