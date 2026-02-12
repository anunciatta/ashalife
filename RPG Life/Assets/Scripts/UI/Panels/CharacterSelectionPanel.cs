using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionPanel : Panel
{
    [SerializeField] private TextMeshProUGUI classText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button rightArrowButton;
    [SerializeField] private Button leftArrowButton;
    [SerializeField] private Button newCharacterButton;
    [SerializeField] private Button beginJourneyButton;
    [SerializeField] private Button deleteCharacterButton;

    public List<Character> characters = new();
    public int characterIndex = 0;

    private int eyeIndex = 0;
    private int beardIndex = 0;
    private int hairIndex = 0;
    private int headIndex = 0;
    private int eyebrowsIndex = 0;
    private int earsIndex = 0;
    private int mouthIndex = 0;
    private int makeupIndex = 0;
    private int accessoriesIndex = 0;
    private int classesIndex = 0;

    [SerializeField] private GameObject loading;

    private ItemSO armor, shield, weapon, wing, pet, helmet, jewelry;

    private int skinColorIndex, eyesColorIndex, makeupColorIndex, hairColorIndex, beardColorIndex;

    CharacterCustomizerStatus characterCustomizerStatus;

    public override void Initialize()
    {
        characterCustomizerStatus = panel.GetComponentInChildren<CharacterCustomizerStatus>(true);

        leftArrowButton.onClick.AddListener(OnLeftArrowPressed);
        rightArrowButton.onClick.AddListener(OnRightArrowPressed);
        continueButton.onClick.AddListener(OnContinuePressed);
        deleteCharacterButton.onClick.AddListener(OnDeleteCharacter);
        newCharacterButton.onClick.AddListener(OnNewCharacterButton);
        beginJourneyButton.onClick.AddListener(OnNewCharacterButton);

        base.Initialize();
        loading.SetActive(true);
    }

    void OnDestroy()
    {
        leftArrowButton.onClick.RemoveAllListeners();
        rightArrowButton.onClick.RemoveAllListeners();
        continueButton.onClick.RemoveAllListeners();
        deleteCharacterButton.onClick.RemoveAllListeners();
        newCharacterButton.onClick.RemoveAllListeners();
        beginJourneyButton.onClick.RemoveAllListeners();
    }

    public override void Open()
    {
        loading.SetActive(true);
        base.Open();
        characters = new();
        GetAllCharacters();
        characterIndex = 0;

    }

    async void GetAllCharacters()
    {
        characters = await FirebaseSaveManager.GetAllCharacters();

        if (characters.Count > 0)
        {
            SetUpCharacter(characters[characterIndex]);

            foreach (Character character in characters)
            {
                References.Instance.player.allCharacters.Add(character);
                for (int i = 0; i < character.statuses.Length; i++)
                {
                    character.statuses[i].status = (Status)i;
                }
            }


            DisplayObjects(true);
            SetButtonsInteractable(true);
        }

        else
        {

            DisplayObjects(false);
            SetButtonsInteractable(false);
            beginJourneyButton.interactable = true;
        }

        loading.SetActive(false);
    }

    public void OnBack()
    {
        PlayerPrefs.SetInt(References.Instance.firebaseManager.REMEMBER_ME_KEY, 0);
        References.Instance.firebaseManager.SignOut();
        Bus<TransitionPanelsEvent>.CallEvent(new TransitionPanelsEvent(this, previousPanel));
    }

    private void SetUpCharacter(Character character)
    {
        eyeIndex = character.avatarConfig.eyeIndex;

        beardIndex = character.avatarConfig.beardIndex;
        hairIndex = character.avatarConfig.hairIndex;
        classesIndex = character.avatarConfig.classesIndex;
        headIndex = character.avatarConfig.headIndex;
        eyebrowsIndex = character.avatarConfig.eyebrowsIndex;
        earsIndex = character.avatarConfig.earsIndex;
        mouthIndex = character.avatarConfig.mouthIndex;
        makeupIndex = character.avatarConfig.makeupIndex;
        accessoriesIndex = character.avatarConfig.accessoriesIndex;

        hairColorIndex = character.avatarConfig.hairColorIndex;
        beardColorIndex = character.avatarConfig.beardColorIndex;
        skinColorIndex = character.avatarConfig.skinColorIndex;
        eyesColorIndex = character.avatarConfig.eyesColorIndex;
        makeupColorIndex = character.avatarConfig.makeupColorIndex;

        Bus<SkinColorPickedEvent>.CallEvent(new SkinColorPickedEvent(skinColorIndex));
        Bus<BeardColorPickedEvent>.CallEvent(new BeardColorPickedEvent(beardColorIndex));
        Bus<HairColorPickedEvent>.CallEvent(new HairColorPickedEvent(hairColorIndex));
        Bus<MakeupColorPickedEvent>.CallEvent(new MakeupColorPickedEvent(makeupColorIndex));
        Bus<EyesColorPickedEvent>.CallEvent(new EyesColorPickedEvent(eyesColorIndex));

        Bus<EyesSpriteChangeEvent>.CallEvent(new EyesSpriteChangeEvent(GetSpriteFromArray(References.Instance.characterCustomizerConfigurations.eyeSprites, eyeIndex)));
        Bus<HairSpriteChangeEvent>.CallEvent(new HairSpriteChangeEvent(GetSpriteFromArray(References.Instance.characterCustomizerConfigurations.hairSprites, hairIndex)));
        Bus<BeardSpriteChangeEvent>.CallEvent(new BeardSpriteChangeEvent(GetSpriteFromArray(References.Instance.characterCustomizerConfigurations.beardSprites, beardIndex)));
        Bus<HeadSpriteChangeEvent>.CallEvent(new HeadSpriteChangeEvent(GetSpriteFromArray(References.Instance.characterCustomizerConfigurations.headSprites, headIndex)));
        Bus<EyebrowsSpriteChangeEvent>.CallEvent(new EyebrowsSpriteChangeEvent(GetSpriteFromArray(References.Instance.characterCustomizerConfigurations.eyebrowsSprites, eyebrowsIndex)));
        Bus<EarSpriteChangeEvent>.CallEvent(new EarSpriteChangeEvent(GetSpriteFromArray(References.Instance.characterCustomizerConfigurations.earsSprites, earsIndex)));
        Bus<MouthSpriteChangeEvent>.CallEvent(new MouthSpriteChangeEvent(GetSpriteFromArray(References.Instance.characterCustomizerConfigurations.mouthSprites, mouthIndex)));
        Bus<MakeupSpriteChangeEvent>.CallEvent(new MakeupSpriteChangeEvent(GetSpriteFromArray(References.Instance.characterCustomizerConfigurations.makeupSprites, makeupIndex)));
        Bus<AccessorySpriteChangeEvent>.CallEvent(new AccessorySpriteChangeEvent(GetSpriteFromArray(References.Instance.characterCustomizerConfigurations.accessoriesSprites, accessoriesIndex)));

        armor = References.Instance.inventory.GetItemFromID(character.equippedItems.armor);
        weapon = References.Instance.inventory.GetItemFromID(character.equippedItems.weapon);
        shield = References.Instance.inventory.GetItemFromID(character.equippedItems.shield);
        helmet = References.Instance.inventory.GetItemFromID(character.equippedItems.helmet);
        wing = References.Instance.inventory.GetItemFromID(character.equippedItems.wings);
        jewelry = References.Instance.inventory.GetItemFromID(character.equippedItems.jewelry);

        CallSpritesEvents(armor);
        CallSpritesEvents(weapon);
        CallSpritesEvents(helmet);
        CallSpritesEvents(wing);
        CallSpritesEvents(shield);
        CallSpritesEvents(jewelry);

        if (armor != null)
        {
            ArmorSO armorSO = armor as ArmorSO;
            if (armorSO != null)
                Bus<ArmorSpriteChangeEvent>.CallEvent(new ArmorSpriteChangeEvent(armorSO.armorSprites));
            else
            {
                Debug.LogError("Item is not an ArmorSO!");
            }
        }
        else
        {
            Bus<ArmorSpriteChangeEvent>.CallEvent(new ArmorSpriteChangeEvent(null));
        }

        if (weapon != null)
        {
            WeaponSO weaponSO = weapon as WeaponSO;
            if (weaponSO != null)
                Bus<WeaponSpriteChangeEvent>.CallEvent(new WeaponSpriteChangeEvent(weaponSO.weaponSprites));
            else
            {
                Debug.LogError("Item is not an WeaponSO!");
            }
        }

        else
        {
            Bus<WeaponSpriteChangeEvent>.CallEvent(new WeaponSpriteChangeEvent(null));
        }

        if (helmet != null)
        {
            HelmetSO helmetSO = helmet as HelmetSO;
            if (helmetSO != null)
                Bus<HelmetSpriteChangeEvent>.CallEvent(new HelmetSpriteChangeEvent(helmetSO.helmetSpriteConfig));
            else
            {
                Debug.LogError("Item is not an HelmetSO!");
            }
        }

        else
        {
            Bus<HelmetSpriteChangeEvent>.CallEvent(new HelmetSpriteChangeEvent(null));
            Bus<EarSpriteChangeEvent>.CallEvent(new EarSpriteChangeEvent(GetSpriteFromArray(References.Instance.characterCustomizerConfigurations.earsSprites, earsIndex)));

        }

        if (shield != null)
        {
            ShieldSO shieldSO = shield as ShieldSO;
            WeaponSO weaponSO = shield as WeaponSO;

            if (shieldSO != null)
            {
                Bus<ShieldSpriteChangeEvent>.CallEvent(new ShieldSpriteChangeEvent(shieldSO.sprite));
            }

            else if (weaponSO != null)
            {
                Bus<SecondWeaponSpriteChangeEvent>.CallEvent(new SecondWeaponSpriteChangeEvent(weaponSO.weaponSprites));

            }
            else
            {
                Debug.LogError("Item is not an ShieldSO!");
            }
        }
        else
        {
            Bus<SecondWeaponSpriteChangeEvent>.CallEvent(new SecondWeaponSpriteChangeEvent(null));
        }

        if (wing != null)
        {
            WingSO wingSO = wing as WingSO;
            if (wingSO != null)
                Bus<WingSpriteChangeEvent>.CallEvent(new WingSpriteChangeEvent(wingSO.sprite));
            else
            {
                Debug.LogError("Item is not an WingSO!");
            }
        }

        else
        {
            Bus<WingSpriteChangeEvent>.CallEvent(new WingSpriteChangeEvent(null));
        }

        var modifiers = References.Instance.inventory.GetAllEquippedItensModifiers(character);

        characterCustomizerStatus.UpdateCurrentValues(character, modifiers);

        classText.text = $"{References.Instance.characterCustomizerConfigurations.classes[character.avatarConfig.classesIndex].classType}";
        levelText.text = $"Level {character.level}";
    }

    private void CallSpritesEvents(ItemSO item)
    {
        if (item == null) return;

        switch (item)
        {
            case ArmorSO armorSO:
                Bus<ArmorSpriteChangeEvent>.CallEvent(new ArmorSpriteChangeEvent(armorSO.armorSprites));
                break;

            case WeaponSO weaponSO:
                Bus<WeaponSpriteChangeEvent>.CallEvent(new WeaponSpriteChangeEvent(weaponSO.weaponSprites));
                break;

            case HelmetSO helmetSO:
                Bus<HelmetSpriteChangeEvent>.CallEvent(new HelmetSpriteChangeEvent(helmetSO.helmetSpriteConfig));
                break;

            case ShieldSO shieldSO:
                Bus<ShieldSpriteChangeEvent>.CallEvent(new ShieldSpriteChangeEvent(shieldSO.sprite));
                break;

            default:
                Debug.LogWarning($"Unknown item type: {armor.GetType().Name}");
                break;
        }
    }

    private Sprite GetSpriteFromArray(Sprite[] sprites, int index)
    {
        if (sprites == null || sprites.Length == 0)
            return null;

        return sprites[index % sprites.Length];
    }

    [SerializeField] private Panel characterCustomizationPanel;

    private void OnNewCharacterButton()
    {
        if (characters.Count >= 3)
        {
            return;
        }

        else
        {
            Bus<TransitionPanelsEvent>.CallEvent(new TransitionPanelsEvent(this, characterCustomizationPanel));
        }
    }

    private void OnContinuePressed()
    {
        if (characters.Count <= 0) return;

        References.Instance.player.SetCurrentCharater(characters[characterIndex]);

        Bus<OnClassChangeEvent>.CallEvent(new OnClassChangeEvent(References.Instance.characterCustomizerConfigurations.classes[characters[characterIndex].avatarConfig.classesIndex]));

        Bus<OnCharacterLoad>.CallEvent(new OnCharacterLoad(characters[characterIndex]));
        Bus<TransitionPanelsEvent>.CallEvent(new TransitionPanelsEvent(this, nextPanel));
    }

    [SerializeField] private GameObject[] displayObjects;
    [SerializeField] private GameObject[] hideObjects;

    async private void OnDeleteCharacter()
    {
        if (characters.Count <= 0) return;

        try
        {
            SetButtonsInteractable(false);

            var id = characters[characterIndex].characterId;
            characters.Remove(characters[characterIndex]);

            bool success = await FirebaseSaveManager.RemoveCharacter(id);

            if (success)
            {
                if (characters.Count <= 0)
                {
                    DisplayObjects(false);
                }

                else
                {
                    DisplayObjects(true);
                    characterIndex = 0;
                    SetUpCharacter(characters[characterIndex]);
                }
            }

            else
            {
                Debug.LogWarning($"Error deleting character.");
                Bus<OnAddToast>.CallEvent(new OnAddToast(new PopupConfig(MarkStatus.Neutral, References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.somethingWrong}"), PopupType.System)));
            }



        }
        catch (Exception e)
        {
            Debug.LogWarning($"Error deleting character: {e.Message}");
            return;
        }

        finally
        {
            SetButtonsInteractable(true);
        }




    }

    private void DisplayObjects(bool value)
    {
        foreach (var obj in displayObjects)
        {
            obj.SetActive(value);
        }

        foreach (var obj in hideObjects)
        {
            obj.SetActive(!value);
        }
    }

    private void SetButtonsInteractable(bool isInteractable)
    {
        deleteCharacterButton.interactable = isInteractable;
        newCharacterButton.interactable = isInteractable;
        rightArrowButton.interactable = isInteractable;
        leftArrowButton.interactable = isInteractable;
        beginJourneyButton.interactable = isInteractable;
    }

    private void OnRightArrowPressed()
    {

        if (characters.Count <= 0) return;

        characterIndex = (characterIndex + 1) % characters.Count;
        SetUpCharacter(characters[characterIndex]);
    }

    private void OnLeftArrowPressed()
    {
        if (characters.Count <= 0) return;

        characterIndex = (characterIndex - 1 + characters.Count) % characters.Count;
        SetUpCharacter(characters[characterIndex]);
    }
}




