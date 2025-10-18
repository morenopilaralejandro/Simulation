using UnityEngine;

public class CharacterPossessionIndicator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer indicatorRenderer;

    private void Update()
    {
        var currentCharacter = PossessionManager.Instance.CurrentCharacter;

        bool shouldShow = currentCharacter != null;
        if (indicatorRenderer.enabled != shouldShow)
            indicatorRenderer.enabled = shouldShow;
        
        if (shouldShow)
            transform.position = currentCharacter.transform.position;
    }
}
