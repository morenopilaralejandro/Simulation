using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Aremoreno.Enums.Item;

public class ButtonBagCategory : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private ItemCategory category;
    [SerializeField] private Image imageHighligth;

    private void Start() 
    {
        imageHighligth.enabled = false;
    }

    public void OnButtonPressed() 
    {
        UIEvents.RaiseBagCategoryChanged(category);
    }

    #region Events

    private void OnEnable()
    {
        UIEvents.OnBagCategoryChanged += HandleBagCategoryChanged;
    }

    private void OnDisable()
    {
        UIEvents.OnBagCategoryChanged -= HandleBagCategoryChanged;
    }

    private void HandleBagCategoryChanged(ItemCategory category)
    {
        //if (this.category != category) return;
        imageHighligth.enabled = this.category == category;
    }
    
    #endregion

}
