using UnityEngine;

public class FarmingTester : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) // 按T键测试
        {
            TestFarmingSystem();
        }
    }

    private void TestFarmingSystem()
    {
        Debug.Log("=== 开始耕种系统测试 ===");

        // 测试1: 检查GridPropertiesManager
        if (GridPropertiesManager.Instance == null)
        {
            Debug.LogError("❌ GridPropertiesManager实例不存在");
            return;
        }
        Debug.Log("✅ GridPropertiesManager实例存在");

        // 测试2: 检查网格字典
        if (GridPropertiesManager.Instance.gridPropertyDictionary == null)
        {
            Debug.LogError("❌ 网格属性字典为null");
            return;
        }
        Debug.Log($"✅ 网格属性字典已初始化，包含 {GridPropertiesManager.Instance.gridPropertyDictionary.Count} 个条目");

        // 测试3: 测试特定坐标
        TestGridCoordinate(1, 1);
        TestGridCoordinate(4, 5);

        Debug.Log("=== 测试完成 ===");
    }

    private void TestGridCoordinate(int x, int y)
    {
        Debug.Log($"测试坐标: ({x}, {y})");

        var details = GridPropertiesManager.Instance.GetGridPropertyDetails(x, y);
        if (details == null)
        {
            Debug.LogWarning($"❌ 坐标 ({x}, {y}) 没有网格属性");
            return;
        }

        Debug.Log($"✅ 坐标 ({x}, {y}) 信息:");
        Debug.Log($"  - 可挖掘: {details.isDiggable}");
        Debug.Log($"  - 可放置物品: {details.canDropItem}");
        Debug.Log($"  - 种子代码: {details.seedItemCode}");
        Debug.Log($"  - 生长天数: {details.growthDays}");
    }
}