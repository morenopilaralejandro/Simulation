using UnityEngine;
using Aremoreno.Enums.Item;

public class ItemRecipe : Item
{
    #region Components

    private ItemComponentRecipe recipeComponent;

    #endregion

    #region Initialize

    public ItemRecipe(ItemDataRecipe data) : base(data)
    {
        InitializeItemRecipe(data);
    }

    private void InitializeItemRecipe(ItemDataRecipe data)
    {
        recipeComponent = new ItemComponentRecipe(data, this);
    }

    #endregion

    #region API

    // recipeComponent
    public string RecipeId => recipeComponent.RecipeId;

    #endregion
}
