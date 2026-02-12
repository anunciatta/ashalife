using TMPro;
using UnityEngine;

public class LanguageTextModifier : MonoBehaviour
{
    [SerializeField] private string label;
    [SerializeField] private TextMeshProUGUI text;

    public void UpdateText()
    {
        //if (currentLanguage == language) return;

        if (text == null)
        {
            text = GetComponent<TextMeshProUGUI>();

            if (text == null)
            {
                Debug.LogError("TextMeshProUGUI component not found on " + gameObject.name);
                return;
            }
        }

        text.text = References.Instance.localizationService.GetLocalizationText(label);
    }

    public string Label => label;
}
