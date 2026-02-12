using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryCount : MonoBehaviour
{
    private InventorySlot inventorySlot;
    private TextMeshProUGUI count;
    private Image background;

    public void Initialize(InventorySlot inventorySlot)
    {
        this.inventorySlot = inventorySlot;
        count = GetComponentInChildren<TextMeshProUGUI>(true);
        background = GetComponent<Image>();
        count.text = string.Empty;

        UpdateCounter();
    }

    public void UpdateCounter()
    {
        if (inventorySlot == null) return;

        if (inventorySlot.itemSO == null || inventorySlot.itemSO.GetQuantity() <= 1)
        {
            background.enabled = false;
            count.text = string.Empty;
        }

        else
        {
            background.enabled = true;
            count.text = $"{inventorySlot.itemSO.GetQuantity()}";
        }
    }
}


