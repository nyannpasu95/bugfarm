using MFarm.Inventory;
using UnityEngine;

public class PlayerPositionFarming : MonoBehaviour
{
    [Header("组件引用")]
    public Grid grid;
    public InventoryManager inventoryManager;

    [Header("耕种设置")]
    public float interactionDistance = 1.5f; // 玩家可以耕种的最大距离
    public KeyCode farmKey = KeyCode.F;      // 耕种按键

    private ItemDetails currentItem;
    private Transform playerTransform;

    private void Start()
    {
        playerTransform = transform; // 假设脚本挂在玩家对象上
        Debug.Log("PlayerPositionFarming 初始化完成");
    }

    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;
    }

    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
    }

    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        if (isSelected)
        {
            currentItem = itemDetails;
            Debug.Log($"选中物品: {itemDetails.name} (ID: {itemDetails.itemID})");
        }
        else
        {
            currentItem = null;
            Debug.Log("取消选中物品");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(farmKey))
        {
            TryFarmAtPlayerPosition();
        }
    }

    /// <summary>
    /// 在玩家位置尝试耕种
    /// </summary>
    private void TryFarmAtPlayerPosition()
    {
        Debug.Log("=== 开始耕种检测 ===");

        // 检查必要条件
        if (!CheckPrerequisites()) return;

        // 获取玩家面前的网格位置
        Vector3Int targetGridPos = GetGridPositionInFrontOfPlayer();
        Debug.Log($"目标网格位置: {targetGridPos}");

        // 获取网格属性
        GridPropertyDetails gridDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(targetGridPos.x, targetGridPos.y);

        if (gridDetails == null)
        {
            Debug.LogError($"无法获取网格属性: ({targetGridPos.x}, {targetGridPos.y})");
            return;
        }

        Debug.Log($"网格属性: 可挖掘={gridDetails.isDiggable}, 已挖掘={gridDetails.daysSinceDug}, 已浇水={gridDetails.daysSinceWatered}");

        // 根据当前物品类型执行操作
        switch (currentItem.itemType)
        {
            case ItemType.Tool:
                UseTool(targetGridPos, gridDetails);
                break;
            case ItemType.Seed:
                PlantSeed(targetGridPos, gridDetails);
                break;
            default:
                Debug.Log($"物品类型 {currentItem.itemType} 不支持耕种操作");
                break;
        }

        Debug.Log("=== 耕种操作完成 ===");
    }

    /// <summary>
    /// 检查耕种前提条件
    /// </summary>
    private bool CheckPrerequisites()
    {
        if (currentItem == null)
        {
            Debug.LogWarning("请先选择工具或种子");
            return false;
        }

        if (GridPropertiesManager.Instance == null)
        {
            Debug.LogError("GridPropertiesManager 未初始化");
            return false;
        }

        if (grid == null)
        {
            Debug.LogError("Grid 组件未分配");
            return false;
        }

        return true;
    }

    /// <summary>
    /// 获取玩家面前的网格位置
    /// </summary>
    private Vector3Int GetGridPositionInFrontOfPlayer()
    {
        // 获取玩家朝向
        Vector3 facingDirection = GetPlayerFacingDirection();

        // 计算目标位置（玩家位置 + 朝向 * 交互距离）
        Vector3 targetWorldPos = playerTransform.position + facingDirection * interactionDistance;

        // 转换为网格坐标
        return grid.WorldToCell(targetWorldPos);
    }

    /// <summary>
    /// 获取玩家朝向（简化版，基于玩家旋转）
    /// </summary>
    private Vector3 GetPlayerFacingDirection()
    {
        // 假设玩家使用2D精灵，通过缩放或旋转判断朝向
        // 这里使用一个简单的方法：总是返回玩家前方
        // 你可以根据你的玩家控制系统调整这个逻辑

        float angle = playerTransform.eulerAngles.z;

        // 将角度转换为方向向量
        if (angle >= 315 || angle < 45) return Vector3.up;      // 上
        if (angle >= 45 && angle < 135) return Vector3.right;   // 右
        if (angle >= 135 && angle < 225) return Vector3.down;   // 下
        return Vector3.left;                                    // 左
    }

    /// <summary>
    /// 使用工具
    /// </summary>
    private void UseTool(Vector3Int gridPos, GridPropertyDetails gridDetails)
    {
        Debug.Log($"使用工具: {currentItem.name} (ID: {currentItem.itemID})");

        // 锄头工具 - 挖掘
        if (currentItem.itemID == 6001)
        {
            TryDigGround(gridPos, gridDetails);
        }
        // 浇水工具 - 浇水
        else if (currentItem.itemID == 5008)
        {
            TryWaterGround(gridPos, gridDetails);
        }
        else
        {
            Debug.LogWarning($"未知工具ID: {currentItem.itemID}");
        }
    }

    /// <summary>
    /// 尝试挖掘土地
    /// </summary>
    private void TryDigGround(Vector3Int pos, GridPropertyDetails details)
    {
        if (details.isDiggable && details.daysSinceDug == -1)
        {
            details.daysSinceDug = 0;
            GridPropertiesManager.Instance.SetGridPropertyDetails(pos.x, pos.y, details);
            GridPropertiesManager.Instance.DisplayDugGround(details);
            Debug.Log("✅ 成功挖掘土地");
        }
        else
        {
            Debug.LogWarning("无法挖掘：土地不可挖掘或已被挖掘");
        }
    }

    /// <summary>
    /// 尝试浇水
    /// </summary>
    private void TryWaterGround(Vector3Int pos, GridPropertyDetails details)
    {
        if (details.daysSinceDug > -1 && details.daysSinceWatered == -1)
        {
            details.daysSinceWatered = 0;
            GridPropertiesManager.Instance.SetGridPropertyDetails(pos.x, pos.y, details);
            GridPropertiesManager.Instance.DisplayWateredGround(details);
            Debug.Log("✅ 成功浇水");
        }
        else
        {
            Debug.LogWarning("无法浇水：土地未挖掘或已浇水");
        }
    }

    /// <summary>
    /// 种植种子
    /// </summary>
    private void PlantSeed(Vector3Int pos, GridPropertyDetails details)
    {
        if (details.daysSinceDug > -1 && details.seedItemCode == -1)
        {
            details.seedItemCode = currentItem.itemID;
            details.growthDays = 0;
            GridPropertiesManager.Instance.SetGridPropertyDetails(pos.x, pos.y, details);
            GridPropertiesManager.Instance.DisplayPlantedCrop(details);

            // 从背包移除种子
            inventoryManager.RemoveItem(currentItem.itemID, 1);
            Debug.Log($"✅ 成功种植 {currentItem.name}");
        }
        else
        {
            Debug.LogWarning("无法种植：土地未挖掘或已种植");
        }
    }

    /// <summary>
    /// 可视化调试：在Scene视图中显示交互范围
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (playerTransform == null) return;

        // 绘制玩家位置
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(playerTransform.position, 0.2f);

        // 绘制交互范围
        Gizmos.color = Color.yellow;
        Vector3[] directions = { Vector3.up, Vector3.right, Vector3.down, Vector3.left };

        foreach (Vector3 dir in directions)
        {
            Vector3 targetPos = playerTransform.position + dir * interactionDistance;
            Gizmos.DrawLine(playerTransform.position, targetPos);
            Gizmos.DrawWireCube(targetPos, Vector3.one * 0.3f);
        }
    }

    /// <summary>
    /// 调试命令：强制耕种当前位置
    /// </summary>
    [ContextMenu("强制耕种当前位置")]
    public void ForceFarmCurrentPosition()
    {
        Debug.Log("🚨 强制耕种当前位置");

        if (grid == null) return;

        Vector3Int gridPos = grid.WorldToCell(transform.position);
        GridPropertyDetails details = GridPropertiesManager.Instance.GetGridPropertyDetails(gridPos.x, gridPos.y);

        if (details != null)
        {
            Debug.Log($"强制挖掘位置: ({gridPos.x}, {gridPos.y})");
            details.daysSinceDug = 0;
            GridPropertiesManager.Instance.SetGridPropertyDetails(gridPos.x, gridPos.y, details);
            GridPropertiesManager.Instance.DisplayDugGround(details);
        }
    }
}