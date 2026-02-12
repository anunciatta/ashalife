using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour
{
    [SerializeField] Image defaultIcon;
    [SerializeField] Image equippedItemIcon;
    [SerializeField] ItemType equipmentType;

    [SerializeField] private Sprite[] iconSprites;

    private ItemSO equippedItem;

    public void EquipItem(string equippedItemId)
    {
        equippedItem = References.Instance.inventory.GetItemFromID(equippedItemId);

        if (equippedItem != null)
        {
            equippedItemIcon.sprite = equippedItem.inventoryIcon;
            defaultIcon.enabled = false;
            equippedItemIcon.enabled = true;
        }
    }

    public void UnequipItem()
    {
        equippedItem = null;
        equippedItemIcon.sprite = null;
        defaultIcon.enabled = true;
        equippedItemIcon.enabled = false;
    }

    public void TryEquipItem(string equippedItemId)
    {
        if (equippedItemId == string.Empty)
            UnequipItem();
        else
            EquipItem(equippedItemId);
    }

    public void UpdateEquipment(EquippedItens equippedItens)
    {
        switch (equipmentType)
        {
            case ItemType.Armor:
                TryEquipItem(equippedItens.armor);
                break;
            case ItemType.Weapon:
                TryEquipItem(equippedItens.weapon);
                break;
            case ItemType.Helmet:
                TryEquipItem(equippedItens.helmet);
                break;
            case ItemType.Shield:
                TryEquipItem(equippedItens.shield);
                break;
            case ItemType.Jewelry:
                TryEquipItem(equippedItens.jewelry);
                break;
            case ItemType.Wings:
                TryEquipItem(equippedItens.wings);
                break;
            default:
                UnequipItem();
                break;
        }
    }


}


