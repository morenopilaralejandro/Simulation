using System.Collections.Generic;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Character;

public class CharacterFilterData
{
    public string Name;
    public HashSet<Position> Positions = new HashSet<Position>();
    public HashSet<Element> Elements = new HashSet<Element>();
    public HashSet<Gender> Genders = new HashSet<Gender>();
    public HashSet<string> ExcludedGuids = new HashSet<string>(); // NEW

    public bool IsEmpty =>
        string.IsNullOrEmpty(Name) &&
        Positions.Count == 0 &&
        Elements.Count == 0 &&
        Genders.Count == 0 &&
        ExcludedGuids.Count == 0; // NEW

    public bool Matches(Character character)
    {
        // GUID exclusion filter
        if (ExcludedGuids.Count > 0)
        {
            if (ExcludedGuids.Contains(character.CharacterGuid))
                return false;
        }

        // Name filter
        if (!string.IsNullOrEmpty(Name))
        {
            if (character.CharacterName == null ||
                character.CharacterName.IndexOf(Name, System.StringComparison.OrdinalIgnoreCase) < 0)
                return false;
        }

        // Position filter
        if (Positions.Count > 0)
        {
            if (!Positions.Contains(character.Position))
                return false;
        }

        // Element filter
        if (Elements.Count > 0)
        {
            if (!Elements.Contains(character.Element))
                return false;
        }

        // Gender filter
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
        ExcludedGuids.Clear(); // NEW
    }

    public void ResetUI()
    {
        Name = null;
        Positions.Clear();
        Elements.Clear();
        Genders.Clear();
    }
}
