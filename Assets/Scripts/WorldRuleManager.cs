using System.Collections.Generic;
using UnityEngine;

public class WorldRuleManager : MonoBehaviour
{
    public static WorldRuleManager Instance;

    private HashSet<WorldRuleType> activeRules = new HashSet<WorldRuleType>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ApplyRule(WorldRuleType ruleType)
    {
        if (activeRules.Contains(ruleType)) return;
        activeRules.Add(ruleType);

        switch (ruleType)
        {
            case WorldRuleType.None:
                Debug.Log("No rule");
                break;
            case WorldRuleType.BonusDrop:
                Debug.Log("Applied rule: Bonus harvest drop");
                break;
            case WorldRuleType.DoubleHarvest:
                Debug.Log("Applied rule: Double harvest");
                break;
            case WorldRuleType.UnlockNewItem:
                Debug.Log("Applied rule: Unlock new items");
                break;
            case WorldRuleType.AddAnimalSpawn:
                Debug.Log("Applied rule: Add animal spawns");
                break;
            case WorldRuleType.SpecialRule:
                Debug.Log("Applied rule: Special scene event");
                break;
        }
    }
}
