using UnityEngine;

public class Body : GenericCharacterPart
{
    [SerializeField] private bool isHead = false;
    [SerializeField] private bool isEar = false;

    public bool IsEar() => isEar;
    public bool IsHead() => isHead;

    public override void ResetPart(Color color, Sprite sprite)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;

            if (isHead || isEar)
            {
                spriteRenderer.sprite = sprite;
            }
        }
    }
}
