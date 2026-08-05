using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SelectorCharacterSourceFromStorage : ISelectorSource<Character>
{
    public IEnumerable<Character> Enumerate()
        => CharacterManager.Instance.Characters.Values
            .OrderBy(c => c.CharacterId);
}
