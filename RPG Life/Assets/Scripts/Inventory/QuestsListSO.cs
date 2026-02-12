using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Quests List", menuName = "ScriptableObjects/Quests List")]
public class QuestsListSO : ScriptableObject
{
    public List<QuestSO> quests = new();

    public QuestSO GetItemFromID(string saveableEntityId)
    {
        return quests.FirstOrDefault(item => item.saveableEntityId == saveableEntityId);
    }

#if UNITY_EDITOR
    [ContextMenu("GenerateItemsID")]
    private void GenerateItemsID()
    {
        foreach (QuestSO item in quests)
        {
            item.GenereteID();
            EditorUtility.SetDirty(item);
        }

        AssetDatabase.SaveAssets();

    }
#endif
}