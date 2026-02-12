using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New ClassSO", menuName = "ScriptableObjects/Classes")]
public class ClassSO : ScriptableObject
{
    public string className = string.Empty;

    public InitialConfig initialConfig;

    public ClassType classType;

    //Skills

    public int[] GetInitialAttributesValues()
    {
        int[] modifiers = new int[8];

        modifiers[0] = References.Instance.experienceConfigurations.classDefinitions[(int)classType].stats[0].baseValue;
        modifiers[1] = References.Instance.experienceConfigurations.classDefinitions[(int)classType].stats[1].baseValue;
        modifiers[2] = References.Instance.experienceConfigurations.classDefinitions[(int)classType].stats[2].baseValue;
        modifiers[3] = References.Instance.experienceConfigurations.classDefinitions[(int)classType].stats[3].baseValue;
        modifiers[4] = References.Instance.experienceConfigurations.classDefinitions[(int)classType].stats[4].baseValue;
        modifiers[5] = References.Instance.experienceConfigurations.classDefinitions[(int)classType].stats[5].baseValue;
        modifiers[6] = References.Instance.experienceConfigurations.classDefinitions[(int)classType].stats[6].baseValue;
        modifiers[7] = References.Instance.experienceConfigurations.classDefinitions[(int)classType].stats[7].baseValue;

        return modifiers;
    }
}

[Serializable]
public struct InitialConfig
{
    public ArmorSO armorSO;
    //public WeaponSO weaponSO;
}

public enum ClassType
{
    Archer, Warrior, Mage
}