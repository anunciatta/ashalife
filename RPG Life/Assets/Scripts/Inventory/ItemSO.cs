using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public abstract class ItemSO : ScriptableObject
{
    [ContextMenu("Generate ID")]
    public void GenereteID() => saveableEntityId = Guid.NewGuid().ToString();
    public string saveableEntityId;

    public string nameLabel;
    public string descriptionLabel;
    public string flavorLabel;
    public int level;
    public Rarity rarity;

    public bool deletable = false;
    public List<Attribute> attributes = new();
    public Sprite inventoryIcon;

    public ItemType itemType;

    public Cost cost;
    public bool isEquipped = false;

    [SerializeField] private int quantity = 0;

    public List<ClassType> relatedClasses = new(3);
    public bool purchased = false;

    [ContextMenu("Set Default Attributes")]
    public void SetDefaultAttributes()
    {
        attributes = new()
        {
            new Attribute(Status.Health, 0),
            new Attribute(Status.Attack, 0),
            new Attribute(Status.Magic, 0),
            new Attribute(Status.Agility, 0),
            new Attribute(Status.Defense, 0),
            new Attribute(Status.Critical, 0)
        };
    }

    public int GetQuantity() => quantity;

    public void SetQuantity(int value)
    {
        if (value >= 0)
            quantity = value;

        else
            quantity = 0;
    }

    public void AddQuantity(int value) => quantity += value;

    public void Remove(int value)
    {
        if (quantity - value <= 0)
            quantity = 0;

        else
            quantity -= value;
    }


    public int GetAttributeValue(Status status)
    {
        return attributes.FirstOrDefault(a => a.status == status)?.value ?? 0;
    }

    public int Agility() => GetAttributeValue(Status.Agility);
    public int Attack() => GetAttributeValue(Status.Attack);
    public int Magic() => GetAttributeValue(Status.Magic);
    public int Defense() => GetAttributeValue(Status.Defense);
    public int Health() => GetAttributeValue(Status.Health);
    public int Critical() => GetAttributeValue(Status.Critical);
}

[Serializable]
public class Cost
{
    public int coinCost;
    public int gemCost;
}

