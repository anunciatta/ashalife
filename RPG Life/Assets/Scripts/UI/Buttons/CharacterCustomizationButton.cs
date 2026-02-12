using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCustomizationButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private GameObject focusedIndicator;
    [Header("[0] Defautlt, [1] Selected")]
    [SerializeField] private Color[] colors;

    [SerializeField] private CharacterPartType characterPartType;

    private Button button;
    private bool isSelected = false;
    private CharacterCustomizer characterCustomizer;
    private GameObject colorPickerPanel;

    public void Initialize(CharacterCustomizer characterCustomizer, bool isSelected, GameObject colorPickerPanel)
    {
        if (this.characterCustomizer == null)
        {
            this.characterCustomizer = characterCustomizer;
        }

        if (colorPickerPanel == null)
        {
            this.colorPickerPanel = null;
        }

        else
        {
            if (this.colorPickerPanel == null)
            {
                this.colorPickerPanel = colorPickerPanel;
            }
        }

        if (button == null)
        {
            button = GetComponent<Button>();
            if (button != null)
                button.onClick.AddListener(OnButtonPressed);
        }

        this.isSelected = isSelected;
        UpdateVisuals(isSelected);

        focusedIndicator.GetComponent<Image>().color = new Color(0, 0, 0, 0);

    }

    void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(OnButtonPressed);
    }

    private void OnButtonPressed()
    {
        if (isSelected)
            return;

        foreach (var btn in characterCustomizer.customizationButtons)
        {
            if (btn != this)
                btn.OnDeselect();
        }

        OnSelect();
    }

    public void OnSelect()
    {
        isSelected = true;
        UpdateVisuals(isSelected);

        if (colorPickerPanel != null)
        {
            colorPickerPanel.SetActive(true);
        }

        characterCustomizer.currentPartType = characterPartType;
    }

    public void OnDeselect()
    {
        isSelected = false;

        if (colorPickerPanel != null)
        {
            colorPickerPanel.SetActive(false);
        }

        UpdateVisuals(isSelected);
    }

    private void UpdateVisuals(bool isSelected)
    {
        if (label != null)
        {
            label.color = isSelected ? colors[1] : colors[0];
        }

        if (focusedIndicator != null)
        {
            focusedIndicator.SetActive(isSelected);
        }
    }

}
