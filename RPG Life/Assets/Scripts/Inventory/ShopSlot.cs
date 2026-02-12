using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour
{

    [SerializeField] private Image background;
    [SerializeField] private Image focusedIndicator;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI coinCostText;
    [SerializeField] private TextMeshProUGUI gemCostText;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;

    [SerializeField] private GameObject priceContainer;
    [SerializeField] private GameObject coinPriceContainer;
    [SerializeField] private GameObject gemPriceContainer;
    [SerializeField] private GameObject purchasedContainer;

    [Header("[0] Even, [1] Odd")]
    [SerializeField] private Color[] colors;
    private ShopPanel shopPanel;
    private bool isSelected = false;

    public ItemSO itemSO;

    public void Initialize(ShopPanel shopPanel, bool isSelected, ItemSO itemSO, int index)
    {
        if (this.shopPanel == null)
        {
            this.shopPanel = shopPanel;
        }

        this.isSelected = isSelected;
        this.itemSO = itemSO;

        UpdateVisuals(isSelected);

        if (background != null)
        {
            background.color = index % 2 == 0 ? colors[1] : colors[0];
        }

        //Todo! Fix cost display for multiple currencies

        if (itemSO.cost.coinCost > 0 && itemSO.cost.gemCost > 0)
        {
            coinPriceContainer.SetActive(true);
            gemPriceContainer.SetActive(true);

            coinCostText.text = $"{itemSO.cost.coinCost}";
            gemCostText.text = $"{itemSO.cost.gemCost}";
        }

        else
        {
            if (itemSO.cost.gemCost > 0)
            {
                gemCostText.text = $"{itemSO.cost.gemCost}";
                coinPriceContainer.SetActive(false);
                gemPriceContainer.SetActive(true);
            }
            else if (itemSO.cost.coinCost > 0)
            {
                coinCostText.text = $"{itemSO.cost.coinCost}";
                gemPriceContainer.SetActive(false);
                coinPriceContainer.SetActive(true);
            }
        }
      
        title.text = References.Instance.localizationService.GetLocalizationText($"{itemSO.nameLabel}");
        description.text = References.Instance.localizationService.GetLocalizationText($"{itemSO.descriptionLabel}") + "<br>" + References.Instance.localizationService.GetLocalizationText($"{itemSO.flavorLabel}");
        itemIcon.sprite = itemSO.inventoryIcon;

        priceContainer.SetActive(!itemSO.purchased);
        purchasedContainer.SetActive(itemSO.purchased);
    }

    public void OnButtonPressed()
    {
        if (itemSO == null)
        {
            return;
        }

        if (isSelected)
        {
            return;
        }

        foreach (var btn in shopPanel.activeSlots)
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
        shopPanel.SelectItem(this);
    }

    public void OnDeselect()
    {
        isSelected = false;
        UpdateVisuals(isSelected);
    }

    private void UpdateVisuals(bool isSelected)
    {
        if (focusedIndicator != null)
        {
            focusedIndicator.enabled = isSelected;
        }
    }

    public void Clear()
    {
        itemSO = null;
        isSelected = false;
        UpdateVisuals(isSelected);

        priceContainer.SetActive(false);
        coinPriceContainer.SetActive(false);
        gemPriceContainer.SetActive(false);
        purchasedContainer.SetActive(false);

        itemIcon.sprite = null;
        coinCostText.text = string.Empty;
        gemCostText.text = string.Empty;
        title.text = string.Empty;
        description.text = string.Empty;
    }

    public void OnPurchased()
    {
        itemSO.purchased = true;
        priceContainer.SetActive(false);
        purchasedContainer.SetActive(true);
    }

    public void UpdateLanguage(Language newLanguage)
    {
        title.text = References.Instance.localizationService.GetLocalizationText($"{itemSO.nameLabel}");
        description.text = References.Instance.localizationService.GetLocalizationText($"{itemSO.descriptionLabel}") + "<br>" + References.Instance.localizationService.GetLocalizationText($"{itemSO.flavorLabel}");
    }
}

