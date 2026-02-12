using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TMPro;
using UI.Dates;
using UnityEngine;
using UnityEngine.UI;

public interface ITaskEditable
{
    public void SetDifficulty(Difficulty difficulty);
    public void SetRepetition(Repetition repetition);
    public void AddDayOfTheWeek(DayOfWeek dayOfWeek);
    public void RemoveDayOfTheWeek(DayOfWeek dayOfWeek);
    public void CloseAllRelatedPanels();
    public DifficultyButton[] GetDifficultyButtons();
    public RepetitionButton[] GetRepetitionButtons();
}

public class NewDailyPanel : Panel, ITaskEditable
{
    private DayOfWeekButton[] daysOfTheWeekButtons;
    private List<DayOfWeek> daysOfWeek = new();

    private DifficultyButton[] difficultyButtons;
    public DifficultyButton[] GetDifficultyButtons() => difficultyButtons;

    private RepetitionButton[] repetitionButtons;
    public RepetitionButton[] GetRepetitionButtons() => repetitionButtons;

    [SerializeField] private TMP_InputField titleInput;
    [SerializeField] private TMP_InputField notesInput;
    [SerializeField] private TMP_InputField repeatEveryInput;
    [SerializeField] private TMP_InputField strartDateInput;
    [SerializeField] private TextMeshProUGUI repeatEvery;
    [SerializeField] private GameObject[] relatedPanels;
    [SerializeField] private DatePicker datePicker;

    private DateTime selectedStartDate;
    private Repetition currentRepetition = Repetition.Daily;
    private Difficulty currentDifficulty;
    [SerializeField] private Button continueButton;

    //For Editing Tasks
    public DailyContent editingDailyContent;
    [SerializeField] private TextMeshProUGUI buttonText;

