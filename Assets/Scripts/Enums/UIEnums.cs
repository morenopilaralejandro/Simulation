namespace Aremoreno.Enums.UI
{
    public enum MenuTeamMode
    {
        Edit,
        Battle
    }

    public enum CharacterSelectorModePopulate
    {
        GetFromStorage,
        GetFromTeam,
        ExcludeFromTeam
    }

    public enum CharacterSelectorModeClick
    {
        SelectCharacter,
        OpenDetail
    }

    public enum MoveSelectorModePopulate
    {
        GetFromLearnset,
        GetFromLearned,
        GetFromLearnedExcludeEquiped
    }

    public enum MenuTeamState 
    { 
        Idle, 
        Swapping, 
        Replacing, 
        Closing 
    }

    public enum CharacterDetailState 
    { 
        Idle, 
        SwappingMove 
    }

}
