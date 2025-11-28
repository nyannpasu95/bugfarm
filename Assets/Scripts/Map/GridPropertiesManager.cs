using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridPropertiesManager : SingletonMonobehaviour<GridPropertiesManager>
{
    [Header("网格范围设置")]
    public int startX = -15;  // 左下角X
    public int startY = -20;  // 左下角Y  
    public int width = 40;    // 长度
    public int height = 34;   // 宽度

    [Header("组件引用")]
    public Transform cropParentTransform;
    public Tilemap groundDecoration1;
    public Tilemap groundDecoration2;
    public Grid grid;

    [Header("地块图片")]
    public Tile dugTile;      // 挖掘后的地块
    public Tile wateredTile;  // 浇水后的地块

    [Header("作物配置")]
    public SO_CropDetailsList so_CropDetailsList;

    [HideInInspector]
    public Dictionary<string, GridPropertyDetails> gridPropertyDictionary;

    private bool isInitialized = false;

    protected override void Awake()
    {
        base.Awake();
        Debug.Log("GridPropertiesManager Awake完成");
    }

    private void OnEnable()
    {
        EventHandler.AdvanceGameDayEvent += AdvanceDay;
    }

    private void OnDisable()
    {
        EventHandler.AdvanceGameDayEvent -= AdvanceDay;
    }

    private void Start()
    {
        Debug.Log("GridPropertiesManager Start开始");

        // 确保只初始化一次
        if (!isInitialized)
        {
            InitializeGridProperties();
            isInitialized = true;
        }

        Debug.Log($"✅ GridPropertiesManager 初始化完成，网格数: {gridPropertyDictionary?.Count}");
    }

    /// <summary>
    /// 初始化网格属性 - 根据固定范围创建
    /// </summary>
    private void InitializeGridProperties()
    {
        Debug.Log($"开始初始化网格属性: 范围({startX},{startY}) 到 ({startX + width - 1},{startY + height - 1})");

        // 创建字典
        gridPropertyDictionary = new Dictionary<string, GridPropertyDetails>();

        // 创建指定范围内的所有网格
        for (int x = startX; x < startX + width; x++)
        {
            for (int y = startY; y < startY + height; y++)
            {
                string key = $"x{x}y{y}";

                GridPropertyDetails details = new GridPropertyDetails
                {
                    gridX = x,
                    gridY = y,
                    isDiggable = true,        // 默认都可挖掘
                    canDropItem = true,       // 默认都可放置物品
                    canPlaceFurniture = false,// 默认不可放置家具
                    isPath = false,           // 默认不是路径
                    isNPCObstacle = false,    // 默认不是NPC障碍
                    daysSinceDug = -1,        // 未挖掘
                    daysSinceWatered = -1,    // 未浇水
                    seedItemCode = -1,        // 未种植
                    growthDays = -1           // 未生长
                };

                gridPropertyDictionary[key] = details;
            }
        }

        Debug.Log($"✅ 成功创建 {gridPropertyDictionary.Count} 个网格属性");
    }

    /// <summary>
    /// 获取网格属性详情
    /// </summary>
    public GridPropertyDetails GetGridPropertyDetails(int gridX, int gridY)
    {
        if (gridPropertyDictionary == null)
        {
            Debug.LogError("网格属性字典未初始化");
            return null;
        }

        string key = $"x{gridX}y{gridY}";

        if (gridPropertyDictionary.TryGetValue(key, out GridPropertyDetails details))
        {
            return details;
        }

        // 如果坐标不在预设范围内，动态创建一个
        Debug.LogWarning($"坐标({gridX},{gridY})不在预设范围内，动态创建属性");

        GridPropertyDetails newDetails = new GridPropertyDetails
        {
            gridX = gridX,
            gridY = gridY,
            isDiggable = true,
            canDropItem = true,
            daysSinceDug = -1,
            daysSinceWatered = -1,
            seedItemCode = -1,
            growthDays = -1
        };

        gridPropertyDictionary[key] = newDetails;
        return newDetails;
    }

    /// <summary>
    /// 设置网格属性详情
    /// </summary>
    public void SetGridPropertyDetails(int gridX, int gridY, GridPropertyDetails gridPropertyDetails)
    {
        if (gridPropertyDictionary == null)
        {
            Debug.LogError("网格属性字典未初始化");
            return;
        }

        string key = $"x{gridX}y{gridY}";
        gridPropertyDictionary[key] = gridPropertyDetails;

        // 立即更新显示
        UpdateGridDisplay(gridPropertyDetails);
    }

    /// <summary>
    /// 更新网格显示
    /// </summary>
    private void UpdateGridDisplay(GridPropertyDetails gridPropertyDetails)
    {
        if (groundDecoration1 == null || groundDecoration2 == null)
        {
            Debug.LogWarning("Tilemap组件未分配，无法更新显示");
            return;
        }

        Vector3Int tilePosition = new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0);

        // 显示挖掘状态
        if (gridPropertyDetails.daysSinceDug > -1 && dugTile != null)
        {
            groundDecoration1.SetTile(tilePosition, dugTile);
        }
        else
        {
            groundDecoration1.SetTile(tilePosition, null);
        }

        // 显示浇水状态
        if (gridPropertyDetails.daysSinceWatered > -1 && wateredTile != null)
        {
            groundDecoration2.SetTile(tilePosition, wateredTile);
        }
        else
        {
            groundDecoration2.SetTile(tilePosition, null);
        }
    }

    /// <summary>
    /// 显示已挖掘的地块
    /// </summary>
    public void DisplayDugGround(GridPropertyDetails gridPropertyDetails)
    {
        if (gridPropertyDetails.daysSinceDug > -1 && dugTile != null && groundDecoration1 != null)
        {
            Vector3Int tilePosition = new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0);
            groundDecoration1.SetTile(tilePosition, dugTile);
        }
    }

    /// <summary>
    /// 显示已浇水的地块
    /// </summary>
    public void DisplayWateredGround(GridPropertyDetails gridPropertyDetails)
    {
        if (gridPropertyDetails.daysSinceWatered > -1 && wateredTile != null && groundDecoration2 != null)
        {
            Vector3Int tilePosition = new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0);
            groundDecoration2.SetTile(tilePosition, wateredTile);
        }
    }

    /// <summary>
    /// 显示已种植的作物
    /// </summary>
    public void DisplayPlantedCrop(GridPropertyDetails gridPropertyDetails)
    {
        if (gridPropertyDetails.seedItemCode > -1 && so_CropDetailsList != null)
        {
            CropDetails cropDetails = so_CropDetailsList.GetCropDetails(gridPropertyDetails.seedItemCode);
            if (cropDetails != null && cropParentTransform != null)
            {
                // 计算世界位置
                Vector3 worldPosition = GetWorldPosition(gridPropertyDetails.gridX, gridPropertyDetails.gridY);

                // 创建作物实例
                GameObject cropPrefab = GetCropPrefabForGrowthStage(cropDetails, gridPropertyDetails.growthDays);
                if (cropPrefab != null)
                {
                    GameObject cropInstance = Instantiate(cropPrefab, worldPosition, Quaternion.identity);
                    cropInstance.transform.SetParent(cropParentTransform);

                    // 设置作物网格位置
                    Crop cropComponent = cropInstance.GetComponent<Crop>();
                    if (cropComponent != null)
                    {
                        cropComponent.cropGridPosition = new Vector2Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 根据生长天数获取对应的作物预制体
    /// </summary>
    private GameObject GetCropPrefabForGrowthStage(CropDetails cropDetails, int growthDays)
    {
        if (cropDetails.growthPrefab == null || cropDetails.growthPrefab.Length == 0)
            return null;

        int growthStages = cropDetails.growthPrefab.Length;
        int currentStage = 0;

        // 找到当前生长阶段
        for (int i = growthStages - 1; i >= 0; i--)
        {
            if (growthDays >= cropDetails.growthDays[i])
            {
                currentStage = i;
                break;
            }
        }

        return cropDetails.growthPrefab[currentStage];
    }

    /// <summary>
    /// 获取网格的世界位置
    /// </summary>
    private Vector3 GetWorldPosition(int gridX, int gridY)
    {
        if (grid != null)
        {
            return grid.GetCellCenterWorld(new Vector3Int(gridX, gridY, 0));
        }

        // 备用计算方法
        return new Vector3(gridX + 0.5f, gridY + 0.5f, 0);
    }

    /// <summary>
    /// 获取作物详情
    /// </summary>
    public CropDetails GetCropDetails(int seedItemCode)
    {
        if (so_CropDetailsList != null)
        {
            return so_CropDetailsList.GetCropDetails(seedItemCode);
        }
        return null;
    }

    /// <summary>
    /// 时间推进处理
    /// </summary>
    private void AdvanceDay()
    {
        Debug.Log("时间推进，更新作物生长");

        if (gridPropertyDictionary == null) return;

        // 更新所有网格的生长状态
        foreach (var key in new List<string>(gridPropertyDictionary.Keys))
        {
            GridPropertyDetails details = gridPropertyDictionary[key];

            // 推进生长
            if (details.growthDays > -1)
            {
                details.growthDays += 1;
                Debug.Log($"作物生长: ({details.gridX},{details.gridY}) -> {details.growthDays}天");
            }

            // 重置浇水状态
            if (details.daysSinceWatered > -1)
            {
                details.daysSinceWatered = -1;
            }

            gridPropertyDictionary[key] = details;

            // 更新显示
            UpdateGridDisplay(details);

            // 重新显示作物（如果存在）
            if (details.seedItemCode > -1)
            {
                // 先清除旧的作物
                ClearCropAtPosition(details.gridX, details.gridY);
                // 显示新的生长阶段
                DisplayPlantedCrop(details);
            }
        }
    }

    /// <summary>
    /// 清除指定位置的作物
    /// </summary>
    private void ClearCropAtPosition(int gridX, int gridY)
    {
        if (cropParentTransform == null) return;

        Vector2Int targetPosition = new Vector2Int(gridX, gridY);

        foreach (Transform child in cropParentTransform)
        {
            Crop crop = child.GetComponent<Crop>();
            if (crop != null && crop.cropGridPosition == targetPosition)
            {
                Destroy(child.gameObject);
                break;
            }
        }
    }

    /// <summary>
    /// 调试方法：检查网格状态
    /// </summary>
    [ContextMenu("调试网格状态")]
    public void DebugGridStatus()
    {
        Debug.Log("=== 网格状态调试 ===");
        Debug.Log($"总网格数: {gridPropertyDictionary?.Count}");

        int dugCount = 0;
        int wateredCount = 0;
        int plantedCount = 0;

        if (gridPropertyDictionary != null)
        {
            foreach (var details in gridPropertyDictionary.Values)
            {
                if (details.daysSinceDug > -1) dugCount++;
                if (details.daysSinceWatered > -1) wateredCount++;
                if (details.seedItemCode > -1) plantedCount++;
            }
        }

        Debug.Log($"已挖掘: {dugCount}, 已浇水: {wateredCount}, 已种植: {plantedCount}");
        Debug.Log("=== 调试结束 ===");
    }

    /// <summary>
    /// 重置所有网格状态（用于测试）
    /// </summary>
    [ContextMenu("重置所有网格")]
    public void ResetAllGrids()
    {
        Debug.Log("重置所有网格状态");

        // 清除Tilemap显示
        if (groundDecoration1 != null) groundDecoration1.ClearAllTiles();
        if (groundDecoration2 != null) groundDecoration2.ClearAllTiles();

        // 清除所有作物
        if (cropParentTransform != null)
        {
            foreach (Transform child in cropParentTransform)
            {
                Destroy(child.gameObject);
            }
        }

        // 重新初始化网格属性
        InitializeGridProperties();

        Debug.Log("✅ 所有网格已重置");
    }
}