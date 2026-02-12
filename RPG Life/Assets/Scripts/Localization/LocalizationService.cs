using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class LocalizationService : MonoBehaviour
{
    public bool forceEnglishLocalization = false; // For testing purposes

    public Language[] availableLanguages =
    {
        Language.English,
        Language.Portuguese,
        //Language.Chinese,
        //Language.Russian,
        //Language.Korean
    };

    public List<Dictionary<string, string>> availableDicts = new List<Dictionary<string, string>>();
    public Language currentLanguage;

    public List<LanguageTextModifier> languageTextModifiers = new List<LanguageTextModifier>();
    private Dictionary<string, string> englishDict = new Dictionary<string, string>();
    private Dictionary<string, string> portugueseDict = new Dictionary<string, string>();
    private Dictionary<string, string> russianDict = new Dictionary<string, string>();
    private Dictionary<string, string> chineseDict = new Dictionary<string, string>();
    private Dictionary<string, string> koreanDict = new Dictionary<string, string>();
    private Dictionary<string, string> currentDict;

    private void Awake() => Initialize();

    private void Initialize()
    {
        availableDicts.Clear();
        availableDicts.Add(englishDict);
        availableDicts.Add(portugueseDict);
        //availableDicts.Add(chineseDict);
        //availableDicts.Add(russianDict);
        //availableDicts.Add(koreanDict);

        PopulateLocalizationDictionary();

        currentLanguage = GetLocalization();
        currentDict = availableDicts[(int)currentLanguage];

        if (forceEnglishLocalization)
        {
            currentLanguage = Language.English;
            SetLocalization(currentLanguage);
        }

        else
        {
            foreach (var modifier in languageTextModifiers)
            {
                modifier.UpdateText();
            }
        }
    }

    private Language GetLocalization()
    {
        if (PlayerPrefs.HasKey("localization") == false)
        {
            Language language = Application.systemLanguage.ToString() switch
            {
                "English" => Language.English,
                "Portuguese" => Language.Portuguese,
                //"Chinese" => Language.Chinese,
                //"Russian" => Language.Russian,
                //"Korean" => Language.Korean,
                _ => Language.English, // Default to English if system language is not supported
            };

            Debug.Log($"Localization not found in PlayerPrefs, setting to system language: {language}");

            return language;
        }

        else
        {
            Language language = PlayerPrefs.GetString("localization", Application.systemLanguage.ToString()) switch
            {
                "English" => Language.English,
                "Portuguese" => Language.Portuguese,
                //"Chinese" => Language.Chinese,
                //"Russian" => Language.Russian,
                //"Korean" => Language.Korean,
                _ => Language.English, // Default to English if not found
            };

            return language;
        }
    }

    private void SetLocalization(Language language)
    {
        PlayerPrefs.SetString("localization", language.ToString());
        PlayerPrefs.Save();
    }

    [ContextMenu("Populate Localization Dictionary")]
    private void PopulateLocalizationDictionary()
    {
        LoadLanguage("english.csv", englishDict);
        LoadLanguage("portuguese.csv", portugueseDict);

        //LoadLanguage("chinese.csv", chineseDict);
        //LoadLanguage("russian.csv", russianDict);
        //LoadLanguage("korean.csv", koreanDict);
    }

    public string GetLocalizedClassesDescription(ClassType nodeType)
    {
        return nodeType switch
        {
            ClassType.Archer => GetLocalizationText($"{LocalizationLabels.archer}"),
            ClassType.Mage => GetLocalizationText($"{LocalizationLabels.mage}"),
            ClassType.Warrior => GetLocalizationText($"{LocalizationLabels.warrior}"),

            _ => GetLocalizationText($"{LocalizationLabels.archer}"),
        };
    }

    [ContextMenu("Get Text Modifiers")]
    private void GetTextModifiers()
    {
        foreach (var parent in visualsParents)
        {
            var virtualList = parent.GetComponentsInChildren<LanguageTextModifier>(true).ToList();

            foreach (var modifier in virtualList)
            {
                if (string.IsNullOrEmpty(modifier.Label))
                {
                    Debug.Log(modifier.gameObject.name);
                }

                if (!languageTextModifiers.Contains(modifier))
                {
                    languageTextModifiers.Add(modifier);
                }
            }
        }
    }

    public string GetLocalizationText(string key)
    {
        if (currentDict == null)
        {
            Debug.LogError("No language dictionary loaded!");
            return key;
        }

        if (currentDict.TryGetValue(key, out string localizedText))
        {
            return localizedText;
        }

        // Fallback to English if key not found in current language
        if (currentLanguage != Language.English && englishDict.TryGetValue(key, out string fallbackText))
        {

            Debug.LogWarning($"Key '{key}' not found in {currentLanguage}, using English fallback");
            return fallbackText;
        }

        // If still not found, return the key itself
        Debug.LogWarning($"Localization key '{key}' not found in any language");

        return key;
    }


    public void ChangeLanguage(int index)
    {
        if (currentLanguage == availableLanguages[index]) return;

        currentLanguage = availableLanguages[index];
        currentDict = availableDicts[(int)currentLanguage];

        Bus<OnLanguageChanged>.CallEvent(new OnLanguageChanged(currentLanguage));

        foreach (var modifier in languageTextModifiers)
        {
            modifier.UpdateText();
        }
    }

    public Transform[] visualsParents;

    void LoadLanguage(string fileName, Dictionary<string, string> targetDict)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "Localization", fileName);

        if (!File.Exists(filePath))
        {
            Debug.LogError($"Localization file not found: {filePath}");
            return;
        }

        try
        {
            string[] lines = File.ReadAllLines(filePath);
            targetDict.Clear();

            for (int i = 1; i < lines.Length; i++) // Skip header row
            {
                string line = lines[i].Trim();

                // Skip empty lines and comments
                if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                    continue;

                ParseCSVLine(line, targetDict);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading localization file {fileName}: {e.Message}");
        }
    }

    void ParseCSVLine(string line, Dictionary<string, string> targetDict)
    {
        // Simple CSV parser that handles quoted values
        int commaIndex = line.IndexOf(',');
        if (commaIndex == -1) return;

        string key = line.Substring(0, commaIndex).Trim();
        string value = line.Substring(commaIndex + 1).Trim();

        // Remove quotes if present
        if (value.StartsWith("\"") && value.EndsWith("\""))
        {
            value = value.Substring(1, value.Length - 2);
            // Handle escaped quotes
            value = value.Replace("\"\"", "\"");
        }

        if (!string.IsNullOrEmpty(key))
        {
            targetDict[key] = value;
        }
    }
}

public enum Language
{
    English,
    Portuguese,
    Chinese,
    Russian,
    Korean,
}

public enum LocalizationLabels
{
    //Classes
    archer, warrior, mage,

    //Popups
    receivingExperience, receivingHealth, receivingCoins, receivingGems,
    losingExperience, losingHealth, losingCoins, losingGems, spendingCoins, spendingGems, levelDown, levelUp,

    dailyAlreadyCounted, weeklyAlreadyCounted, monthlyAlreadyCounted, yearlyAlreadyCounted,

    confirmDeleteDailyTitle, confirmDeleteHabitTitle, confirmDeleteCharacterTitle, confirmDeleteDaily, confirmDeleteHabit, confirmDeleteCharacter,

    beginAgain, somethingWrong, invalidInput,

    //Shop
    notEnoughToBuy, newItemAdded, equipped,

    // New Habits Panel
    dailyFlavor, weeklyFlavor, monthlyFlavor, yearlyFlavor, activeToday, activeOn, notToday, archived, nextYearAvailable, nextMonthAvailable, nextWeekAvailable, habitCompleted,

    // Buttons
    createButton, saveButton, status,

    //Quests
    cost, failure, success, chance, test
}


