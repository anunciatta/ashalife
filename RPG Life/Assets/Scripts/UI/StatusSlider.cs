using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusSlider : MonoBehaviour
{
   public Status status;

   [SerializeField] private Slider slider;
   [SerializeField] private TextMeshProUGUI text;

   public void SetMaxValue(int value)
   {
      slider.maxValue = value;
      text.text = $"{slider.value}/{slider.maxValue}";
   }

   public void SetCurrentValue(int value)
   {
      slider.value = value;
      text.text = $"{slider.value}/{slider.maxValue}";
   }
}


