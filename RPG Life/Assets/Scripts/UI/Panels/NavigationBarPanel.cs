public class NavigationBarPanel : Panel
{
    void OnDestroy()
    {
        Bus<DisplayNavigationBarEvent>.OnEvent -= OnDisplayNavigationBarEvent;
    }

    private void OnDisplayNavigationBarEvent(DisplayNavigationBarEvent _event)
    {
        panel.SetActive(_event.Display);
    }

    public override void Initialize()
    {
        Bus<DisplayNavigationBarEvent>.OnEvent += OnDisplayNavigationBarEvent;
        base.Initialize();
    }

}



