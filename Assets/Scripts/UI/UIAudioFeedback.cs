using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Aremoreno.Enums.UI;

[AddComponentMenu("UI/UI Audio Feedback")]
public class UIAudioFeedback : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler, IPointerClickHandler,
    IBeginDragHandler, IEndDragHandler, IDropHandler,
    ISelectHandler, ISubmitHandler
{
    [SerializeField] internal UIAudioTrigger enabledTriggers = UIAudioTrigger.Submit;

    [SerializeField] internal string hoverSoundId   = "";
    [SerializeField] internal string exitSoundId    = "";
    [SerializeField] internal string downSoundId    = "";
    [SerializeField] internal string upSoundId      = "";
    [SerializeField] internal string clickSoundId   = "";
    [SerializeField] internal string beginDragId    = "";
    [SerializeField] internal string endDragId      = "";
    [SerializeField] internal string dropSoundId    = "";
    [SerializeField] internal string selectSoundId  = "sfx-menu_selected";
    [SerializeField] internal string submitSoundId  = "sfx-menu_tap";

    private void Play(UIAudioTrigger trigger, string id)
    {
        if ((enabledTriggers & trigger) == 0) return;
        if (string.IsNullOrEmpty(id)) return;

        if (AudioManager.Instance == null)
        {
            LogManager.Warning("[UIAudioFeedback] AudioManager is null.");
            return;
        }

        AudioManager.Instance.PlaySfxUI(id);
    }

    public void OnPointerEnter(PointerEventData e) => Play(UIAudioTrigger.PointerEnter, hoverSoundId);
    public void OnPointerExit (PointerEventData e) => Play(UIAudioTrigger.PointerExit,  exitSoundId);
    public void OnPointerDown (PointerEventData e) => Play(UIAudioTrigger.PointerDown,  downSoundId);
    public void OnPointerUp   (PointerEventData e) => Play(UIAudioTrigger.PointerUp,    upSoundId);
    public void OnPointerClick(PointerEventData e) => Play(UIAudioTrigger.Click,        clickSoundId);
    public void OnBeginDrag   (PointerEventData e) => Play(UIAudioTrigger.BeginDrag,    beginDragId);
    public void OnEndDrag     (PointerEventData e) => Play(UIAudioTrigger.EndDrag,      endDragId);
    public void OnDrop        (PointerEventData e) => Play(UIAudioTrigger.Drop,         dropSoundId);
    public void OnSelect      (BaseEventData e)    => Play(UIAudioTrigger.Select,       selectSoundId);
    public void OnSubmit      (BaseEventData e)    => Play(UIAudioTrigger.Submit,       submitSoundId);
}
