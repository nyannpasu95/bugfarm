using MFarm.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerFarming : MonoBehaviour
{
    [Header("依赖引用")]
    public Camera mainCamera;
    public Grid grid;
    public CursorManager cursorManager;
    public InventoryManager inventoryManager;
    public GridPropertiesManager gridPropertyManager; 


    private ItemDetails currentItem;
    private bool canClick => !EventSystem.current.IsPointerOverGameObject();

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
        if (isSelected) currentItem = itemDetails;
        else currentItem = null;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && canClick)
        {
            TryHandleClick();
        }
    }

    private void TryHandleClick()
    {
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int gridPos = grid.WorldToCell(mouseWorldPos);

        Debug.Log($"=== 点击调试 ===");
        Debug.Log($"屏幕位置: {Input.mousePosition}");
        Debug.Log($"世界位置: {mouseWorldPos}");
        Debug.Log($"网格位置: {gridPos}");

        // 检查网格组件
        if (grid == null)
        {
            Debug.LogError("❌ Grid组件为null");
            return;
        }

        // 检查GridPropertiesManager
        if (GridPropertiesManager.Instance == null)
        {
            Debug.LogError("❌ GridPropertiesManager实例为null");
            return;
        }

        // 检查字典状态
        if (GridPropertiesManager.Instance.gridPropertyDictionary == null)
        {
            Debug.LogError("❌ 网格属性字典为null");
            return;
        }

        Debug.Log($"字典总数: {GridPropertiesManager.Instance.gridPropertyDictionary.Count}");

        GridPropertyDetails gridDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(gridPos.x, gridPos.y);

        if (gridDetails == null)
        {
            Debug.LogError($"❌ 无法获取坐标 ({gridPos.x}, {gridPos.y}) 的网格属性");

            // 检查附近坐标
            Debug.Log("检查附近坐标:");
            for (int x = gridPos.x - 2; x <= gridPos.x + 2; x++)
            {
                for (int y = gridPos.y - 2; y <= gridPos.y + 2; y++)
                {
                    var nearbyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(x, y);
                    if (nearbyDetails != null)
                    {
                        Debug.Log($"✅ 附近坐标 ({x}, {y}) 存在, 可挖掘: {nearbyDetails.isDiggable}");
                    }
                }
            }
            return;
        }

        Debug.Log($"✅ 找到网格属性:");
        Debug.Log($"  - 可挖掘: {gridDetails.isDiggable}");
        Debug.Log($"  - 可放置: {gridDetails.canPlaceFurniture}");
        Debug.Log($"  - 已挖掘: {gridDetails.daysSinceDug}");
        Debug.Log($"  - 已浇水: {gridDetails.daysSinceWatered}");

        if (currentItem == null)
        {
            Debug.Log("ℹ️ 当前未选择物品");
            return;
        }

        Debug.Log($"当前选中物品: {currentItem.name} (ID: {currentItem.itemID}, 类型: {currentItem.itemType})");

        switch (currentItem.itemType)
        {
            case ItemType.Tool:
                UseTool(gridPos, gridDetails);
                break;
            case ItemType.Seed:
                PlantSeed(gridPos, gridDetails);
                break;
            default:
                Debug.Log($"ℹ️ 物品类型 {currentItem.itemType} 暂无处理逻辑");
                break;
        }

        Debug.Log("=== 点击处理完成 ===");
    }

    private void UseTool(Vector3Int gridPos, GridPropertyDetails gridDetails)
    {
        // First, check if there's a crop at this position that can be harvested
        Crop cropAtPosition = GetCropAtGridPosition(gridPos);

        if (cropAtPosition != null)
        {
            // There's a crop here, try to harvest it with the current tool
            Debug.Log($"✅ Found crop at position, attempting to harvest with tool {currentItem.itemID}");
            ProcessCropWithTool(cropAtPosition, gridDetails);
            return;
        }

        // No crop, so use tool on ground
        if (currentItem.itemID == 6001)
        {
            TryDigGround(gridPos, gridDetails);
        }
        else if (currentItem.itemID == 5008)
        {
            TryWaterGround(gridPos, gridDetails);
        }
    }

    private Crop GetCropAtGridPosition(Vector3Int gridPos)
    {
        // Convert grid position to world position
        Vector3 worldPos = grid.CellToWorld(gridPos);
        // Adjust to center of cell
        worldPos += new Vector3(0.5f, 0.5f, 0f);

        // Check for crop at this position using a small overlap check
        Collider2D[] colliders = Physics2D.OverlapCircleAll(worldPos, 0.5f);

        foreach (Collider2D collider in colliders)
        {
            Crop crop = collider.GetComponent<Crop>();
            if (crop != null)
            {
                Debug.Log($"✅ Found crop GameObject: {crop.gameObject.name}");
                return crop;
            }
        }

        Debug.Log($"ℹ️ No crop found at world position {worldPos}");
        return null;
    }

    private void ProcessCropWithTool(Crop crop, GridPropertyDetails gridDetails)
    {
        // Determine tool direction based on player position relative to crop
        // For now, using default right/up direction
        // You can enhance this later to detect actual player direction
        bool isToolRight = true;
        bool isToolLeft = false;
        bool isToolDown = false;
        bool isToolUp = false;

        // Call the crop's ProcessToolAction method
        Debug.Log($"Calling crop.ProcessToolAction with tool {currentItem.name}");
        crop.ProcessToolAction(currentItem, isToolRight, isToolLeft, isToolDown, isToolUp);
    }

    private void TryDigGround(Vector3Int pos, GridPropertyDetails details)
    {
        if (details.isDiggable && details.daysSinceDug == -1)
        {
            details.daysSinceDug = 0;
            GridPropertiesManager.Instance.SetGridPropertyDetails(pos.x, pos.y, details);
            GridPropertiesManager.Instance.DisplayDugGround(details);
        }
    }

    private void TryWaterGround(Vector3Int pos, GridPropertyDetails details)
    {
        if (details.daysSinceDug > -1 && details.daysSinceWatered == -1)
        {
            details.daysSinceWatered = 0;
            GridPropertiesManager.Instance.SetGridPropertyDetails(pos.x, pos.y, details);
            GridPropertiesManager.Instance.DisplayWateredGround(details);
        }
    }

    private void PlantSeed(Vector3Int pos, GridPropertyDetails details)
    {
        if (details.daysSinceDug > -1 && details.seedItemCode == -1)
        {
            details.seedItemCode = currentItem.itemID;
            details.growthDays = 0;
            GridPropertiesManager.Instance.SetGridPropertyDetails(pos.x, pos.y, details);
            GridPropertiesManager.Instance.DisplayPlantedCrop(details);

            inventoryManager.RemoveItem(currentItem.itemID, 1); // 扣除1个种子
        }
    }

}
