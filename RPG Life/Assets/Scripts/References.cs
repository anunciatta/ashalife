using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

public class References : MonoBehaviour
{
    //For testing 
    public DateTime testCurrentDateTime;

    public static References Instance;

    public bool HasLoadGame = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);

        player = GetComponent<Player>();
        inventory = GetComponent<Inventory>();
        firebaseManager = GetComponent<FirebaseManager>();
        localizationService = GetComponent<LocalizationService>();
        newDailyPanel = GetComponent<NewDailyPanel>();
        newHabitPanel = GetComponent<NewHabitPanel>();

        characterCustomizerConfigurations.characterParts = GetComponentsInChildren<GenericCharacterPart>(true);

        iconChangeableOnClassSelections = GetComponentsInChildren<ChangeableOnClassSelection>(true);

        characterCustomizerConfigurations.hairList = characterCustomizerConfigurations.characterParts.OfType<Hair>().ToList();
        characterCustomizerConfigurations.eyesList = characterCustomizerConfigurations.characterParts.OfType<Eyes>().ToList();
        characterCustomizerConfigurations.eyebrowsList = characterCustomizerConfigurations.characterParts.OfType<Eyebrows>().ToList();
        characterCustomizerConfigurations.beardList = characterCustomizerConfigurations.characterParts.OfType<Beard>().ToList();
        characterCustomizerConfigurations.mouthList = characterCustomizerConfigurations.characterParts.OfType<Mouth>().ToList();
        characterCustomizerConfigurations.accessoriesList = characterCustomizerConfigurations.characterParts.OfType<Accessory>().ToList();
        characterCustomizerConfigurations.makeupList = characterCustomizerConfigurations.characterParts.OfType<Makeup>().ToList();
        characterCustomizerConfigurations.armorsList = characterCustomizerConfigurations.characterParts.OfType<Armor>().ToList();
        characterCustomizerConfigurations.weaponList = characterCustomizerConfigurations.characterParts.OfType<Weapon>().ToList();
        characterCustomizerConfigurations.secondWeapon = characterCustomizerConfigurations.characterParts.OfType<SecondWeapon>().ToList();

        characterCustomizerConfigurations.helmetList = characterCustomizerConfigurations.characterParts.OfType<Helmet>().ToList();
        characterCustomizerConfigurations.shieldList = characterCustomizerConfigurations.characterParts.OfType<Shield>().ToList();
        characterCustomizerConfigurations.wingList = characterCustomizerConfigurations.characterParts.OfType<Wing>().ToList();


        characterCustomizerConfigurations.earsList = characterCustomizerConfigurations.characterParts.OfType<Body>().Where(part => part is Body && part.IsEar()).ToList();
        characterCustomizerConfigurations.headList = characterCustomizerConfigurations.characterParts.OfType<Body>().Where(part => part is Body && part.IsHead()).ToList();
        characterCustomizerConfigurations.bodyList = characterCustomizerConfigurations.characterParts.OfType<Body>().Where(part => part is Body && !part.IsHead() && !part.IsEar()).ToList();

        Bus<OnClassChangeEvent>.OnEvent += OnClassChanged;
        Bus<OnReferenceSignInFirebase>.OnEvent += OnSignIn;
    }

    public DateTime ParseDateTime(string dateTimeString)
    {
        dateTimeString = dateTimeString.Trim('"');

        DateTime savedUtc = DateTime.ParseExact(
            dateTimeString,
            "O",
            CultureInfo.InvariantCulture,
            DateTimeStyles.RoundtripKind
        );

        return savedUtc.ToLocalTime();
    }

    private async void OnSignIn(OnReferenceSignInFirebase data)
    {
        string lastDate = await FirebaseSaveManager.GetLastSavedData();
        lastDate = lastDate.Trim('"');

        DateTime lastSavedUtc = DateTime.ParseExact(
            lastDate,
            "O",
            CultureInfo.InvariantCulture,
            DateTimeStyles.RoundtripKind
        );
        lastSavedDate = lastSavedUtc.ToLocalTime();

        Bus<OnSignInFirebase>.CallEvent(new OnSignInFirebase(true));
    }

    private void OnClassChanged(OnClassChangeEvent _event)
    {
        foreach (var changeable in iconChangeableOnClassSelections)
        {
            changeable.ChangeIcon(_event.NewClass);
        }
    }

    public DateTime lastSavedDate;

    void OnDestroy()
    {
        Bus<OnClassChangeEvent>.OnEvent -= OnClassChanged;
        Bus<OnReferenceSignInFirebase>.OnEvent -= OnSignIn;
    }

    [HideInInspector] public Player player;
    [HideInInspector] public Inventory inventory;
    [HideInInspector] public FirebaseManager firebaseManager;
    [HideInInspector] public LocalizationService localizationService;
    [HideInInspector] public NewHabitPanel newHabitPanel;
    [HideInInspector] public NewDailyPanel newDailyPanel;

    public ExperienceConfigurations experienceConfigurations;

    public CharacterCustomizerConfigurations characterCustomizerConfigurations;

    private ChangeableOnClassSelection[] iconChangeableOnClassSelections;

    #region Helpers

    public void AddToast(string message, MarkStatus popupStatus, PopupType popupType) =>
        Bus<OnAddToast>.CallEvent(new OnAddToast(new PopupConfig(popupStatus, message, popupType)));

    private void AddDescriptionReplacedToast(int value, MarkStatus markStatus, LocalizationLabels label, PopupType popupType)
    {
        var description = localizationService.GetLocalizationText($"{label}");
        var descriptionReplaced = description.Replace("{value}", $"{value}");
        Bus<OnAddToast>.CallEvent(new OnAddToast(new PopupConfig(markStatus, descriptionReplaced, popupType)));
    }

    public void ReceivingExperienceToast(int value) => AddDescriptionReplacedToast(value, MarkStatus.Positive, LocalizationLabels.receivingExperience, PopupType.Experience);
    public void LosingExperienceToast(int value) => AddDescriptionReplacedToast(value, MarkStatus.Negative, LocalizationLabels.losingExperience, PopupType.Experience);
    public void ReceivingGemsToast(int value) => AddDescriptionReplacedToast(value, MarkStatus.Positive, LocalizationLabels.receivingGems, PopupType.Gem);
    public void LosingGemsToast(int value) => AddDescriptionReplacedToast(value, MarkStatus.Negative, LocalizationLabels.losingGems, PopupType.Gem);
    public void ReceivingCoinsToast(int value) => AddDescriptionReplacedToast(value, MarkStatus.Positive, LocalizationLabels.receivingCoins, PopupType.Coins);
    public void LosingCoinsToast(int value) => AddDescriptionReplacedToast(value, MarkStatus.Negative, LocalizationLabels.losingCoins, PopupType.Coins);
    public void LevelUpToast() => AddToast(localizationService.GetLocalizationText($"{LocalizationLabels.levelUp}"), MarkStatus.Positive, PopupType.Experience);
    public void LevelDownToast() => AddToast(localizationService.GetLocalizationText($"{LocalizationLabels.levelDown}"), MarkStatus.Negative, PopupType.Experience);

    #endregion
}

