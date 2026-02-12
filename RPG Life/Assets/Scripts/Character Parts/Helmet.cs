using UnityEngine;

public class Helmet : GenericCharacterPart
{

    public void ChangeSprite(HelmetSpriteConfig helmetSpriteConfig)
    {
        if (helmetSpriteConfig == null)
        {
            spriteRenderer.sprite = null;

            foreach (var hair in References.Instance.characterCustomizerConfigurations.hairList)
                hair.ChangeSprite(GetSpriteFromArray(References.Instance.characterCustomizerConfigurations.hairSprites, References.Instance.player.currentCharacter.avatarConfig.hairIndex));

            foreach (var ear in References.Instance.characterCustomizerConfigurations.earsList)
                ear.ChangeSprite(GetSpriteFromArray(References.Instance.characterCustomizerConfigurations.earsSprites, References.Instance.player.currentCharacter.avatarConfig.earsIndex));
            return;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = helmetSpriteConfig.sprite;
        }

        if (helmetSpriteConfig.coverHair == true)
        {
            foreach (var hair in References.Instance.characterCustomizerConfigurations.hairList)
                hair.ResetPart(References.Instance.characterCustomizerConfigurations.hairColors[References.Instance.player.currentCharacter.avatarConfig.hairColorIndex], null);
        }

        else
        {
            foreach (var hair in References.Instance.characterCustomizerConfigurations.hairList)
                hair.ChangeSprite(GetSpriteFromArray(References.Instance.characterCustomizerConfigurations.hairSprites, References.Instance.player.currentCharacter.avatarConfig.hairIndex));
        }

        if (helmetSpriteConfig.coverEars == true)
        {
            foreach (var ear in References.Instance.characterCustomizerConfigurations.earsList)
                ear.ResetPart(References.Instance.characterCustomizerConfigurations.hairColors[References.Instance.player.currentCharacter.avatarConfig.skinColorIndex], null);
        }

        else
        {
            foreach (var ear in References.Instance.characterCustomizerConfigurations.earsList)
                ear.ChangeSprite(GetSpriteFromArray(References.Instance.characterCustomizerConfigurations.earsSprites, References.Instance.player.currentCharacter.avatarConfig.earsIndex));
        }
    }

    public virtual void RemovePart(ArmorSpriteConfig sprites)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = null;
        }
    }

    private Sprite GetSpriteFromArray(Sprite[] sprites, int index)
    {
        if (sprites == null || sprites.Length == 0)
            return null;

        return sprites[index % sprites.Length];
    }
}
