using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Image background;
    [SerializeField] private Image focusedIndicator;

    //[SerializeField] private GameObject equippedIndicator;
    [SerializeField] private GameObject optionsPanel;

    [Header("[0] Defautlt, [1] Selected")]
    [SerializeField] private Color[] colors;

    public GameObject deleteButton;

    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemDescription;

    private InventoryPanel inventoryPanel;
    private bool isSelected = false;
    [HideInInspector] public ItemSO itemSO;
    private bool isProcessingEquipment = false;
    private InventoryCount inventoryCount;

    public void Initialize(InventoryPanel inventoryPanel, bool isSelected, ItemSO itemSO, int index)
    {
        if (this.inventoryPanel == null)
        {
            this.inventoryPanel = inventoryPanel;
        }

        if (background != null)
        {
            background.color = index % 2 == 0 ? colors[1] : colors[0];
        }

        this.isSelected = isSelected;
        this.itemSO = itemSO;

        icon.sprite = this.itemSO.inventoryIcon;

        itemDescription.text = References.Instance.localizationService.GetLocalizationText($"{this.itemSO.descriptionLabel}") + "<br>" + References.Instance.localizationService.GetLocalizationText($"{this.itemSO.flavorLabel}");


        UpdateVisuals(isSelected);

        inventoryCount = GetComponentInChildren<InventoryCount>(true);

        if (inventoryCount)
            inventoryCount.Initialize(this);

        isProcessingEquipment = false;

        if (itemSO.isEquipped)
        {
            itemName.text = "<color=#719CF0>" + References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.equipped}") + ":</color> " + References.Instance.localizationService.GetLocalizationText($"{this.itemSO.nameLabel}");
        }

        else
        {
            itemName.text = References.Instance.localizationService.GetLocalizationText($"{this.itemSO.nameLabel}");
        }

        deleteButton.SetActive(itemSO.deletable);
    }

    public void OnButtonPressed()
    {
        if (itemSO == null || isProcessingEquipment)
        {
            return;
        }

        if (isSelected)
        {
            OnDeselect();
            inventoryPanel.SelectItem(null);
            return;
        }

        foreach (var slot in inventoryPanel.activeSlots)
        {
            if (slot != this)
                slot.OnDeselect();
        }

        OnSelect();
    }

    public void OnSelect()
    {

        isSelected = true;
        UpdateVisuals(isSelected);
        inventoryPanel.SelectItem(this);

    }

    public void OnDeselect()
    {
        isSelected = false;
        UpdateVisuals(isSelected);
        optionsPanel.SetActive(isSelected);
    }

    private void UpdateVisuals(bool isSelected)
    {
        if (focusedIndicator != null)
        {
            focusedIndicator.gameObject.SetActive(isSelected);
        }
    }

    public void Clear()
    {
        itemSO = null;
        inventoryCount.UpdateCounter();
        isSelected = false;
        UpdateVisuals(isSelected);
        icon.sprite = null;
    }

    public void RemoveEquipment()
    {
        itemSO.isEquipped = false;
        itemName.text = References.Instance.localizationService.GetLocalizationText($"{this.itemSO.nameLabel}");
    }

    public async void OnEquip()
    {
        if (itemSO == null || isProcessingEquipment)
        {
            return;
        }

        if (itemSO.isEquipped)
        {
            OnDeselect();
            inventoryPanel.SelectItem(null);
            return;
        }

        try
        {
            isProcessingEquipment = true;
            ItemSO currentEquippedItem = null;
            InventorySlot currentEquippedSlot = null;
            bool success = false;


            switch (itemSO)
            {
                case ArmorSO:
                    currentEquippedItem = References.Instance.inventory.GetItemFromID(References.Instance.player.currentCharacter.equippedItems.armor);
                    break;
                case WeaponSO:

                    if (itemSO.itemType == ItemType.LightSword || itemSO.itemType == ItemType.Dagger)
                    {
                        currentEquippedItem = References.Instance.inventory.GetItemFromID(References.Instance.player.currentCharacter.equippedItems.shield);
                    }

                    else
                    {
                        currentEquippedItem = References.Instance.inventory.GetItemFromID(References.Instance.player.currentCharacter.equippedItems.weapon);
                    }

                    break;
                case HelmetSO:
                    currentEquippedItem = References.Instance.inventory.GetItemFromID(References.Instance.player.currentCharacter.equippedItems.helmet);
                    break;
                case ShieldSO:
                    currentEquippedItem = References.Instance.inventory.GetItemFromID(References.Instance.player.currentCharacter.equippedItems.shield);
                    break;
                case JewelrySO:
                    currentEquippedItem = References.Instance.inventory.GetItemFromID(References.Instance.player.currentCharacter.equippedItems.jewelry);
                    break;
                case WingSO:
                    currentEquippedItem = References.Instance.inventory.GetItemFromID(References.Instance.player.currentCharacter.equippedItems.wings);
                    break;
                default:
                    currentEquippedItem = null;
                    break;
            }

            currentEquippedSlot = inventoryPanel.GetSlotByItem(currentEquippedItem);

            success = await FirebaseSaveManager.UpdateEquipment(References.Instance.player.currentCharacter, itemSO);

            if (success)
            {
                itemName.text = "<color=#719CF0>" + References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.equipped}") + ":</color> " + References.Instance.localizationService.GetLocalizationText($"{this.itemSO.nameLabel}");
                itemSO.isEquipped = true;
                OnDeselect();
                inventoryPanel.SelectItem(null);

                References.Instance.player.currentCharacter.EquipItem(itemSO);

                if (currentEquippedSlot != null) currentEquippedSlot.RemoveEquipment();

                //Update UI
                inventoryPanel.UpdateEquipment();
                inventoryPanel.UpdateModifiers();


                switch (itemSO)
                {
                    case ArmorSO armor:
                        Bus<ArmorSpriteChangeEvent>.CallEvent(new ArmorSpriteChangeEvent(armor.armorSprites));
                        break;
                    case WeaponSO weapon:

                        if (itemSO.itemType == ItemType.LightSword || itemSO.itemType == ItemType.Dagger)
                        {
                            Bus<SecondWeaponSpriteChangeEvent>.CallEvent(new SecondWeaponSpriteChangeEvent(weapon.weaponSprites));
                        }

                        else
                        {
                            Bus<WeaponSpriteChangeEvent>.CallEvent(new WeaponSpriteChangeEvent(weapon.weaponSprites));
                        }

                        break;
                    case HelmetSO helmet:
                        Bus<HelmetSpriteChangeEvent>.CallEvent(new HelmetSpriteChangeEvent(helmet.helmetSpriteConfig));
                        break;
                    case ShieldSO shield:
                        Bus<ShieldSpriteChangeEvent>.CallEvent(new ShieldSpriteChangeEvent(shield.sprite));
                        break;
                    case JewelrySO jewelry:
                        //Whtat?
                        break;
                    case WingSO wing:
                        Bus<WingSpriteChangeEvent>.CallEvent(new WingSpriteChangeEvent(wing.sprite));
                        break;
                    default:
                        currentEquippedItem = null;
                        break;
                }
            }

            else
            {
                Bus<OnAddToast>.CallEvent(new OnAddToast(new PopupConfig(MarkStatus.Neutral, References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.somethingWrong}"), PopupType.System)));
            }
        }

        catch (Exception e)
        {
            Debug.LogWarning($"Error updating habit: {e.Message}");
            return;
        }

        finally
        {
            isProcessingEquipment = false;
        }
    }

    public void OnOptions()
    {
        if (itemSO == null || isProcessingEquipment)
        {
            return;
        }

        if (isSelected == false)
        {
            foreach (var slot in inventoryPanel.activeSlots)
            {
                if (slot != this)
                    slot.OnDeselect();
            }

            OnSelect();
        }

        foreach (var slot in inventoryPanel.activeSlots)
        {
            if (slot != this)
                slot.OnDeselect();
        }

        optionsPanel.SetActive(!optionsPanel.activeSelf);
    }

    public void OnDelete()
    {
        if (itemSO == null || isProcessingEquipment)
        {
            return;
        }
        //Todo
    }


    public void UpdateLanguage(Language newLanguage)
    {
        if (itemSO.isEquipped)
        {
            itemName.text = "<color=#719CF0>" + References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.equipped}") + ":</color> " + References.Instance.localizationService.GetLocalizationText($"{itemSO.nameLabel}");
        }

        else
        {
            itemName.text = References.Instance.localizationService.GetLocalizationText($"{itemSO.nameLabel}");
        }

        itemDescription.text = References.Instance.localizationService.GetLocalizationText($"{itemSO.descriptionLabel}") + "<br>" + References.Instance.localizationService.GetLocalizationText($"{itemSO.flavorLabel}");
    }
}


