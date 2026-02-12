
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
using Firebase.Analytics;

using UnityEngine.AddressableAssets;
using System;

[Serializable]
public class AssetReferenceAllItems : AssetReferenceT<AllItemsSO>
{
    public AssetReferenceAllItems(string guid)
        : base(guid)
    {
    }
}

public class MainPanel : Panel
{
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private AssetReferenceAllItems allItems;
    public TextMeshProUGUI percentageText;

    [Header("Settings")]
    public float duration = 2f; // Duration in seconds
    public Ease easeType = Ease.Linear;

    public override void Open()
    {
        base.Open();

        loadingSlider.value = 0;
        percentageText.text = "Loading... 0%";
        panel.SetActive(true);

        // Initialize slider at 0
        loadingSlider.value = 0;
        percentageText.text = "Loading... 0%";

        CheckLogin();
    }

    public override void Close()
    {
        base.Close();
    }

    async void CheckLogin()
    {
        await InitializeLogin();
    }

    async void CheckForUpdates()
    {
        var handle = Addressables.CheckForCatalogUpdates();
        await handle.Task;

        if (handle.Result.Count > 0)
        {
            Debug.Log("New content available! Downloading...");
            await Addressables.UpdateCatalogs(handle.Result).Task;
            // Reload your items
        }
        else
        {
            Debug.Log("Content is up to date!");
        }
    }

    async Task InitializeLogin()
    {

        var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();

        if (dependencyStatus == DependencyStatus.Available)
        {
            FirebaseDatabase.DefaultInstance.SetPersistenceEnabled(false);

            References.Instance.firebaseManager.database = FirebaseDatabase.DefaultInstance.RootReference;
            References.Instance.firebaseManager.auth = FirebaseAuth.DefaultInstance;
            References.Instance.firebaseManager.currentUser = References.Instance.firebaseManager.auth.CurrentUser;
            References.Instance.firebaseManager.user = References.Instance.firebaseManager.auth.CurrentUser;

            Debug.Log("✓ Firebase initialized");

            FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

            StartLoading();
            /*
                        try
                        {

                            // Initialize (downloads catalog from Firebase)
                            Debug.Log("1. Initializing Addressables...");
                            var initHandle = Addressables.InitializeAsync();
                            await initHandle.Task;
                            Debug.Log("✓ Addressables initialized!");

                            // Load items
                            Debug.Log("2. Loading items from 'All Items' group...");
                            var containerHandle = Addressables.LoadAssetAsync<AllItemsSO>(allItems);
                            await containerHandle.Task;

                            if (containerHandle.Status == AsyncOperationStatus.Succeeded)
                            {
                                AllItemsSO container = containerHandle.Result;
                                Debug.Log($"✓ Container loaded! Contains {container.allItems.Count} items");

                                // Now access the list inside
                                foreach (var item in container.allItems)
                                {
                                    Debug.Log($"  - {item.nameLabel} | Icon: {(item.inventoryIcon.name != null ? "✓" : "✗")}");
                                }

                                // Use the items in your shop
                                //DisplayShopItems(container.allItems);
                            }
                            else
                            {
                                Debug.LogError($"✗ FAILED to load container!");
                                Debug.LogError($"Error: {containerHandle.OperationException}");
                            }

                            StartLoading();
                        }

                        catch (System.Exception e)
                        {
                            Debug.LogError($"✗ Exception: {e.Message}");
                            Debug.LogError($"Stack: {e.StackTrace}");
                        }
            */


        }
        else
        {
            Debug.LogWarning($"Firebase initialization failed: {dependencyStatus}");
        }
    }

    public override void Initialize()
    {
        loadingSlider.value = 0;
        percentageText.text = "Loading... 0%";
        panel.SetActive(true);

        // Initialize slider at 0
        loadingSlider.value = 0;
        percentageText.text = "Loading... 0%";

        CheckLogin();
    }

    [SerializeField] private Panel characterSelectionPanel;

    public void StartLoading()
    {
        // Animate slider from 0 to 1
        loadingSlider.DOValue(1f, duration)
            .SetEase(easeType)
            .OnUpdate(() =>
            {
                // Update percentage text
                int percentage = Mathf.RoundToInt(loadingSlider.value * 100);
                percentageText.text = "Loading... " + percentage + "%";
            }).OnComplete(async () =>
            {
                // Check auto-login
                bool rememberMe = PlayerPrefs.GetInt(References.Instance.firebaseManager.REMEMBER_ME_KEY, 0) == 1;

                if (rememberMe && References.Instance.firebaseManager.auth.CurrentUser != null)
                {
                    Debug.Log("Login with user: " + References.Instance.firebaseManager.auth.CurrentUser.UserId);

                    // Auto-login successful!

                    References.Instance.player.userName = await FirebaseSaveManager.GetUsername();

                    Bus<OnReferenceSignInFirebase>.CallEvent(new OnReferenceSignInFirebase(true));

                    Bus<TransitionPanelsEvent>.CallEvent(new TransitionPanelsEvent(this, characterSelectionPanel));
                }
                else
                {
                    // Show login screen
                    Bus<TransitionPanelsEvent>.CallEvent(new TransitionPanelsEvent(this, previousPanel));
                }


            });
    }
}



