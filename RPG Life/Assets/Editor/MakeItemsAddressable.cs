#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;

public class MakeItemsAddressable : Editor
{
    // [MenuItem("Tools/Make All Items Addressable")]
    // static void MakeAllItemsAddressable()
    // {
    //     var settings = AddressableAssetSettingsDefaultObject.Settings;

    //     if (settings == null)
    //     {
    //         Debug.LogError("Addressables not set up! Go to Window > Asset Management > Addressables > Groups");
    //         return;
    //     }

    //     // Find or create "Shop Items" group
    //     var group = settings.FindGroup("All Items");
    //     if (group == null)
    //     {
    //         group = settings.CreateGroup("All Items", false, false, true, null);
    //     }

    //     // Find all ItemSO assets
    //     string[] guids = AssetDatabase.FindAssets("t:ItemSO");
    //     int count = 0;

    //     foreach (string guid in guids)
    //     {
    //         string path = AssetDatabase.GUIDToAssetPath(guid);
    //         var entry = settings.RemoveAssetEntry(guid, group);
    //         //entry.address = path.Replace("Assets/", "").Replace(".asset", "");
    //         count++;
    //     }

    //     AssetDatabase.SaveAssets();
    //     Debug.Log($"Made {count} items addressable!");
    // }
}
#endif