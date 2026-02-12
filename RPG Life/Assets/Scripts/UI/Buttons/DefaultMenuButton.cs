using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DefaultMenuButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private Image icon;
    [Header("[0] Defautlt, [1] Selected")]
    [SerializeField] private Color[] colors;
    private Button button;
    private DefaultPanel defaultPanel;
    private bool isSelected = false;
    private Panel relatedPanel;
    [SerializeField] private MenuTab tab;



    public void Initialize(DefaultPanel defaultPanel, bool isSelected, Panel relatedPanel)
    {
        if (this.defaultPanel == null)
        {
            this.defaultPanel = defaultPanel;
        }

        if (relatedPanel == null)
        {
            this.relatedPanel = null;
        }

        else
        {
            if (this.relatedPanel == null)
            {
                this.relatedPanel = relatedPanel;
            }
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
        {
            return;
        }

        foreach (var btn in defaultPanel.menuButtons)
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

        if (relatedPanel != null)
            relatedPanel.Open();

        defaultPanel.currentTab = tab;


    }

    public void OnDeselect()
    {
        isSelected = false;

        if (relatedPanel != null)
            relatedPanel.Close();

        UpdateVisuals(isSelected);
    }

    private void UpdateVisuals(bool isSelected)
    {
        if (label != null)
            label.color = isSelected ? colors[1] : colors[0];

        if (icon != null)
            icon.color = isSelected ? colors[1] : colors[0];
    }
}
