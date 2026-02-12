public class Wing : GenericCharacterPart
{
    public virtual void RemovePart()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = null;
        }
    }
}