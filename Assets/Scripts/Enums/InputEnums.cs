namespace Simulation.Enums.Input 
{
    public enum ControlScheme
    {
        Traditional,
        Touch
    }

    public enum CustomAction
    {
        //BattleActions
        Battle_Move,
        Battle_Pass,
        Battle_Shoot,
        Battle_ChangeManual,
        Battle_ChangeAuto,
        Battle_Dribble,
        Battle_Block,
        //BattleUIActions
        BattleUI_ClickEastButton,
        BattleUI_ClickWestButton,
        BattleUI_ClickNorthButton,
        BattleUI_ClickSouthButton,
        BattleUI_CloseMoveMenu,
        BattleUI_NextMove,
        BattleUI_OpenBattleMenu,
        BattleUI_CloseBattleMenu,
        BattleUI_OpenDimensionMenu,
        BattleUI_CloseDimensionMenu,
        BattleUI_DimensionShortcutPause
    }
}
