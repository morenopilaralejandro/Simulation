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
        Afro,
        Bald,
        Balding,
        Bangs_Bun,
        Bangslong2,
        Bangslong,
        Bangsshort,
        Bedhead,
        Bob_Side_Part,
        Bob,
        Braid2,
        Braid,
        Bunches,
        Buzzcut,
        Cornrows,
        Cowlick,
        Cowlick_Tall,
        Curly_Long,
        Curly_Short,
        Curly_Short_2,
        Curtain,
        Curtains_Long,
        Dreadlocks_Long,
        Dreadlocks_Short,
        Flat_Top_Fade,
        Flat_Top_Straight,
        Halfmessy,
        Half_Up,
        High_And_Tight,
        High_Ponytail,
        Idol,
        Jewfro,
        Large_Curls,
        Large_Curls_Xlong,
        Lob,
        Long,
        Long_Band,
        Long_Center_Part,
        Longhawk,
        Long_Messy,
        Long_Messy2,
        Long_Straight,
        Long_Tied,
        Long_Topknot,
        Long_Topknot_2,
        Loose,
        Messy1,
        Messy2,
        Messy3,
        Mop,
        Natural,
        Page,
        Page2,
        Parted,
        Parted_2,
        Parted_3,
        Pigtails,
        Pigtails_Bangs,
        Pixie,
        Plain,
        Ponytail,
        Ponytail2,
        Princess,
        Relm_Short,
        Relm_With_Ponytail,
        Relm_Xlong,
        Sara,
        Shorthawk,
        Shoulderl,
        Shoulderr,
        Side_Parted_With_Bangs,
        Side_Parted_With_Bangs_2,
        Side_Swoop,
        Single,
        Spiked,
        Spiked2,
        Spiked_Beehive,
        Spiked_Liberty,
        Spiked_Liberty2,
        Spiked_Porcupine,
        Swoop,
        Twists_Fade,
        Twists_Straight,
        Unkempt,
        Wavy,
        Xlong_Wavy,
        Xlong
    }

}
