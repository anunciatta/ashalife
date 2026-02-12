using UnityEngine;

public class AvatarController : MonoBehaviour
{
    [SerializeField] private Body[] bodies;

    void Awake()
    {
        Bus<HairColorPickedEvent>.OnEvent += SetHairColor;
        Bus<HairSpriteChangeEvent>.OnEvent += ChangeHairSprite;

        Bus<EyesColorPickedEvent>.OnEvent += SetEyesColor;
        Bus<EyesSpriteChangeEvent>.OnEvent += ChangeEyesSprite;

        Bus<MouthSpriteChangeEvent>.OnEvent += ChangeMouthSprite;

        Bus<BeardColorPickedEvent>.OnEvent += SetBeardColor;
        Bus<BeardSpriteChangeEvent>.OnEvent += ChangeBeardSprite;

        Bus<ArmorSpriteChangeEvent>.OnEvent += ChangeArmorSprite;

        Bus<HelmetSpriteChangeEvent>.OnEvent += ChangeHelmetSprite;
        Bus<ShieldSpriteChangeEvent>.OnEvent += ChangeShieldSprite;

        Bus<WeaponSpriteChangeEvent>.OnEvent += ChangeWeaponSprite;

        Bus<MakeupSpriteChangeEvent>.OnEvent += ChangeMakeupSprite;
        Bus<WingSpriteChangeEvent>.OnEvent += ChangeWingSprite;
        Bus<MakeupColorPickedEvent>.OnEvent += SetMakeupColor;

        Bus<SecondWeaponSpriteChangeEvent>.OnEvent += ChangeSecondWeaponSprite;
        Bus<AccessorySpriteChangeEvent>.OnEvent += ChangeAccessorySprite;

        Bus<SkinColorPickedEvent>.OnEvent += SetSkinColor;
        Bus<HeadSpriteChangeEvent>.OnEvent += ChangeHeadSprite;
        Bus<EarSpriteChangeEvent>.OnEvent += ChangeEarSprite;

    }


    void OnDestroy()
    {
        Bus<HairColorPickedEvent>.OnEvent -= SetHairColor;
        Bus<HairSpriteChangeEvent>.OnEvent -= ChangeHairSprite;

        Bus<EyesColorPickedEvent>.OnEvent -= SetEyesColor;
        Bus<EyesSpriteChangeEvent>.OnEvent -= ChangeEyesSprite;

        Bus<MouthSpriteChangeEvent>.OnEvent -= ChangeMouthSprite;

        Bus<BeardColorPickedEvent>.OnEvent -= SetBeardColor;
        Bus<BeardSpriteChangeEvent>.OnEvent -= ChangeBeardSprite;

        Bus<ArmorSpriteChangeEvent>.OnEvent -= ChangeArmorSprite;

        Bus<HelmetSpriteChangeEvent>.OnEvent -= ChangeHelmetSprite;
        Bus<ShieldSpriteChangeEvent>.OnEvent -= ChangeShieldSprite;

        Bus<SecondWeaponSpriteChangeEvent>.OnEvent -= ChangeSecondWeaponSprite;
        Bus<WingSpriteChangeEvent>.OnEvent -= ChangeWingSprite;

        Bus<WeaponSpriteChangeEvent>.OnEvent -= ChangeWeaponSprite;

        Bus<MakeupSpriteChangeEvent>.OnEvent -= ChangeMakeupSprite;
        Bus<MakeupColorPickedEvent>.OnEvent -= SetMakeupColor;

        Bus<AccessorySpriteChangeEvent>.OnEvent -= ChangeAccessorySprite;

        Bus<SkinColorPickedEvent>.OnEvent -= SetSkinColor;
        Bus<HeadSpriteChangeEvent>.OnEvent -= ChangeHeadSprite;
        Bus<EarSpriteChangeEvent>.OnEvent -= ChangeEarSprite;

    }

    #region Hair

    public void SetHairColor(HairColorPickedEvent _event)
    {
        foreach (Hair hair in References.Instance.characterCustomizerConfigurations.hairList)
        {
            hair.ChangeColor(References.Instance.characterCustomizerConfigurations.hairColors[_event.Color]);
        }
    }

    public void ChangeHairSprite(HairSpriteChangeEvent _event)
    {
        foreach (Hair hair in References.Instance.characterCustomizerConfigurations.hairList)
        {
            hair.ChangeSprite(_event.Icon);
        }
    }

    #endregion

    #region Eyes
    public void SetEyesColor(EyesColorPickedEvent _event)
    {
        foreach (Eyes part in References.Instance.characterCustomizerConfigurations.eyesList)
        {
            part.ChangeColor(References.Instance.characterCustomizerConfigurations.eyesColors[_event.Color]);
        }
    }

    public void ChangeEyesSprite(EyesSpriteChangeEvent _event)
    {
        foreach (Eyes part in References.Instance.characterCustomizerConfigurations.eyesList)
        {
            part.ChangeSprite(_event.Icon);
        }
    }
    #endregion

