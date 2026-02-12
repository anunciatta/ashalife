using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Daily : MonoBehaviour
{
    public DailyContent dailyContent;
    public GameObject optionsPanel;

    [Header("[0]Trivial [1]Easy [2]Medium [3]Hard")]
    public Sprite[] difficultyBackgrounds;

    [Header("[0]Not Completed [1]Completed")]
    public Sprite[] completedBackgrounds;

    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI notes;
    [SerializeField] private TextMeshProUGUI state;
    [SerializeField] private Image difficultyBackground;
    [SerializeField] private Image background;
    [SerializeField] private Image completed;

    [Header("[0] Even, [1] Odd")]
    [SerializeField] private Color[] colors;

    [Header("[0] Active, [1] Archived")]
    [SerializeField] private Color[] stateColors;

    private bool isCompleted = false;

    public void RestoreInitialConfiguration()
    {
        isCompleted = false;
        completed.sprite = completedBackgrounds[0];
    }

    private DailiesPanel dailiesPanel;

    private bool isProcessingTask = false;
    private bool isActive;

    public void UpdateContent(DailyContent dailyContent, DailiesPanel dailiesPanel)
    {
        if (this.dailiesPanel == null)
        {
            this.dailiesPanel = dailiesPanel;
        }

        this.dailyContent = dailyContent;
        difficultyBackground.sprite = difficultyBackgrounds[(int)dailyContent.difficulty];
        title.text = dailyContent.title;
        notes.text = dailyContent.notes;
        bool hasNotes = dailyContent.notes != string.Empty;
        notes.gameObject.SetActive(hasNotes);
        optionsPanel.SetActive(false);

        isProcessingTask = false;
        DateTime today = DateTime.Now.Date;
        isCompleted = dailyContent.isCompleted;
        completed.sprite = dailyContent.isCompleted ? completedBackgrounds[1] : completedBackgrounds[0];
        completed.color = colors[dailyContent.isCompleted ? 1 : 0];
        state.color = stateColors[dailyContent.isActive ? 0 : 1];

        if (dailyContent.isActive)
        {
            if (dailyContent.IsActiveOnDate(today))
            {
                state.text = References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.activeToday}");
                isActive = true;
            }
            else
            {
                DateTime? next = dailyContent.GetNextActiveDate(today);
                state.text = References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.activeOn}") + $" {next.Value:yyyy-MM-dd}";
                isActive = false;
            }
        }

        else
        {
            state.text = References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.archived}");
            isActive = false;
        }
    }

    public void UpdateLanguage(Language newLanguage)
    {
        if (dailyContent.isActive)
        {
            DateTime today = DateTime.Now.Date;

            if (dailyContent.IsActiveOnDate(today))
            {
                state.text = References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.activeToday}");
                isActive = true;
            }
            else
            {
                DateTime? next = dailyContent.GetNextActiveDate(today);
                state.text = References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.activeOn}") + $" {next.Value:yyyy-MM-dd}";
                isActive = false;
            }
        }

        else
        {
            state.text = References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.archived}");
            isActive = false;
        }
    }

    public async void OnComplete()
    {

        if (isProcessingTask == true) return;

        if (!isActive)
        {
            Bus<OnAddToast>.CallEvent(new OnAddToast(new PopupConfig(MarkStatus.Neutral, References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.notToday}"), PopupType.System)));
            return;
        }

        try
        {
            isProcessingTask = true;

            string coinId = References.Instance.inventory.coin.saveableEntityId;
            string gemId = References.Instance.inventory.gem.saveableEntityId;

            int xpReward = References.Instance.experienceConfigurations.experiencesTierFromDaily[(int)dailyContent.difficulty].experience;
            int coinReward = References.Instance.experienceConfigurations.experiencesTierFromDaily[(int)dailyContent.difficulty].coins;
            int gemReward = References.Instance.experienceConfigurations.experiencesTierFromDaily[(int)dailyContent.difficulty].gems;
            int overflowXp = 0;
            int nextLevelXp = 0;
            int referenceExperience = xpReward;
            bool success = false;

            if (!isCompleted)
            {
                if (References.Instance.player.currentCharacter.statuses[(int)Status.Experience].currentValue + xpReward >= References.Instance.player.currentCharacter.statuses[(int)Status.Experience].maxValue)
                {
                    //Level up
                    xpReward = References.Instance.player.currentCharacter.statuses[(int)Status.Experience].maxValue - References.Instance.player.currentCharacter.statuses[(int)Status.Experience].currentValue;
                    overflowXp = References.Instance.experienceConfigurations.experiencesTierFromDaily[(int)dailyContent.difficulty].experience - xpReward;
                    nextLevelXp = References.Instance.experienceConfigurations.GetXpForNextLevel(References.Instance.player.currentCharacter.level + 1);
                    success = await FirebaseSaveManager.UpdateDailyOnMark(dailyContent.id, "isCompleted", !isCompleted, References.Instance.player.currentCharacter, xpReward, coinReward, gemReward, overflowXp, nextLevelXp);
                }

                else
                {
                    //normal gain
                    success = await FirebaseSaveManager.UpdateDailyOnMark(dailyContent.id, "isCompleted", !isCompleted, References.Instance.player.currentCharacter, xpReward, coinReward, gemReward, overflowXp);
                }

                if (success)
                {
                    bool didLevelUp = overflowXp >= 0 && nextLevelXp > 0;
                    References.Instance.player.currentCharacter.inventory[coinId] += coinReward;
                    References.Instance.player.currentCharacter.inventory[gemId] += gemReward;
                    Bus<ChangeCoinsEvent>.CallEvent(new ChangeCoinsEvent(References.Instance.player.currentCharacter.inventory[coinId]));
                    Bus<ChangeGemsEvent>.CallEvent(new ChangeGemsEvent(References.Instance.player.currentCharacter.inventory[gemId]));

                    isCompleted = !isCompleted;
                    dailyContent.isCompleted = !dailyContent.isCompleted;
                    completed.color = colors[isCompleted ? 1 : 0];
                    completed.sprite = completedBackgrounds[isCompleted ? 1 : 0];

                    if (isCompleted)
                    {
                        if (dailiesPanel.currentSortingType == SortingType.Pending)
                        {
                            dailiesPanel.OnDailyCompleted(this);
                        }
                    }

                    else
                    {
                        if (dailiesPanel.currentSortingType == SortingType.Done)
                        {
                            dailiesPanel.OnDailyCompleted(this);
                        }
                    }

                    if (didLevelUp)
                    {
                        References.Instance.player.currentCharacter.level += 1;
                        References.Instance.player.currentCharacter.statuses[6].SetMaxValue(nextLevelXp);
                        References.Instance.player.currentCharacter.statuses[6].OnValueChange(overflowXp);

                        // Update local stats
                        var classStats = References.Instance.experienceConfigurations
                            .classDefinitions[References.Instance.player.currentCharacter.avatarConfig.classesIndex].stats;

                        for (int i = 0; i < 6; i++)
                        {
                            int newStatValue = References.Instance.experienceConfigurations.StatAtLevel(
                                classStats[i].baseValue,
                                classStats[i].growth,
                                References.Instance.player.currentCharacter.level
                            );

                            References.Instance.player.currentCharacter.statuses[i].SetMaxValue(newStatValue);
                            References.Instance.player.currentCharacter.statuses[i].OnValueChange(newStatValue);
                        }

                        Bus<OnLevelUp>.CallEvent(new OnLevelUp(References.Instance.player.currentCharacter));
                        References.Instance.LevelUpToast();
                    }

                    else
                    {
                        References.Instance.player.currentCharacter.statuses[6].OnValueChange(References.Instance.player.currentCharacter.statuses[6].currentValue + referenceExperience);
                        Debug.Log($"✓ Daily {dailyContent.id} updated | +{xpReward} XP, +{coinReward} coins, +{gemReward} gems");
                    }

                    if (referenceExperience > 0)
                        References.Instance.ReceivingExperienceToast(referenceExperience);
                    if (coinReward > 0)
                        References.Instance.ReceivingCoinsToast(coinReward);
                    if (gemReward > 0)
                        References.Instance.ReceivingGemsToast(gemReward);
                }

                else
                {
                    Debug.LogWarning($"Error updating daily.");
                    Bus<OnAddToast>.CallEvent(new OnAddToast(new PopupConfig(MarkStatus.Neutral, References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.somethingWrong}"), PopupType.System)));
                }
            }

            else
            {
                if (References.Instance.player.currentCharacter.statuses[(int)Status.Experience].currentValue - xpReward <= 0 && References.Instance.player.currentCharacter.level > 1)
                {
                    //Level Down
                    nextLevelXp = References.Instance.experienceConfigurations.GetXpForNextLevel(References.Instance.player.currentCharacter.level - 1);
                    overflowXp = nextLevelXp - (References.Instance.experienceConfigurations.experiencesTierFromDaily[(int)dailyContent.difficulty].experience - References.Instance.player.currentCharacter.statuses[(int)Status.Experience].currentValue);
                    xpReward = References.Instance.player.currentCharacter.statuses[(int)Status.Experience].currentValue;
                    success = await FirebaseSaveManager.UpdateDailyOnMark(dailyContent.id, "isCompleted", !isCompleted, References.Instance.player.currentCharacter, -xpReward, -coinReward, -gemReward, overflowXp, nextLevelXp);
                }

                else
                {
                    if (References.Instance.player.currentCharacter.statuses[(int)Status.Experience].currentValue - xpReward <= 0)
                    {
                        success = await FirebaseSaveManager.UpdateDailyOnMark(dailyContent.id, "isCompleted", !isCompleted, References.Instance.player.currentCharacter, -References.Instance.player.currentCharacter.statuses[(int)Status.Experience].currentValue, -coinReward, -gemReward, 0, 0);

                    }

                    else
                    {
                        success = await FirebaseSaveManager.UpdateDailyOnMark(dailyContent.id, "isCompleted", !isCompleted, References.Instance.player.currentCharacter, -xpReward, -coinReward, -gemReward, 0, 0);
                    }
                }


                if (success)
                {
                    References.Instance.player.currentCharacter.inventory[coinId] -= coinReward;
                    References.Instance.player.currentCharacter.inventory[gemId] -= gemReward;
                    Bus<ChangeCoinsEvent>.CallEvent(new ChangeCoinsEvent(References.Instance.player.currentCharacter.inventory[coinId]));
                    Bus<ChangeGemsEvent>.CallEvent(new ChangeGemsEvent(References.Instance.player.currentCharacter.inventory[gemId]));

                    bool didLevelDown = overflowXp >= 0 && nextLevelXp > 0;

                    isCompleted = !isCompleted;
                    dailyContent.isCompleted = !dailyContent.isCompleted;
                    completed.color = colors[isCompleted ? 1 : 0];
                    completed.sprite = completedBackgrounds[isCompleted ? 1 : 0];

                    if (isCompleted)
                    {
                        if (dailiesPanel.currentSortingType == SortingType.Pending)
                        {
                            dailiesPanel.OnDailyCompleted(this);
                        }
                    }

                    else
                    {
                        if (dailiesPanel.currentSortingType == SortingType.Done)
                        {
                            dailiesPanel.OnDailyCompleted(this);
                        }
                    }

                    if (didLevelDown)
                    {
                        References.Instance.player.currentCharacter.level -= 1;
                        References.Instance.player.currentCharacter.statuses[6].OnValueChange(overflowXp);
                        References.Instance.player.currentCharacter.statuses[6].SetMaxValue(nextLevelXp);

                        // Update local stats
                        var classStats = References.Instance.experienceConfigurations
                            .classDefinitions[References.Instance.player.currentCharacter.avatarConfig.classesIndex].stats;

                        for (int i = 0; i < 6; i++)
                        {
                            int newStatValue = References.Instance.experienceConfigurations.StatAtLevel(
                                classStats[i].baseValue,
                                classStats[i].growth,
                                References.Instance.player.currentCharacter.level
                            );

                            References.Instance.player.currentCharacter.statuses[i].SetMaxValue(newStatValue);
                            References.Instance.player.currentCharacter.statuses[i].OnValueChange(newStatValue);
                        }

                        References.Instance.LevelDownToast();
                        Bus<OnLevelDown>.CallEvent(new OnLevelDown(References.Instance.player.currentCharacter));
                    }

                    else
                    {
                        References.Instance.player.currentCharacter.statuses[6].OnValueChange(References.Instance.player.currentCharacter.statuses[6].currentValue - referenceExperience);
                    }

                    if (referenceExperience > 0)
                        References.Instance.LosingExperienceToast(referenceExperience);
                    if (coinReward > 0)
                        References.Instance.LosingCoinsToast(coinReward);
                    if (gemReward > 0)
                        References.Instance.LosingGemsToast(gemReward);

                }

                else
                {
                    Debug.LogWarning($"Error updating daily.");
                    Bus<OnAddToast>.CallEvent(new OnAddToast(new PopupConfig(MarkStatus.Neutral, References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.somethingWrong}"), PopupType.System)));
                }
            }
        }

        catch (Exception e)
        {
            Debug.LogWarning($"Error updating daily: {e.Message}");
            return;
        }

        finally
        {
            isProcessingTask = false;
        }
    }

    public void OnOptions()
    {
        optionsPanel.SetActive(!optionsPanel.activeSelf);
    }

    public void OnEdit()
    {
        Bus<OnEditDailyOptionEvent>.CallEvent(new OnEditDailyOptionEvent(dailyContent));
    }

    public void OnDelete()
    {
        Bus<OnRemoveDailyOption>.CallEvent(new OnRemoveDailyOption(dailyContent));
    }

    public void OnUp()
    {

    }

    public void OnDown()
    {

    }
}

public enum Difficulty
{
    Trivial = 0, Easy = 1, Moderate = 2, Hard = 3, All = 4
}

public enum Rarity
{
    Common = 0, Uncommon = 1, Rare = 2, Epic = 3, Legendary = 4
}


public enum ItemType
{
    All = 0, Weapon = 1, Shield = 2, Armor = 3, Potion = 4, Wings = 5, Jewelry = 6, Pet = 7, Helmet = 8, LightSword = 9, Staff = 10, Dagger = 11, Bow = 12
}
