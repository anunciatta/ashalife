using UnityEngine;
using UnityEngine.InputSystem;

public class PanelsController : MonoBehaviour
{
    public bool PreviousPanelExists() => previousPanel != null || currentPanel == panels[panels.Length - 1];

    private Panel previousPanel = null;
    private Panel currentPanel = null;

    public Panel PreviousPanel() => previousPanel;
    public Panel CurrentPanel() => currentPanel;

    private Panel[] panels;

    void Awake()
    {
        previousPanel = null;

        panels = GetComponentsInChildren<Panel>(true);


    }



    public void SetCurrentPanel(SetCurrentPanelEvent _event) => currentPanel = _event.Panel;
    public void SetPreviousPanel(SetPreviousPanelEvent _event) => previousPanel = _event.Panel;

    public void TransitionPanels(TransitionPanelsEvent _event)
    {
        _event.To.Open();
        _event.From.Close();
    }

    BackButtonHandler backButtonHandler;

    void Start()
    {
        foreach (Panel panel in panels)
        {
            panel.Initialize();
        }

        backButtonHandler = GetComponent<BackButtonHandler>();

        Bus<SetPreviousPanelEvent>.OnEvent += SetPreviousPanel;
        Bus<SetCurrentPanelEvent>.OnEvent += SetCurrentPanel;
        Bus<TransitionPanelsEvent>.OnEvent += TransitionPanels;
    }

    private void OnDestroy()
    {
        Bus<SetPreviousPanelEvent>.OnEvent -= SetPreviousPanel;
        Bus<SetCurrentPanelEvent>.OnEvent -= SetCurrentPanel;
        Bus<TransitionPanelsEvent>.OnEvent -= TransitionPanels;
    }

    void Update()
    {
        //CheckBackButton();
    }

    private void CheckBackButton()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame && PreviousPanelExists())
            {
                backButtonHandler.HandleBackButton();
                return;
            }
        }
    }
}


