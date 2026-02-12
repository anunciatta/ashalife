using UnityEngine;
using UnityEngine.UI;

public class ChangeableOnClassSelection : MonoBehaviour
{
    [SerializeField] private Sprite[] iconSprites;
    [SerializeField] Image icon;

    public void ChangeIcon(ClassSO classSO)
    {
        icon.sprite = iconSprites[(int)classSO.classType];
    }
}


