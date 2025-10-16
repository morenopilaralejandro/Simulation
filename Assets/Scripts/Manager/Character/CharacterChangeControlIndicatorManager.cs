using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Simulation.Enums.Input;

public class CharacterChangeControlIndicatorManager : MonoBehaviour
{
    public static CharacterChangeControlIndicatorManager Instance { get; private set; }

    private Transform indicator;
    private Character target => CharacterChangeControlManager.Instance.CurrentCharacter;

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

    void Start()
    {

    }

    void Update()
    {
        if (target && !BattleManager.Instance.IsTimeFrozen)
        {
            indicator.gameObject.SetActive(true);
            TrackTarget();
        } else 
        {
            indicator?.gameObject.SetActive(false);
        }
          
    }

    public void RegisterIndicator(Transform indicator)
    {
        this.indicator = indicator;
        indicator.gameObject.SetActive(false);
    }

    public void UnregisterIndicator()
    {
        this.indicator = null;
    }

    public void TrackTarget()
    {
        indicator.position = target.transform.position;
    }

}
