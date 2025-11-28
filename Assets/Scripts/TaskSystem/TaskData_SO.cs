using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TaskData_SO", menuName = "Date/Task")]
public class TaskData_SO : ScriptableObject
{
    public List<TaskDate> TaskDateList;
}

public enum WorldRuleType
{
    None,           // No rule
    BonusDrop,      // Bonus harvest drop
    DoubleHarvest,  // Double harvest
    UnlockNewItem,  // Unlock new items
    AddAnimalSpawn, // Add animal spawns
    SpecialRule     // Special scene event
}

[System.Serializable]
public class TaskDate
{
    [Header("Stage Number (1-5)")]
    public int stage;

    [Header("Task Description")]
    [TextArea(2, 4)]
    public string description;

    [Header("Show UI Submit Button")]
    public bool hasUI = true;

    [Header("Required Items (only when hasUI = true)")]
    public List<ItemRequirement> requirements;

    [Header("Character State (Animator State Name)")]
    public string animatorStateName;

    [Header("World Rule Triggered After Task Completion")]
    public WorldRuleType worldRuleType;

    [Header("Task Completion Rewards (can be empty)")]
    public List<ItemRequirement> rewards;
}


[System.Serializable]
public class ItemRequirement
{
    [Header("Item ID (matches inventory system)")]
    public int itemID;
    [Header("Required Amount")]
    public int amount;
}
