using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Item;
using Aremoreno.Enums.Move;
using Aremoreno.Enums.Kit;
using Aremoreno.Enums.Wings;

public static class ColorManager
{
    public static string ColorToHex(Color color, bool includeAlpha = false)
    {
        int r = Mathf.RoundToInt(color.r * 255f);
        int g = Mathf.RoundToInt(color.g * 255f);
        int b = Mathf.RoundToInt(color.b * 255f);
        int a = Mathf.RoundToInt(color.a * 255f);

        if (includeAlpha)
            return $"#{r:X2}{g:X2}{b:X2}{a:X2}";
        else
            return $"#{r:X2}{g:X2}{b:X2}";
    }

    private static readonly Dictionary<string, Color> teamIndicatorColors = new Dictionary<string, Color>()
    {
        {"ally-default", new Color(0.00f, 0.00f, 0.50f)},
        {"opponent-default", new Color(0.50f, 0.00f, 0.00f)},
        {"ally-highlight", new Color(0.40f, 0.40f, 1.00f)},
        {"opponent-highlight", new Color(1.00f, 0.40f, 0.40f)}
    };

    public static Color GetTeamIndicatorColor(TeamSide teamSide, bool highlight)
    {
        TeamSide userSide = BattleManager.Instance.GetUserSide();
        if (teamSide == userSide) 
        {
            return highlight ? teamIndicatorColors["ally-highlight"] : teamIndicatorColors["ally-default"];
        } else 
        {
            return highlight ? teamIndicatorColors["opponent-highlight"] : teamIndicatorColors["opponent-default"];
        }
    }

    private static readonly Dictionary<string, Color> teamIndicatorTextColors = new Dictionary<string, Color>()
    {
        {"ally",    Color.blue},
        {"opponent",     Color.red}
    };

    public static Color GetTeamIndicatorTextColor(TeamSide teamSide)
    {
        TeamSide userSide = BattleManager.Instance.GetUserSide();
        if (teamSide == userSide) 
        {
            return teamIndicatorTextColors["ally"];
        } else 
        {
            return teamIndicatorTextColors["opp"];
        }
    }

    private static readonly Dictionary<string, Color> duelOutcomeTextColors = new Dictionary<string, Color>()
    {
        {"win",     new Color(0.98f, 0.84f, 0.09f)},
        {"lose",    new Color(0.61f, 0.61f, 1.00f)}
    };

    public static Color GetDuelOutcomeTextColor(string duelOutcomeTextColor)
    {
        if(duelOutcomeTextColors.TryGetValue(duelOutcomeTextColor.ToLower(), out var color))
            return color;
        return duelOutcomeTextColors["win"];
    }

    private static readonly Dictionary<Category, Color> categoryColors = new Dictionary<Category, Color>()
    {
        {Category.Shoot,    new Color(0.906f, 0.420f, 0.482f, 1.0f)},
        {Category.Dribble,  new Color(0.290f, 0.420f, 0.839f, 1.0f)},
        {Category.Block,    new Color(0.224f, 0.710f, 0.160f, 1.0f)},
        {Category.Catch,    new Color(0.871f, 0.549f, 0.0f, 1.0f)}
    };

    public static Color GetCategoryColor(Category category)
    {
        if (categoryColors.TryGetValue(category, out Color color))
            return color;
        return Color.white;
    }

    private static readonly Dictionary<Element, Color> elementColors = new Dictionary<Element, Color>()
    {
        { Element.Fire,      new Color(1f, 0f, 0f, 1f) },           // Red
        { Element.Ice,       new Color(0.5f, 0.8f, 1f, 1f) },       // Light Blue
        { Element.Holy,      new Color(1f, 1f, 0f, 1f) },           // Yellow
        { Element.Evil,      new Color(0.8f, 0.6f, 0.8f, 1f) },     // Purple
        { Element.Air,       new Color(1f, 1f, 1f, 1f) },           // White
        { Element.Forest,    new Color(0.2f, 0.8f, 0.2f, 1f) },     // Green
        { Element.Earth,     new Color(0.6f, 0.4f, 0.2f, 1f) },     // Brown
        { Element.Electric,  new Color(1f, 0.92f, 0.016f, 1f) },    // Yellow Spark
        { Element.Water,     new Color(0.18f, 0.44f, 1f, 1f) }      // Blue
    };

    public static Color GetElementColor(Element element)
    {
        if (elementColors.TryGetValue(element, out Color color))
            return color;
        return Color.white;
    }

