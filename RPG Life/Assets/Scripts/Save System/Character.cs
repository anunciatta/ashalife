using System.Collections.Generic;
using System;
using UnityEngine;


[Serializable]
public class Character
{
    public CharacterStatus[] statuses = new CharacterStatus[7];
    public AvatarConfig avatarConfig;
    public int index;
    public string characterId;
    public Dictionary<string, int> inventory = new();
    public EquippedItens equippedItems;

    public int level;

    public Character(AvatarConfig avatarConfig, int index)
    {
        //Authentication
        this.index = index;

        //Class
        var classSO = References.Instance.characterCustomizerConfigurations.classes[avatarConfig.classesIndex];

        //Avatar Config
        this.avatarConfig = avatarConfig;

        //Status

        level = 1;
        //experienceForNextLevel = References.Instance.experienceConfigurations.GetXpForNextLevel(level);

        var initialClassConfig = classSO.GetInitialAttributesValues();
        statuses = new CharacterStatus[Enum.GetNames(typeof(Status)).Length];

        for (int i = 0; i < statuses.Length; i++)
        {
            if (statuses[i] == null)
                statuses[i] = new CharacterStatus();

            statuses[i].status = (Status)i;

            if (i == (int)Status.Experience)
                statuses[i].SetMaxValue(References.Instance.experienceConfigurations.GetXpForNextLevel(level));
            else if (i == (int)Status.Energy)
                statuses[i].SetMaxValue(References.Instance.experienceConfigurations.GetEnergyForNextLevel(level));
            else
                statuses[i].SetMaxValue(initialClassConfig[i]);
        }

        for (int i = 0; i < statuses.Length; i++)
        {
            statuses[i].OnValueChange(initialClassConfig[i]);
        }

        EquipItem(classSO.initialConfig.armorSO);
        //EquipItem(classSO.initialConfig.weaponSO);

        //Inventory
        inventory = new Dictionary<string, int>
        {
            { References.Instance.inventory.coin.saveableEntityId, 1000 },
            { References.Instance.inventory.gem.saveableEntityId, 100 },
            { classSO.initialConfig.armorSO.saveableEntityId, 1 }
        };

        classSO.initialConfig.armorSO.isEquipped = true;
        classSO.initialConfig.armorSO.deletable = false;
    }

    public void EquipItem(ItemSO equipment)
    {
        switch (equipment)
        {
            case ArmorSO:
                equippedItems.armor = equipment.saveableEntityId;
                break;
            case WeaponSO:

                if (equipment.itemType == ItemType.LightSword || equipment.itemType == ItemType.Dagger)
                {
                    equippedItems.shield = equipment.saveableEntityId;
                }

                else
                {
                    equippedItems.weapon = equipment.saveableEntityId;
                }

                break;
            case HelmetSO:
                equippedItems.helmet = equipment.saveableEntityId;
                break;
            case ShieldSO:
                equippedItems.shield = equipment.saveableEntityId;
                break;
            case JewelrySO:
                equippedItems.jewelry = equipment.saveableEntityId;
                break;
            case WingSO:
                equippedItems.wings = equipment.saveableEntityId;
                break;
            default:
                break;
        }
    }

    public bool HasEnoughCoins(int value) => inventory.TryGetValue($"{References.Instance.inventory.coin.saveableEntityId}", out int currentCoins) && currentCoins >= value;
    public bool HasEnoughGems(int value) => inventory.TryGetValue($"{References.Instance.inventory.gem.saveableEntityId}", out int currentGems) && currentGems >= value;

    public void RemoveItemFromInventory(ItemSO item, int amount)
    {
        if (inventory.TryGetValue(item.saveableEntityId, out var existingItem))
        {
            if (existingItem - amount <= 0)
            {
                inventory.Remove(item.saveableEntityId);
            }
            else
            {
                inventory[item.saveableEntityId] = existingItem - amount;
            }
        }
        else
        {
            // Item not found in inventory
        }
    }

    public void AddItemToInventory(ItemSO item, int amount)
    {

        if (string.IsNullOrEmpty(item.saveableEntityId))
        {
            Debug.LogError("Item has no saveableEntityId");
            return;
        }

        if (inventory == null)
        {
            inventory = new Dictionary<string, int>();
        }

        if (inventory.TryGetValue(item.saveableEntityId, out var existingItem))
        {
            inventory[item.saveableEntityId] = existingItem + amount;
        }
        else
        {
            inventory.Add(item.saveableEntityId, amount);
        }
    }
}
