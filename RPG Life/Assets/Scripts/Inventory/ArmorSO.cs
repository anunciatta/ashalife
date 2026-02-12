using System;
using UnityEngine;
using System.Collections.Generic;






[CreateAssetMenu(fileName = "New Armor", menuName = "ScriptableObjects/Equipment/ArmorSO", order = 1)]
public class ArmorSO : EquipmentSO
{
    [Header("[0] Body [1] ArmL [2] ArmR [3] SleeveL [4] SleeveR [5] HandL [6] HandR [7] LegL [8] LegR")]
    public ArmorSpriteConfig armorSprites;
}

[Serializable]
public class ArmorSpriteConfig
{
    public List<Sprite> sprites;
}

[Serializable]
public class Attribute
{
    public Status status;
    public int value;

    public Attribute(Status status, int value)
    {
        this.status = status;
        this.value = value;
    }
}



