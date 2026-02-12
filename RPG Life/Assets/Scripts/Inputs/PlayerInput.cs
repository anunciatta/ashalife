using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private PanelsController panelsController;
    [SerializeField] private BackButtonHandler backButtonHandler;

    public void OnBack()
    {
        if (!panelsController.PreviousPanelExists())
        {
            backButtonHandler.HandleBackButton();
            return;
        }

        else
        {
            Bus<TransitionPanelsEvent>.CallEvent(new TransitionPanelsEvent(panelsController.CurrentPanel(), panelsController.PreviousPanel()));

        }
    }

}
