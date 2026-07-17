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

    private void Clear()
    {
        scrollForwarder = null;
        textDialog.text = "";
    }
}
