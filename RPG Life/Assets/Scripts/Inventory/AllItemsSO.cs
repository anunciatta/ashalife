using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "All Items", menuName = "ScriptableObjects/AllItems")]
public class AllItemsSO : ScriptableObject
{
    public List<ItemSO> allItems = new();

    #region Helpers

#if UNITY_EDITOR
    [ContextMenu("GenerateItemsID")]
    private void GenerateItemsID()
    {
        foreach (ItemSO item in allItems)
        {
            item.GenereteID();
            item.deletable = true;

            if (item.attributes == null || item.attributes.Count == 0)
                item.SetDefaultAttributes();

            EditorUtility.SetDirty(item);
        }

        AssetDatabase.SaveAssets();

    }

    [ContextMenu("FindAndAddAllItems")]
    private void FindAndAddAllItems()
    {
        if (allItems == null)
        {
            EditorUtility.DisplayDialog("Error", "Please select an AllItems ScriptableObject", "OK");
            return;
        }

        string allItemsPath = AssetDatabase.GetAssetPath(this);
        string folderPath = System.IO.Path.GetDirectoryName(allItemsPath);

        string[] guids = AssetDatabase.FindAssets("t:ItemSO", new[] { folderPath });

        int addedCount = 0;

        Undo.RecordObject(this, "Add Items to AllItems");

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            ItemSO item = AssetDatabase.LoadAssetAtPath<ItemSO>(assetPath);

            if (item != null && !allItems.Contains(item))
            {
                allItems.Add(item);
                addedCount++;
            }
        }

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();

        EditorUtility.DisplayDialog("Complete",
            $"Added {addedCount} new item(s).\nTotal items: {allItems.Count}",
            "OK");
    }
#endif
    #endregion
}
