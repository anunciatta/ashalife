using TMPro;
using UnityEngine;

public class StatusInformation : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentValue;
    [SerializeField] private TextMeshProUGUI modifierValue;
    [Header("[0] Positive, [1] Negative")]
    [SerializeField] private Color[] colors;

    [SerializeField] private bool isHealth;

    public void CheckModifierValues(int currentModifiers, int selectetItemModifiers)
    {
        Clear();

        if (currentModifiers == selectetItemModifiers)
        {
            modifierValue.text = string.Empty;
        }

        else if (currentModifiers > selectetItemModifiers)
        {
            modifierValue.text = $"-{currentModifiers - selectetItemModifiers}";
            modifierValue.color = colors[1];
        }

        else
        {
            modifierValue.text = $"+{selectetItemModifiers - currentModifiers}";
            modifierValue.color = colors[0];
        }
    }

    public void UpdateValues(int statusValue, int equippedItemModifier, int maxValue)
    {

        if (!isHealth)
            currentValue.text = $"{statusValue + equippedItemModifier}";

        else
            currentValue.text = $"{statusValue + equippedItemModifier}<#7c8a97>/{maxValue}";
    }

    public void Clear()
    {
        modifierValue.text = string.Empty;
    }
}

public enum Status
{
    Health = 0, Attack = 1, Magic = 2, Agility = 3, Defense = 4, Critical = 5, Experience = 6, Energy = 7
}
