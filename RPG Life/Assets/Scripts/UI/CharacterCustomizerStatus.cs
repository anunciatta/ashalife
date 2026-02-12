using TMPro;
using UnityEngine;

public class CharacterCustomizerStatus : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI className;
    [SerializeField] TextMeshProUGUI health;
    [SerializeField] TextMeshProUGUI critical;
    [SerializeField] TextMeshProUGUI defense;
    [SerializeField] TextMeshProUGUI attack;
    [SerializeField] TextMeshProUGUI agility;
    [SerializeField] TextMeshProUGUI magic;


    public void UpdateCurrentValues(Character character, int[] modifiers)
    {
        int attackValue = character.statuses[1].currentValue + modifiers[1];
        int magicValue = character.statuses[2].currentValue + modifiers[2];
        int defenseValue = character.statuses[4].currentValue + modifiers[4];
        int criticalValue = character.statuses[5].currentValue + modifiers[5];
        int healthValue = character.statuses[0].currentValue + modifiers[0];
        int agilityValue = character.statuses[3].currentValue + modifiers[3];

        className.text = $"{References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.status}")}";
        health.text = $"{character.statuses[0].currentValue}<#7c8a97>/{healthValue}";
        attack.text = $"{attackValue}";
        magic.text = $"{magicValue}";
        agility.text = $"{agilityValue}";
        defense.text = $"{defenseValue}";
        critical.text = $"{criticalValue}";
    }

    public void UpdateValues(ClassSO classSO)
    {
        className.text = $"{References.Instance.localizationService.GetLocalizedClassesDescription(classSO.classType)}";
        health.text = $"{References.Instance.experienceConfigurations.classDefinitions[(int)classSO.classType].stats[0].baseValue}";
        attack.text = $"{References.Instance.experienceConfigurations.classDefinitions[(int)classSO.classType].stats[1].baseValue}";
        magic.text = $"{References.Instance.experienceConfigurations.classDefinitions[(int)classSO.classType].stats[2].baseValue}";
        defense.text = $"{References.Instance.experienceConfigurations.classDefinitions[(int)classSO.classType].stats[3].baseValue}";
        agility.text = $"{References.Instance.experienceConfigurations.classDefinitions[(int)classSO.classType].stats[4].baseValue}";
        critical.text = $"{References.Instance.experienceConfigurations.classDefinitions[(int)classSO.classType].stats[5].baseValue}";
    }
}
