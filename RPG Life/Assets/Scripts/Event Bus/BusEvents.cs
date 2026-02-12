using System;
using UnityEngine;

public class BusEvents
{

}

public struct SkinColorPickedEvent : IEvent
{
    public int Color { get; private set; }

    public SkinColorPickedEvent(int color)
    {
        Color = color;
    }
}

public struct HairColorPickedEvent : IEvent
{
    public int Color { get; private set; }

    public HairColorPickedEvent(int color)
    {
        Color = color;
    }
}

public struct MakeupColorPickedEvent : IEvent
{
    public int Color { get; private set; }

    public MakeupColorPickedEvent(int color)
    {
        Color = color;
    }
}

public struct BeardColorPickedEvent : IEvent
{
    public int Color { get; private set; }

    public BeardColorPickedEvent(int color)
    {
        Color = color;
    }
}

public struct EyesColorPickedEvent : IEvent
{
    public int Color { get; private set; }

    public EyesColorPickedEvent(int color)
    {
        Color = color;
    }
}

public struct HeadSpriteChangeEvent : IEvent
{
    public Sprite Icon { get; private set; }

    public HeadSpriteChangeEvent(Sprite icon)
    {
        Icon = icon;
    }
}

public struct EarSpriteChangeEvent : IEvent
{
    public Sprite Icon { get; private set; }

    public EarSpriteChangeEvent(Sprite icon)
    {
        Icon = icon;
    }
}

public struct HairSpriteChangeEvent : IEvent
{
    public Sprite Icon { get; private set; }

    public HairSpriteChangeEvent(Sprite icon)
    {
        Icon = icon;
    }
}

public struct ArmorSpriteChangeEvent : IEvent
{
    public ArmorSpriteConfig ArmorSpriteConfig { get; private set; }

    public ArmorSpriteChangeEvent(ArmorSpriteConfig armorSpriteConfig)
    {
        ArmorSpriteConfig = armorSpriteConfig;
    }
}

public struct WeaponSpriteChangeEvent : IEvent
{
    public WeaponSpriteConfig WeaponSpriteConfig { get; private set; }

    public WeaponSpriteChangeEvent(WeaponSpriteConfig weaponSpriteConfig)
    {
        WeaponSpriteConfig = weaponSpriteConfig;
    }
}

public struct ShieldSpriteChangeEvent : IEvent
{
    public Sprite Sprite { get; private set; }

    public ShieldSpriteChangeEvent(Sprite sprite)
    {
        Sprite = sprite;
    }
}

public struct WingSpriteChangeEvent : IEvent
{
    public Sprite Sprite { get; private set; }

    public WingSpriteChangeEvent(Sprite sprite)
    {
        Sprite = sprite;
    }
}

public struct SecondWeaponSpriteChangeEvent : IEvent
{
    public WeaponSpriteConfig WeaponSpriteConfig { get; private set; }

    public SecondWeaponSpriteChangeEvent(WeaponSpriteConfig weaponSpriteConfig)
    {
        WeaponSpriteConfig = weaponSpriteConfig;
    }
}



public struct HelmetSpriteChangeEvent : IEvent
{
    public HelmetSpriteConfig HelmetSpriteConfig { get; private set; }

    public HelmetSpriteChangeEvent(HelmetSpriteConfig helmetSpriteConfig)
    {
        HelmetSpriteConfig = helmetSpriteConfig;
    }
}

public struct EyesSpriteChangeEvent : IEvent
{
    public Sprite Icon { get; private set; }

    public EyesSpriteChangeEvent(Sprite icon)
    {
        Icon = icon;
    }
}

public struct EyebrowsSpriteChangeEvent : IEvent
{
    public Sprite Icon { get; private set; }

    public EyebrowsSpriteChangeEvent(Sprite icon)
    {
        Icon = icon;
    }
}

public struct BeardSpriteChangeEvent : IEvent
{
    public Sprite Icon { get; private set; }

    public BeardSpriteChangeEvent(Sprite icon)
    {
        Icon = icon;
    }
}

public struct MouthSpriteChangeEvent : IEvent
{
    public Sprite Icon { get; private set; }

    public MouthSpriteChangeEvent(Sprite icon)
    {
        Icon = icon;
    }
}

public struct AccessorySpriteChangeEvent : IEvent
{
    public Sprite Icon { get; private set; }

    public AccessorySpriteChangeEvent(Sprite icon)
    {
        Icon = icon;
    }
}

public struct MakeupSpriteChangeEvent : IEvent
{
    public Sprite Icon { get; private set; }

    public MakeupSpriteChangeEvent(Sprite icon)
    {
        Icon = icon;
    }
}


public struct SetPreviousPanelEvent : IEvent
{
    public Panel Panel { get; private set; }

    public SetPreviousPanelEvent(Panel panel)
    {
        Panel = panel;
    }
}

public struct SetCurrentPanelEvent : IEvent
{
    public Panel Panel { get; private set; }

