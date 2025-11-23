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
        Move,
        Pass,
        Shoot,
        Change,
        Dribble,
        Block,
        //BattleUIActions
        BattleUI_ClickEastButton,
        BattleUI_ClickWestButton,
        BattleUI_ClickNorthButton,
        BattleUI_ClickSouthButton,
        BattleUI_CloseMoveMenu,
        BattleUI_NextMove,
        BattleUI_OpenBattleMenu,
        BattleUI_CloseBattleMenu,
        BattleUI_BattleMenuShortcutPause
    }
}
