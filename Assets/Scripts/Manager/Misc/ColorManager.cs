using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Battle;
using Simulation.Enums.Character;
using Simulation.Enums.Move;
using Simulation.Enums.Kit;

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

    private static readonly Dictionary<KitColor, Color> kitColors = new Dictionary<KitColor, Color>()
    {
        { KitColor.Red,        new Color(218f/255f, 24f/255f, 24f/255f, 1f) },
        { KitColor.Red_Light,  new Color(255f/255f, 102f/255f, 102f/255f, 1f) },
        { KitColor.Red_Dark,   new Color(139f/255f, 0f/255f, 0f/255f, 1f) },

        { KitColor.Blue,       new Color(30f/255f, 62f/255f, 186f/255f, 1f) },
        { KitColor.Blue_Light, new Color(100f/255f, 149f/255f, 237f/255f, 1f) },
        { KitColor.Blue_Dark,  new Color(0f/255f, 0f/255f, 139f/255f, 1f) },

        { KitColor.Yellow,       new Color(255f/255f, 214f/255f, 0f/255f, 1f) },
        { KitColor.Yellow_Light, new Color(255f/255f, 239f/255f, 153f/255f, 1f) },
        { KitColor.Yellow_Dark,  new Color(204f/255f, 173f/255f, 0f/255f, 1f) },

        { KitColor.Orange,       new Color(255f/255f, 140f/255f, 0f/255f, 1f) },
        { KitColor.Orange_Light, new Color(255f/255f, 190f/255f, 120f/255f, 1f) },
        { KitColor.Orange_Dark,  new Color(205f/255f, 102f/255f, 0f/255f, 1f) },

        { KitColor.Purple,       new Color(128f/255f, 0f/255f, 128f/255f, 1f) },
        { KitColor.Purple_Light, new Color(186f/255f, 85f/255f, 211f/255f, 1f) },
        { KitColor.Purple_Dark,  new Color(75f/255f, 0f/255f, 130f/255f, 1f) },

        { KitColor.Pink,       new Color(255f/255f, 105f/255f, 180f/255f, 1f) },
        { KitColor.Pink_Light, new Color(255f/255f, 182f/255f, 193f/255f, 1f) },
        { KitColor.Pink_Dark,  new Color(199f/255f, 21f/255f, 133f/255f, 1f) },

        { KitColor.Green,       new Color(34f/255f, 139f/255f, 34f/255f, 1f) },
        { KitColor.Green_Light, new Color(144f/255f, 238f/255f, 144f/255f, 1f) },
        { KitColor.Green_Dark,  new Color(0f/255f, 100f/255f, 0f/255f, 1f) },

        { KitColor.Brown,       new Color(139f/255f, 69f/255f, 19f/255f, 1f) },
        { KitColor.Brown_Light, new Color(205f/255f, 133f/255f, 63f/255f, 1f) },
        { KitColor.Brown_Dark,  new Color(92f/255f, 51f/255f, 23f/255f, 1f) },

        { KitColor.Grey,       new Color(128f/255f, 128f/255f, 128f/255f, 1f) },
        { KitColor.Grey_Light, new Color(211f/255f, 211f/255f, 211f/255f, 1f) },
        { KitColor.Grey_Dark,  new Color(64f/255f, 64f/255f, 64f/255f, 1f) },

        { KitColor.Black,       new Color(0f/255f, 0f/255f, 0f/255f, 1f) },
        { KitColor.White,       new Color(255f/255f, 255f/255f, 255f/255f, 1f) },
        { KitColor.None,       new Color(255f/255f, 255f/255f, 255f/255f, 0f) }

    };

    public static Color GetKitColor(KitColor kitColor)
    {
        if (kitColors.TryGetValue(kitColor, out Color color))
            return color;
        return Color.white;
    }

    private static readonly Dictionary<BodyColorType, Color> bodyColors =
        new Dictionary<BodyColorType, Color>()
    {
        { BodyColorType.White,      new Color(1f, 0.95f, 0.9f, 1f) },
        { BodyColorType.Pale,       new Color(0.85f, 0.75f, 0.65f, 1f) },
        { BodyColorType.Generic,    new Color(0.953f, 0.776f, 0.631f, 1f) },
        { BodyColorType.Tanned,     new Color(0.76f, 0.60f, 0.42f, 1f) },
        { BodyColorType.Asian,      new Color(0.90f, 0.78f, 0.64f, 1f) },
        { BodyColorType.Hispanic,   new Color(0.72f, 0.56f, 0.40f, 1f) },
        { BodyColorType.Indian,     new Color(0.62f, 0.45f, 0.30f, 1f) },
        { BodyColorType.Arab,       new Color(0.70f, 0.52f, 0.36f, 1f) },
        { BodyColorType.African,    new Color(0.36f, 0.25f, 0.18f, 1f) },
        { BodyColorType.Black,      new Color(0.15f, 0.10f, 0.08f, 1f) },
        { BodyColorType.Green,      new Color(0.2f, 0.8f, 0.2f, 1f) }
    };

    public static Color GetBodyColor(BodyColorType bodyColorType)
    {
        return bodyColors.TryGetValue(bodyColorType, out var color)
            ? color : Color.white;
    }

    private static readonly Dictionary<EyeColorType, Color> eyeColors =
        new Dictionary<EyeColorType, Color>()
    {
        { EyeColorType.Brown,  new Color(0.36f, 0.24f, 0.15f, 1f) },
        { EyeColorType.Blue,   new Color(0.2f, 0.4f, 0.8f, 1f) },
        { EyeColorType.Green,  new Color(0.2f, 0.6f, 0.3f, 1f) },
        { EyeColorType.Gray,   new Color(0.6f, 0.6f, 0.6f, 1f) },
        { EyeColorType.Red,    new Color(0.8f, 0.1f, 0.1f, 1f) },
        { EyeColorType.Purple, new Color(0.5f, 0.2f, 0.6f, 1f) },
        { EyeColorType.Gold,   new Color(1f, 0.84f, 0.0f, 1f) },
        { EyeColorType.Silver, new Color(0.75f, 0.75f, 0.75f, 1f) },
        { EyeColorType.Black,  new Color(0f, 0f, 0f, 1f) },
        { EyeColorType.White,  new Color(1f, 1f, 1f, 1f) },
        { EyeColorType.Pink,   new Color(1f, 0.6f, 0.8f, 1f) }
    };

    public static Color GetEyeColor(EyeColorType eyeColorType)
    {
        return eyeColors.TryGetValue(eyeColorType, out var color)
            ? color : Color.white;
    }

    private static readonly Dictionary<HairColorType, Color> hairColors =
        new Dictionary<HairColorType, Color>()
    {
        { HairColorType.Black,  new Color(0.05f, 0.05f, 0.05f, 1f) },
        { HairColorType.Blonde, new Color(0.95f, 0.85f, 0.55f, 1f) },
        { HairColorType.Blue,   new Color(0.2f, 0.4f, 0.9f, 1f) },
        { HairColorType.Brown,  new Color(0.4f, 0.25f, 0.1f, 1f) },
        { HairColorType.Green,  new Color(0.2f, 0.7f, 0.3f, 1f) },
        { HairColorType.Orange, new Color(1f, 0.5f, 0.1f, 1f) },
        { HairColorType.Pink,   new Color(1f, 0.6f, 0.8f, 1f) },
        { HairColorType.Purple, new Color(0.6f, 0.3f, 0.7f, 1f) },
        { HairColorType.Red,    new Color(0.8f, 0.2f, 0.1f, 1f) },
        { HairColorType.White,  new Color(0.95f, 0.95f, 0.95f, 1f) }
    };

    public static Color GetHairColor(HairColorType hairColorType)
    {
        return hairColors.TryGetValue(hairColorType, out var color)
            ? color : Color.white;
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

}
