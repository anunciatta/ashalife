public class SecondWeapon : GenericCharacterPart
{
    public virtual void RemovePart()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = null;
        }
    }
}

