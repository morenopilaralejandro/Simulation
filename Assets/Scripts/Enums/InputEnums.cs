namespace Aremoreno.Enums.Input 
{
    public enum ControlScheme
    {
        Traditional,
        Touch
    }

    public enum InputDeviceType
    {
        KeyboardMouse,
        Gamepad,
        Touch
    }

    public enum DirectionalInputMode
    {
        Joystick,
        Dpad,
        Both
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
        BattleUI_DimensionShortcutPause,
        BattleUI_OpenTeamMenu,
        BattleUI_CloseTeamMenu,
        BattleUI_DeadBallConfirm,
        //WorldActions
        World_Move,
        World_Run,
        World_Interact,
        World_Submit,
        World_Cancel,
        World_OpenSideMenu,
        World_CloseSideMenu,
        World_OpenPauseMenu,
        World_ClosePauseMenu,
        //DialogActions
        Dialog_Submit,
        Dialog_Cancel,
        //NavigationActions
        Navigation_Back,
        Navigation_ShortcutTeamBattleType,
        Navigation_ShortcutTeamActive,
        Navigation_ShortcutTeamActions,
        Navigation_ShortcutTeamCharacterSummary,
        Navigation_ShortcutTeamCharacterMove,
        Navigation_ShortcutTeamCharacterReplace
    }
}
