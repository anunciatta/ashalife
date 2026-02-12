using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestTabButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    [Header("[0] Defautlt, [1] Selected")]
    [SerializeField] private Color[] colors;
    private Button button;
    private QuestsPanel questsPanel;
    private bool isSelected = false;
    [SerializeField] private Difficulty difficulty;

    public void Initialize(QuestsPanel questsPanel, bool isSelected)
    {
        if (this.questsPanel == null)
        {
            this.questsPanel = questsPanel;
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
            button.onClick.RemoveAllListeners();
    }

    private void OnButtonPressed()
    {
        if (isSelected)
            return;


        foreach (var btn in questsPanel.tabButtons)
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

        questsPanel.OpenTab(difficulty);
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
