using UnityEngine;

public class CharacterFilterAdapter : ISelectorFilter<Character>
{
    private readonly CharacterFilterData data;
    public CharacterFilterAdapter(CharacterFilterData data) => this.data = data;
    public bool Matches(Character c) => data == null || data.IsEmpty || data.Matches(c);
}
