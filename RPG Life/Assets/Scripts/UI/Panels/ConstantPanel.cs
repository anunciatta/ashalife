using DG.Tweening;
using TMPro;
using UnityEngine;

public class ConstantPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI username;

    //Coin
    [SerializeField] private TextMeshProUGUI coinValue;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color errorColor = new Color(1f, 0.3f, 0.3f);

    private Sequence notEnoughCoinsSequence;
    private Vector3 originalPosition;

    //Gem
    [SerializeField] private TextMeshProUGUI gemValue;

    void Awake()
    {
        coinValue.color = normalColor;

        Bus<OnHasNotEnoughCurrencyEvent>.OnEvent += OnHasNotEnoughCoins;
        Bus<OnCharacterLoad>.OnEvent += OnCharacterLoad;
        Bus<ChangeGemsEvent>.OnEvent += OnChangeGems;
        Bus<ChangeCoinsEvent>.OnEvent += OnChangeCoins;
        Bus<OnLevelDown>.OnEvent += OnLevelDown;
        Bus<OnLevelUp>.OnEvent += OnLevelUp;
    }


    private void OnEnable()
    {
        originalPosition = coinValue.rectTransform.anchoredPosition;
    }

    void OnDestroy()
    {
        Bus<OnHasNotEnoughCurrencyEvent>.OnEvent -= OnHasNotEnoughCoins;
        Bus<OnCharacterLoad>.OnEvent -= OnCharacterLoad;
        Bus<ChangeGemsEvent>.OnEvent -= OnChangeGems;
        Bus<ChangeCoinsEvent>.OnEvent -= OnChangeCoins;
        Bus<OnLevelDown>.OnEvent -= OnLevelDown;
        Bus<OnLevelUp>.OnEvent -= OnLevelUp;
    }

    private void OnLevelDown(OnLevelDown data)
    {
        username.text = $"@{References.Instance.player.userName}  - Level {data.Character.level} {References.Instance.localizationService.GetLocalizedClassesDescription((ClassType)data.Character.avatarConfig.classesIndex)}";
    }

    private void OnLevelUp(OnLevelUp data)
    {
        username.text = $"@{References.Instance.player.userName}  - Level {data.Character.level} {References.Instance.localizationService.GetLocalizedClassesDescription((ClassType)data.Character.avatarConfig.classesIndex)}";
    }

    private void OnChangeGems(ChangeGemsEvent data)
    {
        gemValue.text = data.NewValue.ToString();
    }

    private void OnChangeCoins(ChangeCoinsEvent data)
    {
        coinValue.text = $"{data.NewValue}";
    }

    private void OnCharacterLoad(OnCharacterLoad data)
    {
        username.text = $"@{References.Instance.player.userName}  - Level {data.Character.level} {References.Instance.localizationService.GetLocalizedClassesDescription((ClassType)data.Character.avatarConfig.classesIndex)}";

        if (data.Character.inventory.TryGetValue(
        References.Instance.inventory.coin.saveableEntityId,
        out int coins))
        {
            coinValue.text = $"{coins}";
        }
        else
        {
            coinValue.text = "0";
        }

        if (data.Character.inventory.TryGetValue(
     References.Instance.inventory.gem.saveableEntityId,
     out int gems))
        {
            gemValue.text = gems.ToString();
        }
        else
        {
            gemValue.text = "0";
        }
    }



    private void OnHasNotEnoughCoins(OnHasNotEnoughCurrencyEvent data)
    {

        Bus<OnAddToast>.CallEvent(new OnAddToast(new PopupConfig(MarkStatus.Negative, References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.notEnoughToBuy}"), PopupType.NotEnoughToBuy)));

        // Kill previous animation (if any)
        notEnoughCoinsSequence?.Kill();

        // Reset to base state
        coinValue.rectTransform.anchoredPosition = originalPosition;
        coinValue.color = normalColor;

        notEnoughCoinsSequence = DOTween.Sequence();

        notEnoughCoinsSequence
            // Shake / punch horizontally
            .Append(
                coinValue.rectTransform
                    .DOPunchAnchorPos(new Vector3(-5f, 0f, 0f), 0.25f, 12, 0.8f)
            )

            // Flash red
            .Join(
                coinValue.DOColor(errorColor, 0.1f)
            )

            // Return color smoothly
            .Append(
                coinValue.DOColor(normalColor, 0.2f)
            )

            // Safety cleanup
            .OnKill(() =>
            {
                coinValue.rectTransform.anchoredPosition = originalPosition;
                coinValue.color = normalColor;
            });
    }
}

