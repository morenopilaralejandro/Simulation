using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Controls the dialog box UI. Handles:
/// - Text display with typewriter effect
/// - Character name and portrait
/// - Yes/No buttons
/// - Multiple choice list
/// - Continue indicator
/// </summary>
public class DialogUIController : MonoBehaviour
{
    [Header("Dialog Box")]
    [SerializeField] private CanvasGroup _dialogBoxCanvasGroup;
    [SerializeField] private TextMeshProUGUI _dialogText;
    [SerializeField] private TextMeshProUGUI _characterNameText;
    [SerializeField] private CanvasGroup _characterNamePanel;
    [SerializeField] private CanvasGroup _portraitPanel;
    [SerializeField] private CharacterPortraitSpeaker _portraitSpeaker;
    [SerializeField] private CanvasGroup _continueIndicator;

    [Header("Yes/No Panel")]
    [SerializeField] private CanvasGroup _yesNoPanel;
    [SerializeField] private Button _yesButton;
    [SerializeField] private Button _noButton;
    [SerializeField] private TextMeshProUGUI _yesButtonText;
    [SerializeField] private TextMeshProUGUI _noButtonText;

    [Header("Choice List")]
    [SerializeField] private CanvasGroup _choiceListPanel;
    [SerializeField] private Transform _choiceListContainer;
    [SerializeField] private GameObject _choiceButtonPrefab;

    [Header("Settings")]
    [SerializeField] private float _typewriterSpeed = 0.03f;
    [SerializeField] private float _fastTypewriterSpeed = 0.005f;
    [SerializeField] private float _fadeSpeed = 0.3f;
    [SerializeField] private AudioClip _typewriterSFX;
    [SerializeField] private int _typewriterSFXInterval = 2;

    // State
    private bool _isTyping;
    private bool _skipRequested;
    private bool _isFading;

    private Coroutine _typewriterCoroutine;
    private Coroutine _fadeCoroutine;

    private readonly List<GameObject> _spawnedChoiceButtons = new();

    private DialogLocalizationBridge _locBridge;
    private AudioManager _audioManager;
    private DialogManager _dialogManager;

    public bool IsVisible => _dialogBoxCanvasGroup.alpha > 0.001f;
    public bool IsTyping => _isTyping;
    public bool IsFading => _isFading;

    private int _yesChoiceIndex = 0;
    private int _noChoiceIndex = 1;

    private void Awake()
    {
        _dialogManager = DialogManager.Instance;
        _dialogManager.RegisterUIController(this);
    }

    private void OnDestroy()
    {
        _dialogManager.UnregisterUIController();
    }

    public void Initialize(DialogLocalizationBridge locBridge)
    {
        _locBridge = locBridge;

        _yesButton.onClick.AddListener(() =>
            DialogEvents.RaiseChoiceSelected(_yesChoiceIndex));

        _noButton.onClick.AddListener(() =>
            DialogEvents.RaiseChoiceSelected(_noChoiceIndex));

        _audioManager = AudioManager.Instance;

        Clear();
        SetCanvasGroupVisible(_dialogBoxCanvasGroup, false);
    }

    public void Show()
    {
        if (_fadeCoroutine != null)
            StopCoroutine(_fadeCoroutine);

        _isFading = true;

        SetCanvasGroupInteractable(_dialogBoxCanvasGroup, true);

        _fadeCoroutine = StartCoroutine(
            FadeCanvasGroup(
                _dialogBoxCanvasGroup,
                _dialogBoxCanvasGroup.alpha,
                1f,
                _fadeSpeed,
                () =>
                {
                    _isFading = false;
                    _fadeCoroutine = null;
                }));
    }

    public void Hide()
    {
        if (_fadeCoroutine != null)
            StopCoroutine(_fadeCoroutine);

        _fadeCoroutine = null;

        HideChoices();
        SetCanvasGroupVisible(_continueIndicator, false);

        _isFading = true;

        _fadeCoroutine = StartCoroutine(
            FadeCanvasGroup(
                _dialogBoxCanvasGroup,
                _dialogBoxCanvasGroup.alpha,
                0f,
                _fadeSpeed,
                () =>
                {
                    _isFading = false;
                    _fadeCoroutine = null;

                    SetCanvasGroupInteractable(_dialogBoxCanvasGroup, false);
                }));
    }

    public void Clear()
    {
        if (_typewriterCoroutine != null)
        {
            StopCoroutine(_typewriterCoroutine);
            _typewriterCoroutine = null;
        }

        _isTyping = false;
        _skipRequested = false;

        _dialogText.text = string.Empty;
        _dialogText.maxVisibleCharacters = 0;

        _characterNameText.text = string.Empty;
        _portraitSpeaker.Clear();

        HideChoices();

        SetCanvasGroupVisible(_characterNamePanel, false);
        SetCanvasGroupVisible(_portraitPanel, false);
        SetCanvasGroupVisible(_continueIndicator, false);
    }

