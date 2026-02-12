using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Habit : MonoBehaviour
{
    public HabitContent habitContent;
    public GameObject optionsPanel;

    [Header("[0]Trivial [1]Easy [2]Medium [3]Hard")]
    public Sprite[] difficultyBackgrounds;

    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI notes;
    [SerializeField] private TextMeshProUGUI count;
    [SerializeField] private TextMeshProUGUI state;
    [SerializeField] private Image difficultyBackground;

    private bool isCompleted = false;

    [SerializeField] private Button positiveButton;
    [SerializeField] private Button negativeButton;

    [Header("[0] Active, [1] Completed")]
    [SerializeField] private Color[] stateColors;

    public void RestoreInitialConfiguration()
    {
        isCompleted = false;
    }

    public void UpdateLanguage(Language newLanguage)
    {
        if (!isCompleted)
        {
            if (habitContent.isActive)
            {
                state.text = References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.activeToday}");
            }

            else
            {

                if (habitContent.repetition == (int)Repetition.Weekly)
                {
                    state.text = References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.nextWeekAvailable}");
                }

                else if (habitContent.repetition == (int)Repetition.Monthly)
                {
                    state.text = References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.nextMonthAvailable}");
                }

                else if (habitContent.repetition == (int)Repetition.Yearly)
                {
                    state.text = References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.nextYearAvailable}");
                }

                else
                {
                    Debug.LogWarning("Daily habit should never be inactive");
                }
            }
        }
        else
        {
            state.text = References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.habitCompleted}");
        }
    }


    public void UpdateContent(HabitContent habitContent)
    {
        this.habitContent = habitContent;
        difficultyBackground.sprite = difficultyBackgrounds[(int)habitContent.difficulty];
        title.text = habitContent.title;
        notes.text = habitContent.notes;
        bool hasNotes = habitContent.notes != string.Empty;
        notes.gameObject.SetActive(hasNotes);
        optionsPanel.SetActive(false);

        isCompleted = habitContent.isCompleted;

        UpdateCountText();

        state.color = stateColors[habitContent.isCompleted ? 0 : 1];

        if (!isCompleted)
        {
            positiveButton.gameObject.SetActive(true);
            negativeButton.gameObject.SetActive(true);

            positiveButton.interactable = habitContent.isActive;
            negativeButton.interactable = habitContent.isActive;

            if (habitContent.isActive)
            {
                state.text = References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.activeToday}");
            }

            else
            {

                if (habitContent.repetition == (int)Repetition.Weekly)
                {
                    state.text = References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.nextWeekAvailable}");
                }

                else if (habitContent.repetition == (int)Repetition.Monthly)
                {
                    state.text = References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.nextMonthAvailable}");
                }

                else if (habitContent.repetition == (int)Repetition.Yearly)
                {
                    state.text = References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.nextYearAvailable}");
                }

                else
                {
                    Debug.LogWarning("Daily habit should never be inactive");
                }
            }
        }

        else
        {
            state.text = References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.habitCompleted}");
            positiveButton.gameObject.SetActive(false);
            negativeButton.gameObject.SetActive(false);
        }

    }

    private void UpdateCountText()
    {
        if (!isCompleted)
        {
            count.gameObject.SetActive(true);
            count.text = (Repetition)habitContent.repetition switch
            {
                Repetition.Daily => $"{habitContent.count}/{90}",
                Repetition.Weekly => $"{habitContent.count}/{12}",
                Repetition.Monthly => $"{habitContent.count}/{6}",
                Repetition.Yearly => $"{habitContent.count}/{3}",
                _ => "Unknown" // Default case

            };
        }

        else
        { count.gameObject.SetActive(false); }

    }

    public async void OnPositive()
    {
        if (!habitContent.isActive)
        {
            return;
        }

        (bool successfullyGetValue, int value) = await FirebaseSaveManager.GetLastSavedHabitMark(habitContent);

        if ((false, 0) == (successfullyGetValue, value))
        {
            Bus<OnAddToast>.CallEvent(new OnAddToast(new PopupConfig(MarkStatus.Neutral, References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.somethingWrong}"), PopupType.System)));
            Debug.LogWarning("Error getting last saved habit mark.");
            return;
        }

        if (MarkStatus.Positive == (MarkStatus)value)
        {
            if (habitContent.repetition == (int)Repetition.Daily)
                Bus<OnAddToast>.CallEvent(new OnAddToast(new PopupConfig(MarkStatus.Neutral, References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.dailyAlreadyCounted}"), PopupType.System)));
            else if (habitContent.repetition == (int)Repetition.Weekly)
                Bus<OnAddToast>.CallEvent(new OnAddToast(new PopupConfig(MarkStatus.Neutral, References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.weeklyAlreadyCounted}"), PopupType.System)));
            else if (habitContent.repetition == (int)Repetition.Monthly)
                Bus<OnAddToast>.CallEvent(new OnAddToast(new PopupConfig(MarkStatus.Neutral, References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.monthlyAlreadyCounted}"), PopupType.System)));
            else if (habitContent.repetition == (int)Repetition.Yearly)
                Bus<OnAddToast>.CallEvent(new OnAddToast(new PopupConfig(MarkStatus.Neutral, References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.yearlyAlreadyCounted}"), PopupType.System)));

            return;
        }

        HabitContent updatedHabit = new HabitContent(habitContent.title, habitContent.notes, habitContent.difficulty, habitContent.repetition, habitContent.isCompleted, habitContent.count += 1, DateTime.UtcNow.ToString("o"), (int)MarkStatus.Positive, habitContent.isActive);

        try
        {
            positiveButton.interactable = false;
            negativeButton.interactable = false;

            string coinId = References.Instance.inventory.coin.saveableEntityId;
            string gemId = References.Instance.inventory.gem.saveableEntityId;

            int xpReward = References.Instance.experienceConfigurations.experiencesTierFromHabits[(int)habitContent.difficulty].experience;
            int coinReward = References.Instance.experienceConfigurations.experiencesTierFromHabits[(int)habitContent.difficulty].coins;
            int gemReward = References.Instance.experienceConfigurations.experiencesTierFromHabits[(int)habitContent.difficulty].gems;
            int overflowXp = 0;
            int nextLevelXp = 0;
            int gainExperience = xpReward;
            bool success = false;

            if (References.Instance.player.currentCharacter.statuses[(int)Status.Experience].currentValue + xpReward >= References.Instance.player.currentCharacter.statuses[(int)Status.Experience].maxValue)
            {
                //Level up
                xpReward = References.Instance.player.currentCharacter.statuses[(int)Status.Experience].maxValue - References.Instance.player.currentCharacter.statuses[(int)Status.Experience].currentValue;
                overflowXp = References.Instance.experienceConfigurations.experiencesTierFromHabits[(int)habitContent.difficulty].experience - xpReward;
                nextLevelXp = References.Instance.experienceConfigurations.GetXpForNextLevel(References.Instance.player.currentCharacter.level + 1);
                success = await FirebaseSaveManager.UpdateHabitOnMark(habitContent.id, updatedHabit, References.Instance.player.currentCharacter, xpReward, coinReward, gemReward, overflowXp, nextLevelXp);
            }

            else
            {
                //normal gain
                success = await FirebaseSaveManager.UpdateHabitOnMark(habitContent.id, updatedHabit, References.Instance.player.currentCharacter, xpReward, coinReward, gemReward, overflowXp);
            }

            if (success)
            {
                string id = habitContent.id;
                habitContent = updatedHabit;
                updatedHabit.id = id;
                habitContent.id = id;
                References.Instance.player.currentCharacter.inventory[coinId] += coinReward;
                References.Instance.player.currentCharacter.inventory[gemId] += gemReward;

                UpdateCountText();

                bool didLevelUp = overflowXp >= 0 && nextLevelXp > 0;

                Bus<ChangeCoinsEvent>.CallEvent(new ChangeCoinsEvent(References.Instance.player.currentCharacter.inventory[coinId]));
                Bus<ChangeGemsEvent>.CallEvent(new ChangeGemsEvent(References.Instance.player.currentCharacter.inventory[gemId]));

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
                    References.Instance.player.currentCharacter.statuses[6].OnValueChange(References.Instance.player.currentCharacter.statuses[6].currentValue + gainExperience);
                    Debug.Log($"✓ Habit {habitContent.id} updated | +{xpReward} XP, +{coinReward} coins, +{gemReward} gems");
                }

                if (gainExperience > 0)
                    References.Instance.ReceivingExperienceToast(gainExperience);
                if (coinReward > 0)
                    References.Instance.ReceivingCoinsToast(coinReward);
                if (gemReward > 0)
                    References.Instance.ReceivingGemsToast(gemReward);
            }

            else
            {
                Debug.LogWarning($"Error updating habit.");
                Bus<OnAddToast>.CallEvent(new OnAddToast(new PopupConfig(MarkStatus.Neutral, References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.somethingWrong}"), PopupType.System)));
            }
        }

        catch (Exception e)
        {
            Debug.LogWarning($"Error updating habit: {e.Message}");
            return;
        }

        finally
        {
            if (habitContent.isActive)
            {
                positiveButton.interactable = true;
                negativeButton.interactable = true;
            }
        }
    }

    public async void OnNegative()
    {
        if (!habitContent.isActive)
        {
            return;
        }

        (bool successfullyGetValue, int value) = await FirebaseSaveManager.GetLastSavedHabitMark(habitContent);

        if ((false, 0) == (successfullyGetValue, value))
        {
            Bus<OnAddToast>.CallEvent(new OnAddToast(new PopupConfig(MarkStatus.Neutral, References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.somethingWrong}"), PopupType.System)));
            Debug.LogWarning("Error getting last saved habit mark.");
            return;
        }

        if (MarkStatus.Negative == (MarkStatus)value)
        {
            if (habitContent.repetition == (int)Repetition.Daily)
                Bus<OnAddToast>.CallEvent(new OnAddToast(new PopupConfig(MarkStatus.Neutral, References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.dailyAlreadyCounted}"), PopupType.System)));
            else if (habitContent.repetition == (int)Repetition.Weekly)
                Bus<OnAddToast>.CallEvent(new OnAddToast(new PopupConfig(MarkStatus.Neutral, References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.weeklyAlreadyCounted}"), PopupType.System)));
            else if (habitContent.repetition == (int)Repetition.Monthly)
                Bus<OnAddToast>.CallEvent(new OnAddToast(new PopupConfig(MarkStatus.Neutral, References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.monthlyAlreadyCounted}"), PopupType.System)));
            else if (habitContent.repetition == (int)Repetition.Yearly)
                Bus<OnAddToast>.CallEvent(new OnAddToast(new PopupConfig(MarkStatus.Neutral, References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.yearlyAlreadyCounted}"), PopupType.System)));

            return;
        }

        HabitContent updatedHabit = new HabitContent(habitContent.title, habitContent.notes, habitContent.difficulty, habitContent.repetition, habitContent.isCompleted, 0, DateTime.UtcNow.ToString("o"), (int)MarkStatus.Negative, habitContent.isActive);

        try
        {
            positiveButton.interactable = false;
            negativeButton.interactable = false;

            string coinId = References.Instance.inventory.coin.saveableEntityId;
            string gemId = References.Instance.inventory.gem.saveableEntityId;

            int xpReward = References.Instance.experienceConfigurations.experiencesTierFromHabits[(int)habitContent.difficulty].experience;
            int coinReward = References.Instance.experienceConfigurations.experiencesTierFromHabits[(int)habitContent.difficulty].coins;
            int gemReward = References.Instance.experienceConfigurations.experiencesTierFromHabits[(int)habitContent.difficulty].gems;
            int overflowXp = 0;
            int nextLevelXp = 0;
            int losingExperience = xpReward;

            bool success = false;


            if (References.Instance.player.currentCharacter.statuses[(int)Status.Experience].currentValue - xpReward <= 0 && References.Instance.player.currentCharacter.level > 1)
            {
                //Level Down
                nextLevelXp = References.Instance.experienceConfigurations.GetXpForNextLevel(References.Instance.player.currentCharacter.level - 1);
                overflowXp = nextLevelXp - (References.Instance.experienceConfigurations.experiencesTierFromHabits[(int)habitContent.difficulty].experience - References.Instance.player.currentCharacter.statuses[(int)Status.Experience].currentValue);
                xpReward = References.Instance.player.currentCharacter.statuses[(int)Status.Experience].currentValue;
                success = await FirebaseSaveManager.UpdateHabitOnMark(habitContent.id, updatedHabit, References.Instance.player.currentCharacter, -xpReward, -coinReward, -gemReward, overflowXp, nextLevelXp);
            }

            else
            {
                if (References.Instance.player.currentCharacter.statuses[(int)Status.Experience].currentValue - xpReward <= 0)
                {
                    success = await FirebaseSaveManager.UpdateHabitOnMark(habitContent.id, updatedHabit, References.Instance.player.currentCharacter, -References.Instance.player.currentCharacter.statuses[(int)Status.Experience].currentValue, -coinReward, -gemReward, 0, 0);
                }
                else
                {
                    success = await FirebaseSaveManager.UpdateHabitOnMark(habitContent.id, updatedHabit, References.Instance.player.currentCharacter, -xpReward, -coinReward, -gemReward, 0, 0);
                }

            }

            if (success)
            {
                string id = habitContent.id;
                habitContent = updatedHabit;
                updatedHabit.id = id;
                habitContent.id = id;
                habitContent.count = 0;

                //remove reward
                References.Instance.player.currentCharacter.inventory[coinId] -= coinReward;
                References.Instance.player.currentCharacter.inventory[gemId] -= gemReward;

                UpdateCountText();

                bool didLevelDown = overflowXp >= 0 && nextLevelXp > 0;

                Bus<ChangeCoinsEvent>.CallEvent(new ChangeCoinsEvent(References.Instance.player.currentCharacter.inventory[coinId]));
                Bus<ChangeGemsEvent>.CallEvent(new ChangeGemsEvent(References.Instance.player.currentCharacter.inventory[gemId]));

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
                    References.Instance.player.currentCharacter.statuses[6].OnValueChange(References.Instance.player.currentCharacter.statuses[6].currentValue - losingExperience);
                }

                if (losingExperience > 0)
                    References.Instance.LosingExperienceToast(losingExperience);
                if (coinReward > 0)
                    References.Instance.LosingCoinsToast(coinReward);
                if (gemReward > 0)
                    References.Instance.LosingGemsToast(gemReward);
            }

            else
            {
                Debug.LogWarning($"Error updating habit.");
                Bus<OnAddToast>.CallEvent(new OnAddToast(new PopupConfig(MarkStatus.Neutral, References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.somethingWrong}"), PopupType.System)));
            }
        }

        catch (Exception e)
        {
            Debug.LogWarning($"Error updating habit: {e.Message}");
            return;
        }

        finally
        {
            positiveButton.interactable = true;
            negativeButton.interactable = true;
        }
    }

    public void OnOptions()
    {
        optionsPanel.SetActive(!optionsPanel.activeSelf);
    }

    public void OnEdit()
    {
        Bus<OnEditHabitOptionEvent>.CallEvent(new OnEditHabitOptionEvent(habitContent));
    }

    public void OnDelete()
    {
        Bus<OnRemoveHabitOption>.CallEvent(new OnRemoveHabitOption(habitContent));
    }

    public void OnUp()
    {

    }

    public void OnDown()
    {

    }
}
