using UnityEngine;
using System.Collections.Generic;

public class SelectorCharacterSourceFromStorage : ISelectorSource<Character>
{
    public IEnumerable<Character> Enumerate()
        => CharacterManager.Instance.Characters.Values;
}
