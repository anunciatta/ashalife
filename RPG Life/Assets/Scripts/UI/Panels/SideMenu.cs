using System.Linq;
using UnityEngine;
using DG.Tweening;

public class SideMenu : Panel
{
    [Header("Menu References")]
    [SerializeField] private RectTransform buttonsContainer; // The sliding panel
    [SerializeField] private CanvasGroup background; // Background with CanvasGroup

    [Header("Animation Settings")]
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private Ease slideEase = Ease.OutCubic;
    [SerializeField] private float hiddenOffsetX = 400f; // Width of the panel (off-screen)
    [SerializeField] private float visibleOffsetX = 0f; // Final position (on-screen)

    private LanguageButton[] languageButtons;

    private Sequence menuSequence;
    private bool isOpen = false;
    private bool isAnimating = false;

    public override void Initialize()
    {
        base.Initialize();

        isAnimating = false;
        isOpen = false;

        background.alpha = 0f;
        buttonsContainer.anchoredPosition = new Vector2(hiddenOffsetX, buttonsContainer.anchoredPosition.y);
        languageContainer.SetActive(false);

        languageButtons = panel.GetComponentsInChildren<LanguageButton>(true);

        foreach (var button in languageButtons)
        {
            button.Initialize();
        }
    }

    [SerializeField] private GameObject languageContainer;

    public void ToggleLanguageContainer()
    {
        languageContainer.SetActive(!languageContainer.activeSelf);
        Canvas.ForceUpdateCanvases();
    }

    public override void Open()
    {

        base.Open();
    }


    public void ToggleMenu()
    {
        if (isAnimating) return; // Prevent multiple clicks

        if (isOpen)
            CloseMenu();
        else
            OpenMenu();
    }

    private void OpenMenu()
    {
        if (isAnimating) return;

        isAnimating = true;
        isOpen = true;

        // Kill any existing sequence
        menuSequence?.Kill();

        // Set initial states
        panel.SetActive(true);
        buttonsContainer.anchoredPosition = new Vector2(hiddenOffsetX, buttonsContainer.anchoredPosition.y);
        background.alpha = 0f;

        // Create sequence
        menuSequence = DOTween.Sequence();

        menuSequence.Append(buttonsContainer.DOAnchorPosX(visibleOffsetX, animationDuration).SetEase(slideEase));
        menuSequence.Join(background.DOFade(1f, animationDuration));
        menuSequence.OnComplete(() => isAnimating = false);
    }

    private void CloseMenu()
    {
        if (isAnimating) return;

        isAnimating = true;
        isOpen = false;

        // Kill any existing sequence
        menuSequence?.Kill();

        // Create sequence
        menuSequence = DOTween.Sequence();

        menuSequence.Append(buttonsContainer.DOAnchorPosX(hiddenOffsetX, animationDuration).SetEase(slideEase));
        menuSequence.Join(background.DOFade(0f, animationDuration));
        menuSequence.OnComplete(() =>
        {
            panel.SetActive(false);
            languageContainer.SetActive(false);
            isAnimating = false;
        });
    }

    private void OnDestroy()
    {
        // Clean up sequence when object is destroyed
        menuSequence?.Kill();
    }
}



