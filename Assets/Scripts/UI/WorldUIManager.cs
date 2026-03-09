using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Simulation.Enums.Character;
using Simulation.Enums.Battle;
using Simulation.Enums.Move;
using Simulation.Enums.Duel;

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
