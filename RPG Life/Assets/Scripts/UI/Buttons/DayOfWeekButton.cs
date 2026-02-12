using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DayOfWeekButton : MonoBehaviour
{
    private TextMeshProUGUI label;
    [SerializeField] private GameObject focusedIndicator;
    [Header("[0] Defautlt, [1] Selected")]
    [SerializeField] private Color[] colors;
    private Button button;
    private ITaskEditable taskEditablePanel;
    private bool isSelected = false;
    private DayOfWeek dayOfWeek;

    public void Initialize(ITaskEditable taskEditablePanel, bool isSelected, DayOfWeek dayOfWeek)
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
        this.dayOfWeek = dayOfWeek;

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
        {
            OnDeselect();
        }

        else
        {
            OnSelect();
        }
    }

    public void OnSelect()
    {
        isSelected = true;
        UpdateVisuals(isSelected);
        taskEditablePanel.AddDayOfTheWeek(dayOfWeek);

    }

    public void OnDeselect()
    {
        isSelected = false;
        UpdateVisuals(isSelected);
        taskEditablePanel.RemoveDayOfTheWeek(dayOfWeek);
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
}

