namespace Aremoreno.Enums.Animation
{
    //character
    public enum CharacterAnimationEntryDirection
    {
        FourDir,
        DownOnly
    }

    public enum AnimationPriority
    {
        Low = 0,
        Medium = 1,
        High = 2,
        Critical = 3
    }

    public enum CharacterAnimationState
    {
        Idle = 0,
        Walk = 1,
        Run = 2,
        Jump = 3,
        Combat = 4,
        Emote = 5,
        Slash = 6,
        Backslash1H = 7,
        Spellcast = 8,
        Hurt = 9
    }

    public enum CharacterDirection
    {
        Down = 0,
        Up = 1,
        Left = 2,
        Right = 3
    }

    public enum AnimationFacingMode
    {
        Transform,          // uses model.forward
        Formation,          // uses FormationCoord default
        ActionOverride,     // uses stored action direction (vector2 as parameter)
        DownOnly            // like hurt
    }
}
