using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewHabitPanel : Panel, ITaskEditable
{
    private DifficultyButton[] difficultyButtons;
    public DifficultyButton[] GetDifficultyButtons() => difficultyButtons;

    private RepetitionButton[] repetitionButtons;
    public RepetitionButton[] GetRepetitionButtons() => repetitionButtons;

    [SerializeField] private TMP_InputField titleInput;
    [SerializeField] private TMP_InputField notesInput;
    [SerializeField] private TMP_InputField countInput;
    [SerializeField] private GameObject somethingWrongWithHabitCount;
    [SerializeField] private GameObject somethingWrongWithHabitName;

    private Difficulty currentDifficulty;
    private Repetition currentRepetition = Repetition.Daily;
    [SerializeField] private Button continueButton;
    //For Editing Tasks
    public HabitContent editingHabitContent;
    [SerializeField] private TextMeshProUGUI buttonText;

    private void OnEditHabit(OnEditHabitOptionEvent data)
    {
        editingHabitContent = data.HabitContent;
    }

    private void OnDestroy()
    {
        Bus<OnEditHabitOptionEvent>.OnEvent -= OnEditHabit;
        Bus<OnLanguageChanged>.OnEvent -= OnLanguageChange;
    }

    private void OnLanguageChange(OnLanguageChanged data)
    {
        foreach (var btn in difficultyButtons)
        {
            btn.UpdateLanguage();
        }

        foreach (var btn in repetitionButtons)
        {
            btn.UpdateLanguage();
        }
    }

    public override void Initialize()
    {
        base.Initialize();

        Bus<OnEditHabitOptionEvent>.OnEvent += OnEditHabit;
        Bus<OnLanguageChanged>.OnEvent += OnLanguageChange;

        difficultyButtons = panel.GetComponentsInChildren<DifficultyButton>(true);
        repetitionButtons = panel.GetComponentsInChildren<RepetitionButton>(true);

        for (int i = 0; i < difficultyButtons.Length; i++)
        {
            difficultyButtons[i].Initialize(this, false, (Difficulty)i);
        }

        for (int i = 0; i < repetitionButtons.Length; i++)
        {
            repetitionButtons[i].Initialize(this, false, (Repetition)i);
        }

        SetRepetition(Repetition.Daily);
        ResetInputs();
    }

    public override void Open()
    {
        ResetInputs();

        base.Open();

        if (editingHabitContent != null && editingHabitContent?.id != string.Empty)
        {
            buttonText.text = References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.saveButton}");

            // Populate fields for editing
            titleInput.text = editingHabitContent.title;
            notesInput.text = editingHabitContent.notes;
            countInput.text = editingHabitContent.count.ToString();

            foreach (var btn in GetDifficultyButtons())
            {
                btn.OnDeselect();
            }

            SetDifficulty((Difficulty)editingHabitContent.difficulty);
            difficultyButtons[(int)editingHabitContent.difficulty].OnSelect();

            foreach (var btn in GetRepetitionButtons())
            {
                btn.OnDeselect();
            }

            SetRepetition((Repetition)editingHabitContent.repetition);
            repetitionButtons[(int)editingHabitContent.repetition].OnSelect();
        }

        else
        {
            SetRepetition(Repetition.Daily);

            CloseAllRelatedPanels();

            foreach (var btn in GetDifficultyButtons())
            {
                btn.OnDeselect();
            }

            foreach (var btn in GetRepetitionButtons())
            {
                btn.OnDeselect();
            }

            difficultyButtons[0].OnSelect();
            repetitionButtons[0].OnSelect();
        }
    }

    public override void Close()
    {
        base.Close();
        ResetInputs();
    }

    private void ResetInputs()
    {
        titleInput.text = string.Empty;
        notesInput.text = string.Empty;
        countInput.text = string.Empty;

        somethingWrongWithHabitCount.SetActive(false);
        somethingWrongWithHabitName.SetActive(false);
    }

    public async void OnAddButton()
    {
        somethingWrongWithHabitCount.SetActive(false);
        somethingWrongWithHabitName.SetActive(false);

        if (string.IsNullOrWhiteSpace(titleInput.text))
        {
            somethingWrongWithHabitName.SetActive(true);
            return;
        }

        HabitContent habitContent;

        bool success;
        bool isActive = false;

        int count;

        if (!int.TryParse(countInput.text, out count))
        {

            if (string.IsNullOrWhiteSpace(countInput.text))
            {
                count = 0;
            }

            else
            {
                // Invalid input
                somethingWrongWithHabitCount.SetActive(true);
                return;
            }
        }

        //Editing Existing Habit
        if (editingHabitContent != null && editingHabitContent?.id != string.Empty)
        {
            try
            {
                continueButton.interactable = false;

                if ((Repetition)currentRepetition == Repetition.Daily && !editingHabitContent.isCompleted)
                {

                    if (count > 89 || count < 0)
                    {
                        somethingWrongWithHabitCount.SetActive(true);
                        return;
                    }

                    isActive = true;
                }

                else if ((Repetition)currentRepetition == Repetition.Weekly && !editingHabitContent.isCompleted && DateTime.Now.Date.DayOfWeek == DayOfWeek.Sunday)
                {
                    if (count > 11 || count < 0)
                    {
                        somethingWrongWithHabitCount.SetActive(true);
                        return;
                    }

                    isActive = true;
                }

                else if ((Repetition)currentRepetition == Repetition.Monthly && !editingHabitContent.isCompleted && DateTime.Now.Date.Day == DateTime.DaysInMonth(DateTime.Now.Date.Year, DateTime.Now.Date.Month))
                {
                    if (count > 5 || count < 0)
                    {
                        somethingWrongWithHabitCount.SetActive(true);
                        return;
                    }

                    isActive = true;
                }

                else if ((Repetition)currentRepetition == Repetition.Yearly && !editingHabitContent.isCompleted && DateTime.Now.Date.Day == 31 && DateTime.Now.Date.Month == 12)
                {
                    if (count > 2 || count < 0)
                    {
                        somethingWrongWithHabitCount.SetActive(true);
                        return;
                    }

                    isActive = true;
                }

                habitContent =
                        new HabitContent(
                            titleInput.text,
                            notesInput.text,
                            (int)currentDifficulty,
                            (int)currentRepetition,
                            editingHabitContent.isCompleted,
                            editingHabitContent.count,
                            editingHabitContent.lastPositiveCheck,
                            editingHabitContent.lastMark,
                            isActive
                            );

                habitContent.id = editingHabitContent.id;

                success = await FirebaseSaveManager.EditHabit(editingHabitContent.id, habitContent);

                if (success)
                {
                    Bus<OnUpdateHabit>.CallEvent(new OnUpdateHabit(habitContent));
                    Bus<TransitionPanelsEvent>.CallEvent(new TransitionPanelsEvent(this, nextPanel));
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
                continueButton.interactable = true;
            }
        }

        else
        {
            try
            {
                continueButton.interactable = false;

                if ((Repetition)currentRepetition == Repetition.Daily)
                {
                    isActive = true;
                }

                else if ((Repetition)currentRepetition == Repetition.Weekly && DateTime.Now.Date.DayOfWeek == DayOfWeek.Sunday)
                {
                    isActive = true;
                }

                else if ((Repetition)currentRepetition == Repetition.Monthly && DateTime.Now.Date.Day == DateTime.DaysInMonth(DateTime.Now.Date.Year, DateTime.Now.Date.Month))
                {
                    isActive = true;
                }

                else if ((Repetition)currentRepetition == Repetition.Yearly && DateTime.Now.Date.Day == 31 && DateTime.Now.Date.Month == 12)
                {
                    isActive = true;
                }

                habitContent =
                        new HabitContent(
                            titleInput.text,
                            notesInput.text,
                            (int)currentDifficulty,
                            (int)currentRepetition,
                            false,
                            0,
                            DateTime.MinValue.ToUniversalTime().ToString("o"),
                            (int)MarkStatus.Neutral,
                            isActive
                            );

                success = await FirebaseSaveManager.AddNewHabit(habitContent);

                if (success)
                {
                    Bus<OnAddNewHabit>.CallEvent(new OnAddNewHabit(habitContent));
                    Bus<TransitionPanelsEvent>.CallEvent(new TransitionPanelsEvent(this, nextPanel));
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
                continueButton.interactable = true;
            }
        }


    }

    public void SetDifficulty(Difficulty difficulty) => currentDifficulty = difficulty;

    public void SetRepetition(Repetition repetition) => currentRepetition = repetition;

    public void OnBack()
    {
        Bus<TransitionPanelsEvent>.CallEvent(new TransitionPanelsEvent(this, previousPanel));
    }

    #region Not in use

    public void AddDayOfTheWeek(DayOfWeek dayOfWeek)
    {
        return;
    }

    public void RemoveDayOfTheWeek(DayOfWeek dayOfWeek)
    {
        return;
    }

    public void CloseAllRelatedPanels()
    {
        return;
    }
    #endregion

}

