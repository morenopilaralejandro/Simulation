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
/*

        [Header("Dialog Box")]
        [SerializeField] private GameObject _dialogBoxRoot;
        [SerializeField] private CanvasGroup _dialogBoxCanvasGroup;
        [SerializeField] private TextMeshProUGUI _dialogText;
        [SerializeField] private TextMeshProUGUI _characterNameText;
        [SerializeField] private Image _characterPortrait;
        [SerializeField] private GameObject _characterNamePanel;
        [SerializeField] private GameObject _portraitPanel;
        [SerializeField] private GameObject _continueIndicator; // little arrow/triangle

        [Header("Yes/No Panel")]
        [SerializeField] private GameObject _yesNoPanel;
        [SerializeField] private Button _yesButton;
        [SerializeField] private Button _noButton;
        [SerializeField] private TextMeshProUGUI _yesButtonText;
        [SerializeField] private TextMeshProUGUI _noButtonText;

        [Header("Choice List")]
        [SerializeField] private GameObject _choiceListPanel;
        [SerializeField] private Transform _choiceListContainer;
        [SerializeField] private GameObject _choiceButtonPrefab;

        [Header("Settings")]
        [SerializeField] private float _typewriterSpeed = 0.03f;
        [SerializeField] private float _fastTypewriterSpeed = 0.005f;
        [SerializeField] private float _fadeSpeed = 0.3f;
        [SerializeField] private AudioClip _typewriterSFX;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private int _typewriterSFXInterval = 2; // play sfx every N characters

        // State
        private bool _isTyping = false;
        private bool _skipRequested = false;
        private Coroutine _typewriterCoroutine;
        private List<GameObject> _spawnedChoiceButtons = new List<GameObject>();

        // Events
        public event Action OnTextDisplayComplete;
        public event Action<int> OnChoiceSelected;
        public event Action OnContinueRequested;

        private CharacterDatabase _characterDb;
        private DialogLocalizationBridge _locBridge;

        public bool IsVisible => _dialogBoxRoot.activeSelf;
        public bool IsTyping => _isTyping;

        public void Initialize(CharacterDatabase characterDb, DialogLocalizationBridge locBridge)
        {
            _characterDb = characterDb;
            _locBridge = locBridge;
            
            _yesButton.onClick.AddListener(() => OnChoiceSelected?.Invoke(GetYesChoiceIndex()));
            _noButton.onClick.AddListener(() => OnChoiceSelected?.Invoke(GetNoChoiceIndex()));
            
            Hide();
        }
        
        private int _yesChoiceIndex = 0;
        private int _noChoiceIndex = 1;
        
        private int GetYesChoiceIndex() => _yesChoiceIndex;
        private int GetNoChoiceIndex() => _noChoiceIndex;

        public void Show()
        {
            _dialogBoxRoot.SetActive(true);
            if (_dialogBoxCanvasGroup != null)
            {
                StartCoroutine(FadeCanvasGroup(_dialogBoxCanvasGroup, 0f, 1f, _fadeSpeed));
            }
        }

        public void Hide()
        {
            HideChoices();
            _continueIndicator.SetActive(false);
            
            if (_dialogBoxCanvasGroup != null)
            {
                StartCoroutine(FadeCanvasGroup(_dialogBoxCanvasGroup, 1f, 0f, _fadeSpeed, () =>
                {
                    _dialogBoxRoot.SetActive(false);
                }));
            }
            else
            {
                _dialogBoxRoot.SetActive(false);
            }
        }

        /// <summary>
        /// Display a dialog line with typewriter effect.
        /// </summary>
        public void ShowLine(DialogLine line)
        {
            // Stop any current typewriter
            if (_typewriterCoroutine != null)
                StopCoroutine(_typewriterCoroutine);
            
            HideChoices();
            _continueIndicator.SetActive(false);

            // Set character info
            if (!line.IsSystemMessage && !string.IsNullOrEmpty(line.SpeakerId))
            {
                var character = _characterDb.GetCharacter(line.SpeakerId);
                if (character != null)
                {
                    // Character name (localized)
                    _characterNameText.text = _locBridge.ResolveCharacterName(line.SpeakerId);
                    _characterNameText.color = character.nameColor;
                    _characterNamePanel.SetActive(true);
                    
                    // Portrait
                    var portrait = character.GetPortrait(line.Mood);
                    if (portrait != null)
                    {
                        _characterPortrait.sprite = portrait;
                        _portraitPanel.SetActive(true);
                    }
                    else
                    {
                        _portraitPanel.SetActive(false);
                    }
                }
                else
                {
                    _characterNameText.text = line.SpeakerId;
                    _characterNamePanel.SetActive(true);
                    _portraitPanel.SetActive(false);
                }
            }
            else
            {
                // System message or no speaker
                _characterNamePanel.SetActive(line.IsSystemMessage);
                if (line.IsSystemMessage)
                    _characterNameText.text = ""; // or "System" if you want
                _portraitPanel.SetActive(false);
            }

            // Start typewriter
            _typewriterCoroutine = StartCoroutine(TypewriterEffect(line.ResolvedText));
        }

        /// <summary>
        /// Show choices - automatically detects Yes/No vs multiple choice.
        /// </summary>
        public void ShowChoices(List<DialogChoice> choices)
        {
            _continueIndicator.SetActive(false);
            
            // Check if this is a Yes/No choice
            bool isYesNo = choices.Exists(c => c.IsYesNoChoice);
            
            if (isYesNo)
            {
                ShowYesNoChoices(choices);
            }
            else
            {
                ShowMultipleChoices(choices);
            }
        }

        private void ShowYesNoChoices(List<DialogChoice> choices)
        {
            _yesNoPanel.SetActive(true);
            _choiceListPanel.SetActive(false);
            
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
            _yesNoPanel.SetActive(false);
            _choiceListPanel.SetActive(true);
            
            // Clear old buttons
            ClearChoiceButtons();
            
            foreach (var choice in choices)
            {
                var buttonObj = Instantiate(_choiceButtonPrefab, _choiceListContainer);
                buttonObj.SetActive(true);
                
                var buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                    buttonText.text = choice.ResolvedText;
                
                var button = buttonObj.GetComponent<Button>();
                int capturedIndex = choice.Index; // capture for closure
                button.onClick.AddListener(() => OnChoiceSelected?.Invoke(capturedIndex));
                
                _spawnedChoiceButtons.Add(buttonObj);
            }
        }

        public void HideChoices()
        {
            _yesNoPanel.SetActive(false);
            _choiceListPanel.SetActive(false);
            ClearChoiceButtons();
        }

        private void ClearChoiceButtons()
        {
            foreach (var btn in _spawnedChoiceButtons)
            {
                if (btn != null) Destroy(btn);
            }
            _spawnedChoiceButtons.Clear();
        }

        /// <summary>
        /// Called when player presses confirm/advance button.
        /// If typing, skip to end. If done typing, request continue.
        /// </summary>
        public void HandleAdvanceInput()
        {
            if (_isTyping)
            {
                _skipRequested = true;
            }
            else
            {
                OnContinueRequested?.Invoke();
            }
        }

        private IEnumerator TypewriterEffect(string fullText)
        {
            _isTyping = true;
            _skipRequested = false;
            _dialogText.text = "";
            
            // Use TMP's maxVisibleCharacters for proper rich text support
            _dialogText.text = fullText;
            _dialogText.maxVisibleCharacters = 0;
            
            int totalCharacters = fullText.Length;
            int visibleCount = 0;
            int sfxCounter = 0;

            while (visibleCount < totalCharacters)
            {
                if (_skipRequested)
                {
                    // Show all text immediately
                    _dialogText.maxVisibleCharacters = totalCharacters;
                    break;
                }

                visibleCount++;
                _dialogText.maxVisibleCharacters = visibleCount;
                
                // Typewriter sound
                sfxCounter++;
                if (_typewriterSFX != null && _audioSource != null 
                    && sfxCounter >= _typewriterSFXInterval)
                {
                    _audioSource.PlayOneShot(_typewriterSFX);
                    sfxCounter = 0;
                }

                float speed = _skipRequested ? _fastTypewriterSpeed : _typewriterSpeed;
                yield return new WaitForSeconds(speed);
            }

            _isTyping = false;
            _continueIndicator.SetActive(true);
            OnTextDisplayComplete?.Invoke();
        }

        private IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float duration, Action onComplete = null)
        {
            float elapsed = 0f;
            cg.alpha = from;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                cg.alpha = Mathf.Lerp(from, to, elapsed / duration);
                yield return null;
            }
            
            cg.alpha = to;
            onComplete?.Invoke();
        }
*/
    }
