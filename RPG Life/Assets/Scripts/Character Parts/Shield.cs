
public class Shield : GenericCharacterPart
{
    public virtual void RemovePart()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = null;
        }
    }
}

