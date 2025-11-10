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
        BattleUI_CloseMoveMenu,
        BattleUI_NextMove
    }
}
