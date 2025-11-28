using MFarm.Inventory;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridCursor : MonoBehaviour
{
    private GameObject player;
    private Canvas canvas;
    private Grid grid;
    private Camera mainCamera;
    [SerializeField] private Image cursorImage = null;
    [SerializeField] private RectTransform cursorRectTransform = null;
    [SerializeField] private Sprite greenCursorSprite = null;
    [SerializeField] private Sprite redCursorSprite = null;

    [SerializeField] private SO_CropDetailsList so_CropDetailsList = null;

    private bool _cursorPositionIsValid = false;
    public bool CursorPositionIsValid { get => _cursorPositionIsValid; set => _cursorPositionIsValid = value; }

    private int _itemUseGridRadius = 0;
    public int ItemUseGridRadius { get => _itemUseGridRadius; set => _itemUseGridRadius = value; }

    private ItemType _selectedItemType;
    public ItemType SelectedItemType { get => _selectedItemType; set => _selectedItemType = value; }

    private bool _cursorIsEnabled = false;
    public bool CursorIsEnabled { get => _cursorIsEnabled; set => _cursorIsEnabled = value; }

    // 存储当前选中的物品详情
    private ItemDetails _selectedItemDetails;

    private void OnEnable()
    {
        // 订阅物品选中事件和场景加载事件
        EventHandler.ItemSelectedEvent += OnItemSelected;
        EventHandler.AfterSceneLoadEvent += SceneLoaded;
    }

    private void OnDisable()
    {
        // 取消订阅
        EventHandler.ItemSelectedEvent -= OnItemSelected;
        EventHandler.AfterSceneLoadEvent -= SceneLoaded;
    }

    private void OnItemSelected(ItemDetails itemDetails, bool isSelected)
    {
        if (isSelected)
        {
            _selectedItemDetails = itemDetails;
            _selectedItemType = itemDetails.itemType;
        }
        else
        {
            _selectedItemDetails = null;
            _selectedItemType = ItemType.Seed; // 默认类型
        }
    }

    // 临时的HelperMethods替代方案
    private bool GetComponentsAtBoxLocation<T>(out List<T> components, Vector3 cursorPosition, Vector2 cursorSize, float angle) where T : class
    {
        components = new List<T>();
        Collider2D[] colliders = Physics2D.OverlapBoxAll(cursorPosition, cursorSize, angle);

        foreach (Collider2D collider in colliders)
        {
            T component = collider.GetComponent<T>();
            if (component != null)
            {
                components.Add(component);
            }
        }

        return components.Count > 0;
    }

    // Start is called before the first frame update
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        mainCamera = Camera.main;
        canvas = GetComponentInParent<Canvas>();

        // Initialize and get Grid
        grid = FindFirstObjectByType<Grid>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (CursorIsEnabled)
        {
            DisplayCursor();
        }
    }

    private Vector3Int DisplayCursor()
    {
        if (grid != null)
        {
            // Get grid position for cursor
            Vector3Int gridPosition = GetGridPositionForCursor();

            // Get grid position for player
            Vector3Int playerGridPosition = GetGridPositionForPlayer();

            // Set cursor sprite
            SetCursorValidity(gridPosition, playerGridPosition);

            // Get rect transform position for cursor
            cursorRectTransform.position = GetRectTransformPositionForCursor(gridPosition);

            return gridPosition;
        }
        else
        {
            return Vector3Int.zero;
        }
    }

    private void SceneLoaded()
    {
        grid = FindFirstObjectByType<Grid>();
        Debug.Log("GridCursor: Scene loaded, re-acquiring Grid component");
    }

    private void SetCursorValidity(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {
        SetCursorToValid();

        // Check item use radius is valid
        if (Mathf.Abs(cursorGridPosition.x - playerGridPosition.x) > ItemUseGridRadius
            || Mathf.Abs(cursorGridPosition.y - playerGridPosition.y) > ItemUseGridRadius)
        {
            SetCursorToInvalid();
            return;
        }

        // 使用存储的选中物品详情
        if (_selectedItemDetails == null)
        {
            SetCursorToInvalid();
            return;
        }

        // 检查GridPropertiesManager是否可用
        if (GridPropertiesManager.Instance == null)
        {
            Debug.LogWarning("GridPropertiesManager实例不可用");
            SetCursorToInvalid();
            return;
        }

        // Get grid property details at cursor position
        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cursorGridPosition.x, cursorGridPosition.y);

        if (gridPropertyDetails != null)
        {
            // Determine cursor validity based on inventory item selected and grid property details
            switch (_selectedItemDetails.itemType)
            {
                case ItemType.Seed:
                    if (!IsCursorValidForSeed(gridPropertyDetails))
                    {
                        SetCursorToInvalid();
                        return;
                    }
                    break;

                case ItemType.Tool:
                    if (!IsCursorValidForTool(gridPropertyDetails, _selectedItemDetails))
                    {
                        SetCursorToInvalid();
                        return;
                    }
                    break;

                case ItemType.Placeable:
                    if (!IsCursorValidForPlaceable(gridPropertyDetails))
                    {
                        SetCursorToInvalid();
                        return;
                    }
                    break;

                case ItemType.Crop:
                case ItemType.Material:
                case ItemType.Creature:
                case ItemType.Consumable:
                    // 这些类型的验证逻辑可以后续添加
                    break;

                default:
                    break;
            }
        }
        else
        {
            SetCursorToInvalid();
            return;
        }
    }

    /// <summary>
    /// Set the cursor to be invalid
    /// </summary>
    private void SetCursorToInvalid()
    {
        cursorImage.sprite = redCursorSprite;
        CursorPositionIsValid = false;
    }

    /// <summary>
    /// Set the cursor to be valid
    /// </summary>
    private void SetCursorToValid()
    {
        cursorImage.sprite = greenCursorSprite;
        CursorPositionIsValid = true;
    }

    /// <summary>
    /// Set cursor validity for a seed for the target gridPropertyDetails. Returns true if valid, false if invalid
    /// </summary>
    private bool IsCursorValidForSeed(GridPropertyDetails gridPropertyDetails)
    {
        return gridPropertyDetails.isDiggable && gridPropertyDetails.daysSinceDug > -1 &&
               gridPropertyDetails.seedItemCode == -1 && gridPropertyDetails.growthDays == -1;
    }

    /// <summary>
    /// Sets the cursor as either valid or invalid for the tool for the target gridPropertyDetails. Returns true if valid or false if invalid
    /// </summary>
    private bool IsCursorValidForTool(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
    {
        // 根据工具用途判断有效性
        // 这里需要根据你的工具系统具体实现

        // 锄头工具：检查是否可挖掘且未被挖掘
        if (itemDetails.itemType == ItemType.Tool && gridPropertyDetails.isDiggable && gridPropertyDetails.daysSinceDug == -1)
        {
            // 检查该位置是否有可收割的场景物品
            Vector3 cursorWorldPosition = GetWorldPositionForCursor();
            List<Crop> itemList = new List<Crop>();

            if (GetComponentsAtBoxLocation(out itemList, cursorWorldPosition, new Vector2(1f, 1f), 0f))
            {
                // 如果有作物，不让挖掘
                return itemList.Count == 0;
            }
            return true;
        }

        // 浇水工具：检查是否已挖掘但未浇水
        if (itemDetails.itemType == ItemType.Tool && gridPropertyDetails.daysSinceDug > -1 && gridPropertyDetails.daysSinceWatered == -1)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Test cursor validity for a placeable item for the target gridPropertyDetails. Returns true if valid, false if invalid
    /// </summary>
    private bool IsCursorValidForPlaceable(GridPropertyDetails gridPropertyDetails)
    {
        return gridPropertyDetails.canPlaceFurniture;
    }

    public void DisableCursor()
    {
        cursorImage.color = Color.clear;
        CursorIsEnabled = false;
    }

    public void EnableCursor()
    {
        cursorImage.color = new Color(1f, 1f, 1f, 1f);
        CursorIsEnabled = true;
    }

    public Vector3Int GetGridPositionForCursor()
    {
        if (grid == null)
            return Vector3Int.zero;

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));
        return grid.WorldToCell(worldPosition);
    }

    public Vector3Int GetGridPositionForPlayer()
    {
        if (grid == null)
            return Vector3Int.zero;

        return grid.WorldToCell(player.transform.position);
    }

    public Vector2 GetRectTransformPositionForCursor(Vector3Int gridPosition)
    {
        if (grid == null)
            return Vector2.zero;

        Vector3 gridWorldPosition = grid.CellToWorld(gridPosition);
        Vector2 gridScreenPosition = mainCamera.WorldToScreenPoint(gridWorldPosition);
        return RectTransformUtility.PixelAdjustPoint(gridScreenPosition, cursorRectTransform, canvas);
    }

    public Vector3 GetWorldPositionForCursor()
    {
        if (grid == null)
            return Vector3.zero;

        return grid.CellToWorld(GetGridPositionForCursor());
    }
}
