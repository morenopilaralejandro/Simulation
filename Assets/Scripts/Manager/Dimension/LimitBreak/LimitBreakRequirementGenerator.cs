using UnityEngine;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.LimitBreak;
using Aremoreno.Enums.Move;
using System.Collections.Generic;

public class LimitBreakRequirementGenerator
{
    #region Field

    private DatabaseManager databaseManager;
    private List<MaterialRequirement> list = new List<MaterialRequirement>();
    private LimitBreakType type;

    #endregion

    #region Constructor

    public LimitBreakRequirementGenerator()
    {
        this.databaseManager = DatabaseManager.Instance;
    }

    #endregion

    #region Logic

    public List<MaterialRequirement> GenerateRequirementMoveLimitBreak(Move move)
    {
        type = LimitBreakType.Move;

        list.Clear();

        list.Add(new MaterialRequirement{ ItemId = "item-material-00106-limitbreak_essence", Amount = 1 });
        list.Add(GenerateRequirementElement(move.Element, 2, type));
        list.Add(GenerateRequirementCategory(move.Category, 2, type));

        return list;
    }

    public List<MaterialRequirement> GenerateRequirementWingLimitBreak(Wing wing)
    {
        type = LimitBreakType.Wing;

        list.Clear();

        list.Add(new MaterialRequirement{ ItemId = "item-material-00106-limitbreak_essence", Amount = 1 });
        list.Add(GenerateRequirementElement(wing.Elements[0], 2, type));
        list.Add(GenerateRequirementElement(wing.Elements[1], 2, type));
        list.Add(GenerateRequirementGenderWing(Gender.Male, 2, type));
        list.Add(GenerateRequirementGenderWing(Gender.Female, 2, type));

        return list;
    }

    public List<MaterialRequirement> GenerateRequirementCharacterAwaken(Character character)
    {
        type = LimitBreakType.Character;

        list.Clear();

        list.Add(new MaterialRequirement{ ItemId = "item-material-00105-awaken_essence", Amount = 1 });
        list.Add(GenerateRequirementElement(character.Element, 2, type));
        list.Add(GenerateRequirementPosition(character.Position, 2, type));
        list.Add(GenerateRequirementGender(character.Gender, 2, type));

        return list;
    }


    #endregion

    #region Helper

    private int GetMultiplier(LimitBreakType type)
    {
        return type switch
        {
            LimitBreakType.Move => 1,
            LimitBreakType.Wing => 2,
            LimitBreakType.Character => 4,
            _ => 1
        };
    }

    public MaterialRequirement GenerateRequirementElement(Element element, int baseAmount, LimitBreakType type)
    {
        int multiplier = GetMultiplier(type);

        return new MaterialRequirement
        {
            ItemId = databaseManager.GetMaterialByElementData(element).Material.ItemId,
            Amount = baseAmount * multiplier
        };
    }

    public MaterialRequirement GenerateRequirementPosition(Position enumValue, int baseAmount, LimitBreakType type)
    {
        int multiplier = GetMultiplier(type);

        return new MaterialRequirement
        {
            ItemId = databaseManager.GetMaterialByPositionData(enumValue).Material.ItemId,
            Amount = baseAmount * multiplier
        };
    }

    public MaterialRequirement GenerateRequirementCategory(Category enumValue, int baseAmount, LimitBreakType type)
    {
        int multiplier = GetMultiplier(type);

        return new MaterialRequirement
        {
            ItemId = databaseManager.GetMaterialByCategoryMoveData(enumValue).Material.ItemId,
            Amount = baseAmount * multiplier
        };
    }

    public MaterialRequirement GenerateRequirementGender(Gender enumValue, int baseAmount, LimitBreakType type)
    {
        int multiplier = GetMultiplier(type);

        return new MaterialRequirement
        {
            ItemId = databaseManager.GetMaterialByGenderData(enumValue).Material.ItemId,
            Amount = baseAmount * multiplier
        };
    }

    public MaterialRequirement GenerateRequirementGenderWing(Gender enumValue, int baseAmount, LimitBreakType type)
    {
        int multiplier = GetMultiplier(type);

        return new MaterialRequirement
        {
            ItemId = databaseManager.GetMaterialByGenderWingData(enumValue).Material.ItemId,
            Amount = baseAmount * multiplier
        };
    }

    #endregion
}
