using UnityEngine;

public class LanguageButton : MonoBehaviour
{
    [SerializeField] Language language;
    [SerializeField] CanvasGroup flag;

    public void OnButtonPressed()
    {
        References.Instance.localizationService.ChangeLanguage((int)language);
    }

    public void Initialize()
    {
        Bus<OnLanguageChanged>.OnEvent += OnLanguageChanged;
        UpdateVisuals(References.Instance.localizationService.currentLanguage);
    }

    void OnDestroy()
    {
        Bus<OnLanguageChanged>.OnEvent -= OnLanguageChanged;
    }

    private void OnLanguageChanged(OnLanguageChanged data)
    {
        UpdateVisuals(data.NewLanguage);
    }

    private void UpdateVisuals(Language currentLanguage)
    {
        flag.alpha = (currentLanguage == language)? 1f : 0.5f;
    }
}
