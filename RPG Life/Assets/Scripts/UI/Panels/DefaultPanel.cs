using UnityEngine;

public class DefaultPanel : Panel
{
    [HideInInspector] public DefaultMenuButton[] menuButtons;

    [HideInInspector] public MenuTab currentTab = MenuTab.Dailies;

    [SerializeField] private Panel[] midlePanels;

    public override void Open()
    {
        base.Open();
        Bus<SetPreviousPanelEvent>.CallEvent(new SetPreviousPanelEvent(null));
        midlePanels[1].Open();
    }

    public override void Close()
    {
        base.Close();
    }

    public override void Initialize()
    {
        menuButtons = panel.GetComponentsInChildren<DefaultMenuButton>(true);
        RestoreInitialConfiguration();

        base.Initialize();

    }

    private void RestoreInitialConfiguration()
    {
        currentTab = MenuTab.Dailies;

        for (int i = 0; i < menuButtons.Length; i++)
        {
            menuButtons[i].Initialize(this, false, midlePanels[i]);
        }

        menuButtons[1].Initialize(this, true, midlePanels[1]); // Set first button as selected
    }


}

public enum MenuTab
{
    Dailies, Habits, Inventory, Challenges
}



