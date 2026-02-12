using System;

[Serializable]

public class SaveData
{
    //Las Save
    public string lastSaved;

    public string username;

    public SaveData(Player player)
    {
        lastSaved = DateTime.UtcNow.ToString("o");
        username = player.userName;
    }
}


/*


[Serializable]
public class CharacterData
{
    public int index;

    public int health, attack, magic, agility, defense, critical, currentHealth, currentExperience, level;



    //Current Player Avatar Configuration
    public int eyeIndex, beardIndex, hairIndex, headIndex, eyebrowsIndex, earsIndex, mouthIndex, makeupIndex, accessoriesIndex, classesIndex, skinColorIndex, eyesColorIndex, makeupColorIndex, hairColorIndex, beardColorIndex;

    //Inventory
    public List<ItemData> items = new();

    public string equippedAmor, equippedWeapon, equippedShield, equippedHelmet, equippedWings, equippedJewelry;

    public CharacterData(Character character)
    {
        #region Authentication
        index = character.index;
        #endregion

        #region Status


        level = character.level;
        currentExperience = character.statuses[6].currentValue;

        health = character.statuses[0].maxValue;
        attack = character.statuses[1].maxValue;
        magic = character.statuses[2].maxValue;
        agility = character.statuses[3].maxValue;
        defense = character.statuses[4].maxValue;
        critical = character.statuses[5].maxValue;
        currentHealth = character.statuses[0].currentValue;

        #endregion

        #region Avatar Config

        eyeIndex = character.avatarConfig.eyeIndex;
        beardIndex = character.avatarConfig.beardIndex;
        hairIndex = character.avatarConfig.hairIndex;
        classesIndex = character.avatarConfig.classesIndex;
        headIndex = character.avatarConfig.headIndex;
        eyebrowsIndex = character.avatarConfig.eyebrowsIndex;
        earsIndex = character.avatarConfig.earsIndex;
        mouthIndex = character.avatarConfig.mouthIndex;
        makeupIndex = character.avatarConfig.makeupIndex;
        accessoriesIndex = character.avatarConfig.accessoriesIndex;

        skinColorIndex = character.avatarConfig.skinColorIndex;
        eyesColorIndex = character.avatarConfig.eyesColorIndex;
        makeupColorIndex = character.avatarConfig.makeupColorIndex;
        beardColorIndex = character.avatarConfig.beardColorIndex;
        hairColorIndex = character.avatarConfig.hairColorIndex;

        #endregion

        #region Inventory

        items.Clear();

        foreach (ItemSO item in References.Instance.inventory.items)
        {
            items.Add(new ItemData(item.saveableEntityId, item.GetQuantity()));
        }

        if (References.Instance.inventory.equippedItems.armor == null)
            equippedAmor = string.Empty;
        else
            equippedAmor = References.Instance.inventory.equippedItems.armor;

        if (References.Instance.inventory.equippedItems.jewelry == null)
            equippedJewelry = string.Empty;
        else
            equippedJewelry = References.Instance.inventory.equippedItems.jewelry;

        if (References.Instance.inventory.equippedItems.wings == null)
            equippedWings = string.Empty;
        else
            equippedWings = References.Instance.inventory.equippedItems.wings;

        if (References.Instance.inventory.equippedItems.weapon == null)
            equippedWeapon = string.Empty;
        else
            equippedWeapon = References.Instance.inventory.equippedItems.weapon;

        if (References.Instance.inventory.equippedItems.shield == null)
            equippedShield = string.Empty;
        else
            equippedShield = References.Instance.inventory.equippedItems.shield;

        if (References.Instance.inventory.equippedItems.helmet == null)
            equippedHelmet = string.Empty;
        else
            equippedHelmet = References.Instance.inventory.equippedItems.helmet;

        #endregion
    }
}
[Serializable]
public class TasksData
{

}



[Serializable]
public class ItemData
{
    public string itemID;
    public int quantity;

    public ItemData(string id, int qty)
    {
        itemID = id;
        quantity = qty;
    }
}
*/

