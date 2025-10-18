using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Move;
using Simulation.Enums.Character;

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
}
