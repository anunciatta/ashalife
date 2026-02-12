
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DailiesPanel : Panel
{
    public DailySortingButton[] sortingButtons;
    public SortingType currentSortingType;

    [SerializeField] private Button newDailyButton;

    [SerializeField] private GameObject dailyPrefab;

    private List<Daily> activeDailies = new();
    private List<Daily> dailiesPool = new();
    private List<DailyContent> activeDailyContent = new();
    private List<DailyContent> activeTasks = new();


    [SerializeField] private Transform poolContainer;
    [SerializeField] private Transform dailiesContainer;

    public void OnDailyCompleted(Daily daily)
    {
        if (activeDailies.Contains(daily))
        {
            daily.transform.SetParent(poolContainer);
            daily.gameObject.SetActive(false);
            dailiesPool.Add(daily);
            activeDailies.Remove(daily);
        }
    }

    public void AddDaily(OnAddNewDaily data)
    {
        activeDailyContent.Add(data.DailyContent);
    }

    public void OnUpdateDaily(OnUpdateDaily data)
    {
        int index = activeDailyContent.FindIndex(x => x.id == data.DailyContent.id);

        if (index != -1)
        {
            activeDailyContent[index] = data.DailyContent;
        }

        else
        {
            Debug.LogWarning($"Daily with ID {data.DailyContent.id} not found in activeDailyContent during update.");
        }
    }

    public void OnRemoveDailyComfirmed(OnRemoveDailyConfirm data)
    {
        int index = activeDailyContent.FindIndex(x => x.id == data.DailyContent.id);
        int _index = activeDailies.FindIndex(x => x.dailyContent.id == data.DailyContent.id);

        if (_index != -1 && index != -1)
        {
            Daily dailyToRemove = activeDailies[_index];
            dailyToRemove.transform.SetParent(poolContainer);
            dailyToRemove.gameObject.SetActive(false);
            dailiesPool.Add(dailyToRemove);
            activeDailies.RemoveAt(_index);
            activeDailyContent.RemoveAt(index);
        }

        else
        {
            Debug.LogWarning($"Daily with ID {data.DailyContent.id} not found in activeDailyContent during removal.");
        }
    }

    public void ShowDailiesForToday()
    {
        ClearDailies();

        // Get today's date (local time, date only)
        DateTime today = DateTime.Now.Date;

        activeTasks.Clear();

        if (currentSortingType == SortingType.Today)
        {
            // Filter tasks that are active today
            activeTasks = activeDailyContent
                 .Where(task => task.IsActiveOnDate(today))
                 .ToList();
        }

        else if (currentSortingType == SortingType.Done)
        {
            // Filter tasks that are active today
            activeTasks = activeDailyContent
                 .Where(task => task.IsActiveOnDate(today) && task.isCompleted)
                 .ToList();
        }

        else if (currentSortingType == SortingType.Pending)
        {
            // Filter tasks that are active today
            activeTasks = activeDailyContent
                 .Where(task => task.IsActiveOnDate(today) && !task.isCompleted)
                 .ToList();
        }

        else
        {
            activeTasks.AddRange(activeDailyContent);
        }

        DisplayDailies(activeTasks);
    }

    private void DisplayDailies(List<DailyContent> activeTasks)
    {
        for (int i = 0; i < activeTasks.Count; i++)
        {
            if (dailiesPool.Count <= 0)
            {
                Daily newDaily = Instantiate(dailyPrefab, dailiesContainer).GetComponent<Daily>();
                newDaily.UpdateContent(activeTasks[i], this);
                activeDailies.Add(newDaily);
            }

            else
            {
                dailiesPool[0].UpdateContent(activeTasks[i], this);
                activeDailies.Add(dailiesPool[0]);
                dailiesPool[0].transform.SetParent(dailiesContainer);
                dailiesPool[0].gameObject.SetActive(true);
                dailiesPool.RemoveAt(0);
            }
        }
    }

    private void OnLanguageChanged(OnLanguageChanged data)
    {
        foreach (Daily daily in activeDailies)
        {
            daily.UpdateLanguage(data.NewLanguage);
        }
    }

    private void ClearDailies()
    {
        foreach (var daily in activeDailies)
        {
            daily.transform.SetParent(poolContainer);
            daily.gameObject.SetActive(false);
            dailiesPool.Add(daily);
        }

        activeDailies.Clear();
    }

    public override void Open()
    {
        foreach (var button in sortingButtons)
        {
            button.Initialize(this, false);
        }

        sortingButtons[0].OnSelect();

        base.Open();

        Bus<DisplayNavigationBarEvent>.CallEvent(new DisplayNavigationBarEvent(true));
        Bus<DisplayStatusBarEvent>.CallEvent(new DisplayStatusBarEvent(true));

    }

    public override void Close()
    {
        base.Close();
        ClearDailies();
    }

    public override void Initialize()
    {
        panel.SetActive(true);

        newDailyButton.onClick.AddListener(OnNewDaily);

        Bus<OnAddNewDaily>.OnEvent += AddDaily;
        Bus<OnSignInFirebase>.OnEvent += OnSignIn;
        Bus<OnEditDailyOptionEvent>.OnEvent += OnEditDailyOption;
        Bus<OnUpdateDaily>.OnEvent += OnUpdateDaily;
        Bus<OnRemoveDailyConfirm>.OnEvent += OnRemoveDailyComfirmed;
        Bus<OnLanguageChanged>.OnEvent += OnLanguageChanged;

        sortingButtons = panel.GetComponentsInChildren<DailySortingButton>(true);

        foreach (var button in sortingButtons)
        {
            button.Initialize(this, false);
        }
    }

    private void OnSignIn(OnSignInFirebase data)
    {
        GetAllDailies();
    }

    async void GetAllDailies()
    {
        activeDailyContent = await FirebaseSaveManager.GetAllDailies();
    }

    private void OnDestroy()
    {
        newDailyButton.onClick.RemoveAllListeners();
        Bus<OnAddNewDaily>.OnEvent -= AddDaily;
        Bus<OnSignInFirebase>.OnEvent -= OnSignIn;
        Bus<OnLanguageChanged>.OnEvent -= OnLanguageChanged;
        Bus<OnEditDailyOptionEvent>.OnEvent -= OnEditDailyOption;
        Bus<OnUpdateDaily>.OnEvent -= OnUpdateDaily;
        Bus<OnRemoveDailyConfirm>.OnEvent -= OnRemoveDailyComfirmed;
    }

    private void OnEditDailyOption(OnEditDailyOptionEvent data)
    {
        References.Instance.newDailyPanel.editingDailyContent = data.DailyContent;

        Bus<DisplayNavigationBarEvent>.CallEvent(new DisplayNavigationBarEvent(false));
        Bus<DisplayStatusBarEvent>.CallEvent(new DisplayStatusBarEvent(false));
        Bus<TransitionPanelsEvent>.CallEvent(new TransitionPanelsEvent(this, nextPanel));
    }

    private void OnNewDaily()
    {
        References.Instance.newDailyPanel.editingDailyContent = null;

        Bus<DisplayNavigationBarEvent>.CallEvent(new DisplayNavigationBarEvent(false));
        Bus<DisplayStatusBarEvent>.CallEvent(new DisplayStatusBarEvent(false));
        Bus<TransitionPanelsEvent>.CallEvent(new TransitionPanelsEvent(this, nextPanel));
    }
}