    private static readonly Dictionary<Position, Color> positionColors = new Dictionary<Position, Color>()
    {
        { Position.FW,  new Color(0.8549f, 0.0941f, 0.0941f, 1f) },
        { Position.MF,  new Color(30f/255f, 62f/255f, 186f/255f, 1f) },
        { Position.DF,  new Color(2f/255f, 122f/255f, 4f/255f, 1f) },
        { Position.GK,  new Color(226f/255f, 120f/255f, 0f/255f, 1f) }
    };

    public static Color GetPositionColor(Position position)
    {
        if (positionColors.TryGetValue(position, out Color color))
            return color;
        return Color.white;
    }

    private static readonly Dictionary<MessageType, Color> battleMessageColors =
        new Dictionary<MessageType, Color>()
    {
        { MessageType.Goal,         new Color(1f, 0.5948128f, 0.015686274f, 1f) },
        { MessageType.HalfTime,     new Color(1f, 1f, 1f, 1f) },
        { MessageType.FullTime,     new Color(1f, 1f, 1f, 1f) },
        { MessageType.TimeUp,       new Color(1f, 1f, 1f, 1f) },
        { MessageType.Foul,       new Color(1f, 1f, 1f, 1f) },
        { MessageType.Offside,    new Color(1f, 1f, 1f, 1f) }
    };

    public static Color GetBattleMessageColor(MessageType messageType)
    {
        return battleMessageColors.TryGetValue(messageType, out var color)
            ? color : Color.white;
    }

    private static readonly Dictionary<ItemSpriteColor, Color> itemSpriteColors =
        new Dictionary<ItemSpriteColor, Color>()
    {
        { ItemSpriteColor.Move_Fire,    new Color(1f, 0.5948128f, 0.015686274f, 1f) },
        { ItemSpriteColor.Spike_Red,    new Color(1f, 1f, 1f, 1f) }
    };

    public static Color GetItemSpriteColor(ItemSpriteColor itemSpriteColor)
    {
        return itemSpriteColors.TryGetValue(itemSpriteColor, out var color)
            ? color : Color.white;
    }

    private static readonly Dictionary<BodyColorType, Color> bodyColors =
        new Dictionary<BodyColorType, Color>()
    {
        { BodyColorType.Light,      new Color(0xF9/255f, 0xD5/255f, 0xBA/255f, 1f) },
        { BodyColorType.Amber,      new Color(0xFD/255f, 0xD0/255f, 0x82/255f, 1f) },
        { BodyColorType.Olive,      new Color(0xD3/255f, 0x8B/255f, 0x59/255f, 1f) },
        { BodyColorType.Taupe,      new Color(0xBA/255f, 0x84/255f, 0x54/255f, 1f) },
        { BodyColorType.Bronze,     new Color(0xAE/255f, 0x6B/255f, 0x3F/255f, 1f) },
        { BodyColorType.Brown,      new Color(0x9C/255f, 0x66/255f, 0x3E/255f, 1f) },
        { BodyColorType.Black,      new Color(0x60/255f, 0x34/255f, 0x29/255f, 1f) },

        { BodyColorType.Blue,       new Color(0xA9/255f, 0xC9/255f, 0xCA/255f, 1f) },
        { BodyColorType.Green,      new Color(0x39/255f, 0xAA/255f, 0x4E/255f, 1f) },

        { BodyColorType.Fur_Black,  new Color(0x1B/255f, 0x2C/255f, 0x36/255f, 1f) },
        { BodyColorType.Fur_White,  new Color(0xB8/255f, 0xBB/255f, 0xBC/255f, 1f) },

        { BodyColorType.Lpcr_Porcelain,  new Color(0xFA/255f, 0xEC/255f, 0xE7/255f, 1f) },
        { BodyColorType.Lpcr_Amethyst,   new Color(0xA4/255f, 0xB0/255f, 0xDC/255f, 1f) },
        { BodyColorType.Lpcr_Purple,     new Color(0x65/255f, 0x57/255f, 0x89/255f, 1f) }
    };

    public static Color GetBodyColor(BodyColorType bodyColorType)
    {
        return bodyColors.TryGetValue(bodyColorType, out var color)
            ? color : Color.white;
    }

