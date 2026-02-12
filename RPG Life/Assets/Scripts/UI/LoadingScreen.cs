using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;

public class LoadingScreen : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private Image progressBarFill;
    [SerializeField] private TextMeshProUGUI percentageText;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Animation Settings")]
    [SerializeField] private float fadeInDuration = 0.3f;
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private Ease progressEase = Ease.OutQuad;

    [Header("Loading Messages")]
    [SerializeField]
    private string[] loadingMessages = new string[]
    {
        "Loading your items...",
        "Preparing shop...",
        "Almost there...",
        "Getting ready..."
    };

    private void Awake()
    {
        // Start with loading panel hidden
        if (canvasGroup == null)
            canvasGroup = loadingPanel.GetComponent<CanvasGroup>();

        loadingPanel.SetActive(false);
        progressBarFill.fillAmount = 0f;
    }

    public async void LoadItemsWithProgress()
    {
        await ShowLoadingScreen();

        try
        {
            // Initialize Addressables
            SetLoadingMessage(loadingMessages[0]);
            var initHandle = Addressables.InitializeAsync();
            await UpdateProgress(initHandle, 0f, 0.3f); // 0% to 30%

            // Load AllItemsSO
            SetLoadingMessage(loadingMessages[1]);
            var containerHandle = Addressables.LoadAssetAsync<AllItemsSO>("AllItemsSO");
            await UpdateProgress(containerHandle, 0.3f, 1f); // 30% to 100%

            if (containerHandle.Status == AsyncOperationStatus.Succeeded)
            {
                AllItemsSO container = containerHandle.Result;
                Debug.Log($"✓ Loaded {container.allItems.Count} items!");

                // Notify shop manager or whoever needs the items
                OnItemsLoaded(container.allItems);
            }
            else
            {
                Debug.LogError("Failed to load items!");
                OnLoadingFailed();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Loading error: {e.Message}");
            OnLoadingFailed();
        }
        finally
        {
            await HideLoadingScreen();
        }
    }

    private async System.Threading.Tasks.Task UpdateProgress<T>(
        AsyncOperationHandle<T> handle,
        float startProgress,
        float endProgress)
    {
        float currentProgress = startProgress;

        while (!handle.IsDone)
        {
            float targetProgress = Mathf.Lerp(startProgress, endProgress, handle.PercentComplete);
            currentProgress = Mathf.MoveTowards(currentProgress, targetProgress, Time.deltaTime * 0.5f);

            // Animate progress bar
            progressBarFill.DOFillAmount(currentProgress, 0.2f).SetEase(progressEase);

            // Update percentage text with DOTween
            DOTween.To(() => percentageText.text,
                x => percentageText.text = x,
                $"{Mathf.RoundToInt(currentProgress * 100)}%",
                0.2f);

            await System.Threading.Tasks.Task.Yield();
        }

        // Ensure we reach the end progress
        progressBarFill.DOFillAmount(endProgress, 0.3f).SetEase(progressEase);
        percentageText.text = $"{Mathf.RoundToInt(endProgress * 100)}%";
    }

    private async System.Threading.Tasks.Task ShowLoadingScreen()
    {
        loadingPanel.SetActive(true);
        canvasGroup.alpha = 0f;
        progressBarFill.fillAmount = 0f;
        percentageText.text = "0%";

        // Fade in
        await canvasGroup.DOFade(1f, fadeInDuration).SetEase(Ease.OutQuad).AsyncWaitForCompletion();

        // Pulse loading text
        loadingText.transform.DOScale(1.1f, 0.8f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    private async System.Threading.Tasks.Task HideLoadingScreen()
    {
        // Kill loading text animation
        loadingText.transform.DOKill();
        loadingText.transform.localScale = Vector3.one;

        // Fade out
        await canvasGroup.DOFade(0f, fadeOutDuration).SetEase(Ease.InQuad).AsyncWaitForCompletion();

        loadingPanel.SetActive(false);
    }

    private void SetLoadingMessage(string message)
    {
        loadingText.text = message;

        // Optional: Animate text change
        loadingText.transform.DOPunchScale(Vector3.one * 0.1f, 0.3f, 1, 0.5f);
    }

    // Events to connect to your shop system
    private void OnItemsLoaded(List<ItemSO> items)
    {
        // Send loaded items to ShopManager or wherever you need them
        //FindObjectOfType<ShopManager>()?.DisplayItems(items);
    }

    private void OnLoadingFailed()
    {
        loadingText.text = "Failed to load items. Please check your connection.";
        loadingText.color = Color.red;
    }
}