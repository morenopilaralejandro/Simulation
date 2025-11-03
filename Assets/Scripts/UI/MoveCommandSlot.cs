using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MoveCommandSlot : MonoBehaviour
{
    private Move move;
    [SerializeField] private TMP_Text textName;
    [SerializeField] private TMP_Text textCost;

    public Move Move => move;

    public void SetMove(Move move)
    {
        this.move = move;
        textName.text = move.MoveName;
        textName.color = ColorManager.GetElementColor(move.Element);
        textCost.text = $"{move.Cost}";
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
