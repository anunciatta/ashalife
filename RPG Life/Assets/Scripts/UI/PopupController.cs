using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupController : MonoBehaviour
{
   [SerializeField] private Popup popup;
   [SerializeField] private float popupTime = 2.5f;
   [Header("[0]Experience [1]Coins [2] Health [3]Gem [4]System [5]NotEnoughToBuy")]
   [SerializeField] private Sprite[] popupTypeIcons;

   [Header("[0]Neutral [1]Positive [2] Negative")]
   [SerializeField] private Color[] popupColors;
   [SerializeField] private Color[] textColors;

   private Coroutine displayPopupsCoroutine;
   private Queue<PopupConfig> messagesToShow = new Queue<PopupConfig>();


   void Awake()
   {
      Bus<OnAddToast>.OnEvent += AddPopup;
   }

   void OnDestroy()
   {
      Bus<OnAddToast>.OnEvent -= AddPopup;
      CancelAllPopups();
   }

   public void AddPopup(OnAddToast data)
   {
      messagesToShow.Enqueue(data.PopupConfig);

      if (displayPopupsCoroutine == null)
      {
         displayPopupsCoroutine = StartCoroutine(DisplayPopups());
      }
   }

   public IEnumerator DisplayPopups()
   {
      while (messagesToShow.Count > 0)
      {
         var popupData = messagesToShow.Dequeue();
         popup.Initialize(popupData.message, popupColors[(int)popupData.status], popupTypeIcons[(int)popupData.popupType], textColors[(int)popupData.status]);
         yield return new WaitForSecondsRealtime(popupTime);
      }

      displayPopupsCoroutine = null;
   }

   public void CancelAllPopups()
   {
      if (displayPopupsCoroutine != null)
      {
         StopCoroutine(displayPopupsCoroutine);
         displayPopupsCoroutine = null;
      }

      messagesToShow.Clear();
   }
}

public struct PopupConfig
{
   public MarkStatus status;
   public string message;
   public PopupType popupType;

   public PopupConfig(MarkStatus status, string message, PopupType popupType)
   {
      this.status = status;
      this.message = message;
      this.popupType = popupType;
   }
}

public enum PopupType
{
   Experience = 0, Coins = 1, Health = 2, Gem = 3, System = 4, NotEnoughToBuy = 5
}

public enum MarkStatus
{
   Neutral = 0, Positive = 1, Negative = 2
}


