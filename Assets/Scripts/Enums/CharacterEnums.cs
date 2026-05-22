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
        Male,
        Female
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
        Lpcr_Porcelain,
        Lpcr_Amethyst,
        Lpcr_Purple
    }
    // replace . with _ converting casting from string to enum

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
        Lpcr_Black,
        Lpcr_Hazel
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
        Bedhead,
        Spiked_beehive,
        Bob,
        Long_center_part,
        Spiked_porcupine,
        Long_straight,
        Spiked2,
        Plain,
        Curtains_long,
        Loose,
        Messy3,
        Sara,
        Xlong,
        Lob,
        Relm_Short
    }

}
