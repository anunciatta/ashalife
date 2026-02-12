using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ForgotPasswordPanel : Panel
{
    [Header("Login UI")]
    public TMP_InputField emailInput;
    public TextMeshProUGUI statusText;
    public Button sendResetButton;
    public EmailSentPanel emailSentPanel;

    Color baseColor;

    public override void Initialize()
    {
        base.Initialize();
        sendResetButton.onClick.AddListener(OnSendClicked);
        baseColor = statusText.color;
    }

    void OnDestroy()
    {
        sendResetButton.onClick.RemoveAllListeners();
    }

    public override void Open()
    {
        base.Open();
        statusText.text = "";
        emailInput.text = string.Empty;
        emailInput.Select();
        statusText.text = "Please enter your email address";
        statusText.color = baseColor;
        sendResetButton.interactable = true;
    }

    public async void OnSendClicked()
    {
        string email = emailInput.text.Trim();

        if (string.IsNullOrEmpty(email))
        {
            statusText.color = Color.red;
            statusText.text = "Please enter your email address";
            return;
        }

        // Disable button during process
        sendResetButton.interactable = false;
        statusText.color = baseColor;
        statusText.text = "Sending reset email...";

        // Send reset email
        var (success, message) = await References.Instance.firebaseManager.SendPasswordResetEmail(email);

        if (success)
        {
            // Show success message
            Bus<TransitionPanelsEvent>.CallEvent(new TransitionPanelsEvent(this, nextPanel));
            emailSentPanel.ShowSuccessPanel(email);
        }
        else
        {
            // Show error
            statusText.color = Color.red;
            statusText.text = message;
            sendResetButton.interactable = true;
        }
    }

    public void OnLoginClicked()
    {
        Bus<TransitionPanelsEvent>.CallEvent(new TransitionPanelsEvent(this, previousPanel));
    }
}




