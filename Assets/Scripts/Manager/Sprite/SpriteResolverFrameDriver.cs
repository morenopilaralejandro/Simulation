using UnityEngine;
using UnityEngine.U2D.Animation;

[RequireComponent(typeof(SpriteResolver))]
public class SpriteResolverFrameDriver : MonoBehaviour
{
    [SerializeField] private SpriteResolver resolver;
    [SerializeField] private string category;
    [SerializeField] private string direction;

    [SerializeField] private int frame;

    private int lastFrame = -1;

    private void Reset()
    {
        resolver = GetComponent<SpriteResolver>();
    }

    private void LateUpdate()
    {
        if (frame != lastFrame)
        {
            Apply();
        }
    }

    private void Apply()
    {
        if (resolver == null) return;

        lastFrame = frame;

        resolver.SetCategoryAndLabel(
            category,
            $"{category}_{direction}_{frame}"
        );
    }

    public void Configure(string animCategory, string animDirection)
    {
        category = animCategory;
        direction = animDirection;
        //lastFrame = -1;
    }
}
