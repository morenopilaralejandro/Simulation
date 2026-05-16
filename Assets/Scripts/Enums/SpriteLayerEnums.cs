namespace Aremoreno.Enums.SpriteLayer
{
    public enum CharacterSpriteLayer
    {
        Aura,
        Armor,
        Hair,
        Body,
        EyeBase,
        EyeIris,
        KitBase,
        KitDetail,
        KitShocks,
        Gloves,
        Spikes
    }

    public enum CharacterDirection
    {
        Up = 0,
        Left = 1,
        Down = 2,
        Right = 3
    }

    public enum CharacterAnimationState
    {
        Idle = 0,
        Walk = 1,
        Run = 2,
        Jump = 3,
        Combat = 4,
        Sit = 5,
        Emote = 6,
        Slash = 7,
        Backslash1H = 8,
        Spellcast = 9,
        Hurt = 10
    }
}
