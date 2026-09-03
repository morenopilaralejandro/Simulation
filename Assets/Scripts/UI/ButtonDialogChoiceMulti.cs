using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonDialogChoiceMulti : MonoBehaviour
{
    [Header("Common")]
    [SerializeField] protected Button button;
    [SerializeField] private ScrollRectForwarder scrollForwarder;
    [SerializeField] private TMP_Text textDialog;

    private int capturedIndex;
    private ScrollRect parentScrollRect;
    public ScrollRect ParentScrollRect => parentScrollRect;
    public Button Button => button;

    public void SetScrollRect(ScrollRect sr)
    {
        parentScrollRect = sr;
        if (scrollForwarder != null) scrollForwarder.SetScrollRect(sr);
    }

    public void SetText(string text)
    {
        textDialog.text = text;
    }

    public void SetIndex(int capturedIndex)
    {
        this.capturedIndex = capturedIndex;
    }

    private void Clear()
    {
        capturedIndex = -1;
        scrollForwarder = null;
        textDialog.text = "";
    }

    public void OnButtonClicked()
    {
        DialogEvents.RaiseChoiceSelected(capturedIndex);
    }
}
