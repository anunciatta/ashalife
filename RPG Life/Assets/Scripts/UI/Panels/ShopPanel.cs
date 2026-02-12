using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class ShopPanel : Panel
{
    public InventoryTabButton[] tabButtons;

    [SerializeField] private Button buyButton;

    [SerializeField] private List<ItemsListSO> availableItems = new();
    private ShopSlot selectedSlot;

    [SerializeField] private GameObject shopSlotPrefab;
    [SerializeField] private Transform activeSlotsParent;
    [SerializeField] private Transform poolSlotsParent;

    [HideInInspector] public List<ShopSlot> activeSlots = new();
    private List<ShopSlot> poolSlots = new();

    public override void Open()
    {
        base.Open();

        foreach (var btn in tabButtons)
        {
            btn.OnDeselect();
        }

        tabButtons[0].OnSelect();
        buyButton.interactable = true;
    }

    public override void Close()
    {
        base.Close();
    }

    public override void Initialize()
    {
        base.Initialize();

        tabButtons = panel.GetComponentsInChildren<InventoryTabButton>(true);
        buyButton.onClick.AddListener(OnBuyButton);

        for (int i = 0; i < tabButtons.Length; i++)
        {
            tabButtons[i].Initialize(this, false);
        }

        Bus<OnLanguageChanged>.OnEvent += OnLanguageChanged;
    }

    private void OnLanguageChanged(OnLanguageChanged data)
    {
        foreach (var slot in activeSlots)
        {
            slot.UpdateLanguage(data.NewLanguage);
        }
    }

    private async void OnBuyButton()
    {
        if (selectedSlot == null) return;


        if (!References.Instance.player.currentCharacter.HasEnoughCoins(selectedSlot.itemSO.cost.coinCost) ||
            !References.Instance.player.currentCharacter.HasEnoughGems(selectedSlot.itemSO.cost.gemCost))
        {
            Bus<OnHasNotEnoughCurrencyEvent>.CallEvent(new OnHasNotEnoughCurrencyEvent(selectedSlot.itemSO));
            return;
        }

        buyButton.interactable = false;

        string coinId = References.Instance.inventory.coin.saveableEntityId;
        string gemId = References.Instance.inventory.gem.saveableEntityId;

        References.Instance.player.currentCharacter.inventory[coinId] -= selectedSlot.itemSO.cost.coinCost;
        References.Instance.player.currentCharacter.inventory[gemId] -= selectedSlot.itemSO.cost.gemCost;

        References.Instance.player.currentCharacter.AddItemToInventory(selectedSlot.itemSO, 1);

        bool success = await FirebaseSaveManager.UpdateCharacterField(References.Instance.player.currentCharacter.characterId, "inventory", new Dictionary<string, int>(References.Instance.player.currentCharacter.inventory));

        if (success)
        {
            Bus<ChangeCoinsEvent>.CallEvent(new ChangeCoinsEvent(References.Instance.player.currentCharacter.inventory[coinId]));
            Bus<ChangeGemsEvent>.CallEvent(new ChangeGemsEvent(References.Instance.player.currentCharacter.inventory[gemId]));

            //update UI

            selectedSlot.OnDeselect();
            selectedSlot.OnPurchased();
            selectedSlot = null;

            buyButton.interactable = true;
        }

        else
        {
            //Revert changes
            References.Instance.player.currentCharacter.inventory[coinId] += selectedSlot.itemSO.cost.coinCost;
            References.Instance.player.currentCharacter.inventory[gemId] += selectedSlot.itemSO.cost.gemCost;
            References.Instance.player.currentCharacter.RemoveItemFromInventory(selectedSlot.itemSO, 1);

            //something went wrong
            Bus<OnAddToast>.CallEvent(new OnAddToast(new PopupConfig(MarkStatus.Neutral, References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.somethingWrong}"), PopupType.System)));
            buyButton.interactable = true;
        }
    }

    void OnDestroy()
    {
        buyButton.onClick.RemoveAllListeners();
        Bus<OnLanguageChanged>.OnEvent -= OnLanguageChanged;
    }


    private List<T> GetItemsForPlayerClass<T>() where T : ItemSO
    {
        return currentItemList.items
            .OfType<T>() // Filter by specific type (Armor, Weapon, etc.)
            .ToList();
    }


    private List<T> GetItemsForPlayerClass<T>(ItemType itemType) where T : ItemSO
    {
        return currentItemList.items
        .Where(item => item.itemType == itemType)
            .OfType<T>() // Filter by specific type (Armor, Weapon, etc.)
            .ToList();
    }


    private ItemsListSO GetCurrentItemListForPlayerClass(ClassType playerClass)
    {
        return availableItems[(int)playerClass];
    }

    private void SetSlots<T>(List<T> items) where T : ItemSO
    {
        for (int i = 0; i < items.Count; i++)
        {
            ItemSO itemSO = items[i]; // This works because T : ItemSO

            if (poolSlots.Count <= 0)
            {
                ShopSlot newSlot = Instantiate(shopSlotPrefab, activeSlotsParent).GetComponent<ShopSlot>();
                activeSlots.Add(newSlot);
                newSlot.gameObject.SetActive(true);
                newSlot.Initialize(this, false, itemSO, i);
            }

            else
            {
                poolSlots[0].transform.SetParent(activeSlotsParent);
                poolSlots[0].Initialize(this, false, itemSO, i);
                poolSlots[0].gameObject.SetActive(true);
                activeSlots.Add(poolSlots[0]);
                poolSlots.RemoveAt(0);
            }
        }
    }

    private List<ShieldSO> shiledList = new();
    private List<WeaponSO> weaponList = new();
    private List<ArmorSO> armorList = new();
    private List<HelmetSO> helmetList = new();
    private List<PotionSO> potionList = new();
    private List<WingSO> wingsList = new();
    private List<JewelrySO> jewelryList = new();
    private List<PetSO> petsList = new();

    ItemsListSO currentItemList;

    public void OpenTab(ItemType itemType)
    {
        var classType = References.Instance.characterCustomizerConfigurations.classes[References.Instance.player.currentCharacter.avatarConfig.classesIndex].classType;

        currentItemList = GetCurrentItemListForPlayerClass(classType);

        ClearSlots();

        selectedSlot = null;

        shiledList.Clear();
        weaponList.Clear();
        armorList.Clear();
        potionList.Clear();
        wingsList.Clear();
        jewelryList.Clear();
        helmetList.Clear();
        petsList.Clear();


        switch (itemType)
        {
            case ItemType.All:
                List<ItemSO> allItems = new List<ItemSO>();
                allItems.AddRange(currentItemList.items);
                SetSlots(allItems);
                break;

            case ItemType.Weapon:
                if (References.Instance.player.currentCharacter.avatarConfig.classesIndex == (int)ClassType.Archer)
                {
                    weaponList = GetItemsForPlayerClass<WeaponSO>(ItemType.Bow);
                }

                else if (References.Instance.player.currentCharacter.avatarConfig.classesIndex == (int)ClassType.Warrior)
                {
                    weaponList = GetItemsForPlayerClass<WeaponSO>();
                }
                else
                {
                    weaponList = GetItemsForPlayerClass<WeaponSO>(ItemType.Staff);
                }
                SetSlots(weaponList);
                break;

            case ItemType.Shield:
                if (References.Instance.player.currentCharacter.avatarConfig.classesIndex == (int)ClassType.Archer)
                {
                    weaponList = GetItemsForPlayerClass<WeaponSO>(ItemType.Dagger);
                    SetSlots(weaponList);
                }

                else if (References.Instance.player.currentCharacter.avatarConfig.classesIndex == (int)ClassType.Warrior)
                {
                    shiledList = GetItemsForPlayerClass<ShieldSO>();
                    SetSlots(shiledList);
                }
                else
                {
                    weaponList = GetItemsForPlayerClass<WeaponSO>(ItemType.LightSword);
                    SetSlots(weaponList);
                }


                break;

            case ItemType.Helmet:
                helmetList = GetItemsForPlayerClass<HelmetSO>();
                SetSlots(helmetList);
                break;

            case ItemType.Armor:

                armorList = GetItemsForPlayerClass<ArmorSO>();
                SetSlots(armorList);
                break;

            case ItemType.Potion:
                potionList = GetItemsForPlayerClass<PotionSO>();
                SetSlots(potionList);
                break;

            case ItemType.Wings:
                wingsList = GetItemsForPlayerClass<WingSO>();
                SetSlots(wingsList);
                break;

            case ItemType.Jewelry:
                jewelryList = GetItemsForPlayerClass<JewelrySO>();
                SetSlots(jewelryList);
                break;

            case ItemType.Pet:
                petsList = GetItemsForPlayerClass<PetSO>();
                SetSlots(petsList);
                break;

            default: break;
        }
    }

    private void ClearSlots()
    {
        foreach (var slot in activeSlots)
        {
            slot.Clear();
            slot.transform.SetParent(poolSlotsParent);
            slot.gameObject.SetActive(false);

            if (!poolSlots.Contains(slot))
                poolSlots.Add(slot);
        }

        activeSlots.Clear();
    }

    public void SelectItem(ShopSlot slot)
    {
        if (slot == null) return;
        if (slot.itemSO == null) return;

        selectedSlot = slot;
    }
}