    private static readonly Dictionary<HairColorType, Color> hairColors =
        new Dictionary<HairColorType, Color>()
    {
        { HairColorType.Orange,       new Color(0xE5/255f, 0x56/255f, 0x00/255f, 1f) },
        { HairColorType.Ash,          new Color(0xED/255f, 0xDF/255f, 0x95/255f, 1f) },
        { HairColorType.Platinum,     new Color(0xED/255f, 0xDF/255f, 0x95/255f, 1f) },

        { HairColorType.White,        new Color(1f, 1f, 1f, 1f) },
        { HairColorType.Gray,         new Color(0xAA/255f, 0xAA/255f, 0xAA/255f, 1f) },

        { HairColorType.Blonde,       new Color(0xFC/255f, 0xCF/255f, 0x56/255f, 1f) },
        { HairColorType.Sandy,        new Color(0xED/255f, 0xDC/255f, 0x7E/255f, 1f) },
        { HairColorType.Strawberry,   new Color(0xFA/255f, 0xF0/255f, 0x80/255f, 1f) },
        { HairColorType.Gold,         new Color(0xFF/255f, 0xE4/255f, 0x53/255f, 1f) },

        { HairColorType.Ginger,       new Color(0xFA/255f, 0xA3/255f, 0x01/255f, 1f) },
        { HairColorType.Carrot,       new Color(0xF6/255f, 0x87/255f, 0x64/255f, 1f) },

        { HairColorType.Redhead,      new Color(0xE2/255f, 0x14/255f, 0x14/255f, 1f) },
        { HairColorType.Red,          new Color(0xE2/255f, 0x14/255f, 0x14/255f, 1f) },

        { HairColorType.Light_Brown,  new Color(0xAE/255f, 0x68/255f, 0x2A/255f, 1f) },
        { HairColorType.Chestnut,     new Color(0xB6/255f, 0x55/255f, 0x0E/255f, 1f) },
        { HairColorType.Dark_Brown,   new Color(0x5F/255f, 0x1F/255f, 0x04/255f, 1f) },

        { HairColorType.Dark_Gray,    new Color(0x7C/255f, 0x7C/255f, 0x7C/255f, 1f) },
        { HairColorType.Black,        new Color(0x31/255f, 0x31/255f, 0x3E/255f, 1f) },
        { HairColorType.Raven,        new Color(0x0D/255f, 0x38/255f, 0x4D/255f, 1f) },

        { HairColorType.Rose,         new Color(0xFA/255f, 0xBB/255f, 0xC6/255f, 1f) },
        { HairColorType.Pink,         new Color(0xE9/255f, 0x76/255f, 0xC4/255f, 1f) },

        { HairColorType.Purple,       new Color(0xA9/255f, 0x66/255f, 0xDD/255f, 1f) },
        { HairColorType.Violet,       new Color(0x56/255f, 0x62/255f, 0xF3/255f, 1f) },

        { HairColorType.Navy,         new Color(0x3C/255f, 0x49/255f, 0xAD/255f, 1f) },
        { HairColorType.Blue,         new Color(0x00/255f, 0x74/255f, 0xCB/255f, 1f) },

        { HairColorType.Green,        new Color(0x00/255f, 0x7C/255f, 0x00/255f, 1f) }
    };

    public static Color GetHairColor(HairColorType hairColorType)
    {
        return hairColors.TryGetValue(hairColorType, out var color)
            ? color : Color.white;
    }