    public async void OnAddButton()
    {
        if (string.IsNullOrWhiteSpace(titleInput.text) || string.IsNullOrWhiteSpace(strartDateInput.text))
        {
            return;
        }

        List<int> _daysOfWeek = daysOfWeek.Select(day => (int)day).ToList();
        int repeatEvery = 0;

        // Handle empty or whitespace
        if (string.IsNullOrWhiteSpace(repeatEveryInput.text))
        {
            repeatEvery = 0;
        }

        // Parse and validate >= 0
        else if (int.TryParse(repeatEveryInput.text, out int value) && value >= 0)
        {
            repeatEvery = value;
        }

        else
        {
            repeatEvery = 0;
            // Invalid input (negative, just "-", etc.)
            References.Instance.AddToast(References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.invalidInput}"), MarkStatus.Neutral, PopupType.System);
            return;
        }

        bool success = false;

        //Editing Existing Daily
        if (editingDailyContent != null && editingDailyContent?.id != string.Empty)
        {
            try
            {
                DateTime utcStartDate = selectedStartDate.ToUniversalTime();
                DailyContent dailyContent;

                bool isActive;

                if (repeatEvery == 0 && utcStartDate.Date < DateTime.Now.Date)
                {
                    isActive = false;
                }

                else
                {
                    isActive = true;
                }

                dailyContent =
                            new DailyContent(
                                titleInput.text,
                                notesInput.text,
                                (int)currentDifficulty,
                                utcStartDate.ToString("o"),
                                (int)currentRepetition,
                                _daysOfWeek,
                                repeatEvery,
                                editingDailyContent.isCompleted,
                                isActive
                                );

                dailyContent.id = editingDailyContent.id;
                continueButton.interactable = false;


                success = await FirebaseSaveManager.EditDaily(editingDailyContent.id, dailyContent);

                if (success)
                {
                    Bus<OnUpdateDaily>.CallEvent(new OnUpdateDaily(dailyContent));
                    Bus<TransitionPanelsEvent>.CallEvent(new TransitionPanelsEvent(this, nextPanel));
                }

                else
                {
                    Debug.LogWarning($"Error updating daily.");
                    Bus<OnAddToast>.CallEvent(new OnAddToast(new PopupConfig(MarkStatus.Neutral, References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.somethingWrong}"), PopupType.System)));
                }
            }

            catch (Exception e)
            {
                Debug.LogWarning($"Error updating daily: {e.Message}");
                return;
            }

            finally
            {
                continueButton.interactable = true;
            }
        }

        //Creating New Daily
        else
        {
            try
            {
                if (selectedStartDate < DateTime.Now.Date)
                {
                    selectedStartDate = DateTime.Now.Date;
                }

                // Convert to UTC for storage
                DateTime utcStartDate = selectedStartDate.ToUniversalTime();
                DailyContent dailyContent;

                dailyContent =
                            new DailyContent(
                                titleInput.text,
                                notesInput.text,
                                (int)currentDifficulty,
                                utcStartDate.ToString("o"),
                                (int)currentRepetition,
                                _daysOfWeek,
                                repeatEvery,
                                false, true
                                );

                continueButton.interactable = false;

                success = await FirebaseSaveManager.AddNewDaily(dailyContent);

                if (success)
                {
                    Bus<OnAddNewDaily>.CallEvent(new OnAddNewDaily(dailyContent));
                    Bus<TransitionPanelsEvent>.CallEvent(new TransitionPanelsEvent(this, nextPanel));
                }

                else
                {
                    Debug.LogWarning($"Error adding new daily.");
                    Bus<OnAddToast>.CallEvent(new OnAddToast(new PopupConfig(MarkStatus.Neutral, References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.somethingWrong}"), PopupType.System)));
                }
            }

            catch (Exception e)
            {
                Debug.LogWarning($"Error updating daily: {e.Message}");
                return;
            }

            finally
            {
                continueButton.interactable = true;
            }
        }
    }

    public void SetDifficulty(Difficulty difficulty) => currentDifficulty = difficulty;

    public void OnDateSelected(DateTime dateTime)
    {
        selectedStartDate = dateTime;
    }

    public override void Open()
    {
        base.Open();

        if (editingDailyContent != null && editingDailyContent?.id != string.Empty)
        {
            buttonText.text = References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.saveButton}");

            // Populate fields for editing
            titleInput.text = editingDailyContent.title;
            notesInput.text = editingDailyContent.notes;

            datePicker.DayButtonClicked(References.Instance.ParseDateTime(editingDailyContent.startDate));

            foreach (var btn in GetDifficultyButtons())
            {
                btn.OnDeselect();
            }

            SetDifficulty((Difficulty)editingDailyContent.difficulty);
            difficultyButtons[(int)editingDailyContent.difficulty].OnSelect();

            foreach (var btn in GetRepetitionButtons())
            {
                btn.OnDeselect();
            }

            SetRepetition((Repetition)editingDailyContent.repetition);
            repetitionButtons[(int)editingDailyContent.repetition].OnSelect();

            daysOfWeek.Clear();

            foreach (var day in daysOfTheWeekButtons)
            {
                day.OnDeselect();
            }

            if (editingDailyContent.daysOfWeek != null)
            {
                foreach (var day in editingDailyContent.daysOfWeek)
                {
                    DayOfWeek dayOfWeek = (DayOfWeek)day;
                    daysOfWeek.Add(dayOfWeek);
                    daysOfTheWeekButtons[(int)dayOfWeek].OnSelect();
                }
            }


            repeatEveryInput.text = editingDailyContent.repeatEveryDays.ToString();
        }

        else
        {
            buttonText.text = References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.createButton}");

            SetRepetition(Repetition.Daily);
            ResetInputs();
            CloseAllRelatedPanels();

            foreach (var btn in GetDifficultyButtons())
            {
                btn.OnDeselect();
            }

            foreach (var btn in GetRepetitionButtons())
            {
                btn.OnDeselect();
            }

            foreach (var btn in daysOfTheWeekButtons)
            {
                btn.OnDeselect();
            }

            daysOfWeek.Clear();

            difficultyButtons[0].OnSelect();
            repetitionButtons[0].OnSelect();
        }

    }
    public override void Close()
    {
        base.Close();
        ResetInputs();
    }

    private void OnDestroy()
    {
        Bus<OnEditDailyOptionEvent>.OnEvent -= OnEditDaily;
        Bus<OnLanguageChanged>.OnEvent -= OnLanguageChanged;
    }

    private void OnEditDaily(OnEditDailyOptionEvent data)
    {
        editingDailyContent = data.DailyContent;
    }

    private void OnLanguageChanged(OnLanguageChanged data)
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

        Bus<OnEditDailyOptionEvent>.OnEvent += OnEditDaily;
        Bus<OnLanguageChanged>.OnEvent += OnLanguageChanged;
        difficultyButtons = panel.GetComponentsInChildren<DifficultyButton>(true);
        repetitionButtons = panel.GetComponentsInChildren<RepetitionButton>(true);
        daysOfTheWeekButtons = panel.GetComponentsInChildren<DayOfWeekButton>(true);

        for (int i = 0; i < difficultyButtons.Length; i++)
        {
            difficultyButtons[i].Initialize(this, false, (Difficulty)i);
        }

        for (int i = 0; i < repetitionButtons.Length; i++)
        {
            repetitionButtons[i].Initialize(this, false, (Repetition)i);
        }

        for (int i = 0; i < daysOfTheWeekButtons.Length; i++)
        {
            daysOfTheWeekButtons[i].Initialize(this, false, (DayOfWeek)i);
        }

        SetRepetition(Repetition.Daily);
        ResetInputs();
    }

    public void SetRepetition(Repetition repetition)
    {
        currentRepetition = repetition;
        repeatEvery.text = repetition switch
        {
            Repetition.Daily => "Days",
            Repetition.Weekly => "Weeks",
            Repetition.Monthly => "Months",
            Repetition.Yearly => "Years",
            _ => "Unknown" // Default case
        };
    }

    private void ResetInputs()
    {
        titleInput.text = string.Empty;
        notesInput.text = string.Empty;
        repeatEveryInput.text = string.Empty;
        strartDateInput.text = string.Empty;

        editingDailyContent = null;
    }

    public void AddDayOfTheWeek(DayOfWeek dayOfWeek)
    {
        if (!daysOfWeek.Contains(dayOfWeek))
            daysOfWeek.Add(dayOfWeek);
    }

    public void RemoveDayOfTheWeek(DayOfWeek dayOfWeek)
    {
        if (daysOfWeek.Contains(dayOfWeek))
            daysOfWeek.Remove(dayOfWeek);
    }

    public void CloseAllRelatedPanels()
    {
        foreach (var relatedPanel in relatedPanels)
            relatedPanel.SetActive(false);
    }



    public void OnBack()
    {
        Bus<TransitionPanelsEvent>.CallEvent(new TransitionPanelsEvent(this, previousPanel));
    }

}

public enum Repetition
{
    Daily, Weekly, Monthly, Yearly, None
}



