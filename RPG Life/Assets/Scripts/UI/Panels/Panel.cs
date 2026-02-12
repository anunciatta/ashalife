using UnityEngine;

public class Panel : MonoBehaviour
{
    public GameObject panel;

    public Panel nextPanel;
    public Panel previousPanel;

    [SerializeField] private bool hideUpperPanel = false;

    public virtual void Open()
    {
        Bus<SetCurrentPanelEvent>.CallEvent(new SetCurrentPanelEvent(this));
        Bus<DisplayStatusBarEvent>.CallEvent(new DisplayStatusBarEvent(!hideUpperPanel));
        panel.SetActive(true);
    }

    public virtual void Close()
    {
        Bus<SetPreviousPanelEvent>.CallEvent(new SetPreviousPanelEvent(this));
        panel.SetActive(false);
    }

    public virtual void Initialize()
    {
        panel.SetActive(false);
    }
}


