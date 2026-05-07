using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Character;

public class SelectorCharacterListItem : SelectorListItem<Character>
{
    [Header("Character UI")]
    [SerializeField] private CharacterCard characterCard;
    [SerializeField] private BarHPSP barHp;
    [SerializeField] private BarHPSP barSp;
    [SerializeField] private TMP_Text textLv;

    protected override void OnBind(Character c)
    {
        characterCard.SetCharacter(c, c.Position);
        barHp.SetCharacter(c, Stat.Hp);
        barSp.SetCharacter(c, Stat.Sp);
        textLv.text = $"{c.Level}";
    }

    protected override void OnUnbind()
    {
        characterCard.Clear();
        textLv.text = "";
    }
}
