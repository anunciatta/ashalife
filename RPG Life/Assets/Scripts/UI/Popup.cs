using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
   [SerializeField] private TextMeshProUGUI message;
   [SerializeField] private Image background;
   [SerializeField] private Image iconBackground;
   [SerializeField] private Image icon;
   [SerializeField] private RectTransform rectTransform;

   private CanvasGroup canvasGroup;

   private const float START_Y = 0f;
   private const float TARGET_Y = 325f;

   private const float MOVE_TIME = 0.15f;
   private const float HOLD_TIME = 1.5f;

   private void Awake()
   {
      canvasGroup = GetComponent<CanvasGroup>();
   }

   void Start()
   {
      CancelAnimation();
   }

   public void Initialize(string message, Color backgroundColor, Sprite iconSprite, Color textColor)
   {
      this.message.text = message;
      iconBackground.color = backgroundColor;
      background.color = backgroundColor;
      icon.sprite = iconSprite;
      this.message.color = textColor;

      PlayAnimation();
   }

   public void PlayAnimation()
   {
      CancelAnimation();

      canvasGroup.interactable = false;
      canvasGroup.blocksRaycasts = false;

      // Move up + fade in
      LeanTween.moveY(rectTransform, TARGET_Y, MOVE_TIME)
          .setEaseOutCubic();

      LeanTween.alphaCanvas(canvasGroup, 1f, MOVE_TIME * 0.7f);

      // Hold → move down + fade out
      LeanTween.delayedCall(MOVE_TIME + HOLD_TIME, () =>
      {
         LeanTween.moveY(rectTransform, START_Y, MOVE_TIME)
           .setEaseInCubic();

         LeanTween.alphaCanvas(canvasGroup, 0f, MOVE_TIME)
           .setEaseInCubic();
      });
   }

   public void CancelAnimation()
   {
      LeanTween.cancel(rectTransform);
      LeanTween.cancel(gameObject);

      canvasGroup.alpha = 0f;

      rectTransform.anchoredPosition =
          new Vector2(rectTransform.anchoredPosition.x, START_Y);
   }
}


