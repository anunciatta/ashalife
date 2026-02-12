using System;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCustomizer : Panel
{
    public int classesIndex = 0;
    [HideInInspector] public CharacterCustomizationButton[] customizationButtons;

    private GenericColorPickerButton[] colorPickerButtons;
    [SerializeField] private GameObject[] colorPickerPanels;

    [SerializeField] private Button leftArrowButton;
    [SerializeField] private Button rightArrowButton;
    [SerializeField] private Button continueButton;

    CharacterCustomizerStatus characterCustomizerStatus;

    private int eyeIndex = 0;
    private int beardIndex = 0;
    private int hairIndex = 0;
    private int headIndex = 0;
    private int eyebrowsIndex = 0;
    private int earsIndex = 0;
    private int mouthIndex = 0;
    private int makeupIndex = 0;
    private int accessoriesIndex = 0;

    private int skinColorIndex, eyesColorIndex, makeupColorIndex, hairColorIndex, beardColorIndex;


    [HideInInspector] public CharacterPartType currentPartType;

    private void OnDestroy()
    {
        leftArrowButton.onClick.RemoveAllListeners();
        rightArrowButton.onClick.RemoveAllListeners();
        continueButton.onClick.RemoveAllListeners();

        Bus<HairColorPickedEvent>.OnEvent -= _event => hairColorIndex = _event.Color;
        Bus<SkinColorPickedEvent>.OnEvent -= _event => skinColorIndex = _event.Color;
        Bus<BeardColorPickedEvent>.OnEvent -= _event => beardColorIndex = _event.Color;
        Bus<MakeupColorPickedEvent>.OnEvent -= _event => makeupColorIndex = _event.Color;
        Bus<EyesColorPickedEvent>.OnEvent -= _event => eyesColorIndex = _event.Color;

        Bus<LoadCharacterEvent>.OnEvent -= LoadCurrentCharacter;
    }

    void Awake()
    {
        Bus<LoadCharacterEvent>.OnEvent += LoadCurrentCharacter;

        Bus<HairColorPickedEvent>.OnEvent += _event => hairColorIndex = _event.Color;
        Bus<SkinColorPickedEvent>.OnEvent += _event => skinColorIndex = _event.Color;
        Bus<BeardColorPickedEvent>.OnEvent += _event => beardColorIndex = _event.Color;
        Bus<MakeupColorPickedEvent>.OnEvent += _event => makeupColorIndex = _event.Color;
        Bus<EyesColorPickedEvent>.OnEvent += _event => eyesColorIndex = _event.Color;
    }

    private void LoadCurrentCharacter(LoadCharacterEvent data)
    {
        eyeIndex = data.Character.avatarConfig.eyeIndex;
        beardIndex = data.Character.avatarConfig.beardIndex;
        hairIndex = data.Character.avatarConfig.hairIndex;
        classesIndex = data.Character.avatarConfig.classesIndex;
        headIndex = data.Character.avatarConfig.headIndex;
        eyebrowsIndex = data.Character.avatarConfig.eyebrowsIndex;
        earsIndex = data.Character.avatarConfig.earsIndex;
        mouthIndex = data.Character.avatarConfig.mouthIndex;
        makeupIndex = data.Character.avatarConfig.makeupIndex;
        accessoriesIndex = data.Character.avatarConfig.accessoriesIndex;

        hairColorIndex = data.Character.avatarConfig.hairColorIndex;
        beardColorIndex = data.Character.avatarConfig.beardColorIndex;
        skinColorIndex = data.Character.avatarConfig.skinColorIndex;
        eyesColorIndex = data.Character.avatarConfig.eyesColorIndex;
        makeupColorIndex = data.Character.avatarConfig.makeupColorIndex;

        Bus<EyesSpriteChangeEvent>.CallEvent(new EyesSpriteChangeEvent(GetSpriteFromArray(References.Instance.characterCustomizerConfigurations.eyeSprites, eyeIndex)));
        Bus<HairSpriteChangeEvent>.CallEvent(new HairSpriteChangeEvent(GetSpriteFromArray(References.Instance.characterCustomizerConfigurations.hairSprites, hairIndex)));
        Bus<BeardSpriteChangeEvent>.CallEvent(new BeardSpriteChangeEvent(GetSpriteFromArray(References.Instance.characterCustomizerConfigurations.beardSprites, beardIndex)));
        Bus<HeadSpriteChangeEvent>.CallEvent(new HeadSpriteChangeEvent(GetSpriteFromArray(References.Instance.characterCustomizerConfigurations.headSprites, headIndex)));
        Bus<EyebrowsSpriteChangeEvent>.CallEvent(new EyebrowsSpriteChangeEvent(GetSpriteFromArray(References.Instance.characterCustomizerConfigurations.eyebrowsSprites, eyebrowsIndex)));
        Bus<EarSpriteChangeEvent>.CallEvent(new EarSpriteChangeEvent(GetSpriteFromArray(References.Instance.characterCustomizerConfigurations.earsSprites, earsIndex)));
        Bus<MouthSpriteChangeEvent>.CallEvent(new MouthSpriteChangeEvent(GetSpriteFromArray(References.Instance.characterCustomizerConfigurations.mouthSprites, mouthIndex)));
        Bus<MakeupSpriteChangeEvent>.CallEvent(new MakeupSpriteChangeEvent(GetSpriteFromArray(References.Instance.characterCustomizerConfigurations.makeupSprites, makeupIndex)));
        Bus<AccessorySpriteChangeEvent>.CallEvent(new AccessorySpriteChangeEvent(GetSpriteFromArray(References.Instance.characterCustomizerConfigurations.accessoriesSprites, accessoriesIndex)));
        Bus<ArmorSpriteChangeEvent>.CallEvent(new ArmorSpriteChangeEvent(References.Instance.characterCustomizerConfigurations.classes[classesIndex].initialConfig.armorSO.armorSprites));
        Bus<WeaponSpriteChangeEvent>.CallEvent(new WeaponSpriteChangeEvent(null));
        Bus<SkinColorPickedEvent>.CallEvent(new SkinColorPickedEvent(skinColorIndex));
        Bus<BeardColorPickedEvent>.CallEvent(new BeardColorPickedEvent(beardColorIndex));
        Bus<HairColorPickedEvent>.CallEvent(new HairColorPickedEvent(hairColorIndex));
        Bus<MakeupColorPickedEvent>.CallEvent(new MakeupColorPickedEvent(makeupColorIndex));
        Bus<EyesColorPickedEvent>.CallEvent(new EyesColorPickedEvent(eyesColorIndex));

        characterCustomizerStatus.UpdateValues(References.Instance.characterCustomizerConfigurations.classes[classesIndex]);
    }

    public void OnBack()
    {
        Bus<TransitionPanelsEvent>.CallEvent(new TransitionPanelsEvent(this, previousPanel));
    }

    private async void OnContinuePressed()
    {
        try
        {
            continueButton.interactable = false;

            var avatarConfig = new AvatarConfig(classesIndex, eyeIndex, beardIndex, hairIndex, headIndex, eyebrowsIndex, earsIndex, mouthIndex, makeupIndex, accessoriesIndex, skinColorIndex, eyesColorIndex, makeupColorIndex, hairColorIndex, beardColorIndex);

            Character newCharacter = new Character(avatarConfig, References.Instance.player.allCharacters.Count);

            bool success = await FirebaseSaveManager.AddNewCharacter(newCharacter);


            if (success)
            {
                References.Instance.player.AddNewCharacter(newCharacter);
                References.Instance.player.SetCurrentCharater(newCharacter);
                Bus<OnCharacterLoad>.CallEvent(new OnCharacterLoad(newCharacter));
                Bus<TransitionPanelsEvent>.CallEvent(new TransitionPanelsEvent(this, nextPanel));
                Bus<OnClassChangeEvent>.CallEvent(new OnClassChangeEvent(References.Instance.characterCustomizerConfigurations.classes[classesIndex]));
            }

            else
            {
                Debug.LogWarning($"Error adding new character.");
                Bus<OnAddToast>.CallEvent(new OnAddToast(new PopupConfig(MarkStatus.Neutral, References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.somethingWrong}"), PopupType.System)));
            }
        }

        catch (Exception e)
        {
            Debug.LogWarning($"Error adding new character: {e.Message}");
            return;
        }

        finally
        {
            continueButton.interactable = true;
        }
    }

    private void OnRightArrowPressed()
    {
        if (currentPartType == CharacterPartType.Eyes)
        {
            eyeIndex = (eyeIndex + 1) % References.Instance.characterCustomizerConfigurations.eyeSprites.Length;
            Bus<EyesSpriteChangeEvent>.CallEvent(new EyesSpriteChangeEvent(GetSpriteFromArray(References.Instance.characterCustomizerConfigurations.eyeSprites, eyeIndex)));
        }

        else if (currentPartType == CharacterPartType.Hair)
        {
            hairIndex = (hairIndex + 1) % References.Instance.characterCustomizerConfigurations.hairSprites.Length;
            Bus<HairSpriteChangeEvent>.CallEvent(new HairSpriteChangeEvent(GetSpriteFromArray(References.Instance.characterCustomizerConfigurations.hairSprites, hairIndex)));
        }

        else if (currentPartType == CharacterPartType.Beard)
        {
            beardIndex = (beardIndex + 1) % References.Instance.characterCustomizerConfigurations.beardSprites.Length;
            Bus<BeardSpriteChangeEvent>.CallEvent(new BeardSpriteChangeEvent(GetSpriteFromArray(References.Instance.characterCustomizerConfigurations.beardSprites, beardIndex)));
        }

        else if (currentPartType == CharacterPartType.Body)
        {
            headIndex = (headIndex + 1) % References.Instance.characterCustomizerConfigurations.headSprites.Length;
            Bus<HeadSpriteChangeEvent>.CallEvent(new HeadSpriteChangeEvent(GetSpriteFromArray(References.Instance.characterCustomizerConfigurations.headSprites, headIndex)));
        }

        else if (currentPartType == CharacterPartType.Eyebrows)
        {
            eyebrowsIndex = (eyebrowsIndex + 1) % References.Instance.characterCustomizerConfigurations.eyebrowsSprites.Length;
            Bus<EyebrowsSpriteChangeEvent>.CallEvent(new EyebrowsSpriteChangeEvent(GetSpriteFromArray(References.Instance.characterCustomizerConfigurations.eyebrowsSprites, eyebrowsIndex)));
        }

        else if (currentPartType == CharacterPartType.Ears)
        {
            earsIndex = (earsIndex + 1) % References.Instance.characterCustomizerConfigurations.earsSprites.Length;
            Bus<EarSpriteChangeEvent>.CallEvent(new EarSpriteChangeEvent(GetSpriteFromArray(References.Instance.characterCustomizerConfigurations.earsSprites, earsIndex)));
        }

        else if (currentPartType == CharacterPartType.Mouth)
        {
            mouthIndex = (mouthIndex + 1) % References.Instance.characterCustomizerConfigurations.mouthSprites.Length;
            Bus<MouthSpriteChangeEvent>.CallEvent(new MouthSpriteChangeEvent(GetSpriteFromArray(References.Instance.characterCustomizerConfigurations.mouthSprites, mouthIndex)));
        }

        else if (currentPartType == CharacterPartType.Makeup)
        {
            makeupIndex = (makeupIndex + 1) % References.Instance.characterCustomizerConfigurations.makeupSprites.Length;
            Bus<MakeupSpriteChangeEvent>.CallEvent(new MakeupSpriteChangeEvent(GetSpriteFromArray(References.Instance.characterCustomizerConfigurations.makeupSprites, makeupIndex)));
        }

        else if (currentPartType == CharacterPartType.Accessory)
        {
            accessoriesIndex = (accessoriesIndex + 1) % References.Instance.characterCustomizerConfigurations.accessoriesSprites.Length;
            Bus<AccessorySpriteChangeEvent>.CallEvent(new AccessorySpriteChangeEvent(GetSpriteFromArray(References.Instance.characterCustomizerConfigurations.accessoriesSprites, accessoriesIndex)));
        }

        else if (currentPartType == CharacterPartType.Classes)
        {
            classesIndex = (classesIndex + 1) % References.Instance.characterCustomizerConfigurations.classes.Length;
            Bus<ArmorSpriteChangeEvent>.CallEvent(new ArmorSpriteChangeEvent(References.Instance.characterCustomizerConfigurations.classes[classesIndex].initialConfig.armorSO.armorSprites));

            characterCustomizerStatus.UpdateValues(References.Instance.characterCustomizerConfigurations.classes[classesIndex]);
        }
    }

    private void OnLeftArrowPressed()
    {
        if (currentPartType == CharacterPartType.Eyes)
        {
            eyeIndex = (eyeIndex - 1 + References.Instance.characterCustomizerConfigurations.eyeSprites.Length) % References.Instance.characterCustomizerConfigurations.eyeSprites.Length;
            Bus<EyesSpriteChangeEvent>.CallEvent(new EyesSpriteChangeEvent(GetSpriteFromArray(References.Instance.characterCustomizerConfigurations.eyeSprites, eyeIndex)));
        }

        else if (currentPartType == CharacterPartType.Hair)
        {
            hairIndex = (hairIndex - 1 + References.Instance.characterCustomizerConfigurations.hairSprites.Length) % References.Instance.characterCustomizerConfigurations.hairSprites.Length;
            Bus<HairSpriteChangeEvent>.CallEvent(new HairSpriteChangeEvent(GetSpriteFromArray(References.Instance.characterCustomizerConfigurations.hairSprites, hairIndex)));
        }

        else if (currentPartType == CharacterPartType.Beard)
        {
            beardIndex = (beardIndex - 1 + References.Instance.characterCustomizerConfigurations.beardSprites.Length) % References.Instance.characterCustomizerConfigurations.beardSprites.Length;
            Bus<BeardSpriteChangeEvent>.CallEvent(new BeardSpriteChangeEvent(GetSpriteFromArray(References.Instance.characterCustomizerConfigurations.beardSprites, beardIndex)));
        }

        else if (currentPartType == CharacterPartType.Body)
        {
            headIndex = (headIndex - 1 + References.Instance.characterCustomizerConfigurations.headSprites.Length) % References.Instance.characterCustomizerConfigurations.headSprites.Length;
            Bus<HeadSpriteChangeEvent>.CallEvent(new HeadSpriteChangeEvent(GetSpriteFromArray(References.Instance.characterCustomizerConfigurations.headSprites, headIndex)));
        }

        else if (currentPartType == CharacterPartType.Eyebrows)
        {
            eyebrowsIndex = (eyebrowsIndex - 1 + References.Instance.characterCustomizerConfigurations.eyebrowsSprites.Length) % References.Instance.characterCustomizerConfigurations.eyebrowsSprites.Length;
            Bus<EyebrowsSpriteChangeEvent>.CallEvent(new EyebrowsSpriteChangeEvent(GetSpriteFromArray(References.Instance.characterCustomizerConfigurations.eyebrowsSprites, eyebrowsIndex)));
        }

        else if (currentPartType == CharacterPartType.Ears)
        {
            earsIndex = (earsIndex - 1 + References.Instance.characterCustomizerConfigurations.earsSprites.Length) % References.Instance.characterCustomizerConfigurations.earsSprites.Length;
            Bus<EarSpriteChangeEvent>.CallEvent(new EarSpriteChangeEvent(GetSpriteFromArray(References.Instance.characterCustomizerConfigurations.earsSprites, earsIndex)));
        }

        else if (currentPartType == CharacterPartType.Mouth)
        {
            mouthIndex = (mouthIndex - 1 + References.Instance.characterCustomizerConfigurations.mouthSprites.Length) % References.Instance.characterCustomizerConfigurations.mouthSprites.Length;
            Bus<MouthSpriteChangeEvent>.CallEvent(new MouthSpriteChangeEvent(GetSpriteFromArray(References.Instance.characterCustomizerConfigurations.mouthSprites, mouthIndex)));
        }

        else if (currentPartType == CharacterPartType.Makeup)
        {
            makeupIndex = (makeupIndex - 1 + References.Instance.characterCustomizerConfigurations.makeupSprites.Length) % References.Instance.characterCustomizerConfigurations.makeupSprites.Length;
            Bus<MakeupSpriteChangeEvent>.CallEvent(new MakeupSpriteChangeEvent(GetSpriteFromArray(References.Instance.characterCustomizerConfigurations.makeupSprites, makeupIndex)));
        }

        else if (currentPartType == CharacterPartType.Accessory)
        {
            accessoriesIndex = (accessoriesIndex - 1 + References.Instance.characterCustomizerConfigurations.accessoriesSprites.Length) % References.Instance.characterCustomizerConfigurations.accessoriesSprites.Length;
            Bus<AccessorySpriteChangeEvent>.CallEvent(new AccessorySpriteChangeEvent(GetSpriteFromArray(References.Instance.characterCustomizerConfigurations.accessoriesSprites, accessoriesIndex)));
        }

        else if (currentPartType == CharacterPartType.Classes)
        {
            classesIndex = (classesIndex - 1 + References.Instance.characterCustomizerConfigurations.classes.Length) % References.Instance.characterCustomizerConfigurations.classes.Length;
            Bus<ArmorSpriteChangeEvent>.CallEvent(new ArmorSpriteChangeEvent(References.Instance.characterCustomizerConfigurations.classes[classesIndex].initialConfig.armorSO.armorSprites));
            characterCustomizerStatus.UpdateValues(References.Instance.characterCustomizerConfigurations.classes[classesIndex]);
        }
    }

    private Sprite GetSpriteFromArray(Sprite[] sprites, int index)
    {
        if (sprites == null || sprites.Length == 0)
            return null;

        return sprites[index % sprites.Length];
    }

    private void RestoreInitialConfiguration()
    {
        currentPartType = CharacterPartType.Classes;

        eyeIndex = 0;
        beardIndex = 0;
        hairIndex = 0;
        headIndex = 0;
        eyebrowsIndex = 0;
        earsIndex = 0;
        mouthIndex = 0;
        makeupIndex = 0;
        accessoriesIndex = 0;
        classesIndex = 0;

        skinColorIndex = 0;
        eyesColorIndex = 0;
        makeupColorIndex = 0;
        hairColorIndex = 0;
        beardColorIndex = 0;

        foreach (var part in References.Instance.characterCustomizerConfigurations.characterParts)
        {
            part.Initialize();
        }

        for (int i = 0; i < customizationButtons.Length; i++)
        {
            customizationButtons[i].Initialize(this, false, colorPickerPanels[i]);
        }

        customizationButtons[0].Initialize(this, true, colorPickerPanels[0]); // Set first button as selected

        var eyesButtons = Array.FindAll(colorPickerButtons, button => button is EyesColorColorPickerButton);
        if (eyesButtons != null)
            for (int i = 0; i < eyesButtons.Length; i++)
                eyesButtons[i].Initialize(References.Instance.characterCustomizerConfigurations.eyesColors[i], i);

        var skinButons = Array.FindAll(colorPickerButtons, button => button is SkinColorColorPickerButton);
        if (skinButons != null)
            for (int i = 0; i < skinButons.Length; i++)
                skinButons[i].Initialize(References.Instance.characterCustomizerConfigurations.skinColors[i], i);

        var hairButtons = Array.FindAll(colorPickerButtons, button => button is HairColorColorPickerButton);
        if (hairButtons != null)
            for (int i = 0; i < hairButtons.Length; i++)
                hairButtons[i].Initialize(References.Instance.characterCustomizerConfigurations.hairColors[i], i);

        var beardButtons = Array.FindAll(colorPickerButtons, button => button is BeardColorColorPickerButton);
        if (beardButtons != null)
            for (int i = 0; i < beardButtons.Length; i++)
                beardButtons[i].Initialize(References.Instance.characterCustomizerConfigurations.hairColors[i], i);

        var makeupButtons = Array.FindAll(colorPickerButtons, button => button is MakeupColorPickerButton);
        if (makeupButtons != null)
            for (int i = 0; i < makeupButtons.Length; i++)
                makeupButtons[i].Initialize(References.Instance.characterCustomizerConfigurations.makeupColors[i], i);

        if (References.Instance.characterCustomizerConfigurations.bodyList != null)
            foreach (var body in References.Instance.characterCustomizerConfigurations.bodyList)
                body.ResetPart(References.Instance.characterCustomizerConfigurations.skinColors[skinColorIndex], null);

        if (References.Instance.characterCustomizerConfigurations.headList != null)
            foreach (var head in References.Instance.characterCustomizerConfigurations.headList)
                head.ResetPart(References.Instance.characterCustomizerConfigurations.skinColors[skinColorIndex], GetSpriteFromArray(References.Instance.characterCustomizerConfigurations.headSprites, headIndex));

        if (References.Instance.characterCustomizerConfigurations.eyesList != null)
            foreach (var eye in References.Instance.characterCustomizerConfigurations.eyesList)
                eye.ResetPart(References.Instance.characterCustomizerConfigurations.eyesColors[eyesColorIndex], GetSpriteFromArray(References.Instance.characterCustomizerConfigurations.eyeSprites, eyeIndex));

        if (References.Instance.characterCustomizerConfigurations.hairList != null)
            foreach (var hair in References.Instance.characterCustomizerConfigurations.hairList)
                hair.ResetPart(References.Instance.characterCustomizerConfigurations.hairColors[hairColorIndex], GetSpriteFromArray(References.Instance.characterCustomizerConfigurations.hairSprites, hairIndex));

        if (References.Instance.characterCustomizerConfigurations.beardList != null)
            foreach (var beard in References.Instance.characterCustomizerConfigurations.beardList)
                beard.ResetPart(References.Instance.characterCustomizerConfigurations.hairColors[hairColorIndex], null);

        if (References.Instance.characterCustomizerConfigurations.earsList != null)
            foreach (var ear in References.Instance.characterCustomizerConfigurations.earsList)
                ear.ResetPart(References.Instance.characterCustomizerConfigurations.skinColors[skinColorIndex], GetSpriteFromArray(References.Instance.characterCustomizerConfigurations.earsSprites, earsIndex));

        if (References.Instance.characterCustomizerConfigurations.eyebrowsList != null)
            foreach (var eyebrow in References.Instance.characterCustomizerConfigurations.eyebrowsList)
                eyebrow.ResetPart(Color.white, GetSpriteFromArray(References.Instance.characterCustomizerConfigurations.eyebrowsSprites, eyebrowsIndex));

        if (References.Instance.characterCustomizerConfigurations.mouthList != null)
            foreach (var mouth in References.Instance.characterCustomizerConfigurations.mouthList)
                mouth.ResetPart(Color.white, GetSpriteFromArray(References.Instance.characterCustomizerConfigurations.mouthSprites, mouthIndex));

        if (References.Instance.characterCustomizerConfigurations.accessoriesList != null)
            foreach (var accessory in References.Instance.characterCustomizerConfigurations.accessoriesList)
                accessory.ResetPart(Color.white, null);

        if (References.Instance.characterCustomizerConfigurations.makeupList != null)
            foreach (var makeup in References.Instance.characterCustomizerConfigurations.makeupList)
                makeup.ResetPart(References.Instance.characterCustomizerConfigurations.makeupColors[0], null);

        if (References.Instance.characterCustomizerConfigurations.armorsList != null)
            foreach (var armor in References.Instance.characterCustomizerConfigurations.armorsList)
                armor.ResetPart(References.Instance.characterCustomizerConfigurations.classes[0].initialConfig.armorSO.armorSprites);

        if (References.Instance.characterCustomizerConfigurations.weaponList != null)
            foreach (var weapon in References.Instance.characterCustomizerConfigurations.weaponList) ;

    }

    public override void Initialize()
    {
        colorPickerButtons = panel.GetComponentsInChildren<GenericColorPickerButton>(true);
        customizationButtons = panel.GetComponentsInChildren<CharacterCustomizationButton>(true);
        characterCustomizerStatus = panel.GetComponentInChildren<CharacterCustomizerStatus>(true);

        leftArrowButton.onClick.RemoveAllListeners();
        rightArrowButton.onClick.RemoveAllListeners();
        continueButton.onClick.RemoveAllListeners();

        leftArrowButton.onClick.AddListener(OnLeftArrowPressed);
        rightArrowButton.onClick.AddListener(OnRightArrowPressed);
        continueButton.onClick.AddListener(OnContinuePressed);

        RestoreInitialConfiguration();



        base.Initialize();


    }

    public override void Close()
    {
        base.Close();
    }

    public override void Open()
    {
        Bus<WeaponSpriteChangeEvent>.CallEvent(new WeaponSpriteChangeEvent(null));
        Bus<HelmetSpriteChangeEvent>.CallEvent(new HelmetSpriteChangeEvent(null));
        Bus<ArmorSpriteChangeEvent>.CallEvent(new ArmorSpriteChangeEvent(null));
        Bus<ShieldSpriteChangeEvent>.CallEvent(new ShieldSpriteChangeEvent(null));
        Bus<WingSpriteChangeEvent>.CallEvent(new WingSpriteChangeEvent(null));
        Bus<SecondWeaponSpriteChangeEvent>.CallEvent(new SecondWeaponSpriteChangeEvent(null));

        Initialize();
        base.Open();
    }
}

public enum CharacterPartType
{
    Hair,
    Eyes,
    Body,
    Beard,
    Eyebrows,
    Accessory,
    Ears,
    Mouth,
    Makeup,
    Classes
}
