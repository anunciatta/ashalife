using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Items List", menuName = "ScriptableObjects/Items List")]
public class ItemsListSO : ScriptableObject
{
    public List<ItemSO> items = new();

    public ItemSO GetItemFromID(string saveableEntityId)
    {
        return items.FirstOrDefault(item => item.saveableEntityId == saveableEntityId);
    }

    #region Helpers

#if UNITY_EDITOR
    [ContextMenu("GenerateItemsID")]
    private void GenerateItemsID()
    {
        foreach (ItemSO item in items)
        {
            item.GenereteID();
            EditorUtility.SetDirty(item);
        }

        AssetDatabase.SaveAssets();

    }

    [ContextMenu("FindAndAddAllItems")]
    private void FindAndAddAllItems()
    {
        if (items == null)
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

            if (item != null && !items.Contains(item))
            {
                items.Add(item);
                addedCount++;
            }
        }

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();

        EditorUtility.DisplayDialog("Complete",
            $"Added {addedCount} new item(s).\nTotal items: {items.Count}",
            "OK");
    }
#endif
    #endregion
}
