using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Helmet", menuName = "ScriptableObjects/Equipment/HelmetSO", order = 4)]
public class HelmetSO : EquipmentSO
{
    public HelmetSpriteConfig helmetSpriteConfig;
}

[Serializable]
public class HelmetSpriteConfig
{
    public Sprite sprite;
    public bool coverHair = true;
    public bool coverEars = true;
}
