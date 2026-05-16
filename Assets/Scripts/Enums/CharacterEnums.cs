namespace Aremoreno.Enums.Character
{
    public enum Element 
    {
        Fire, 
        Ice, 
        Holy, 
        Evil, 
        Air, 
        Forest, 
        Earth, 
        Electric, 
        Water 
    }

    public enum Position 
    {
        FW, 
        MF, 
        DF, 
        GK 
    }

    public enum Gender 
    { 
        M, 
        F 
    }

    public enum TeamSide
    {
        Home,
        Away
    }

    public enum ControlType 
    { 
        LocalHuman, 
        RemoteHuman, 
        AI
    }

    public enum Stat 
    { 
        Hp, 
        Sp, 
        Kick, 
        Body, 
        Control, 
        Guard, 
        Speed, 
        Stamina,
        Technique,
        Luck,
        Courage 
    }

    public enum CharacterSize 
    { 
        Size_C0_S,
        Size_C1_M,
        Size_C2_L, 
        Size_C3_XL
    }

    public enum PortraitSize 
    { 
        Size_P0_XS = 0,
        Size_P1_S = 1,
        Size_P2_SM = 2,
        Size_P3_M = 3,
        Size_P4_ML = 4,
        Size_P5_L = 5,
        Size_P6_XL = 6  
    }

    public enum SpeechMessage
    {
        Direct,
        Win,
        Lose,
        Dribble,
        Block,
        Pass,
        Shoot,
        Nice
    }

    public enum FatigueState
    {
        Normal,
        Tired,
        Exhausted
    }

    public enum StatusEffect
    {
        None,
        Stunned,
        Tripping
    }

    public enum CharacterState 
    { 
        Idle,
        Move,
        Dash,
        Kick,
        Control,
        Dribble,
        Block
    }

    public enum CharacterDirection
    {
        Down = 0,
        Up = 1,
        Left = 2,
        Right = 3
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

    public enum AIDifficulty 
    { 
        Easy, 
        Normal, 
        Hard 
    }

    public enum AIState 
    { 
        Idle, 
        Kickoff, 
        KickoffPass, 
        ChaseBall, 
        Defend,
        Combo,        
        Dribble,
        Mark, 
        Support,
        Keeper, 
        Pass, 
        Shoot 
    }

    public enum BodyColorType
    {
        Light,
        Amber,
        Olive,
        Taupe,
        Bronze,
        Brown,
        Black,
        Blue,
        Green,
        Fur_Black,
        Fur_White,
        Porcelain,
        Amethyst,
        Purple
    }

    public enum EyeColorType
    {
        Blue,
        Green,
        Purple,
        Red,
        Orange,
        Yellow,
        Brown,
        Gray,
        Black,
        Hazel
    }

    public enum HairColorType
    {
        Orange,
        Ash,
        Platinum,
        White,
        Gray,
        Blonde,
        Sandy,
        Strawberry,
        Gold,
        Ginger,
        Carrot,
        Redhead,
        Red,
        Light_Brown,
        Chestnut,
        Dark_Brown,
        Dark_Gray,
        Black,
        Raven,
        Rose,
        Pink,
        Purple,
        Violet,
        Navy,
        Blue,
        Green
    }

    public enum HairStyle 
    {
        Short_Bedhead,
        Fade,
        Butterfly,
        Spiky,
        Ponytail1,
        Ponytail2,
        Bald
    }

}
