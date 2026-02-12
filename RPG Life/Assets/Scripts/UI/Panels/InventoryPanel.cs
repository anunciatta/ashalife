using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryPanel : Panel
{
    public InventoryTabButton[] tabButtons;

    [SerializeField] private Transform activeSlotsParent;
    [SerializeField] private Transform poolSlotsParent;

    public List<InventorySlot> activeSlots = new();
    public List<InventorySlot> poolSlots = new();
    [SerializeField] private GameObject inventorySlotPrefab;

    public InventorySlot selectedSlot;
    public EquipmentSlot[] equipmentSlots;

    public StatusInformation[] statusInformations;

    public override void Open()
    {
        base.Open();

        foreach (var btn in tabButtons)
        {
            btn.OnDeselect();
        }

        tabButtons[0].OnSelect();

        UpdateModifiers();
        UpdateEquipment();
    }

    public void UpdateModifiers()
    {
        var modifiers = References.Instance.inventory.GetAllEquippedItensModifiers(References.Instance.player.currentCharacter);

        for (int i = 0; i < statusInformations.LongLength; i++)
        {
            statusInformations[i].Clear();
            statusInformations[i].UpdateValues(References.Instance.player.currentCharacter.statuses[i].currentValue, modifiers[i], References.Instance.player.currentCharacter.statuses[i].maxValue);
        }
    }

    public void UpdateEquipment()
    {
        foreach (var slot in equipmentSlots)
        {
            slot.UpdateEquipment(References.Instance.player.currentCharacter.equippedItems);
        }
    }

    public override void Close()
    {
        base.Close();
    }

    void OnDestroy()
    {
        Bus<OnLanguageChanged>.OnEvent -= OnLanguageChanged;
    }

    public override void Initialize()
    {
        base.Initialize();

        tabButtons = panel.GetComponentsInChildren<InventoryTabButton>(true);
        statusInformations = panel.GetComponentsInChildren<StatusInformation>(true);
        equipmentSlots = panel.GetComponentsInChildren<EquipmentSlot>(true);

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

    private void SetSlots<T>(List<T> items) where T : ItemSO
    {
        for (int i = 0; i < items.Count; i++)
        {
            ItemSO itemSO = items[i]; // This works because T : ItemSO

            if (poolSlots.Count <= 0)
            {
                InventorySlot newSlot = Instantiate(inventorySlotPrefab, activeSlotsParent).GetComponent<InventorySlot>();
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

    private List<T> GetItemsForType<T>() where T : ItemSO
    {
        return References.Instance.inventory.allItems.allItems
            .Where(item => References.Instance.player.currentCharacter.inventory.ContainsKey(item.saveableEntityId))
            .OfType<T>() // Filter by specific type (Armor, Weapon, etc.)
            .ToList();
    }

    private List<T> GetItemsForType<T>(ItemType itemType) where T : ItemSO
    {
        return References.Instance.inventory.allItems.allItems
            .Where(item => References.Instance.player.currentCharacter.inventory.ContainsKey(item.saveableEntityId) && item.itemType == itemType)
            .OfType<T>() // Filter by specific type (Armor, Weapon, etc.)
            .ToList();
    }

    public void OpenTab(ItemType itemType)
    {
        ClearSlots();

        shiledList.Clear();
        weaponList.Clear();
        armorList.Clear();
        potionList.Clear();
        wingsList.Clear();
        jewelryList.Clear();
        petsList.Clear();
        helmetList.Clear();

        switch (itemType)
        {
            case ItemType.All:

                List<ItemSO> allItems = new List<ItemSO>();
                weaponList = GetItemsForType<WeaponSO>();
                shiledList = GetItemsForType<ShieldSO>();
                armorList = GetItemsForType<ArmorSO>();
                potionList = GetItemsForType<PotionSO>();
                jewelryList = GetItemsForType<JewelrySO>();
                wingsList = GetItemsForType<WingSO>();
                petsList = GetItemsForType<PetSO>();
                helmetList = GetItemsForType<HelmetSO>();

                allItems.AddRange(weaponList);
                allItems.AddRange(shiledList);
                allItems.AddRange(armorList);
                allItems.AddRange(helmetList);
                allItems.AddRange(potionList);
                allItems.AddRange(wingsList);
                allItems.AddRange(jewelryList);
                allItems.AddRange(petsList);

                SetSlots(allItems);
                break;

            case ItemType.Weapon:

                if (References.Instance.player.currentCharacter.avatarConfig.classesIndex == (int)ClassType.Archer)
                {
                    weaponList = GetItemsForType<WeaponSO>(ItemType.Bow);
                }

                else if (References.Instance.player.currentCharacter.avatarConfig.classesIndex == (int)ClassType.Warrior)
                {
                    weaponList = GetItemsForType<WeaponSO>();
                }
                else
                {
                    weaponList = GetItemsForType<WeaponSO>(ItemType.Staff);
                }

                SetSlots(weaponList);
                break;

            case ItemType.Shield:

                if (References.Instance.player.currentCharacter.avatarConfig.classesIndex == (int)ClassType.Archer)
                {
                    weaponList = GetItemsForType<WeaponSO>(ItemType.Dagger);
                    SetSlots(weaponList);
                }

                else if (References.Instance.player.currentCharacter.avatarConfig.classesIndex == (int)ClassType.Warrior)
                {
                    shiledList = GetItemsForType<ShieldSO>();
                    SetSlots(shiledList);
                }
                else
                {
                    weaponList = GetItemsForType<WeaponSO>(ItemType.LightSword);
                    SetSlots(weaponList);
                }
                break;

            case ItemType.Helmet:
                helmetList = GetItemsForType<HelmetSO>();
                SetSlots(helmetList);
                break;

            case ItemType.Armor:
                armorList = GetItemsForType<ArmorSO>();
                SetSlots(armorList);
                break;

            case ItemType.Potion:
                potionList = GetItemsForType<PotionSO>();
                SetSlots(potionList);
                break;

            case ItemType.Wings:
                wingsList = GetItemsForType<WingSO>();
                SetSlots(wingsList);
                break;

            case ItemType.Jewelry:
                jewelryList = GetItemsForType<JewelrySO>();
                SetSlots(jewelryList);
                break;

            case ItemType.Pet:
                petsList = GetItemsForType<PetSO>();
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

    private List<ShieldSO> shiledList = new();
    private List<WeaponSO> weaponList = new();
    private List<ArmorSO> armorList = new();
    private List<PotionSO> potionList = new();
    private List<WingSO> wingsList = new();
    private List<JewelrySO> jewelryList = new();
    private List<PetSO> petsList = new();
    private List<HelmetSO> helmetList = new();

    //private int healthValue, attackValue, defenseValue, magicValue, criticalValue, agilityValue;

    Dictionary<Status, int> attributes = new();

    public void SelectItem(InventorySlot slot)
    {
        if (slot == null)
        {
            for (int i = 0; i < statusInformations.LongLength; i++)
            {
                statusInformations[i].CheckModifierValues(0, 0);
            }
            return;
        }
        ;
        if (slot.itemSO == null) return;

        selectedSlot = slot;

        attributes[Status.Health] = slot.itemSO.GetAttributeValue(Status.Health);
        attributes[Status.Attack] = slot.itemSO.GetAttributeValue(Status.Attack);
        attributes[Status.Magic] = slot.itemSO.GetAttributeValue(Status.Magic);
        attributes[Status.Agility] = slot.itemSO.GetAttributeValue(Status.Agility);
        attributes[Status.Defense] = slot.itemSO.GetAttributeValue(Status.Defense);
        attributes[Status.Critical] = slot.itemSO.GetAttributeValue(Status.Critical);

        int[] modifiers;

        switch (slot.itemSO)
        {
            case WeaponSO:
                modifiers = References.Instance.inventory.GetEquippedWeaponModifiers(slot.itemSO.itemType == ItemType.Weapon || slot.itemSO.itemType == ItemType.Staff || slot.itemSO.itemType == ItemType.Bow);
                break;

            case ArmorSO:
                modifiers = References.Instance.inventory.GetEquippedArmorModifiers();
                break;

            case HelmetSO:
                modifiers = References.Instance.inventory.GetEquippedHelmetModifiers();
                break;

            case ShieldSO:
                modifiers = References.Instance.inventory.GetEquippedShieldModifiers();
                break;

            case PetSO:
                modifiers = References.Instance.inventory.GetEquippedJewelryModifiers();
                break;

            case WingSO:
                modifiers = References.Instance.inventory.GetEquippedWingsModifiers();
                break;
            default:
                Debug.Log("Unknown item type");
                modifiers = References.Instance.inventory.GetEquippedWeaponModifiers(true);
                break;
        }

        for (int i = 0; i < statusInformations.LongLength; i++)
        {
            statusInformations[i].CheckModifierValues(modifiers[i], attributes[(Status)i]);
        }



    }

    public InventorySlot GetSlotByItem(ItemSO currentEquippedItem)
    {
        if (currentEquippedItem == null || activeSlots == null)
            return null;

        return activeSlots.FirstOrDefault(slot => slot.itemSO == currentEquippedItem);
    }
}


