using UnityEngine;

public class WingFilterAdapterNotEquipped : ISelectorFilter<Wing>
{
    //private readonly CharacterFilterData data;
    //public CharacterFilterAdapter(CharacterFilterData data) => this.data = data;
    public bool Matches(Wing obj) => !obj.IsEquipped();
}
