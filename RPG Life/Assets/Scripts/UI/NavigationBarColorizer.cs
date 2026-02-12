using System.Collections;
using UnityEngine;

public class NavigationBarColorizer : MonoBehaviour
{

    void Start()
    {
        StartCoroutine(SetWhiteNavigationBarDelayed());
    }

    IEnumerator SetWhiteNavigationBarDelayed()
    {
        yield return new WaitForEndOfFrame();
        SetWhiteNavigationBar();
    }

    void SetWhiteNavigationBar()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        ApplicationChrome.statusBarState = ApplicationChrome.States.VisibleOverContent;
        ApplicationChrome.statusBarColor = ApplicationChrome.navigationBarColor = 0xFF000000;
#endif
    }

}