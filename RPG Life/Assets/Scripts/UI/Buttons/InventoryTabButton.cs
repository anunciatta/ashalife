using UnityEngine;
using UnityEngine.UI;

public class InventoryTabButton : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private GameObject focusedIndicator;
    [Header("[0] Defautlt, [1] Selected")]
    [SerializeField] private Color[] colors;
    private Button button;
    private InventoryPanel inventoryPanel;
    private ShopPanel shopPanel;
    private bool isSelected = false;
    [SerializeField] private ItemType itemType;

    public void Initialize(InventoryPanel inventoryPanel, bool isSelected)
    {
        shopPanel = null;

        if (this.inventoryPanel == null)
        {
            this.inventoryPanel = inventoryPanel;
        }

        if (button == null)
        {
            button = GetComponent<Button>();
            if (button != null)
                button.onClick.AddListener(OnButtonPressed);
        }

        this.isSelected = isSelected;

        UpdateVisuals(isSelected);

        focusedIndicator.GetComponent<Image>().color = new Color(0, 0, 0, 0);
    }

    public void Initialize(ShopPanel shopPanel, bool isSelected)
    {
        inventoryPanel = null;

        if (this.inventoryPanel == null)
        {
            this.shopPanel = shopPanel;
        }

        if (button == null)
        {
            button = GetComponent<Button>();
            if (button != null)
                button.onClick.AddListener(OnButtonPressed);
        }

        this.isSelected = isSelected;

        UpdateVisuals(isSelected);

        focusedIndicator.GetComponent<Image>().color = new Color(0, 0, 0, 0);
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

        if (inventoryPanel != null)
        {
            foreach (var btn in inventoryPanel.tabButtons)
            {
                if (btn != this)
                    btn.OnDeselect();
            }
        }

        else
        {
            foreach (var btn in shopPanel.tabButtons)
            {
                if (btn != this)
                    btn.OnDeselect();
            }
        }

        OnSelect();
    }

    public void OnSelect()
    {
        isSelected = true;
        UpdateVisuals(isSelected);

        if (inventoryPanel != null)
            inventoryPanel.OpenTab(itemType);

        else
            shopPanel.OpenTab(itemType);
    }

    public void OnDeselect()
    {
        isSelected = false;
        UpdateVisuals(isSelected);
    }

    private void UpdateVisuals(bool isSelected)
    {
        if (icon != null)
        {
            icon.color = isSelected ? colors[1] : colors[0];
        }

        if (focusedIndicator != null)
        {
            focusedIndicator.SetActive(isSelected);
        }
    }
}
