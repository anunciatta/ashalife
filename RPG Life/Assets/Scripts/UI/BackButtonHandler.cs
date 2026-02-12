using System;
using System.Collections;
using UnityEngine;

public class BackButtonHandler : MonoBehaviour
{
    private bool backPressedOnce = false;
    private float backPressTime = 0f;
    private const float exitTimeWindow = 1.5f; // Time window in seconds
    private AndroidJavaObject currentToast = null; // Store reference to toast

    public void HandleBackButton()
    {
        if (backPressedOnce && (Time.time - backPressTime) < exitTimeWindow)
        {
            // Cancel the toast before quitting
            CancelToast();

            // Second press within time window - exit app
            Application.Quit();
        }
        else
        {
            // First press or too much time passed - show message
            backPressedOnce = true;
            backPressTime = Time.time;

            // Show toast message to user
            ShowExitMessage();

            // Optional: Reset after time window expires
            StartCoroutine(ResetBackPress());
        }
    }

    void ShowExitMessage()
    {
        Debug.Log("Showing exit message");
        currentToast = AndroidToast.Show("Press back again to exit");
    }

    IEnumerator ResetBackPress()
    {
        yield return new WaitForSeconds(exitTimeWindow);
        backPressedOnce = false;

    }

    void CancelToast()
    {
        if (currentToast != null)
        {
            try
            {
                currentToast.Call("cancel");
            }
            catch (Exception e)
            {
                Debug.Log("Could not cancel toast: " + e.Message);
            }
            currentToast = null;
        }
    }
}

public static class AndroidToast
{
    public static AndroidJavaObject Show(string message)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject toast = null;

            currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                toast = new AndroidJavaClass("android.widget.Toast")
                    .CallStatic<AndroidJavaObject>("makeText", currentActivity, message, 0);
                toast.Call("show");
            }));

            return toast;
        }
        return null;
    }
}
