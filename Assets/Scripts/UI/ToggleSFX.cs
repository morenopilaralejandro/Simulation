using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleSFX : MonoBehaviour
{
    [SerializeField] private Toggle toggle;
    [SerializeField] private string onSfxId = "sfx-dimension_wings_on";
    [SerializeField] private string offSfxId = "sfx-dimension_wings_off";

    void Awake()
    {
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    void OnToggleValueChanged(bool isOn)
    {
        if (isOn)
            AudioManager.Instance.PlaySfxUI(onSfxId, true);
        else
            AudioManager.Instance.PlaySfxUI(offSfxId, true);
    }

    void OnDestroy()
    {
        if (toggle != null) toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
    }
}
