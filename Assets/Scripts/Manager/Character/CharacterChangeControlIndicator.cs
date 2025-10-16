using UnityEngine;

public class CharacterChangeControlIndicator : MonoBehaviour
{
    private void Awake()
    {
        if (CharacterChangeControlIndicatorManager.Instance != null)
            CharacterChangeControlIndicatorManager.Instance.RegisterIndicator(this.transform);
    }

    private void OnDestroy()
    {
        if (CharacterChangeControlIndicatorManager.Instance != null)
            CharacterChangeControlIndicatorManager.Instance.UnregisterIndicator();
    }

    private void Start() 
    {

    }
}