    public SetCurrentPanelEvent(Panel panel)
    {
        Panel = panel;
    }
}

public struct TransitionPanelsEvent : IEvent
{
    public Panel From { get; private set; }
    public Panel To { get; private set; }

    public TransitionPanelsEvent(Panel from, Panel to)
    {
        From = from;
        To = to;
    }
}

public struct OnSignInFirebase : IEvent
{
    public bool HasSignIn { get; private set; }

    public OnSignInFirebase(bool hasSignIn)
    {
        HasSignIn = hasSignIn;
    }
}



public struct OnReferenceSignInFirebase : IEvent
{
    public bool HasSignIn { get; private set; }

    public OnReferenceSignInFirebase(bool hasSignIn)
    {
        HasSignIn = hasSignIn;
    }
}


public struct DisplayStatusBarEvent : IEvent
{
    public bool Display { get; private set; }

    public DisplayStatusBarEvent(bool display)
    {
        Display = display;
    }
}

public struct DisplayNavigationBarEvent : IEvent
{
    public bool Display { get; private set; }

    public DisplayNavigationBarEvent(bool display)
    {
        Display = display;
    }
}

public struct LoadCharacterEvent : IEvent
{
    public Character Character { get; private set; }

    public LoadCharacterEvent(Character data)
    {
        Character = data;
    }
}

public struct OnClassChangeEvent : IEvent
{
    public ClassSO NewClass { get; private set; }

    public OnClassChangeEvent(ClassSO newClass)
    {
        NewClass = newClass;
    }
}

public struct OnItemChangedEvent : IEvent
{
    public ItemSO Item { get; private set; }
    public int OldQuantity { get; private set; }
    public int NewQuantity { get; private set; }

    public OnItemChangedEvent(ItemSO item, int oldQuantity, int newQuantity)
    {
        Item = item;
        OldQuantity = oldQuantity;
        NewQuantity = newQuantity;
    }
}

public struct OnAddNewItemEvent : IEvent
{
    public ItemSO Item { get; private set; }
    public int Quantity { get; private set; }

    public OnAddNewItemEvent(ItemSO item, int quantity)
    {
        Item = item;
        Quantity = quantity;
    }
}

public struct OnConfirmationPanelEvent : IEvent
{
    public Action ConfirmAction { get; private set; }
    public string Message { get; private set; }
    public string Title { get; private set; }

    public OnConfirmationPanelEvent(Action confirmAction, string message = "", string title = "")
    {
        ConfirmAction = confirmAction;
        Message = message;
        Title = title;
    }
}


public struct OnLanguageChanged : IEvent
{
    public Language NewLanguage { get; private set; }

    public OnLanguageChanged(Language newLanguage)
    {
        NewLanguage = newLanguage;
    }
}


public struct OnRemoveItemEvent : IEvent
{
    public ItemSO Item { get; private set; }

    public OnRemoveItemEvent(ItemSO item)
    {
        Item = item;
    }
}

public struct OnHasNotEnoughCurrencyEvent : IEvent
{
    public ItemSO Item { get; private set; }

    public OnHasNotEnoughCurrencyEvent(ItemSO item)
    {
        Item = item;
    }
}

public struct ChangeCoinsEvent : IEvent
{
    public int NewValue { get; private set; }

    public ChangeCoinsEvent(int newValue)
    {
        NewValue = newValue;
    }
}

public struct ChangeGemsEvent : IEvent
{
    public int NewValue { get; private set; }

    public ChangeGemsEvent(int newValue)
    {
        NewValue = newValue;
    }
}

public struct OnLevelUp : IEvent
{
    public Character Character { get; private set; }

    public OnLevelUp(Character character)
    {
        Character = character;
    }
}

public struct OnLevelDown : IEvent
{
    public Character Character { get; private set; }

    public OnLevelDown(Character character)
    {
        Character = character;
    }
}

public struct ConnectionFailureEvent : IEvent
{
    public string Message { get; private set; }

    public ConnectionFailureEvent(string message)
    {
        Message = message;
    }
}


#region Tasks

#region Daily

public struct OnAddNewDaily : IEvent
{
    public DailyContent DailyContent { get; private set; }

    public OnAddNewDaily(DailyContent dailyContent)
    {
        DailyContent = dailyContent;
    }
}

public struct OnRemoveDailyOption : IEvent
{
    public DailyContent DailyContent { get; private set; }

    public OnRemoveDailyOption(DailyContent dailyContent)
    {
        DailyContent = dailyContent;
    }
}

public struct OnRemoveHabitConfirm : IEvent
{
    public HabitContent HabitContent { get; private set; }

    public OnRemoveHabitConfirm(HabitContent habitContent)
    {
        HabitContent = habitContent;
    }
}

public struct OnRemoveDailyConfirm : IEvent
{
    public DailyContent DailyContent { get; private set; }

    public OnRemoveDailyConfirm(DailyContent dailyContent)
    {
        DailyContent = dailyContent;
    }
}

