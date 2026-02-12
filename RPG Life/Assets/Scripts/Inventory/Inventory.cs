using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public AllItemsSO allItems;
    public ItemSO coin;
    public ItemSO gem;

    private Dictionary<string, ItemSO> itemsById = new();
    Dictionary<Status, int> modifiers = new();

    void Awake()
    {
        Bus<OnCharacterLoad>.OnEvent += LoadCurrentCharacter;

        foreach (var item in allItems.allItems)
        {
            if (!itemsById.ContainsKey(item.saveableEntityId))
                itemsById.Add(item.saveableEntityId, item);
            else
                Debug.LogWarning($"Duplicate Item ID: {item.saveableEntityId}");
        }
    }

    private void LoadCurrentCharacter(OnCharacterLoad data)
    {
        // 1. Reset all items first
        foreach (ItemSO item in allItems.allItems)
        {
            item.purchased = false;
            item.SetQuantity(0);
            item.isEquipped = false;
        }

        // 2. Update items that exist in Firebase inventory
        foreach (ItemSO item in allItems.allItems)
        {
            if (data.Character.inventory.ContainsKey(item.saveableEntityId))
            {
                item.purchased = true;
                item.SetQuantity(data.Character.inventory[item.saveableEntityId]);
            }
        }

        var armor = References.Instance.inventory.GetItemFromID(data.Character.equippedItems.armor);
        if (armor != null)
            armor.isEquipped = true;

        var weapon = References.Instance.inventory.GetItemFromID(data.Character.equippedItems.weapon);
        if (weapon != null)
            weapon.isEquipped = true;

        var shield = References.Instance.inventory.GetItemFromID(data.Character.equippedItems.shield);
        if (shield != null)
            shield.isEquipped = true;

        var helmet = References.Instance.inventory.GetItemFromID(data.Character.equippedItems.helmet);
        if (helmet != null)
            helmet.isEquipped = true;

        var jewelry = References.Instance.inventory.GetItemFromID(data.Character.equippedItems.jewelry);
        if (jewelry != null)
            jewelry.isEquipped = true;

        var wings = References.Instance.inventory.GetItemFromID(data.Character.equippedItems.wings);
        if (wings != null)
            wings.isEquipped = true;

    }


    void OnDestroy()
    {
        Bus<OnCharacterLoad>.OnEvent -= LoadCurrentCharacter;
    }

    #region Equipment


    public int[] GetEquippedArmorModifiers()
    {
        ResetModifiers();
        GetModifiers(GetItemFromID(References.Instance.player.currentCharacter.equippedItems.armor));

        int[] arrayModifiers = Enum.GetValues(typeof(Status))
    .Cast<Status>()
    .Select(status => modifiers.TryGetValue(status, out var value) ? value : 0)
    .ToArray();

        return arrayModifiers;
    }

    public int[] GetEquippedWeaponModifiers(bool isPrimaryWeapom)
    {
        ResetModifiers();
        if (isPrimaryWeapom)
            GetModifiers(GetItemFromID(References.Instance.player.currentCharacter.equippedItems.weapon));
        else
            GetModifiers(GetItemFromID(References.Instance.player.currentCharacter.equippedItems.shield));

        int[] arrayModifiers = Enum.GetValues(typeof(Status))
    .Cast<Status>()
    .Select(status => modifiers.TryGetValue(status, out var value) ? value : 0)
    .ToArray();

        return arrayModifiers;
    }

    public int[] GetEquippedHelmetModifiers()
    {
        ResetModifiers();
        GetModifiers(GetItemFromID(References.Instance.player.currentCharacter.equippedItems.helmet));

        int[] arrayModifiers = Enum.GetValues(typeof(Status))
    .Cast<Status>()
    .Select(status => modifiers.TryGetValue(status, out var value) ? value : 0)
    .ToArray();
        return arrayModifiers;
    }

    public int[] GetEquippedShieldModifiers()
    {
        ResetModifiers();
        GetModifiers(GetItemFromID(References.Instance.player.currentCharacter.equippedItems.shield));
        int[] arrayModifiers = Enum.GetValues(typeof(Status))
   .Cast<Status>()
   .Select(status => modifiers.TryGetValue(status, out var value) ? value : 0)
   .ToArray();
        return arrayModifiers;
    }

    public int[] GetEquippedJewelryModifiers()
    {
        ResetModifiers();
        GetModifiers(GetItemFromID(References.Instance.player.currentCharacter.equippedItems.jewelry));
        int[] arrayModifiers = Enum.GetValues(typeof(Status))
   .Cast<Status>()
   .Select(status => modifiers.TryGetValue(status, out var value) ? value : 0)
   .ToArray();
        return arrayModifiers;
    }

    public int[] GetEquippedWingsModifiers()
    {
        ResetModifiers();
        GetModifiers(GetItemFromID(References.Instance.player.currentCharacter.equippedItems.wings));
        int[] arrayModifiers = Enum.GetValues(typeof(Status))
   .Cast<Status>()
   .Select(status => modifiers.TryGetValue(status, out var value) ? value : 0)
   .ToArray();
        return arrayModifiers;
    }

    public int[] GetAllEquippedItensModifiers(Character character)
    {
        ResetModifiers();

        GetModifiers(GetItemFromID(character.equippedItems.wings));
        GetModifiers(GetItemFromID(character.equippedItems.jewelry));
        GetModifiers(GetItemFromID(character.equippedItems.shield));
        GetModifiers(GetItemFromID(character.equippedItems.helmet));
        GetModifiers(GetItemFromID(character.equippedItems.weapon));
        GetModifiers(GetItemFromID(character.equippedItems.armor));

        int[] arrayModifiers = Enum.GetValues(typeof(Status))
       .Cast<Status>()
       .Select(status => modifiers.TryGetValue(status, out var value) ? value : 0)
       .ToArray();

        return arrayModifiers;
    }

    private void ResetModifiers()
    {
        modifiers[Status.Health] = 0;
        modifiers[Status.Attack] = 0;
        modifiers[Status.Magic] = 0;
        modifiers[Status.Agility] = 0;
        modifiers[Status.Defense] = 0;
        modifiers[Status.Critical] = 0;
    }

    private void GetModifiers(ItemSO item)
    {
        if (item == null)
        {
            modifiers[Status.Health] += 0;
            modifiers[Status.Attack] += 0;
            modifiers[Status.Magic] += 0;
            modifiers[Status.Agility] += 0;
            modifiers[Status.Defense] += 0;
            modifiers[Status.Critical] += 0;
        }
        else
        {

            modifiers[Status.Health] += item.GetAttributeValue(Status.Health);
            modifiers[Status.Attack] += item.GetAttributeValue(Status.Attack);
            modifiers[Status.Magic] += item.GetAttributeValue(Status.Magic);
            modifiers[Status.Agility] += item.GetAttributeValue(Status.Agility);
            modifiers[Status.Defense] += item.GetAttributeValue(Status.Defense);
            modifiers[Status.Critical] += item.GetAttributeValue(Status.Critical);
        }
    }

    public ItemSO GetItemFromID(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;

        itemsById.TryGetValue(id, out var item);
        return item;
    }

    #endregion
}


