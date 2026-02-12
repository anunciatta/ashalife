public class Weapon : GenericCharacterPart
{
    public WeaponPosition weaponPosition;

    public virtual void ResetPart(WeaponSpriteConfig sprites)
    {
        if (spriteRenderer != null)
        {
            ChangeSprite(sprites);
        }
    }

    public void ChangeSprite(WeaponSpriteConfig WeaponSpriteConfig)
    {
        if (WeaponSpriteConfig == null)
        {
            spriteRenderer.sprite = null;
            return;
        }

        if (WeaponSpriteConfig.rightHand != null && weaponPosition == WeaponPosition.RightHand)
        {
            spriteRenderer.sprite = WeaponSpriteConfig.rightHand;
            return;
        }

        else
        {
            spriteRenderer.sprite = null;
        }

        if (WeaponSpriteConfig.back != null && weaponPosition == WeaponPosition.Back)
        {
            spriteRenderer.sprite = WeaponSpriteConfig.back;
            return;
        }

        else
        {
            spriteRenderer.sprite = null;
        }

        if (WeaponSpriteConfig.leftHand.Count == 1 && weaponPosition == WeaponPosition.LeftHand)
        {
            spriteRenderer.sprite = WeaponSpriteConfig.leftHand[0];
            return;
        }

        else
        {
            spriteRenderer.sprite = null;
        }

        if (WeaponSpriteConfig.leftHand.Count > 1 && weaponPosition == WeaponPosition.BowHandle)
        {
            spriteRenderer.sprite = WeaponSpriteConfig.leftHand[0];
            return;
        }

        else
        {
            spriteRenderer.sprite = null;
        }

        if (WeaponSpriteConfig.leftHand.Count > 1 && weaponPosition == WeaponPosition.BowLimbL)
        {
            spriteRenderer.sprite = WeaponSpriteConfig.leftHand[1];
            return;
        }

        else
        {
            spriteRenderer.sprite = null;
        }

        if (WeaponSpriteConfig.leftHand.Count > 1 && weaponPosition == WeaponPosition.BowLimbU)
        {
            spriteRenderer.sprite = WeaponSpriteConfig.leftHand[2];
            return;
        }

        else
        {
            spriteRenderer.sprite = null;
        }
    }
}

public enum WeaponPosition
{
    RightHand, LeftHand, BowHandle, BowLimbU, BowLimbL, Back
}