    private static readonly Dictionary<WingsColorType, Color> wingsColors =
        new Dictionary<WingsColorType, Color>()
    {
        { WingsColorType.Amber,         new Color(0xFB/255f, 0xE7/255f, 0xA4/255f, 1f) },
        { WingsColorType.Ash,           new Color(0xFF/255f, 0xF1/255f, 0xC1/255f, 1f) },
        { WingsColorType.Black,         new Color(0x4A/255f, 0x50/255f, 0x57/255f, 1f) },
        { WingsColorType.Blonde,        new Color(0xFC/255f, 0xCF/255f, 0x56/255f, 1f) },
        { WingsColorType.Blue,          new Color(0x1E/255f, 0x85/255f, 0xEF/255f, 1f) },
        { WingsColorType.Bluegray,      new Color(0x79/255f, 0x97/255f, 0x9D/255f, 1f) },
        { WingsColorType.Bright_Green,  new Color(0x99/255f, 0xD2/255f, 0x48/255f, 1f) },
        { WingsColorType.Bronze,        new Color(0xD3/255f, 0x8B/255f, 0x59/255f, 1f) },
        { WingsColorType.Brown,         new Color(0x9C/255f, 0x66/255f, 0x3E/255f, 1f) },
        { WingsColorType.Carrot,        new Color(0xFF/255f, 0xB3/255f, 0x9C/255f, 1f) },
        { WingsColorType.Ceramic,       new Color(0x7D/255f, 0x60/255f, 0x4D/255f, 1f) },
        { WingsColorType.Chestnut,      new Color(0xD2/255f, 0x81/255f, 0x02/255f, 1f) },
        { WingsColorType.Dark_Brown,    new Color(0x79/255f, 0x28/255f, 0x06/255f, 1f) },
        { WingsColorType.Dark_Gray,     new Color(0x7C/255f, 0x7C/255f, 0x7C/255f, 1f) },
        { WingsColorType.Dark_Green,    new Color(0x50/255f, 0x9E/255f, 0x59/255f, 1f) },
        { WingsColorType.Dragonfly,     new Color(0x83/255f, 0xE5/255f, 0xF7/255f, 1f) },
        { WingsColorType.Forest,        new Color(0x1B/255f, 0x55/255f, 0x02/255f, 1f) },

        { WingsColorType.Fur_Black,     new Color(0x15/255f, 0x42/255f, 0x59/255f, 1f) },
        { WingsColorType.Fur_Brown,     new Color(0x62/255f, 0x41/255f, 0x35/255f, 1f) },
        { WingsColorType.Fur_Copper,    new Color(0xCC/255f, 0x69/255f, 0x01/255f, 1f) },
        { WingsColorType.Fur_Gold,      new Color(0xFC/255f, 0xCF/255f, 0x56/255f, 1f) },
        { WingsColorType.Fur_Grey,      new Color(0x90/255f, 0x96/255f, 0x99/255f, 1f) },
        { WingsColorType.Fur_Tan,       new Color(0xB8/255f, 0x87/255f, 0x51/255f, 1f) },
        { WingsColorType.Fur_White,     new Color(0xB8/255f, 0xBB/255f, 0xBC/255f, 1f) },

        { WingsColorType.Ginger,        new Color(0xFA/255f, 0xA3/255f, 0x01/255f, 1f) },
        { WingsColorType.Gold,          new Color(0xFF/255f, 0xE4/255f, 0x53/255f, 1f) },
        { WingsColorType.Gray,          new Color(0xD9/255f, 0xD9/255f, 0xD9/255f, 1f) },
        { WingsColorType.Green,         new Color(0x00/255f, 0xA7/255f, 0x00/255f, 1f) },
        { WingsColorType.Iron,          new Color(0x34/255f, 0x30/255f, 0x43/255f, 1f) },
        { WingsColorType.Lavender,      new Color(0xFB/255f, 0xEC/255f, 0xE6/255f, 1f) },
        { WingsColorType.Light,         new Color(0xFA/255f, 0xEC/255f, 0xE7/255f, 1f) },
        { WingsColorType.Light_Brown,   new Color(0xC8/255f, 0x8D/255f, 0x58/255f, 1f) },
        { WingsColorType.Lunar,         new Color(0xAB/255f, 0xEA/255f, 0xA9/255f, 1f) },
        { WingsColorType.Maroon,        new Color(0xAE/255f, 0x42/255f, 0x4A/255f, 1f) },
        { WingsColorType.Monarch,       new Color(0xE6/255f, 0x91/255f, 0x10/255f, 1f) },
        { WingsColorType.Navy,          new Color(0x46/255f, 0x6A/255f, 0xC9/255f, 1f) },
        { WingsColorType.Olive,         new Color(0xE4/255f, 0xA4/255f, 0x7C/255f, 1f) },
        { WingsColorType.Orange,        new Color(0xE5/255f, 0x56/255f, 0x00/255f, 1f) },
        { WingsColorType.Pale_Green,    new Color(0xAD/255f, 0xCC/255f, 0xA6/255f, 1f) },
        { WingsColorType.Pink,          new Color(0xEA/255f, 0x95/255f, 0xD5/255f, 1f) },
        { WingsColorType.Pixie,         new Color(0x25/255f, 0xD3/255f, 0xF3/255f, 1f) },
        { WingsColorType.Platinum,      new Color(0xF6/255f, 0xF6/255f, 0xF3/255f, 1f) },
        { WingsColorType.Purple,        new Color(0xA9/255f, 0x66/255f, 0xDD/255f, 1f) },
        { WingsColorType.Raven,         new Color(0x1A/255f, 0x53/255f, 0x69/255f, 1f) },
        { WingsColorType.Red,           new Color(0xF1/255f, 0x58/255f, 0x3A/255f, 1f) },
        { WingsColorType.Redhead,       new Color(0xE7/255f, 0x47/255f, 0x16/255f, 1f) },
        { WingsColorType.Rose,          new Color(0xFA/255f, 0xE1/255f, 0xE5/255f, 1f) },
        { WingsColorType.Sandy,         new Color(0xF6/255f, 0xF6/255f, 0xC2/255f, 1f) },
        { WingsColorType.Silver,        new Color(0x81/255f, 0x8B/255f, 0x8B/255f, 1f) },
        { WingsColorType.Sky,           new Color(0xB9/255f, 0xE3/255f, 0xF7/255f, 1f) },
        { WingsColorType.Strawberry,    new Color(0xF6/255f, 0xF6/255f, 0xC2/255f, 1f) },
        { WingsColorType.Taupe,         new Color(0xC7/255f, 0x93/255f, 0x5F/255f, 1f) },
        { WingsColorType.Teal,          new Color(0x2D/255f, 0xAC/255f, 0xD9/255f, 1f) },
        { WingsColorType.Violet,        new Color(0x57/255f, 0x92/255f, 0xF2/255f, 1f) },
        { WingsColorType.White,         Color.white },
        { WingsColorType.Yellow,        new Color(0xF3/255f, 0xC0/255f, 0x3F/255f, 1f) },
        { WingsColorType.Zombie_Green,  new Color(0xF2/255f, 0xF0/255f, 0xC4/255f, 1f) }
    };

