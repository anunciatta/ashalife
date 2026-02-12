using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HabitsPanel : Panel
{
    public HabitSortingButton[] sortingButtons;
    //public Repetition currentSortingType;
    public SortingType currentSortingType;

    [SerializeField] private Button newHabitButton;

    [SerializeField] private GameObject habitPrefab;

    private List<Habit> activeHabits = new();
    private List<Habit> habitsPool = new();
    private List<HabitContent> activeHabitContent = new();

    [SerializeField] private Transform poolContainer;
    [SerializeField] private Transform habitsContainer;

    private List<HabitContent> activeTasks = new();

    public void RemoveHabit(Habit habit)
    {
        if (activeHabits.Contains(habit))
        {
            habit.transform.SetParent(poolContainer);
            habit.gameObject.SetActive(false);
            habitsPool.Add(habit);
            activeHabitContent.Remove(habit.habitContent);
            activeHabits.Remove(habit);
        }
    }

    public void TransitionHabit(Habit daily)
    {
        if (activeHabits.Contains(daily))
        {
            daily.transform.SetParent(poolContainer);
            daily.gameObject.SetActive(false);
            habitsPool.Add(daily);
            activeHabits.Remove(daily);
        }
    }

    private void ClearHabits()
    {
        foreach (var habit in activeHabits)
        {
            habit.transform.SetParent(poolContainer);
            habit.gameObject.SetActive(false);
            habitsPool.Add(habit);
        }

        activeHabits.Clear();
    }

    public void ShowHabitsForToday()
    {
        ClearHabits();

        activeTasks.Clear();

        if (currentSortingType == SortingType.All)
        {
            // Display tasks
            DisplayHabits(activeHabitContent);
        }

        else if (currentSortingType == SortingType.Done)
        {
            activeTasks = activeHabitContent
                .Where(task => task.isCompleted)
                .ToList();

            DisplayHabits(activeTasks);
        }

        else if (currentSortingType == SortingType.Today)
        {
            activeTasks = activeHabitContent
                .Where(task => task.isActive)
                .ToList();

            DisplayHabits(activeTasks);
        }

        else
        {
            activeTasks = activeHabitContent
               .Where(task => !task.isCompleted)
               .ToList();

            DisplayHabits(activeTasks);
        }
    }

    private void DisplayHabits(List<HabitContent> activeTasks)
    {
        for (int i = 0; i < activeTasks.Count; i++)
        {
            if (habitsPool.Count <= 0)
            {
                Habit newHabit = Instantiate(habitPrefab, habitsContainer).GetComponent<Habit>();
                newHabit.UpdateContent(activeTasks[i]);
                activeHabits.Add(newHabit);
            }

            else
            {
                habitsPool[0].UpdateContent(activeTasks[i]);
                activeHabits.Add(habitsPool[0]);
                habitsPool[0].transform.SetParent(habitsContainer);
                habitsPool[0].gameObject.SetActive(true);
                habitsPool.RemoveAt(0);
            }
        }
    }

    public void AddHabit(OnAddNewHabit data)
    {
        activeHabitContent.Add(data.HabitContent);
    }

    public override void Open()
    {
        foreach (var button in sortingButtons)
        {
            button.Initialize(this, false);
        }

        sortingButtons[0].OnSelect();

        ShowHabitsForToday();

        base.Open();

        Bus<DisplayNavigationBarEvent>.CallEvent(new DisplayNavigationBarEvent(true));
        Bus<DisplayStatusBarEvent>.CallEvent(new DisplayStatusBarEvent(true));
    }

    public override void Close()
    {
        base.Close();
        ClearHabits();
    }

    public override void Initialize()
    {
        panel.SetActive(false);
        newHabitButton.onClick.AddListener(OnNewHabit);
        Bus<OnAddNewHabit>.OnEvent += AddHabit;
        Bus<OnSignInFirebase>.OnEvent += OnSignIn;
        Bus<OnLanguageChanged>.OnEvent += OnLanguageChanged;

        Bus<OnEditHabitOptionEvent>.OnEvent += OnEditHabitOption;
        Bus<OnUpdateHabit>.OnEvent += OnUpdateDaily;
        Bus<OnRemoveHabitConfirm>.OnEvent += OnRemoveHabitConfirmed;

        sortingButtons = panel.GetComponentsInChildren<HabitSortingButton>(true);

        foreach (var button in sortingButtons)
        {
            button.Initialize(this, false);
        }
    }

    private void OnLanguageChanged(OnLanguageChanged data)
    {
        foreach (var habit in activeHabits)
        {
            habit.UpdateLanguage(data.NewLanguage);
        }
    }

    private void OnSignIn(OnSignInFirebase data)
    {
        GetAllHabits();
    }

    async void GetAllHabits()
    {
        activeHabitContent = await FirebaseSaveManager.GetAllHabits();
    }

    private void OnDestroy()
    {
        newHabitButton.onClick.RemoveAllListeners();
        Bus<OnAddNewHabit>.OnEvent -= AddHabit;
        Bus<OnSignInFirebase>.OnEvent -= OnSignIn;
        Bus<OnLanguageChanged>.OnEvent -= OnLanguageChanged;

        Bus<OnEditHabitOptionEvent>.OnEvent -= OnEditHabitOption;
        Bus<OnUpdateHabit>.OnEvent -= OnUpdateDaily;
        Bus<OnRemoveHabitConfirm>.OnEvent -= OnRemoveHabitConfirmed;
    }

    private void OnNewHabit()
    {
        Bus<DisplayNavigationBarEvent>.CallEvent(new DisplayNavigationBarEvent(false));
        Bus<DisplayStatusBarEvent>.CallEvent(new DisplayStatusBarEvent(false));
        Bus<TransitionPanelsEvent>.CallEvent(new TransitionPanelsEvent(this, nextPanel));

    }

    private void OnEditHabitOption(OnEditHabitOptionEvent data)
    {
        References.Instance.newHabitPanel.editingHabitContent = data.HabitContent;

        Bus<DisplayNavigationBarEvent>.CallEvent(new DisplayNavigationBarEvent(false));
        Bus<DisplayStatusBarEvent>.CallEvent(new DisplayStatusBarEvent(false));
        Bus<TransitionPanelsEvent>.CallEvent(new TransitionPanelsEvent(this, nextPanel));
    }

    public void OnUpdateDaily(OnUpdateHabit data)
    {
        int index = activeHabitContent.FindIndex(x => x.id == data.HabitContent.id);

        if (index != -1)
        {
            activeHabitContent[index] = data.HabitContent;
        }

        else
        {
            Debug.LogWarning($"Daily with ID {data.HabitContent.id} not found in activeDailyContent during update.");
        }
    }

    public void OnRemoveHabitConfirmed(OnRemoveHabitConfirm data)
    {
        int index = activeHabitContent.FindIndex(x => x.id == data.HabitContent.id);
        int _index = activeHabits.FindIndex(x => x.habitContent.id == data.HabitContent.id);

        if (_index != -1 && index != -1)
        {
            Habit dailyToRemove = activeHabits[_index];
            dailyToRemove.transform.SetParent(poolContainer);
            dailyToRemove.gameObject.SetActive(false);
            habitsPool.Add(dailyToRemove);
            activeHabits.RemoveAt(_index);
            activeHabitContent.RemoveAt(index);
        }

        else
        {
            Debug.LogWarning($"Daily with ID {data.HabitContent.id} not found in activeDailyContent during removal.");
        }
    }
}


