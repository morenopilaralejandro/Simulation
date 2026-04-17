using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Aremoreno.Enums.Character;

public class OffScreenPortraitManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private TeamSide trackedTeamSide;

    [Header("Portrait Components")]
    [SerializeField] private CharacterPortraitBattle characterPortrait;
    [SerializeField] private RectTransform portraitContainer;
    [SerializeField] private Image imageElement;

    [Header("Optional Arrow")]
    [SerializeField] private Image arrowImage;
    private bool showArrow = true;

    [Header("Settings")]
    private float edgeMargin = 0.08f;
    private float updateInterval = 0.01f;

    // Tracked references
    private Transform playerTransform;
    private CharacterEntityBattle currentEntity;

    // Cached values
    private RectTransform canvasRect;
    private CanvasGroup canvasGroup;
    private bool isInitialized;
    private float nextUpdateTime;
    private bool wasOffScreen;

    // Cached calculations
    private Vector3 viewportPos;
    private Vector2 canvasPos;
    private float minViewportX;
    private float maxViewportX;
    private float minViewportY;
    private float maxViewportY;

    private void Awake()
    {
        canvasRect = portraitContainer.GetComponentInParent<Canvas>().GetComponent<RectTransform>();

        canvasGroup = portraitContainer.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = portraitContainer.gameObject.AddComponent<CanvasGroup>();

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (arrowImage != null)
            arrowImage.gameObject.SetActive(showArrow);

        minViewportX = edgeMargin;
        maxViewportX = 1f - edgeMargin;
        minViewportY = edgeMargin;
        maxViewportY = 1f - edgeMargin;

        // Start hidden but keep manager active
        HidePortrait();
    }

    private void Start() 
    {
        trackedTeamSide = BattleManager.Instance.GetUserSide();
    }

    // Subscribe as early as possible
    private void OnEnable()
    {
        CharacterEvents.OnControlChange += HandleControlChange;
    }

    private void OnDisable()
    {
        CharacterEvents.OnControlChange -= HandleControlChange;
    }

    private void HandleControlChange(CharacterEntityBattle characterEntity, TeamSide teamSide)
    {
        if (teamSide != trackedTeamSide)
            return;

        currentEntity = characterEntity;

        if (characterEntity == null)
        {
            isInitialized = false;
            HidePortrait();
            return;
        }

        playerTransform = characterEntity.transform;
        characterPortrait.SetCharacter(characterEntity.Character);
        imageElement.sprite = IconManager.Instance.Element.GetIcon(characterEntity.Element);
        isInitialized = true;
    }

    private void HidePortrait()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        wasOffScreen = false;
    }

    private void ShowPortrait()
    {
        canvasGroup.alpha = 1f;
    }

    private void LateUpdate()
    {
        if (!isInitialized || playerTransform == null)
            return;

        if (Time.time < nextUpdateTime)
            return;

        nextUpdateTime = Time.time + updateInterval;

        viewportPos = mainCamera.WorldToViewportPoint(playerTransform.position);

        bool isOffScreen = viewportPos.x <= 0.02f || viewportPos.x >= 0.98f ||
                          viewportPos.y <= 0.02f || viewportPos.y >= 0.98f ||
                          viewportPos.z < 0;

        if (isOffScreen != wasOffScreen)
        {
            if (isOffScreen)
                ShowPortrait();
            else
                HidePortrait();

            wasOffScreen = isOffScreen;
        }

        if (isOffScreen)
        {
            PositionPortraitOnEdge();
        }
    }

    private void PositionPortraitOnEdge()
    {
        if (viewportPos.z < 0)
        {
            viewportPos.x = 1f - viewportPos.x;
            viewportPos.y = 1f - viewportPos.y;
        }

        float clampedX = Mathf.Clamp(viewportPos.x, minViewportX, maxViewportX);
        float clampedY = Mathf.Clamp(viewportPos.y, minViewportY, maxViewportY);

        if (showArrow && arrowImage != null)
        {
            float dx = viewportPos.x - 0.5f;
            float dy = viewportPos.y - 0.5f;
            float angle = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
            arrowImage.rectTransform.rotation = Quaternion.Euler(0, 0, angle + 90);
        }

        canvasPos.x = (clampedX - 0.5f) * canvasRect.sizeDelta.x;
        canvasPos.y = (clampedY - 0.5f) * canvasRect.sizeDelta.y;

        portraitContainer.anchoredPosition = canvasPos;
    }
}