    public static Color GetWingsColor(WingsColorType wingsColorType)
    {
        return wingsColors.TryGetValue(wingsColorType, out var color)
            ? color
            : Color.white;
    }

    /*
    private static readonly Dictionary<EyeColorType, Color> eyeColors =
        new Dictionary<EyeColorType, Color>()
    {
        { EyeColorType.Blue,    new Color(0x57/255f, 0xCE/255f, 0xE4/255f, 1f) },
        { EyeColorType.Green,   new Color(0x84/255f, 0xEC/255f, 0x50/255f, 1f) },
        { EyeColorType.Purple,  new Color(0xEB/255f, 0xA0/255f, 0xE0/255f, 1f) },
        { EyeColorType.Red,     new Color(0xFF/255f, 0x3D/255f, 0x62/255f, 1f) },
        { EyeColorType.Orange,  new Color(0xEA/255f, 0x9B/255f, 0x71/255f, 1f) },
        { EyeColorType.Yellow,  new Color(0xFE/255f, 0xDF/255f, 0x47/255f, 1f) },
        { EyeColorType.Brown,   new Color(0x7E/255f, 0x4E/255f, 0x20/255f, 1f) },
        { EyeColorType.Gray,    new Color(0xAD/255f, 0xA1/255f, 0x8F/255f, 1f) },
        { EyeColorType.Lpcr_Black,   new Color(0x81/255f, 0x8E/255f, 0x97/255f, 1f) },
        { EyeColorType.Lpcr_Hazel,   new Color(0xA4/255f, 0xDD/255f, 0xDB/255f, 1f) }
    };

    private static readonly Dictionary<HairColorType, Color> hairColors =
        new Dictionary<HairColorType, Color>()
    {
        { HairColorType.Orange,       new Color(0xE5/255f, 0x56/255f, 0x00/255f, 1f) },
        { HairColorType.Ash,          new Color(0xED/255f, 0xDF/255f, 0x95/255f, 1f) },
        { HairColorType.Platinum,     new Color(0xED/255f, 0xDF/255f, 0x95/255f, 1f) },

        { HairColorType.White,        new Color(1f, 1f, 1f, 1f) },
        { HairColorType.Gray,         new Color(0xAA/255f, 0xAA/255f, 0xAA/255f, 1f) },

        { HairColorType.Blonde,       new Color(0xFC/255f, 0xCF/255f, 0x56/255f, 1f) },
        { HairColorType.Sandy,        new Color(0xED/255f, 0xDC/255f, 0x7E/255f, 1f) },
        { HairColorType.Strawberry,   new Color(0xFA/255f, 0xF0/255f, 0x80/255f, 1f) },
        { HairColorType.Gold,         new Color(0xFF/255f, 0xE4/255f, 0x53/255f, 1f) },

        { HairColorType.Ginger,       new Color(0xFA/255f, 0xA3/255f, 0x01/255f, 1f) },
        { HairColorType.Carrot,       new Color(0xF6/255f, 0x87/255f, 0x64/255f, 1f) },

        { HairColorType.Redhead,      new Color(0xE2/255f, 0x14/255f, 0x14/255f, 1f) },
        { HairColorType.Red,          new Color(0xE2/255f, 0x14/255f, 0x14/255f, 1f) },

        { HairColorType.Light_Brown,  new Color(0xAE/255f, 0x68/255f, 0x2A/255f, 1f) },
        { HairColorType.Chestnut,     new Color(0xB6/255f, 0x55/255f, 0x0E/255f, 1f) },
        { HairColorType.Dark_Brown,   new Color(0x5F/255f, 0x1F/255f, 0x04/255f, 1f) },

        { HairColorType.Dark_Gray,    new Color(0x7C/255f, 0x7C/255f, 0x7C/255f, 1f) },
        { HairColorType.Black,        new Color(0x31/255f, 0x31/255f, 0x3E/255f, 1f) },
        { HairColorType.Raven,        new Color(0x0D/255f, 0x38/255f, 0x4D/255f, 1f) },

        { HairColorType.Rose,         new Color(0xFA/255f, 0xBB/255f, 0xC6/255f, 1f) },
        { HairColorType.Pink,         new Color(0xE9/255f, 0x76/255f, 0xC4/255f, 1f) },

        { HairColorType.Purple,       new Color(0xA9/255f, 0x66/255f, 0xDD/255f, 1f) },
        { HairColorType.Violet,       new Color(0x56/255f, 0x62/255f, 0xF3/255f, 1f) },

        { HairColorType.Navy,         new Color(0x3C/255f, 0x49/255f, 0xAD/255f, 1f) },
        { HairColorType.Blue,         new Color(0x00/255f, 0x74/255f, 0xCB/255f, 1f) },

        { HairColorType.Green,        new Color(0x00/255f, 0x7C/255f, 0x00/255f, 1f) }
    };

    private static readonly Dictionary<SockColor, Color> sockColors =
        new Dictionary<SockColor, Color>()
    {
        { SockColor.Leather,    new Color(0x9A/255f, 0x6F/255f, 0x37/255f, 1f) },
        { SockColor.Black,      new Color(0x4A/255f, 0x50/255f, 0x57/255f, 1f) },

        { SockColor.Blue,       new Color(0x61/255f, 0xA0/255f, 0xEF/255f, 1f) },
        { SockColor.Bluegray,   new Color(0x79/255f, 0x97/255f, 0x9D/255f, 1f) },

        { SockColor.Brown,      new Color(0x99/255f, 0x6B/255f, 0x4A/255f, 1f) },
        { SockColor.Charcoal,   new Color(0x6E/255f, 0x76/255f, 0x75/255f, 1f) },

        { SockColor.Forest,     new Color(0x1B/255f, 0x55/255f, 0x02/255f, 1f) },
        { SockColor.Gray,       new Color(0xA2/255f, 0xA0/255f, 0xA4/255f, 1f) },

        { SockColor.Green,      new Color(0x64/255f, 0xA4/255f, 0x2C/255f, 1f) },
        { SockColor.Lavender,   new Color(0xD0/255f, 0x85/255f, 0xED/255f, 1f) },

        { SockColor.Maroon,     new Color(0xAE/255f, 0x42/255f, 0x4A/255f, 1f) },
        { SockColor.Navy,       new Color(0x46/255f, 0x6A/255f, 0xC9/255f, 1f) },

        { SockColor.Orange,     new Color(0xFF/255f, 0xA7/255f, 0x49/255f, 1f) },
        { SockColor.Pink,       new Color(0xE0/255f, 0x80/255f, 0x80/255f, 1f) },

        { SockColor.Purple,     new Color(0x81/255f, 0x30/255f, 0x89/255f, 1f) },
        { SockColor.Red,        new Color(0xCD/255f, 0x24/255f, 0x29/255f, 1f) },

        { SockColor.Rose,       new Color(0xB0/255f, 0x5F/255f, 0x3C/255f, 1f) },
        { SockColor.Sky,        Color.white },

        { SockColor.Slate,      new Color(0xE5/255f, 0xE6/255f, 0xC7/255f, 1f) },
        { SockColor.Tan,        new Color(0xCF/255f, 0xC5/255f, 0x87/255f, 1f) },

        { SockColor.Teal,       new Color(0x00/255f, 0xCF/255f, 0xDF/255f, 1f) },
        { SockColor.Walnut,     new Color(0xA1/255f, 0x7C/255f, 0x50/255f, 1f) },

        { SockColor.White,      Color.white },
        { SockColor.Yellow,     new Color(0xFF/255f, 0xE3/255f, 0x60/255f, 1f) }
    };

    private static readonly Dictionary<SpikesColor, Color> spikesColors =
        new Dictionary<SpikesColor, Color>()
    {
        { SpikesColor.Black,      new Color(0x2A/255f, 0x30/255f, 0x34/255f, 1f) },
        { SpikesColor.Blue,       new Color(0x46/255f, 0x6A/255f, 0xC9/255f, 1f) },
        { SpikesColor.Bluegray,   new Color(0x55/255f, 0x7E/255f, 0x85/255f, 1f) },

        { SpikesColor.Brown,      new Color(0x74/255f, 0x4B/255f, 0x30/255f, 1f) },
        { SpikesColor.Charcoal,   new Color(0x4A/255f, 0x50/255f, 0x57/255f, 1f) },

        { SpikesColor.Forest,     new Color(0x13/255f, 0x45/255f, 0x07/255f, 1f) },
        { SpikesColor.Gray,       new Color(0x79/255f, 0x75/255f, 0x80/255f, 1f) },

        { SpikesColor.Green,      new Color(0x2F/255f, 0x81/255f, 0x36/255f, 1f) },
        { SpikesColor.Lavender,   new Color(0xA9/255f, 0x66/255f, 0xDD/255f, 1f) },

        { SpikesColor.Leather,    new Color(0x75/255f, 0x50/255f, 0x2D/255f, 1f) },
        { SpikesColor.Maroon,     new Color(0x83/255f, 0x21/255f, 0x21/255f, 1f) },

        { SpikesColor.Navy,       new Color(0x3C/255f, 0x49/255f, 0xAD/255f, 1f) },
        { SpikesColor.Orange,     new Color(0xEF/255f, 0x7E/255f, 0x19/255f, 1f) },

        { SpikesColor.Pink,       new Color(0xC3/255f, 0x60/255f, 0x72/255f, 1f) },
        { SpikesColor.Purple,     new Color(0x62/255f, 0x1E/255f, 0x78/255f, 1f) },

        { SpikesColor.Red,        new Color(0xAB/255f, 0x1E/255f, 0x1E/255f, 1f) },
        { SpikesColor.Rose,       new Color(0x8A/255f, 0x3D/255f, 0x28/255f, 1f) },

        { SpikesColor.Sky,        new Color(0xC6/255f, 0xEE/255f, 0xFD/255f, 1f) },
        { SpikesColor.Slate,      new Color(0xB3/255f, 0xAF/255f, 0xA1/255f, 1f) },

        { SpikesColor.Tan,        new Color(0xB7/255f, 0x99/255f, 0x6A/255f, 1f) },
        { SpikesColor.Teal,       new Color(0x00/255f, 0x98/255f, 0xB2/255f, 1f) },

        { SpikesColor.Walnut,     new Color(0x99/255f, 0x6B/255f, 0x4A/255f, 1f) },
        { SpikesColor.White,      Color.white },

        { SpikesColor.Yellow,     new Color(0xF3/255f, 0xC0/255f, 0x3F/255f, 1f) },

        { SpikesColor.Steel,      new Color(0xC4/255f, 0xB5/255f, 0x9F/255f, 1f) },
        { SpikesColor.Iron,       new Color(0x48/255f, 0x41/255f, 0x52/255f, 1f) },
        { SpikesColor.Ceramic,    new Color(0xBA/255f, 0x90/255f, 0x69/255f, 1f) },

        { SpikesColor.Brass,      new Color(0xFD/255f, 0xD0/255f, 0x82/255f, 1f) },
        { SpikesColor.Copper,     new Color(0xEC/255f, 0x85/255f, 0x5C/255f, 1f) },

        { SpikesColor.Bronze,     new Color(0xE7/255f, 0xA8/255f, 0x20/255f, 1f) },
        { SpikesColor.Silver,     new Color(0xD6/255f, 0xE1/255f, 0xD3/255f, 1f) },

        { SpikesColor.Gold,       new Color(0xFF/255f, 0xC9/255f, 0x5A/255f, 1f) }
    };

    private static readonly Dictionary<GlovesColor, Color> glovesColors =
        new Dictionary<GlovesColor, Color>()
    {
        { GlovesColor.Ceramic,    new Color(0xBA/255f, 0x90/255f, 0x69/255f, 1f) },
        { GlovesColor.Brass,      new Color(0xFD/255f, 0xD0/255f, 0x82/255f, 1f) },
        { GlovesColor.Copper,     new Color(0xEC/255f, 0x85/255f, 0x5C/255f, 1f) },
        { GlovesColor.Bronze,     new Color(0xE7/255f, 0xA8/255f, 0x20/255f, 1f) },

        { GlovesColor.Iron,       new Color(0x48/255f, 0x41/255f, 0x52/255f, 1f) },
        { GlovesColor.Steel,      new Color(0xC4/255f, 0xB5/255f, 0x9F/255f, 1f) },

        { GlovesColor.Silver,     new Color(0xD6/255f, 0xE1/255f, 0xD3/255f, 1f) },
        { GlovesColor.Gold,       new Color(0xFF/255f, 0xC9/255f, 0x5A/255f, 1f) },

        { GlovesColor.Cloth_Brown,      new Color(0x99/255f, 0x6B/255f, 0x4A/255f, 1f) },
        { GlovesColor.Cloth_Leather,    new Color(0x9A/255f, 0x6F/255f, 0x37/255f, 1f) },
        { GlovesColor.Cloth_Walnut,     new Color(0xA1/255f, 0x7C/255f, 0x50/255f, 1f) },

        { GlovesColor.Cloth_Yellow,     new Color(0xFF/255f, 0xE3/255f, 0x60/255f, 1f) },
        { GlovesColor.Cloth_Tan,        new Color(0xCF/255f, 0xC5/255f, 0x87/255f, 1f) },

        { GlovesColor.Cloth_Orange,     new Color(0xFF/255f, 0xA7/255f, 0x49/255f, 1f) },
        { GlovesColor.Cloth_Rose,       new Color(0xB0/255f, 0x5F/255f, 0x3C/255f, 1f) },

        { GlovesColor.Cloth_Maroon,     new Color(0xAE/255f, 0x42/255f, 0x4A/255f, 1f) },
        { GlovesColor.Cloth_Red,        new Color(0xCD/255f, 0x24/255f, 0x29/255f, 1f) },

        { GlovesColor.Cloth_Pink,       new Color(0xE0/255f, 0x80/255f, 0x80/255f, 1f) },
        { GlovesColor.Cloth_Lavender,   new Color(0xD0/255f, 0x85/255f, 0xED/255f, 1f) },

        { GlovesColor.Cloth_Purple,     new Color(0x81/255f, 0x30/255f, 0x89/255f, 1f) },
        { GlovesColor.Cloth_Blue,       new Color(0x61/255f, 0xA0/255f, 0xEF/255f, 1f) },

        { GlovesColor.Cloth_Navy,       new Color(0x46/255f, 0x6A/255f, 0xC9/255f, 1f) },
        { GlovesColor.Cloth_Teal,       new Color(0x00/255f, 0xCF/255f, 0xDF/255f, 1f) },

        { GlovesColor.Cloth_Bluegray,   new Color(0x79/255f, 0x97/255f, 0x9D/255f, 1f) },
        { GlovesColor.Cloth_Forest,     new Color(0x1B/255f, 0x55/255f, 0x02/255f, 1f) },

        { GlovesColor.Cloth_Green,      new Color(0x64/255f, 0xA4/255f, 0x2C/255f, 1f) },
        { GlovesColor.Cloth_White,      Color.white },

        { GlovesColor.Cloth_Sky,        new Color(0xC6/255f, 0xEE/255f, 0xFD/255f, 1f) },
        { GlovesColor.Cloth_Slate,      new Color(0xE5/255f, 0xE6/255f, 0xC7/255f, 1f) },

        { GlovesColor.Cloth_Gray,       new Color(0xA2/255f, 0xA0/255f, 0xA4/255f, 1f) },
        { GlovesColor.Cloth_Black,      new Color(0x4A/255f, 0x50/255f, 0x57/255f, 1f) },

        { GlovesColor.Cloth_Charcoal,   new Color(0x6E/255f, 0x76/255f, 0x75/255f, 1f) }
    };

    */
}
