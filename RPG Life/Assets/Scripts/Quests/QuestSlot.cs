using Mono.Cecil.Cil;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestSlot : MonoBehaviour
{
    public QuestSO questSO;
    public float currentChance;

    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI success;
    [SerializeField] private TextMeshProUGUI failure;
    [SerializeField] private TextMeshProUGUI cost;
    [SerializeField] private TextMeshProUGUI chance;
    [SerializeField] private TextMeshProUGUI test;

    [SerializeField] private Image icon;
    [SerializeField] private Image difficultyBanner;
    [SerializeField] private Image background;
    [SerializeField] private GameObject selectionBackground;

    [SerializeField] private Sprite[] difficultySprites;
    [SerializeField] private Color[] colors;
    private QuestsPanel questsPanel;

    private bool isSelected;

    private string successDescription;
    private string failureDescription;
    private string testDescription;


    public void Initialize(QuestsPanel questsPanel, QuestSO questSO, int index)
    {
        if (this.questsPanel == null)
            this.questsPanel = questsPanel;

        this.questSO = questSO;

        UpdateTexts();

        if (background != null)
        {
            background.color = index % 2 == 0 ? colors[1] : colors[0];
        }

        difficultyBanner.sprite = difficultySprites[(int)questSO.difficulty];
        icon.sprite = questSO.icon;

        isSelected = false;
        selectionBackground.SetActive(false);
    }

    public void Clear()
    {
        title.text = string.Empty;
        description.text = string.Empty;
        success.text = string.Empty;
        failure.text = string.Empty;
        cost.text = string.Empty;
        chance.text = string.Empty;
        test.text = string.Empty;

        isSelected = false;
        selectionBackground.SetActive(false);
    }

    public void UpdateTexts()
    {
        title.text = References.Instance.localizationService.GetLocalizationText(questSO.nameLabel);
        description.text = References.Instance.localizationService.GetLocalizationText(questSO.descriptionLabel);

        successDescription = $"{References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.success}")}:<color=#92A4B4>";

        foreach (var outcome in questSO.successStatusOutcomes)
        {
            if (outcome.value > 0)
            {
                successDescription += $" + {outcome.value} {References.Instance.localizationService.GetLocalizationText($"{outcome.status}")}";
            }

            else
            {
                successDescription += $" {outcome.value} {References.Instance.localizationService.GetLocalizationText($"{outcome.status}")}";
            }
        }

        foreach (var outcome in questSO.successItemOutcomes)
        {
            if (outcome.value > 0)
            {
                successDescription += $" + {outcome.value} {References.Instance.localizationService.GetLocalizationText($"{outcome.itemSO.nameLabel}")}";
            }

            else
            {
                successDescription += $" {outcome.value} {References.Instance.localizationService.GetLocalizationText($"{outcome.itemSO.nameLabel}")}";
            }
        }

        successDescription += "</color>";
        success.text = successDescription;

        failureDescription = $"{References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.failure}")}:<color=#92A4B4>";

        foreach (var outcome in questSO.failureStatusOutcomes)
        {
            if (outcome.value > 0)
            {
                failureDescription += $" + {outcome.value} {References.Instance.localizationService.GetLocalizationText($"{outcome.status}")}";
            }

            else
            {
                failureDescription += $" {outcome.value} {References.Instance.localizationService.GetLocalizationText($"{outcome.status}")}";
            }
        }

        foreach (var outcome in questSO.failureItemOutcomes)
        {
            if (outcome.value > 0)
            {
                failureDescription += $" + {outcome.value} {References.Instance.localizationService.GetLocalizationText($"{outcome.itemSO.nameLabel}")}";
            }

            else
            {
                failureDescription += $" {outcome.value} {References.Instance.localizationService.GetLocalizationText($"{outcome.itemSO.nameLabel}")}";
            }
        }

        failureDescription += "</color>";
        failure.text = failureDescription;

        testDescription = $"{References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.test}")}:<color=#92A4B4>";

        float totalStatus = 0.0f;
        int[] totalModifiers = References.Instance.inventory.GetAllEquippedItensModifiers(References.Instance.player.currentCharacter);


        foreach (Status status in questSO.testableStatus)
        {
            totalStatus += References.Instance.player.currentCharacter.statuses[(int)status].currentValue + totalModifiers[(int)status];
            testDescription += $" + {References.Instance.localizationService.GetLocalizationText($"{status}")}";
        }

        Debug.Log("totalStatus===>" + totalStatus);

        currentChance = totalStatus / (totalStatus + References.Instance.experienceConfigurations.questsDifficultyTier[(int)questSO.difficulty]);

        testDescription += $"</color>";
        test.text = testDescription;

        chance.text = $"{References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.chance}")}:<color=#92A4B4> {(currentChance * 100):F2}%";
    }

    public void OnButtonPressed()
    {
        if (questSO == null) return;

        if (isSelected)
        {
            return;
        }

        foreach (var btn in questsPanel.activeSlots)
        {
            if (btn != this)
                btn.Deselect();
        }

        Select();
    }

    public void Select()
    {
        isSelected = true;
        selectionBackground.SetActive(true);
        questsPanel.SelectQuest(this);
    }

    public void Deselect()
    {
        isSelected = false;
        selectionBackground.SetActive(false);
    }

    public void UpdateLanguage(Language newLanguage)
    {
        UpdateTexts();
    }
}
