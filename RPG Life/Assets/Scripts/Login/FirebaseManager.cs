using Firebase;
using Firebase.Database;
using Firebase.Auth;
using UnityEngine;
using System.Threading.Tasks;
using System.Text.RegularExpressions;


public class FirebaseManager : MonoBehaviour
{
    public DatabaseReference database;
    public FirebaseAuth auth;
    public FirebaseUser currentUser;
    public FirebaseUser user;

    public string REMEMBER_ME_KEY = "RememberMe";
    public string SAVED_EMAIL_KEY = "SavedEmail";

    public SaveData CurrentSaveData { get; private set; }

    public async Task<(bool success, string errorMessage)> CreateAccount(string email, string password, string username)
    {
        // 1. Validate email format locally (instant feedback)
        if (string.IsNullOrWhiteSpace(email))
        {
            return (false, "Email cannot be empty");
        }

        if (!IsValidEmail(email))
        {
            return (false, "Please enter a valid email address");
        }

        // 2. Validate password locally
        if (string.IsNullOrWhiteSpace(password))
        {
            return (false, "Password cannot be empty");
        }

        if (password.Length < 6)
        {
            return (false, "Password must be at least 6 characters");
        }

        if (username.Length <= 0)
        {
            return (false, "Please enter a username. You can modify it at any time later.");
        }

        // 3. Try to create account with Firebase
        try
        {
            var result = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            currentUser = result.User;
            user = currentUser;
            // await CreateInitialUserData(currentUser.UserId);

            Debug.Log($"✓ Account created: {email}");
            return (true, "Account created successfully!");
        }
        catch (FirebaseException e)
        {
            // Handle specific Firebase errors
            string errorMessage = GetFirebaseErrorMessage(e);
            Debug.LogError($"Account creation failed: {errorMessage}");
            return (false, errorMessage);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Unexpected error: {e.Message}");
            return (false, "An unexpected error occurred. Please try again.");
        }
    }

    // Email validation helper
    private bool IsValidEmail(string email)
    {
        // Basic email regex pattern
        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern);
    }

    // More comprehensive email validation
    private bool IsValidEmailStrict(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            // Use .NET built-in email validation
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    // Convert Firebase error codes to user-friendly messages
    private string GetFirebaseErrorMessage(FirebaseException e)
    {
        // Firebase error codes: https://firebase.google.com/docs/auth/admin/errors

        if (e.Message.Contains("email-already-in-use"))
        {
            return "This email is already registered. Please sign in instead.";
        }
        else if (e.Message.Contains("invalid-email"))
        {
            return "Please enter a valid email address.";
        }
        else if (e.Message.Contains("weak-password"))
        {
            return "Password is too weak. Please use at least 6 characters.";
        }
        else if (e.Message.Contains("network-request-failed"))
        {
            return "Network error. Please check your internet connection.";
        }
        else
        {
            return "Account creation failed. Please try again.";
        }
    }

    // Send password reset email
    public async Task<(bool success, string message)> SendPasswordResetEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return (false, "Please enter your email address");
        }

        // Validate email format
        if (!IsValidEmail(email))
        {
            return (false, "Please enter a valid email address");
        }

        try
        {
            await auth.SendPasswordResetEmailAsync(email);

            Debug.Log($"Password reset email sent to: {email}");
            return (true, $"Password reset email sent to {email}. Please check your inbox.");
        }
        catch (FirebaseException e)
        {
            Debug.LogError($"Password reset failed: {e.Message}");

            if (e.Message.Contains("user-not-found"))
            {
                // Security: Don't reveal if email exists or not
                // Still return success message
                return (true, $"If an account exists for {email}, a password reset email has been sent.");
            }
            else if (e.Message.Contains("invalid-email"))
            {
                return (false, "Please enter a valid email address");
            }
            else if (e.Message.Contains("network"))
            {
                return (false, "Network error. Please check your connection.");
            }
            else
            {
                return (false, "Failed to send password reset email. Please try again.");
            }
        }
    }

        // Sign out
    public void SignOut()
    {
        auth.SignOut();
        currentUser = null;
        user = null;
        database = null;

        
        Debug.Log("✓ Signed out");
    }
}