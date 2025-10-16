using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Simulation.Enums.Input;

public class CharacterChangeControlManager : MonoBehaviour
{
    public static CharacterChangeControlManager Instance { get; private set; }

    private List<Character> teammates => BattleManager.Instance.Teams[BattleTeamManager.Instance.GetUserSide()].CharacterList;
    private Ball ball => BattleManager.Instance.Ball;
    private int teamSize => BattleManager.Instance.CurrentTeamSize;
    [SerializeField] private Character currentCharacter = null;

    public Character CurrentCharacter => currentCharacter;

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
        if (!BattleManager.Instance.IsTimeFrozen && InputManager.Instance.GetDown(BattleAction.Change))
            ChangeToBallNearest();
    }

    public void ChangeToBallNearest()
    {
        Character nearestCharacter = currentCharacter;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < teamSize; i++)
        {
            float dist = Vector3.Distance(ball.transform.position, teammates[i].transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                nearestCharacter = teammates[i];
            }
        }

        // Only switch if we actually found a different player
        if (nearestCharacter != currentCharacter)
            currentCharacter = nearestCharacter;
    }

}
