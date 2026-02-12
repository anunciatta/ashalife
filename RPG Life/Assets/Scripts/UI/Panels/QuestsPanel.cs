using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class QuestsPanel : Panel
{
    public QuestTabButton[] tabButtons;

    [SerializeField] private Button rollButton;

    [SerializeField] private List<QuestsListSO> availableQuests = new();

    private QuestSlot selectedSlot;

    [SerializeField] private GameObject questSlotPrefab;
    [SerializeField] private Transform activeSlotsParent;
    [SerializeField] private Transform poolSlotsParent;

    [HideInInspector] public List<QuestSlot> activeSlots = new();
    private List<QuestSlot> poolSlots = new();
    private QuestsListSO currentQuestList;
    private List<QuestSO> questsToDisplay = new();

    public override void Open()
    {
        base.Open();

        foreach (var btn in tabButtons)
        {
            btn.OnDeselect();
        }

        tabButtons[0].OnSelect();
        rollButton.interactable = true;
    }

    public override void Close()
    {
        base.Close();
    }

    public override void Initialize()
    {
        base.Initialize();

        tabButtons = panel.GetComponentsInChildren<QuestTabButton>(true);
        rollButton.onClick.AddListener(OnRollButton);

        for (int i = 0; i < tabButtons.Length; i++)
        {
            tabButtons[i].Initialize(this, false);
        }

        Bus<OnLanguageChanged>.OnEvent += OnLanguageChanged;
    }

    private void OnLanguageChanged(OnLanguageChanged data)
    {
        foreach (var slot in activeSlots)
        {
            slot.UpdateLanguage(data.NewLanguage);
        }
    }

    private async void OnRollButton()
    {
        if (selectedSlot == null) return;

        //Check Conditions 

        //If pass all conditions
        rollButton.interactable = false;
    }

    void OnDestroy()
    {
        rollButton.onClick.RemoveAllListeners();
        Bus<OnLanguageChanged>.OnEvent -= OnLanguageChanged;
    }

    public void SelectQuest(QuestSlot questSlot)
    {
        if (questSlot == null) return;
        if (questSlot.questSO == null) return;

        selectedSlot = questSlot;
    }

    public void OpenTab(Difficulty difficulty)
    {
        var classType = References.Instance.characterCustomizerConfigurations.classes[References.Instance.player.currentCharacter.avatarConfig.classesIndex].classType;

        currentQuestList = GetCurrentQuestListForPlayerClass(classType);

        ClearSlots();

        selectedSlot = null;

        questsToDisplay.Clear();

        switch (difficulty)
        {
            case Difficulty.Easy:
                questsToDisplay = GetQuestsByDifficulty<QuestSO>(Difficulty.Easy);
                SetSlots(questsToDisplay);
                break;

            case Difficulty.Moderate:
                questsToDisplay = GetQuestsByDifficulty<QuestSO>(Difficulty.Moderate);
                SetSlots(questsToDisplay);
                break;

            case Difficulty.Hard:
                questsToDisplay = GetQuestsByDifficulty<QuestSO>(Difficulty.Hard);
                SetSlots(questsToDisplay);
                break;

            case Difficulty.Trivial:
                questsToDisplay = GetQuestsByDifficulty<QuestSO>(Difficulty.Trivial);
                SetSlots(questsToDisplay);
                break;

            default:
                SetSlots(currentQuestList.quests);
                break;
        }
    }

    private void ClearSlots()
    {
        foreach (var slot in activeSlots)
        {
            slot.Clear();
            slot.transform.SetParent(poolSlotsParent);
            slot.gameObject.SetActive(false);

            if (!poolSlots.Contains(slot))
                poolSlots.Add(slot);
        }

        activeSlots.Clear();
    }

    private List<T> GetQuestsByDifficulty<T>(Difficulty difficulty) where T : QuestSO
    {
        return currentQuestList.quests
        .Where(item => item.difficulty == difficulty)
            .OfType<T>() // Filter by specific type (Armor, Weapon, etc.)
            .ToList();
    }

    private QuestsListSO GetCurrentQuestListForPlayerClass(ClassType playerClass)
    {
        return availableQuests[(int)playerClass];
    }

    private void SetSlots<T>(List<T> items) where T : QuestSO
    {
        for (int i = 0; i < items.Count; i++)
        {
            QuestSO questSO = items[i]; // This works because T : QuestSO

            if (poolSlots.Count <= 0)
            {
                QuestSlot newSlot = Instantiate(questSlotPrefab, activeSlotsParent).GetComponent<QuestSlot>();
                activeSlots.Add(newSlot);
                newSlot.gameObject.SetActive(true);
                newSlot.Initialize(this, questSO, i);
            }

            else
            {
                poolSlots[0].transform.SetParent(activeSlotsParent);
                poolSlots[0].Initialize(this, questSO, i);
                poolSlots[0].gameObject.SetActive(true);
                activeSlots.Add(poolSlots[0]);
                poolSlots.RemoveAt(0);
            }
        }
    }
}