using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Move;
using Aremoreno.Enums.Duel;

public class WorldUIManager : MonoBehaviour
{
    public static WorldUIManager Instance { get; private set; }

    public TransitionScreen TransitionScreen { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    #region Registration
    public void RegisterTransitionScreen(TransitionScreen transitionScreen)
    {
        TransitionScreen = transitionScreen;
    }

    public void UnregisterTransitionScreen()
    {
        TransitionScreen = null;
    }
    #endregion

}
