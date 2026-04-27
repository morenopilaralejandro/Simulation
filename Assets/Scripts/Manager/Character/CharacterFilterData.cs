using System.Collections.Generic;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Character;

public class CharacterFilterData
{
    public string Name;
    public HashSet<Position> Positions = new HashSet<Position>();
    public HashSet<Element> Elements = new HashSet<Element>();
    public HashSet<Gender> Genders = new HashSet<Gender>();

    public bool IsEmpty =>
        string.IsNullOrEmpty(Name) &&
        Positions.Count == 0 &&
        Elements.Count == 0 &&
        Genders.Count == 0;

    public bool Matches(Character character)
    {
        // Name filter: skip if empty
        if (!string.IsNullOrEmpty(Name))
        {
            if (character.CharacterName == null ||
                character.CharacterName.IndexOf(Name, System.StringComparison.OrdinalIgnoreCase) < 0)
                return false;
        }

        // Position filter: skip if no toggle is on
        if (Positions.Count > 0)
        {
            if (!Positions.Contains(character.Position))
                return false;
        }

        // Element filter: skip if no toggle is on
        if (Elements.Count > 0)
        {
            if (!Elements.Contains(character.Element))
                return false;
        }

        // Gender filter: skip if no toggle is on
        if (Genders.Count > 0)
        {
            if (!Genders.Contains(character.Gender))
                return false;
        }

        return true;
    }

    public void Reset()
    {
        Name = null;
        Positions.Clear();
        Elements.Clear();
        Genders.Clear();
    }
}