[Serializable]
public class ExperienceConfigurations
{
    public int[] questsDifficultyTier;
    public DailyRewardsConfig[] experiencesTierFromDaily;
    public HabitRewardsConfig[] experiencesTierFromHabits;
    public List<ClassDefinition> classDefinitions = new();

    public int GetEnergyForNextLevel(int currentLevel)
    {
        return Mathf.RoundToInt(5 + Mathf.Sqrt(currentLevel) * 3f);
    }

    public int GetXpForNextLevel(int currentLevel)
    {
        return Mathf.RoundToInt(50 + 25 * currentLevel + 10 * currentLevel * currentLevel);
    }

    public int StatAtLevel(int baseStat, float growth, int level)
    {
        return Mathf.RoundToInt(baseStat + growth * (level - 1));
    }
}

[Serializable]
public class DailyRewardsConfig
{
    public int coins;
    public int gems;
    public int experience;
}


[Serializable]
public class HabitRewardsConfig
{
    public int coins;
    public int gems;
    public int experience;

    [Header("On Habit Complete")]
    public int completeCoins;
    public int completeGems;
    public int completeExperience;
    public ItemSO completeItem;
}


[Serializable]
public class ClassDefinition
{
    public ClassType characterClass;
    public List<StatDefinition> stats;
}

[Serializable]
public class StatDefinition
{
    public Status status;
    public int baseValue;
    public float growth;
}


[Serializable]
public class CharacterCustomizerConfigurations
{
    public Sprite[] eyeSprites;
    public Sprite[] beardSprites;
    public Sprite[] hairSprites;
    public Sprite[] headSprites;
    public Sprite[] eyebrowsSprites;
    public Sprite[] earsSprites;
    public Sprite[] mouthSprites;
    public Sprite[] makeupSprites;
    public Sprite[] accessoriesSprites;
    public ClassSO[] classes;

    public Color[] skinColors;
    public Color[] hairColors;
    public Color[] eyesColors;
    public Color[] makeupColors;

    public GenericCharacterPart[] characterParts;

    public List<Hair> hairList = new();
    public List<Eyes> eyesList = new();
    public List<Eyebrows> eyebrowsList = new();
    public List<Beard> beardList = new();
    public List<Body> earsList = new();
    public List<Body> bodyList = new();
    public List<Body> headList = new();
    public List<Accessory> accessoriesList = new();
    public List<Mouth> mouthList = new();
    public List<Makeup> makeupList = new();
    public List<Armor> armorsList = new();
    public List<Weapon> weaponList = new();
    public List<Helmet> helmetList = new();
    public List<Shield> shieldList = new();
    public List<Wing> wingList = new();
    public List<SecondWeapon> secondWeapon = new();
}
