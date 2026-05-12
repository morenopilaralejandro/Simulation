using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Move;

public class MoveCommandSlot : MonoBehaviour
{
    private Move move;
    [SerializeField] private TMP_Text textName;
    [SerializeField] private TMP_Text textCost;
    [SerializeField] private Image imageTrait;
    [SerializeField] private Image imageEvolution;

    public Move Move => move;

    public void SetMove(Move move)
    {
        this.move = move;
        textName.text = move.MoveName;
        textName.color = ColorManager.GetElementColor(move.Element);
        textCost.text = $"{move.Cost}";

        if (move.Trait != Trait.None) 
        {
            imageTrait.sprite = IconManager.Instance.Trait.GetIcon(move.Trait);
            imageTrait.enabled = true;
        } else 
        {
            imageTrait.enabled = false;
        }

        if (move.CurrentEvolution != MoveEvolution.None) 
        {
            imageEvolution.sprite = move.EvolutionSprite;
            imageEvolution.enabled = true;
        } else 
        {
            imageEvolution.enabled = false;
        }
    }

    public void Show() 
    {
        this.gameObject.SetActive(true);
    }

    public void Hide() 
    {
        this.gameObject.SetActive(false);
    }

    public void SetActive(bool isActive) 
    {
        this.gameObject.SetActive(isActive);
    }

    public void SetInteractable(bool isInteractable) 
    {
        this.GetComponent<Button>().interactable = isInteractable;
    }

    public bool CanBeSelected() =>
        this.gameObject.activeSelf && 
        this.GetComponent<Button>().interactable;

}
