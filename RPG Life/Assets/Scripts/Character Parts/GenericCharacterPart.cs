using UnityEngine;

public class GenericCharacterPart : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    public virtual void Initialize()
    {
        if (spriteRenderer != null) return;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public virtual void ResetPart(Color color, Sprite sprite)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
            spriteRenderer.sprite = sprite;
        }
    }

    public void ChangeColor(Color color)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
        }
    }

    public void ChangeSprite(Sprite sprite)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = sprite;
        }
    }
}