public struct OnEditDailyOptionEvent : IEvent
{
    public DailyContent DailyContent { get; private set; }

    public OnEditDailyOptionEvent(DailyContent dailyContent)
    {
        DailyContent = dailyContent;
    }
}

public struct OnUpdateDaily : IEvent
{
    public DailyContent DailyContent { get; private set; }

    public OnUpdateDaily(DailyContent dailyContent)
    {
        DailyContent = dailyContent;
    }
}



#endregion

#region Habit

public struct OnAddNewHabit : IEvent
{
    public HabitContent HabitContent { get; private set; }

    public OnAddNewHabit(HabitContent habitContent)
    {
        HabitContent = habitContent;
    }
}

public struct OnRemoveHabitOption : IEvent
{
    public HabitContent HabitContent { get; private set; }

    public OnRemoveHabitOption(HabitContent habitContent)
    {
        HabitContent = habitContent;
    }
}

public struct OnEditHabitOptionEvent : IEvent
{
    public HabitContent HabitContent { get; private set; }

    public OnEditHabitOptionEvent(HabitContent habitContent)
    {
        HabitContent = habitContent;
    }
}

public struct OnUpdateHabit : IEvent
{
    public HabitContent HabitContent { get; private set; }

    public OnUpdateHabit(HabitContent habitContent)
    {
        HabitContent = habitContent;
    }
}

#endregion

#endregion



public struct OnCharacterLoad : IEvent
{
    public Character Character { get; private set; }

    public OnCharacterLoad(Character character)
    {
        Character = character;
    }
}

public struct OnAddToast : IEvent
{
    public PopupConfig PopupConfig { get; private set; }

    public OnAddToast(PopupConfig popupConfig)
    {
        PopupConfig = popupConfig;
    }
}


#region Status Events

#region Health
public struct OnHealthMaxValue : IEvent
{
    public int Value { get; private set; }

    public OnHealthMaxValue(int value)
    {
        Value = value;
    }
}

public struct OnHealthChange : IEvent
{
    public int OldValue { get; private set; }
    public int NewValue { get; private set; }

    public OnHealthChange(int oldValue, int newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}

public struct OnHealthReachesZero : IEvent
{
    public int CurrentLevel { get; private set; }

    public OnHealthReachesZero(int currentLevel)
    {
        CurrentLevel = currentLevel;
    }
}

#endregion

#region Magic
public struct OnMagicMaxValue : IEvent
{
    public int Value { get; private set; }

    public OnMagicMaxValue(int value)
    {
        Value = value;
    }
}

public struct OnMagicChange : IEvent
{
    public int OldValue { get; private set; }
    public int NewValue { get; private set; }

    public OnMagicChange(int oldValue, int newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}

#endregion

#region Attack
public struct OnAttackMaxValue : IEvent
{
    public int Value { get; private set; }

    public OnAttackMaxValue(int value)
    {
        Value = value;
    }
}

public struct OnAttackChange : IEvent
{
    public int OldValue { get; private set; }
    public int NewValue { get; private set; }

    public OnAttackChange(int oldValue, int newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}

#endregion

#region Defense
public struct OnDefenseMaxValue : IEvent
{
    public int Value { get; private set; }

    public OnDefenseMaxValue(int value)
    {
        Value = value;
    }
}

public struct OnDefenseChange : IEvent
{
    public int OldValue { get; private set; }
    public int NewValue { get; private set; }

    public OnDefenseChange(int oldValue, int newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}

#endregion

#region Critical
public struct OnCriticalMaxValue : IEvent
{
    public int Value { get; private set; }

    public OnCriticalMaxValue(int value)
    {
        Value = value;
    }
}

public struct OnCriticalChange : IEvent
{
    public int OldValue { get; private set; }
    public int NewValue { get; private set; }

    public OnCriticalChange(int oldValue, int newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}

#endregion

#region Agility
public struct OnAgilityMaxValue : IEvent
{
    public int Value { get; private set; }

    public OnAgilityMaxValue(int value)
    {
        Value = value;
    }
}

public struct OnAgilityChange : IEvent
{
    public int OldValue { get; private set; }
    public int NewValue { get; private set; }

    public OnAgilityChange(int oldValue, int newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}

#endregion

#region Experience
public struct OnExperienceMaxValue : IEvent
{
    public int Value { get; private set; }

    public OnExperienceMaxValue(int value)
    {
        Value = value;
    }
}

public struct OnExperienceChange : IEvent
{
    public int OldValue { get; private set; }
    public int NewValue { get; private set; }

    public OnExperienceChange(int oldValue, int newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}

#endregion

#region Energy

public struct OnEnergyMaxValue : IEvent
{
    public int Value { get; private set; }

    public OnEnergyMaxValue(int value)
    {
        Value = value;
    }
}

public struct OnEnergyChange : IEvent
{
    public int OldValue { get; private set; }
    public int NewValue { get; private set; }

    public OnEnergyChange(int oldValue, int newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}


#endregion

#endregion

