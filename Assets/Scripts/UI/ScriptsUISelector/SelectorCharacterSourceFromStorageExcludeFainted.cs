using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SelectorCharacterSourceFromStorageExcludeFainted : ISelectorSource<Character>
{
    public IEnumerable<Character> Enumerate()
        => CharacterManager.Instance.Characters.Values
            .Where(c => !c.IsFainted)
            .OrderBy(c => c.CharacterId);
}
