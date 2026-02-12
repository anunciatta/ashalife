using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class SignInPanel : Panel
{
    [Header("Login UI")]
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public Toggle rememberMeToggle;
    public Button loginButton;
    public Button signOutButton;
    public TextMeshProUGUI statusText;
    public Panel forgotPasswordPanel;

    private const string REMEMBER_ME_KEY = "RememberMe";
    private const string SAVED_EMAIL_KEY = "SavedEmail";

    public override void Open()
    {
        base.Open();

        // Pre-fill email if saved
        string savedEmail = PlayerPrefs.GetString(SAVED_EMAIL_KEY, "");
        if (!string.IsNullOrEmpty(savedEmail))
        {
            emailInput.text = savedEmail;
            rememberMeToggle.isOn = true;
            passwordInput.Select();
        }

        passwordInput.text = string.Empty;
        passwordInput.contentType = TMP_InputField.ContentType.Password;
        passwordIsHiden = true;
        Canvas.ForceUpdateCanvases();
        loginButton.interactable = true;
    }

    public async void OnLoginClicked()
    {
        string email = emailInput.text.Trim();
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            statusText.text = "Please fill in all fields";
            return;
        }

        loginButton.interactable = false;
        statusText.text = "Signing in...";

        try
        {
            await References.Instance.firebaseManager.auth.SignInWithEmailAndPasswordAsync(email, password);

            // Save preferences
            if (rememberMeToggle.isOn)
            {
                PlayerPrefs.SetInt(REMEMBER_ME_KEY, 1);
                PlayerPrefs.SetString(SAVED_EMAIL_KEY, email);
            }
            else
            {
                PlayerPrefs.SetInt(REMEMBER_ME_KEY, 0);
                PlayerPrefs.DeleteKey(SAVED_EMAIL_KEY);
            }
            PlayerPrefs.Save();

            References.Instance.player.userName = await FirebaseSaveManager.GetUsername();
            Bus<OnReferenceSignInFirebase>.CallEvent(new OnReferenceSignInFirebase(true));

            statusText.text = "Login successful!";
            References.Instance.firebaseManager.user = References.Instance.firebaseManager.auth.CurrentUser;
            await Task.Delay(500);
            Bus<TransitionPanelsEvent>.CallEvent(new TransitionPanelsEvent(this, nextPanel));
        }
        catch (System.Exception e)
        {
            statusText.text = GetErrorMessage(e);
            loginButton.interactable = true;
        }
    }

    string GetErrorMessage(System.Exception e)
    {
        if (e.Message.Contains("wrong-password") || e.Message.Contains("user-not-found"))
            return "Invalid email or password";
        else if (e.Message.Contains("network"))
            return "Network error";
        else
            return "Login failed";
    }

    public void OnSignUpClicked()
    {
        Bus<TransitionPanelsEvent>.CallEvent(new TransitionPanelsEvent(this, previousPanel));
    }

    public void OnForgotPasswordClicked()
    {
        Bus<TransitionPanelsEvent>.CallEvent(new TransitionPanelsEvent(this, forgotPasswordPanel));
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
}




