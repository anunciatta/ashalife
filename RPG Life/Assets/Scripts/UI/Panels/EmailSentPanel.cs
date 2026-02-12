using TMPro;

public class EmailSentPanel : Panel
{
    public TextMeshProUGUI text;

    public void OnBackToLoginClicked()
    {
        Bus<TransitionPanelsEvent>.CallEvent(new TransitionPanelsEvent(this, nextPanel));
    }

    internal void ShowSuccessPanel(string email)
    {
        text.text = $"{email}";
    }
}




