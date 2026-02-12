using System;

[Serializable]
public class CharacterStatus
{
    public Status status;
    public int maxValue;
    public int currentValue;

    public void SetMaxValue(int value)
    {
        maxValue = value;

        switch (status)
        {
            case Status.Health:
                Bus<OnHealthMaxValue>.CallEvent(new OnHealthMaxValue(value));
                break;

            case Status.Experience:
                Bus<OnExperienceMaxValue>.CallEvent(new OnExperienceMaxValue(value));
                break;

            case Status.Magic:
                Bus<OnMagicMaxValue>.CallEvent(new OnMagicMaxValue(value));
                break;

            case Status.Attack:
                Bus<OnAttackMaxValue>.CallEvent(new OnAttackMaxValue(value));
                break;

            case Status.Defense:
                Bus<OnDefenseMaxValue>.CallEvent(new OnDefenseMaxValue(value));
                break;

            case Status.Critical:
                Bus<OnCriticalMaxValue>.CallEvent(new OnCriticalMaxValue(value));
                break;

            case Status.Agility:
                Bus<OnAgilityMaxValue>.CallEvent(new OnAgilityMaxValue(value));
                break;

            default: break;
        }
    }

    public void OnValueChange(int newValue)
    {
        var oldValue = currentValue;

        if (currentValue == newValue)
        {
            return;
        }

        else if (newValue >= maxValue)
        {
            currentValue = maxValue;
        }

        else if (newValue <= 0)
        {
            currentValue = 0;
        }

        else
        {
            currentValue = newValue;
        }

        switch (status)
        {
            case Status.Health:
                Bus<OnHealthChange>.CallEvent(new OnHealthChange(oldValue, newValue));
                break;

            case Status.Experience:
                Bus<OnExperienceChange>.CallEvent(new OnExperienceChange(oldValue, newValue));
                break;

            case Status.Magic:
                Bus<OnMagicChange>.CallEvent(new OnMagicChange(oldValue, newValue));
                break;

            case Status.Attack:
                Bus<OnAttackChange>.CallEvent(new OnAttackChange(oldValue, newValue));
                break;

            case Status.Defense:
                Bus<OnDefenseChange>.CallEvent(new OnDefenseChange(oldValue, newValue));
                break;

            case Status.Critical:
                Bus<OnCriticalChange>.CallEvent(new OnCriticalChange(oldValue, newValue));
                break;

            case Status.Agility:
                Bus<OnAgilityChange>.CallEvent(new OnAgilityChange(oldValue, newValue));
                break;

            default: break;
        }
    }
}