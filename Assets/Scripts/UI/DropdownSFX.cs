using TMPro;
using UnityEngine;

public class DropdownSFX : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;

    private string sfxId = "sfx-menu_tap";

    private void Awake()
    {
        dropdown.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnValueChanged(int index)
    {
        AudioManager.Instance.PlaySfxUI(sfxId);
    }

}
