using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyButton : MonoBehaviour
{
    private TextMeshProUGUI label;
    [SerializeField] private GameObject focusedIndicator;
    [Header("[0] Defautlt, [1] Selected")]
    [SerializeField] private Color[] colors;
    private Button button;
    private ITaskEditable taskEditablePanel;
    private bool isSelected = false;
    private Difficulty difficulty;
    [SerializeField] private TextMeshProUGUI flavor;
    [SerializeField] private string flavorLabel;

    public void Initialize(ITaskEditable taskEditablePanel, bool isSelected, Difficulty difficulty)
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
        this.difficulty = difficulty;

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

        foreach (var btn in taskEditablePanel.GetDifficultyButtons())
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

        taskEditablePanel.SetDifficulty(difficulty);

        if (flavor)
        {
            flavor.text = References.Instance.localizationService.GetLocalizationText(flavorLabel);
        }

    }

    public void OnDeselect()
    {
        isSelected = false;
        UpdateVisuals(isSelected);
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
        if (flavor && isSelected)
        {
            flavor.text = References.Instance.localizationService.GetLocalizationText(flavorLabel);
        }
    }
}
