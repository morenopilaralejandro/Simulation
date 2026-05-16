using UnityEngine;

public interface ILayerRenderer
{
    void SetSprite(Sprite sprite);
    void SetColor(Color color);
    void SetVisible(bool visible);
    void SetActive(bool active);

    void PlayAnimation(string stateName);
}
