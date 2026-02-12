public class Armor : GenericCharacterPart
{
    public int index = 0;

    public void ChangeSprite(ArmorSpriteConfig armorSpriteConfig)
    {
        if (armorSpriteConfig == null)
        {
            spriteRenderer.sprite = null;
            return;
        }


        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = armorSpriteConfig.sprites[index];
        }
    }

    public virtual void ResetPart(ArmorSpriteConfig armorSpriteConfig)
    {
        if (armorSpriteConfig == null)
        {
            spriteRenderer.sprite = null;
            return;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = armorSpriteConfig.sprites[index];
        }
    }
}
