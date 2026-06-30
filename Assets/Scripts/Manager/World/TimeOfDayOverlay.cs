using UnityEngine;
using UnityEngine.UI;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.World;

public class TimeOfDayOverlay : MonoBehaviour
{
    #region Fields

    //[SerializeField]
    private Color morningColor = new Color(0.075f, 0f, 1f, 0.2f);
    private Color dayColor = new Color(0.075f, 0f, 1f, 0f);
    private Color eveningColor = new Color(1f, 0.353f, 0f, 0.1f);
    private Color nightColor = new Color(0f, 0f, 0f, 0.2f);
    private float colorTransitionDuration = 0.5f;

    private Image overlayImage;
    private WorldManager worldManager;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        overlayImage = GetComponent<Image>();
        worldManager = WorldManager.Instance;
    }

    private void Start() 
    {
        if (worldManager == null) return;
        ChangeColorWorld(worldManager.CurrentZone, worldManager.CurrentTimeOfDay);
    }

    #endregion

    #region Color Management

    /// <summary>
    /// Gets the color for a given time of day (includes alpha for lighting intensity).
    /// </summary>
    private Color GetColorForTime(TimeOfDay timeOfDay)
    {
        return timeOfDay switch
        {
            TimeOfDay.Morning => morningColor,
            TimeOfDay.Day => dayColor,
            TimeOfDay.Evening => eveningColor,
            TimeOfDay.Night => nightColor,
            _ => dayColor
        };
    }

    /// <summary>
    /// Sets the color of the overlay image immediately.
    /// </summary>
    public void SetColor(Color newColor)
    {
        overlayImage.color = newColor;
    }

    /// <summary>
    /// Lerps the overlay color smoothly over a duration.
    /// </summary>
    public void LerpColor(Color fromColor, Color toColor, float duration)
    {
        StopCoroutine(nameof(LerpColorCoroutine));
        StartCoroutine(LerpColorCoroutine(fromColor, toColor, duration));
    }

    /// <summary>
    /// Coroutine that handles the color lerp animation.
    /// </summary>
    private System.Collections.IEnumerator LerpColorCoroutine(Color fromColor, Color toColor, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            overlayImage.color = Color.Lerp(fromColor, toColor, t);

            yield return null;
        }

        // Ensure we end exactly at the target color
        overlayImage.color = toColor;
    }

    #endregion

    #region Visibility Control

    /// <summary>
    /// Enables the overlay.
    /// </summary>
    public void Show()
    {
        overlayImage.enabled = true;
    }

    /// <summary>
    /// Disables the overlay.
    /// </summary>
    public void Hide()
    {
        overlayImage.enabled = false;
    }

    #endregion

    #region Events

    public void OnEnable()
    {
        WorldEvents.OnTimeOfDayChanged += HandleTimeOfDayChanged;
        WorldEvents.OnZoneChanged += HandleZoneChanged;
        BattleEvents.OnBattleStart += HandleBattleStart;
    }

    public void OnDisable()
    {
        WorldEvents.OnTimeOfDayChanged -= HandleTimeOfDayChanged;
        WorldEvents.OnZoneChanged -= HandleZoneChanged;
        BattleEvents.OnBattleStart -= HandleBattleStart;
    }

    private void HandleTimeOfDayChanged(TimeOfDay timeOfDay)
    {
        Color fromColor = GetColorForTime(worldManager.PreviousTimeOfDay);
        Color toColor = GetColorForTime(worldManager.CurrentTimeOfDay);
        
        LerpColor(fromColor, toColor, colorTransitionDuration);
    }

    private void HandleZoneChanged(ZoneDefinition zonePrevious, ZoneDefinition zoneCurrent, string zoneName)
    {
        ChangeColorWorld(zoneCurrent, worldManager.CurrentTimeOfDay);
    }

    private void ChangeColorWorld(ZoneDefinition zoneCurrent, TimeOfDay timeOfDay)
    {
        if (zoneCurrent.zoneType == ZoneType.Interior) 
        {
            Hide();
        } 
        else 
        {
            SetColor(GetColorForTime(timeOfDay));
            Show();
        }
    }

    private void HandleBattleStart(BattleType battleType)
    {
        SetColor(GetColorForTime(BattleArgs.TimeOfDay));
    }
    
    #endregion
}
