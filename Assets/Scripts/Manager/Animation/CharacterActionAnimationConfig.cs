using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.Animation;

[CreateAssetMenu(
    fileName = "CharacterActionAnimationConfig",
    menuName = "ScriptableObject/Animation/Character Action Animation Config"
)]
public class CharacterActionAnimationConfig : ScriptableObject
{
    [SerializeField]
    private List<ActionAnimationData> animations =
        new List<ActionAnimationData>();

    private Dictionary<CharacterAnimationState, ActionAnimationData>
        cachedDatabase;

    public IReadOnlyDictionary<CharacterAnimationState, ActionAnimationData>
        Database
    {
        get
        {
            if (cachedDatabase == null)
                BuildDatabase();

            return cachedDatabase;
        }
    }

    public bool TryGetData(
        CharacterAnimationState state,
        out ActionAnimationData data)
    {
        return cachedDatabase.TryGetValue(state, out data);
    }

    public void BuildDatabase()
    {
        cachedDatabase =
            new Dictionary<CharacterAnimationState, ActionAnimationData>();

        for (int i = 0; i < animations.Count; i++)
        {
            ActionAnimationData data = animations[i];

            if (cachedDatabase.ContainsKey(data.State))
            {
                Debug.LogWarning(
                    $"Duplicate animation state: {data.State}",
                    this);

                continue;
            }

            cachedDatabase.Add(data.State, data);
        }
    }

#if UNITY_EDITOR

    private void OnValidate()
    {
        cachedDatabase = null;
    }

#endif
}

[System.Serializable]
public struct ActionAnimationData
{
    public CharacterAnimationState State;
    [Min(0f)]
    public float Duration;
    public AnimationPriority Priority;
    public bool LocksMovement;
    public bool Interruptible;
    public AnimationFacingMode FacingMode;

}
