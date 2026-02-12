using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "ScriptableObjects/Quests/Quest", order = 1)]
public class QuestSO : ScriptableObject
{
    [ContextMenu("Generate ID")]
    public void GenereteID() => saveableEntityId = Guid.NewGuid().ToString();
    public string saveableEntityId;

    public string nameLabel;
    public string descriptionLabel;
    public Sprite icon;
    public int cost;
    public Difficulty difficulty;

    public List<StatusOutcome> successStatusOutcomes = new();
    public List<StatusOutcome> failureStatusOutcomes = new();

    public List<ItemOutcome> successItemOutcomes = new();
    public List<ItemOutcome> failureItemOutcomes = new();

    public List<QuestCondition> conditions = new();

    public List<Status> testableStatus = new();

    public bool locked;

    public List<QuestCondition> unlockRequirements;
}
