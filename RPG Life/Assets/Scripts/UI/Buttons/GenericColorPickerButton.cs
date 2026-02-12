using UnityEngine;
using UnityEngine.UI;

public class GenericColorPickerButton : MonoBehaviour
{
    [HideInInspector] public Image colorImage;
    [HideInInspector] public int colorIndex;
    private Button button;

    public virtual void Initialize(Color color, int colorIndex)
    {

        if (button == null)
            button = GetComponent<Button>();

        if (colorImage == null)
            colorImage = GetComponent<Image>();

        if (button != null)
        {
            button.onClick.AddListener(OnButtonPressed);
        }

        colorImage.color = color;
        this.colorIndex = colorIndex;
    }

    public virtual void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(OnButtonPressed);
    }

    public virtual void OnButtonPressed()
    {
        // Implementation
    }
}


