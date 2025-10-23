using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Simulation.Enums.Character;
using Simulation.Enums.Move;

public class DuelCategoryPanel : MonoBehaviour
{
    [SerializeField] private Image imageCategory;

    public void SetCategory(Category category)
    {
        imageCategory.sprite = IconManager.Instance.Category.GetIcon(category);
    }

}