    /// <summary>
    /// Display a dialog line with typewriter effect.
    /// </summary>
    public void ShowLine(DialogLine line)
    {
        if (_typewriterCoroutine != null)
            StopCoroutine(_typewriterCoroutine);

        HideChoices();
        SetCanvasGroupVisible(_continueIndicator, false);

        if (!line.IsSystemMessage && !string.IsNullOrEmpty(line.SpeakerId))
        {
            var speaker = _dialogManager.GetSpeaker(line);

            if (speaker != null)
            {
                _characterNameText.text = speaker.SpeakerName;

                SetCanvasGroupVisible(_characterNamePanel, true);

                _ = _portraitSpeaker.SetSpeakerAsync(speaker);

                SetCanvasGroupVisible(_portraitPanel, true);
            }
            else
            {
                _characterNameText.text = line.SpeakerId;

                SetCanvasGroupVisible(_characterNamePanel, false);
                SetCanvasGroupVisible(_portraitPanel, false);
            }
        }
        else
        {
            _characterNameText.text = string.Empty;

            SetCanvasGroupVisible(_characterNamePanel, false);
            SetCanvasGroupVisible(_portraitPanel, false);
        }

        _typewriterCoroutine = StartCoroutine(
            TypewriterEffect(line.ResolvedText));
    }

    /// <summary>
    /// Show choices - automatically detects Yes/No vs multiple choice.
    /// </summary>
    public void ShowChoices(List<DialogChoice> choices)
    {
        SetCanvasGroupVisible(_continueIndicator, false);

        bool isYesNo = choices.Exists(c => c.IsYesNoChoice);

        if (isYesNo)
            ShowYesNoChoices(choices);
        else
            ShowMultipleChoices(choices);
    }

    private void ShowYesNoChoices(List<DialogChoice> choices)
    {
        SetCanvasGroupVisible(_yesNoPanel, true);
        SetCanvasGroupVisible(_choiceListPanel, false);

        foreach (var choice in choices)
        {
            if (choice.IsYes)
            {
                _yesButtonText.text = choice.ResolvedText;
                _yesChoiceIndex = choice.Index;
            }
            else if (choice.IsNo)
            {
                _noButtonText.text = choice.ResolvedText;
                _noChoiceIndex = choice.Index;
            }
        }
    }

    private void ShowMultipleChoices(List<DialogChoice> choices)
    {
        SetCanvasGroupVisible(_yesNoPanel, false);
        SetCanvasGroupVisible(_choiceListPanel, true);

        ClearChoiceButtons();

        foreach (var choice in choices)
        {
            var buttonObj = Instantiate(_choiceButtonPrefab, _choiceListContainer);

            var buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

            if (buttonText != null)
                buttonText.text = choice.ResolvedText;

            var button = buttonObj.GetComponent<Button>();

            int capturedIndex = choice.Index;

            button.onClick.AddListener(() =>
                DialogEvents.RaiseChoiceSelected(capturedIndex));

            _spawnedChoiceButtons.Add(buttonObj);
        }
    }

    public void HideChoices()
    {
        SetCanvasGroupVisible(_yesNoPanel, false);
        SetCanvasGroupVisible(_choiceListPanel, false);

        ClearChoiceButtons();
    }

    private void ClearChoiceButtons()
    {
        foreach (var btn in _spawnedChoiceButtons)
        {
            if (btn != null)
                Destroy(btn);
        }

        _spawnedChoiceButtons.Clear();
    }

    /// <summary>
    /// Called when player presses confirm/advance button.
    /// If typing, skip to end. If done typing, request continue.
    /// </summary>
    public void HandleAdvanceInput()
    {
        if (_isFading)
            return;

        if (_isTyping)
            _skipRequested = true;
        else
            DialogEvents.RaiseContinueRequested();
    }

    private IEnumerator TypewriterEffect(string fullText)
    {
        _isTyping = true;
        _skipRequested = false;

        _dialogText.text = fullText;
        _dialogText.maxVisibleCharacters = 0;

        int totalCharacters = fullText.Length;
        int visibleCount = 0;
        int sfxCounter = 0;

        while (visibleCount < totalCharacters)
        {
            if (_skipRequested)
            {
                _dialogText.maxVisibleCharacters = totalCharacters;
                break;
            }

            visibleCount++;
            _dialogText.maxVisibleCharacters = visibleCount;

            sfxCounter++;

            if (_typewriterSFX != null &&
                sfxCounter >= _typewriterSFXInterval)
            {
                _audioManager.PlaySfxClip(_typewriterSFX);
                sfxCounter = 0;
            }

            float speed = _skipRequested
                ? _fastTypewriterSpeed
                : _typewriterSpeed;

            yield return new WaitForSeconds(speed);
        }

        _isTyping = false;

        SetCanvasGroupVisible(_continueIndicator, true);

        DialogEvents.RaiseTextDisplayComplete();
    }

    private IEnumerator FadeCanvasGroup(
        CanvasGroup cg,
        float from,
        float to,
        float duration,
        Action onComplete = null)
    {
        float elapsed = 0f;

        cg.alpha = from;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            cg.alpha = Mathf.Lerp(
                from,
                to,
                elapsed / duration);

            yield return null;
        }

        cg.alpha = to;

        onComplete?.Invoke();
    }

    private void SetCanvasGroupVisible(CanvasGroup cg, bool visible)
    {
        if (cg == null)
            return;

        cg.alpha = visible ? 1f : 0f;
        cg.interactable = visible;
        cg.blocksRaycasts = visible;
    }

    private void SetCanvasGroupInteractable(CanvasGroup cg, bool interactable)
    {
        if (cg == null)
            return;

        cg.interactable = interactable;
        cg.blocksRaycasts = interactable;
    }
}
