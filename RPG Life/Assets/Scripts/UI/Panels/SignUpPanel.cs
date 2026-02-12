using TMPro;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine;

public class SignUpPanel : Panel
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_InputField usernameInput;
    public TextMeshProUGUI statusText;
    public Button signUpButton;

    public override void Initialize()
    {
        base.Initialize();
        statusText.text = string.Empty;
        signUpButton.onClick.AddListener(OnSignUpClicked);
    }

    public override void Open()
    {
        base.Open();
        statusText.text = string.Empty;

        usernameInput.text = string.Empty;

        emailInput.text = string.Empty;

        passwordInput.text = string.Empty;
        passwordInput.contentType = TMP_InputField.ContentType.Password;
        passwordIsHiden = true;
        signUpButton.interactable = true;

        Canvas.ForceUpdateCanvases();
    }

    void OnDestroy()
    {
        signUpButton.onClick.RemoveAllListeners();
    }

    public async void OnSignUpClicked()
    {
        string email = emailInput.text.Trim();
        string password = passwordInput.text;
        string username = usernameInput.text;

        // Disable button
        signUpButton.interactable = false;

        // Create account
        var (success, message) = await References.Instance.firebaseManager.CreateAccount(email, password, username);

        if (success)
        {
            statusText.text = "Account created! Logging in...";

            References.Instance.player.userName = username;

            try
            {
                await References.Instance.firebaseManager.auth.SignInWithEmailAndPasswordAsync(email, password);

                References.Instance.firebaseManager.user = References.Instance.firebaseManager.auth.CurrentUser;

                await Task.Delay(500);

                await FirebaseSaveManager.SaveGame(new SaveData(References.Instance.player));

                // Wait a moment then go to game
                await Task.Delay(1000);


                Bus<TransitionPanelsEvent>.CallEvent(new TransitionPanelsEvent(this, nextPanel));
            }
            catch
            {
                statusText.text = message;
                signUpButton.interactable = true;
            }
        }
        else
        {
            statusText.text = message;
            signUpButton.interactable = true;
        }
    }

    private bool passwordIsHiden = true;

    public void OnShowPasswordClicked()
    {
        if (passwordIsHiden)
        {
            passwordInput.contentType = TMP_InputField.ContentType.Standard;
            passwordIsHiden = false;
        }
        else
        {
            passwordInput.contentType = TMP_InputField.ContentType.Password;
            passwordIsHiden = true;
        }

        passwordInput.Select();
        Canvas.ForceUpdateCanvases();

    }

    public void OnSignInClicked()
    {
        Bus<TransitionPanelsEvent>.CallEvent(new TransitionPanelsEvent(this, previousPanel));
    }
}




