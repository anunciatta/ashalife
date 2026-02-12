using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationPanel : Panel
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    public override void Open()
    {
        base.Open();
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, 0.25f);
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public override void Close()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        base.Close();
    }

    public void SetMessage(string title, string message)
    {
        titleText.text = title;
        messageText.text = message;
    }

    public override void Initialize()
    {
        base.Initialize();
        canvasGroup.alpha = 0f;
        Bus<OnConfirmationPanelEvent>.OnEvent += OnConfirmationPanel;

        cancelButton.onClick.AddListener(() =>
        {
            Close();
        });

        Bus<OnRemoveHabitOption>.OnEvent += (data) =>
        {
            SetMessage(References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.confirmDeleteHabitTitle}"), References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.confirmDeleteHabit}"));
            Open();
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(async () =>
            {
                try
                {
                    confirmButton.enabled = false;
                    bool success = await FirebaseSaveManager.RemoveHabit(data.HabitContent.id);

                    if (success)
                    {
                        Bus<OnRemoveHabitConfirm>.CallEvent(new OnRemoveHabitConfirm(data.HabitContent));
                        Close();
                    }
                    else
                    {
                        Bus<OnAddToast>.CallEvent(new OnAddToast(new PopupConfig(MarkStatus.Neutral, References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.somethingWrong}"), PopupType.System)));
                        Close();
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Error removing habit'': {e.Message}");
                    return;
                }

                finally
                {
                    confirmButton.interactable = true;
                }
            });
        };

        Bus<OnRemoveDailyOption>.OnEvent += (data) =>
        {
            SetMessage(References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.confirmDeleteDailyTitle}"), References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.confirmDeleteDaily}"));
            Open();
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(async () =>
            {
                try
                {
                    confirmButton.enabled = false;
                    bool success = await FirebaseSaveManager.RemoveDaily(data.DailyContent.id);

                    if (success)
                    {
                        Bus<OnRemoveDailyConfirm>.CallEvent(new OnRemoveDailyConfirm(data.DailyContent));
                        Close();
                    }
                    else
                    {
                        Bus<OnAddToast>.CallEvent(new OnAddToast(new PopupConfig(MarkStatus.Neutral, References.Instance.localizationService.GetLocalizationText($"{LocalizationLabels.somethingWrong}"), PopupType.System)));
                        Close();
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Error removing daily: {e.Message}");
                    return;
                }

                finally
                {
                    confirmButton.interactable = true;
                }
            });
        };
    }

    private void OnConfirmationPanel(OnConfirmationPanelEvent data)
    {
        SetMessage(data.Title, data.Message);
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() =>
        {
            data.ConfirmAction?.Invoke();
            Close();
        });
    }
}



