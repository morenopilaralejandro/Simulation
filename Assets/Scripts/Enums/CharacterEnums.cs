namespace Simulation.Enums.Character
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
        S, 
        M, 
        L, 
        XL
    }

    public enum PortraitSize 
    { 
        XS, 
        S, 
        SM, 
        M, 
        ML, 
        L, 
        XL    
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
        White,
        Pale,
        Generic,
        Tanned,
        Asian,
        Hispanic,
        Indian,
        Arab,
        African,
        Black,
        Green
    }

    public enum EyeColorType
    {
        Brown,
        Blue,
        Green,
        Gray,
        Red,
        Purple,
        Gold,
        Silver,
        Black,
        White,
        Pink
    }

    public enum HairColorType
    {
        Black,
        Blonde,
        Blue,
        Brown,
        Green,
        Orange,
        Pink,
        Purple,
        Red,
        White
    }

    public enum HairStyle 
    {
        Fade,
        Butterfly,
        Spiky,
        Ponytail1,
        Ponytail2,
        Bald
    }

}
