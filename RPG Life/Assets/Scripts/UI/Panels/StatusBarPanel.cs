
public class StatusBarPanel : Panel
{
    void OnDestroy()
    {
        Bus<DisplayStatusBarEvent>.OnEvent -= OnDisplayStatusBarEvent;
    }

    private void OnDisplayStatusBarEvent(DisplayStatusBarEvent _event)
    {
        panel.SetActive(_event.Display);
    }

    public override void Initialize()
    {
        Bus<DisplayStatusBarEvent>.OnEvent += OnDisplayStatusBarEvent;
        base.Initialize();
    }

    public override void Close()
    {
        base.Close();
    }

}



