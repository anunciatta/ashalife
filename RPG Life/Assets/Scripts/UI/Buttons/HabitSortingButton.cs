using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HabitSortingButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    [Header("[0] Defautlt, [1] Selected")]
    [SerializeField] private Color[] colors;
    //[SerializeField] private Repetition sortingType;
    [SerializeField] private SortingType sortingType;
    private Button button;
    private bool isSelected = false;
    HabitsPanel habitsPanel;

    public void Initialize(HabitsPanel habitsPanel, bool isSelected)
    {
        if (this.habitsPanel == null)
        {
            this.habitsPanel = habitsPanel;
        }

        if (button == null)
        {
            button = GetComponent<Button>();
            if (button != null)
                button.onClick.AddListener(OnButtonPressed);
        }

        this.isSelected = isSelected;
        UpdateVisuals(isSelected);
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

        foreach (var btn in habitsPanel.sortingButtons)
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
        habitsPanel.currentSortingType = sortingType;
        habitsPanel.ShowHabitsForToday();
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
    }
}
