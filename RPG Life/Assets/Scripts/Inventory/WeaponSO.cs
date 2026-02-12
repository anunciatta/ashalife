using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "ScriptableObjects/Equipment/WeaponSO", order = 2)]
public class WeaponSO : EquipmentSO
{
    public WeaponSpriteConfig weaponSprites;
}

[Serializable]
public class WeaponSpriteConfig
{
    [Header("[0]Handler[1]Limb[2]Limb")]
    public List<Sprite> leftHand;
    public Sprite rightHand;
    [Header("Quiver")]
    public Sprite back;
}
