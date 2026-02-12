using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RepetitionButton : MonoBehaviour
{
    [SerializeField] private GameObject[] relatedPanels;
    [SerializeField] private GameObject focusedIndicator;
    [Header("[0] Defautlt, [1] Selected")]
    [SerializeField] private Color[] colors;
    private TextMeshProUGUI label;
    private Button button;
    private ITaskEditable taskEditablePanel;
    private bool isSelected = false;
    private Repetition repetition;

    [SerializeField] private TextMeshProUGUI flavortext;

    public void Initialize(ITaskEditable taskEditablePanel, bool isSelected, Repetition repetition)
    {
        label = GetComponentInChildren<TextMeshProUGUI>(true);

        if (this.taskEditablePanel == null)
        {
            this.taskEditablePanel = taskEditablePanel;
        }

        if (button == null)
        {
            button = GetComponent<Button>();
            if (button != null)
                button.onClick.AddListener(OnButtonPressed);
        }

        this.isSelected = isSelected;
        this.repetition = repetition;

        UpdateVisuals(isSelected);
        focusedIndicator.GetComponent<Image>().color = new Color(0, 0, 0, 0);
    }

    void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(OnButtonPressed);
    }

    private void OnButtonPressed()
    {
        if (isSelected)
            return;

        taskEditablePanel.CloseAllRelatedPanels();

        foreach (var btn in taskEditablePanel.GetRepetitionButtons())
        {
            if (btn != this)
                btn.OnDeselect();
        }

        OnSelect();
    }

    public void OnSelect()
    {
        isSelected = true;
        UpdateVisuals(isSelected);

        taskEditablePanel.SetRepetition(repetition);

        foreach (var panel in relatedPanels)
            panel.SetActive(true);

        if (flavortext)
        {
            flavortext.text = repetition switch
            {
                Repetition.Daily => $"{References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.dailyFlavor}")}",
                Repetition.Weekly => $"{References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.weeklyFlavor}")}",
                Repetition.Monthly => $"{References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.monthlyFlavor}")}",
                Repetition.Yearly => $"{References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.yearlyFlavor}")}",
                Repetition.None => "",
                _ => ""
            };
        }
    }

    public void OnDeselect()
    {
        isSelected = false;
        UpdateVisuals(isSelected);

        if (flavortext)
        {
            flavortext.text = "";
        }
    }

    private void UpdateVisuals(bool isSelected)
    {
        if (label != null)
        {
            label.color = isSelected ? colors[1] : colors[0];
        }

        if (focusedIndicator != null)
        {
            focusedIndicator.SetActive(isSelected);
        }
    }

    internal void UpdateLanguage()
    {
        if (flavortext && isSelected)
        {
            flavortext.text = repetition switch
            {
                Repetition.Daily => $"{References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.dailyFlavor}")}",
                Repetition.Weekly => $"{References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.weeklyFlavor}")}",
                Repetition.Monthly => $"{References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.monthlyFlavor}")}",
                Repetition.Yearly => $"{References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.yearlyFlavor}")}",
                Repetition.None => "",
                _ => ""
            };
        }
    }
}
