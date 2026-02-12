using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum SortingType
{
    All, Done, Pending, Today
}

public class DailySortingButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    [Header("[0] Defautlt, [1] Selected")]
    [SerializeField] private Color[] colors;

    [SerializeField] private SortingType sortingType;

    private Button button;
    private bool isSelected = false;
    DailiesPanel dailiesPanel;

    public void Initialize(DailiesPanel dailiesPanel, bool isSelected)
    {
        if (this.dailiesPanel == null)
        {
            this.dailiesPanel = dailiesPanel;
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

        foreach (var btn in dailiesPanel.sortingButtons)
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
        dailiesPanel.currentSortingType = sortingType;
        dailiesPanel.ShowDailiesForToday();
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