    #region Mouth

    public void ChangeMouthSprite(MouthSpriteChangeEvent _event)
    {
        foreach (Mouth part in References.Instance.characterCustomizerConfigurations.mouthList)
        {
            part.ChangeSprite(_event.Icon);
        }
    }
    #endregion

    #region Beard
    public void SetBeardColor(BeardColorPickedEvent _event)
    {
        foreach (Beard part in References.Instance.characterCustomizerConfigurations.beardList)
        {
            part.ChangeColor(References.Instance.characterCustomizerConfigurations.hairColors[_event.Color]);
        }
    }

    public void ChangeBeardSprite(BeardSpriteChangeEvent _event)
    {
        foreach (Beard part in References.Instance.characterCustomizerConfigurations.beardList)
        {
            part.ChangeSprite(_event.Icon);
        }
    }
    #endregion

    #region Armor

    public void ChangeArmorSprite(ArmorSpriteChangeEvent _event)
    {
        foreach (Armor part in References.Instance.characterCustomizerConfigurations.armorsList)
        {
            part.ChangeSprite(_event.ArmorSpriteConfig);
        }
    }
    #endregion

    #region Shield

    public void ChangeShieldSprite(ShieldSpriteChangeEvent _event)
    {
        foreach (Shield part in References.Instance.characterCustomizerConfigurations.shieldList)
        {
            part.ChangeSprite(_event.Sprite);
        }
    }

    #endregion

    #region Helmet

    public void ChangeHelmetSprite(HelmetSpriteChangeEvent _event)
    {
        foreach (Helmet part in References.Instance.characterCustomizerConfigurations.helmetList)
        {
            part.ChangeSprite(_event.HelmetSpriteConfig);
        }
    }

    #endregion

    #region Weapon

    public void ChangeWeaponSprite(WeaponSpriteChangeEvent _event)
    {
        foreach (Weapon part in References.Instance.characterCustomizerConfigurations.weaponList)
        {
            part.ChangeSprite(_event.WeaponSpriteConfig);
        }
    }

    private void ChangeSecondWeaponSprite(SecondWeaponSpriteChangeEvent _event)
    {

        if (_event.WeaponSpriteConfig != null)
        {
            foreach (SecondWeapon part in References.Instance.characterCustomizerConfigurations.secondWeapon)
            {
                part.ChangeSprite(_event.WeaponSpriteConfig.leftHand[0]);
            }
        }

        else
        {
            foreach (SecondWeapon part in References.Instance.characterCustomizerConfigurations.secondWeapon)
            {
                part.ChangeSprite(null);
            }
        }

    }
    #endregion


    #region Wings

    private void ChangeWingSprite(WingSpriteChangeEvent _event)
    {
        foreach (Wing part in References.Instance.characterCustomizerConfigurations.wingList)
        {
            part.ChangeSprite(_event.Sprite);
        }
    }

    #endregion

    #region Makeup
    public void SetMakeupColor(MakeupColorPickedEvent _event)
    {
        foreach (Makeup part in References.Instance.characterCustomizerConfigurations.makeupList)
        {
            part.ChangeColor(References.Instance.characterCustomizerConfigurations.makeupColors[_event.Color]);
        }
    }

    public void ChangeMakeupSprite(MakeupSpriteChangeEvent _event)
    {
        foreach (Makeup part in References.Instance.characterCustomizerConfigurations.makeupList)
        {
            part.ChangeSprite(_event.Icon);
        }
    }
    #endregion


    #region Accessory

    public void ChangeAccessorySprite(AccessorySpriteChangeEvent _event)
    {
        foreach (Accessory part in References.Instance.characterCustomizerConfigurations.accessoriesList)
        {
            part.ChangeSprite(_event.Icon);
        }
    }
    #endregion

    #region Body

    public void ChangeHeadSprite(HeadSpriteChangeEvent _event)
    {
        foreach (Body part in References.Instance.characterCustomizerConfigurations.headList)
        {
            part.ChangeSprite(_event.Icon);
        }
    }

    public void ChangeEarSprite(EarSpriteChangeEvent _event)
    {
        foreach (Body part in References.Instance.characterCustomizerConfigurations.earsList)
        {
            part.ChangeSprite(_event.Icon);
        }
    }

    public void SetSkinColor(SkinColorPickedEvent _event)
    {
        foreach (Body part in References.Instance.characterCustomizerConfigurations.earsList)
        {
            part.ChangeColor(References.Instance.characterCustomizerConfigurations.skinColors[_event.Color]);
        }

        foreach (Body part in References.Instance.characterCustomizerConfigurations.headList)
        {
            part.ChangeColor(References.Instance.characterCustomizerConfigurations.skinColors[_event.Color]);
        }

        foreach (Body part in References.Instance.characterCustomizerConfigurations.bodyList)
        {
            part.ChangeColor(References.Instance.characterCustomizerConfigurations.skinColors[_event.Color]);
        }
    }

    #endregion
}